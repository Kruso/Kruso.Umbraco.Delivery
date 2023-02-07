using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.ModelGeneration.Templates
{
    [ModelTemplate(TemplateType.Page)]
    public class PageModelTemplate : IModelTemplate
    {
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliPages _deliPages;
        private readonly IDeliUrl _deliUrl;

        public PageModelTemplate(IDeliCulture deliCulture, IDeliPages deliPages, IDeliUrl deliUrl)
        {
            _deliCulture = deliCulture;
            _deliPages = deliPages;
            _deliUrl = deliUrl;
        }

        public virtual JsonNode Create(IModelFactoryContext context, JsonNode props, IPublishedContent content)
        {
            var page = new JsonNode
            {
                Id = content.Key,
                PageId = content.Key,
                ParentPageId = content.Parent?.Key,
                Type = content.ContentType.Alias,
                Culture = context.Culture,
                CompositionTypes = content.ContentType.CompositionAliases?.ToArray()
            };

            page
                .AddProp("name", content.Name)
                .AddProp("urls", CreateUrls(context, content))
                .AddProp("sortOrder", content.SortOrder)
                .CopyAllProps(props);

            return page;
        }

        private JsonNode CreateUrls(IModelFactoryContext context, IPublishedContent content)
        {
            var url = _deliUrl.GetDeliveryUrl(content, context.Culture);
            var slug = content.Parent != null
                ? content.UrlSegment(context.Culture)
                : url.Trim('/');

            var node = new JsonNode()
                .AddProp("slug", slug)
                .AddProp("url", url)
                .AddProp("canonicalUrl", _deliUrl.GetAbsoluteDeliveryUrl(content, context.Culture));

            var startPage = _deliPages.StartPage(content, context.Culture);
            var alts = new List<JsonNode>();

            var altCultures = _deliCulture.GetCultures(startPage)
                .Where(x => x != context.Culture);

            foreach (var altCulture in altCultures)
            {
                var isPublishedInCulture = _deliCulture.IsPublishedInCulture(content, altCulture);
                var selectedContent = isPublishedInCulture
                    ? content
                    : startPage;

                var altUrl = _deliUrl.GetDeliveryUrl(selectedContent, altCulture);
                if (!string.IsNullOrEmpty(altUrl))
                {
                    var altSlug = selectedContent.Parent != null
                        ? selectedContent.UrlSegment(altCulture)
                        : altUrl.Trim('/');

                    var alt = new JsonNode
                    {
                        Culture = altCulture
                    };

                    alts.Add(alt
                        .AddProp("slug", altSlug)
                        .AddProp("url", altUrl)
                        .AddProp("canonicalUrl", _deliUrl.GetAbsoluteDeliveryUrl(selectedContent, altCulture))
                        .AddProp("exists", selectedContent == content));
                }
            }

            node.AddProp("alts", alts);

            return node;
        }
    }
}
