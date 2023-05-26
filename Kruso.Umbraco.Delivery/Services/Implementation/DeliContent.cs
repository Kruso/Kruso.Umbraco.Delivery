using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliContent : IDeliContent
    {
        private readonly IDeliConfig _deliConfig;
        private readonly IDeliCulture _deliCulture;
        private readonly ILogger<DeliContent> _log;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IRelationService _relationService;

        public DeliContent(
            IDeliConfig deliConfig,
            IDeliCulture deliCulture,
            IUmbracoContextAccessor umbracoContextAccessor,
            ILogger<DeliContent> log,
            IRelationService relationService)
        {
            _deliConfig = deliConfig;
            _deliCulture = deliCulture;
            _umbracoContextAccessor = umbracoContextAccessor;
            _log = log;
            _relationService = relationService;
        }

        public IEnumerable<IPublishedContent> PublishedDescendants(IPublishedContent content)
        {
            if (content != null && !IsSettingsType(content))
            {
                if (!IsNotFoundType(content) && content.IsVisible())
                {
                    yield return content;
                }

                foreach (var child in content.Children.SelectMany(x => PublishedDescendants(x)))
                    yield return child;
            }
        }

        public IPublishedContent Root(IEnumerable<IPublishedContent> rootItems)
        {
            var root = rootItems?.FirstOrDefault(x => IsPage(x));
            return root;
        }

        public bool IsRenderablePage(IPublishedContent content)
        {
            if (content == null || content.ItemType != PublishedItemType.Content)
                return false;

            return content.TemplateId > 0;
        }

        public bool IsRenderablePage(IContent content)
        {
            return content != null && content.TemplateId > 0;
        }

        public bool IsPage(IPublishedContent content)
        {
            return IsRenderablePage(content) 
                && !IsSettingsType(content);
        }

        public bool IsNotFoundType(IPublishedContent content)
        {
            var config = _deliConfig.Get();
            return content != null && config.NotFoundType == content.ContentType.Alias;
        }

        public bool IsSettingsType(IPublishedContent content)
        {
            var config = _deliConfig.Get();
            return content != null && content.ContentType.Alias == config.SettingsType;
        }

        public string NameByCulture(IPublishedContent content, string culture)
        {
            string res = null;

            if (content != null && !string.IsNullOrEmpty(culture))
            {
                _deliCulture.WithCultureContext(culture, () =>
                {
                    res = content.Name;
                });
            }

            return res ?? content.Name;
        }

        public IEnumerable<IPublishedContent> RootPublishedContent()
        {
            var context = _umbracoContextAccessor.GetRequiredUmbracoContext();
            var res = context.Content.GetAtRoot();
            return res;

            return Enumerable.Empty<IPublishedContent>();
        }

        public IEnumerable<IPublishedContent> RootPages()
        {
            return RootPublishedContent()
                .Where(x => IsPage(x));
        }

        public IEnumerable<IPublishedContent> RootPages(string culture)
        {
            return RootPages()
                .Where(x => _deliCulture.IsPublishedInCulture(x, culture));
        }

        public IPublishedContent PublishedContent(Guid id)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var context))
            {
                return context.Content.GetById(id);
            }

            return null;
        }

        public IPublishedContent PublishedContent(int id)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var context))
            {
                return context.Content.GetById(id);
            }

            return null;
        }


        public IPublishedContent PublishedContent(string path, string culture)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var context))
            {
                var content = context.Content.GetByRoute(path, null, culture);
                return content;
            }

            return null;
        }

        public IEnumerable<IPublishedContent> PublishedChildren(Guid id, params string[] documentTypeAliases)
        {
            return PublishedChildren(PublishedContent(id), documentTypeAliases);

        }

        public IEnumerable<IPublishedContent> PublishedChildren(int id, params string[] documentTypeAliases)
        {
            return PublishedChildren(PublishedContent(id), documentTypeAliases);
        }

        public IEnumerable<IPublishedContent> PublishedChildren(IPublishedContent content, params string[] documentTypeAliases)
        {
            var res = content?.Children ?? Enumerable.Empty<IPublishedContent>();
            if (documentTypeAliases?.Any() ?? false)
            {
                return res.Where(x =>
                    documentTypeAliases.Any(a => a.Equals(x.ContentType.Alias, StringComparison.InvariantCultureIgnoreCase)));
            }

            return res;
        }

        public IPublishedContent UnpublishedContent(int id)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var context))
            {
                return context.Content.GetById(true, id);
            }

            return null;
        }

        public IPublishedContent UnpublishedContent(Guid id)
        {
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var context))
            {
                return context.Content.GetById(true, id);
            }

            return null;
        }

        public List<IPublishedContent> RelatedPages(int id)
        {
            var res = new List<IPublishedContent>();

            var relations = _relationService.GetByChildId(id);
            foreach (var relation in relations.Where(x => x.RelationType.Alias == "umbDocument"))
            {
                var content = PublishedContent(relation.ParentId);
                if (IsRenderablePage(content))
                    res.Add(content);
                else
                    res.AddRange(RelatedPages(relation.ParentId));
            }

            return res;
        }
    }
}
