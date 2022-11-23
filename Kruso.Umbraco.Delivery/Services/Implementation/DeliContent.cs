using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliContent : IDeliContent
    {
        private readonly IDeliConfig _deliConfig;
        private readonly IDeliContentTypes _deliContentTypes;
        private readonly IDeliCulture _deliCulture;
        private readonly ILogger<DeliContent> _log;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public DeliContent(
            IDeliConfig deliConfig,
            IDeliContentTypes deliContentTypes,
            IDeliCulture deliCulture,
            IUmbracoContextAccessor umbracoContextAccessor,
            ILogger<DeliContent> log)
        {
            _deliConfig = deliConfig;
            _deliContentTypes = deliContentTypes;
            _deliCulture = deliCulture;
            _umbracoContextAccessor = umbracoContextAccessor;
            _log = log;
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
            if (_umbracoContextAccessor.TryGetUmbracoContext(out var context))
            {
                var res = context.Content.GetAtRoot();
                return res;
            }

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

        public IEnumerable<IPublishedContent> PublishedChildren(int id)
        {
            return PublishedContent(id)?.Children ?? Enumerable.Empty<IPublishedContent>();
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
    }
}
