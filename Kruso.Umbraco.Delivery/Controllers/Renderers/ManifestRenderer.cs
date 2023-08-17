using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Helper;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Services;

namespace Kruso.Umbraco.Delivery.Controllers.Renderers
{
    public class ManifestRenderer
    {
        private readonly IDeliPages _deliPages;
        private readonly IDeliDomain _deliDomain;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliContent _deliContent;
        private readonly IDeliCache _deliCache;
        private readonly IModelFactory _modelFactory;
        private readonly IModelConverter _modelConverter;
        private readonly ILocalizationService _localizationService;

        private const string ManifestCacheKey = "deli-manifest";

        private static readonly string[] ExcludeSettingsProps = JsonNode.ReservedProps.All
            .Concat(new[] { "name", "urls" })
            .ToArray();

        public ManifestRenderer(
            IDeliPages deliPages, 
            IDeliDomain deliDomain, 
            IDeliCulture deliCulture, 
            IDeliContent deliContent,
            IDeliCache deliCache,
            IModelFactory modelFactory, 
            IModelConverter modelConverter,
            ILocalizationService localizationService)
        {
            _deliPages = deliPages;
            _deliDomain = deliDomain;
            _deliCulture = deliCulture;
            _deliContent = deliContent;
            _deliCache = deliCache;
            _modelFactory = modelFactory;
            _modelConverter = modelConverter;
            _localizationService = localizationService;
        }

        public JsonNode GetManifest(string[] features, string culture = null, Guid? rootPageId = null)
        {
            JsonNode manifest = GetManifest();

            var domains = manifest
                .Nodes("domains")
                .Where(x => IncludeDomainInManifest(x, culture, rootPageId))
                .Select(x => DomainWithFeatures(x, features))
                .ToList();

            manifest.AddProp("domains", domains);

            return manifest;
        }

        private bool IncludeDomainInManifest(JsonNode domain, string? culture, Guid? rootPageId)
        {
            var domainCulture = domain.Val<string>("culture");
            var domainRootPageId = domain.Val<Guid>("rootPageId");

            return
                (culture is null || culture.Equals(domainCulture, StringComparison.InvariantCultureIgnoreCase)) &&
                (rootPageId is null || rootPageId.Value == domainRootPageId);
        }

        private JsonNode DomainWithFeatures(JsonNode domain, string[] features)
        {
            var domainWithFeatures = new JsonNode()
                .CopyProps(domain, "rootPageId", "culture");

            if (!features.Any() || features.Contains("domain"))
                domainWithFeatures.CopyProp(domain, "domain");

            if (!features.Any() || features.Contains("settings"))
                domainWithFeatures.CopyProp(domain, "settings");

            if (!features.Any() || features.Contains("routes"))
                domainWithFeatures.CopyProp(domain, "routes");

            if (!features.Any() || features.Contains("translations"))
                domainWithFeatures.CopyProp(domain, "translations");

            return domainWithFeatures;
        }

        private JsonNode GetManifest()
        {
            JsonNode manifest = null;
#if !DEBUG
            manifest = _deliCache.GetFromMemory<JsonNode>(ManifestCacheKey);
#endif
            if (manifest == null)
            {
                manifest = CreateManifest();
#if !DEBUG
                _deliCache.AddToMemory(ManifestCacheKey, manifest);
#endif
            }

            return manifest?.Clone();
        }

        private JsonNode CreateManifest()
        {
            var versions = VersionHelper.GetVersions();
            var domainInfos = GetAllDomainInfo();

            var manifestEntries = new List<JsonNode>();
            foreach (var domain in domainInfos)
            {
                var domainCulture = domain.Val<string>("cultureInfo.culture");
                _deliCulture.WithCultureContext(domainCulture, () =>
                {
                    manifestEntries.Add(new JsonNode()
                        .CopyProp(domain, "rootPageId:rootPageGuid")
                        .AddProp("culture", domainCulture)
                        .AddProp("domain", domain)
                        .AddProp("settings", CreateSettings(domainCulture))
                        .AddProp("routes", CreateRouteInfo(domainCulture))
                        .AddProp("translations", CreateTranslationInfo(domainCulture)));
                });
            }

            var manifest = new JsonNode()
                .AddProp("versions", versions)
                .AddProp("domains", manifestEntries);

            return manifest;
        }

        private JsonNode CreateSettings(string domainCulture)
        {
            var settingsContent = _deliPages.SettingsPage(domainCulture);
            if (settingsContent != null)
            {
                var settingsBlock = _modelFactory.CreateBlock(settingsContent, domainCulture);
                var settings = _modelConverter.Convert(settingsBlock, TemplateType.Block);

                return new JsonNode()
                    .CopyAllExceptProps(settings, ExcludeSettingsProps);
            }

            return null;
        }

        private IEnumerable<JsonNode> GetAllDomainInfo()
        {
            var res = new List<JsonNode>();
            foreach (var domain in _deliDomain.GetDomains())
            {
                var startPage = _deliPages.StartPage(domain);
                if (startPage != null)
                {
                    _deliCulture.WithCultureContext(domain.Culture, () =>
                    {
                        res.Add(new JsonNode()
                            .AddProp("rootPageId", startPage.Id)
                            .AddProp("rootPageGuid", startPage.Key)
                            .AddProp("name", startPage.Name(domain))
                            .AddProp("path", domain.Uri.ToString())
                            .AddProp("cultureInfo", _deliCulture.GetCultureInfo(domain.Culture)));
                    });
                }
            }

            return res;
        }

        private List<JsonNode> CreateRouteInfo(string culture = null)
        {
            var res = new List<JsonNode>();

            var startPages = _deliPages.StartPages();

            foreach (var pageCulture in startPages.Keys.Where(x => string.IsNullOrEmpty(culture) || x.Equals(culture, StringComparison.InvariantCultureIgnoreCase)))
            {
                _deliCulture.WithCultureContext(pageCulture, () =>
                {
                    var descendants = _deliContent.PublishedDescendants(startPages[pageCulture]);

                    var nodes = _modelFactory
                        .CreateRoutes(descendants, pageCulture)
                        .Select(x => _modelConverter.Convert(x, TemplateType.Route));

                    res.AddRange(nodes);
                });
            }

            return res;
        }

        public List<JsonNode> CreateTranslationInfo(string culture = null, string filter = null)
        {
            var res = new List<JsonNode>();

            var languages = _localizationService.GetAllLanguages();
            var items = _localizationService.GetDictionaryItemDescendants(null);

            if (!string.IsNullOrEmpty(filter))
            {
                items = filter.EndsWith("*")
                    ? items.Where(x => x.ItemKey.ToLower().StartsWith(filter.ToLower().TrimEnd('*')))
                    : items.Where(x => x.ItemKey.Equals(filter, System.StringComparison.InvariantCultureIgnoreCase));
            }

            foreach (var item in items)
            {
                var translation = new JsonNode()
                    .AddProp("id", item.Key)
                    .AddProp("key", item.ItemKey);

                if (!string.IsNullOrEmpty(culture))
                {
                    var val = item.Translations
                        .FirstOrDefault(x => x.Language.CultureInfo.Name.Equals(culture, System.StringComparison.InvariantCultureIgnoreCase))
                        ?.Value;

                    if (!string.IsNullOrEmpty(val))
                    {
                        translation.AddProp("value", val);
                    }
                }
                else
                {
                    translation.AddProp("translations", item.Translations.Select(x => new
                    {
                        culture = x.Language.CultureInfo.Name.ToLower(),
                        value = x.Value
                    }));
                }

                res.Add(translation);
            }

            return res;
        }
    }
}
