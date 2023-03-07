using Kruso.Umbraco.Delivery.Helper;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using Newtonsoft.Json.Linq;
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

        public JsonNode Get(string[] features, string culture = null)
        {
            var manifest = _deliCache.GetFromMemory<JsonNode>(ManifestCacheKey);
            if (manifest == null)
            {
                manifest = CreateManifest();
                _deliCache.AddToMemory(ManifestCacheKey, manifest);
            }

            manifest = manifest.Clone();
            var domains = new List<JsonNode>();
            foreach (var domain in manifest.Nodes("domains"))
            {
                var clonedDomain = domain.Clone();

                if (!string.IsNullOrEmpty(culture))
                {
                    var domainCulture = clonedDomain
                        .Node("domain")
                            .Node("cultureInfo").Culture;

                    if (!domainCulture.Equals(culture, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                }

                if (features.Any())
                {
                    if (!features.Contains("domain"))
                        clonedDomain.RemoveProp("domain");

                    if (!features.Contains("settings"))
                        clonedDomain.RemoveProp("settings");

                    if (!features.Contains("routes"))
                        clonedDomain.RemoveProp("routes");

                    if (!features.Contains("translations"))
                        clonedDomain.RemoveProp("translations");
                }

                domains.Add(clonedDomain);
            }

            manifest.AddProp("domains", domains);
            return manifest;
        }

        private JsonNode CreateManifest()
        {
            var versions = VersionHelper.GetVersions();
            var domainInfos = GetAllDomainInfo();

            var domains = new List<JsonNode>();
            foreach (var domain in domainInfos)
            {
                var domainCulture = domain
                    .Node("domain")
                        .Node("cultureInfo").Culture;

                domain
                    .AddProp("settings", GetSettings(domainCulture))
                    .AddProp("routes", GetRouteInfo(domainCulture))
                    .AddProp("translations", GetTranslationInfo(domainCulture));

                domains.Add(domain);
            }

            var manifest = new JsonNode()
                .AddProp("versions", versions)
                .AddProp("domains", domains);

            return manifest;
        }

        private JsonNode GetSettings(string domainCulture)
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
            var domainsByCulture = _deliDomain.GetDomainsByRequest()
                .GroupBy(x => x.Culture, (culture, domains) => new { culture, domains });

            foreach (var cultureDomains in domainsByCulture)
            {
                var domain = cultureDomains.domains.First();
                var startPage = _deliPages.StartPage(domain);
                if (startPage is not null)
                {
                    var domainPaths = cultureDomains.domains
                        .Select(x => x.Uri.ToString())
                        .ToArray();

                    res.Add(new JsonNode()
                        .AddProp("domain", new JsonNode()
                            .AddProp("rootPageId", startPage.Key)
                            .AddProp("paths", domainPaths)
                            .AddProp("cultureInfo", _deliCulture.GetCultureInfo(domain.Culture))));
                }
            }

            return res;
        }

        private List<JsonNode> GetRouteInfo(string culture = null)
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

        public List<JsonNode> GetTranslationInfo(string culture = null, string filter = null)
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
