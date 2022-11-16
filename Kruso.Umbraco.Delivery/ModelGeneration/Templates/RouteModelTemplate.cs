﻿using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.ModelGeneration.Templates
{
    [ModelTemplate(TemplateType.Route)]
    public class RouteModelTemplate : IModelTemplate
    {
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliContent _deliContent;
        private readonly IDeliUrl _deliUrl;

        public RouteModelTemplate(IDeliCulture deliCulture, IDeliContent deliContent, IDeliUrl deliUrl)
        {
            _deliCulture = deliCulture;
            _deliContent = deliContent;
            _deliUrl = deliUrl;
        }

        public virtual JsonNode Create(IModelFactoryContext context, JsonNode props, IPublishedContent content)
        {
            var node = new JsonNode(content.Key, context.Culture, content.ContentType.Alias)
            {
                ParentPageId = _deliCulture.IsPublishedInCulture(content, context.Culture)
                    ? content.Parent?.Key 
                    : null
            };

            node
                .AddProp("name", content.Name)
                .AddProp("isRoot", content.Root().Key == content.Key)
                .AddProp("route", _deliUrl.GetDeliveryUrl(content, context.Culture))
                .AddProp("mimeType", _deliContent.IsJsonTemplate(content) ? "application/json" : "text/html")
                .AddProp("visible", content.IsVisible())
                .AddProp("children", content.Children
                    .Where(x => _deliContent.IsPage(x) !&& _deliCulture.IsPublishedInCulture(x, context.Culture))
                    .Select(x => x.Key)
                    .ToList());

            return node;
        }
    }
}