using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System;
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
        private readonly IDeliTemplates _deliTemplates;
        private readonly IDeliUrl _deliUrl;

        public RouteModelTemplate(IDeliCulture deliCulture, IDeliContent deliContent, IDeliUrl deliUrl, IDeliTemplates deliTemplates)
        {
            _deliCulture = deliCulture;
            _deliContent = deliContent;
            _deliUrl = deliUrl;
            _deliTemplates = deliTemplates;
        }

        public virtual JsonNode Create(IModelFactoryContext context, JsonNode props, IPublishedContent content)
        {
            var route = new JsonNode
            {
                Id = content.Key,
                ParentPageId = GetParentPageId(content, context.Culture),
                Type = content.ContentType.Alias,
                Culture = context.Culture,
            };

            route
                .AddProp("name", content.Name)
                .AddProp("isRoot", content.Root().Key == content.Key)
                .AddProp("route", _deliUrl.GetDeliveryUrl(content, context.Culture))
                .AddProp("mimeType", _deliTemplates.IsJsonTemplate(content) ? "application/json" : "text/html")
                .AddProp("visible", content.IsVisible())
                .AddProp("children", content.Children
                    .Where(x => _deliContent.IsPage(x) !&& _deliCulture.IsPublishedInCulture(x, context.Culture))
                    .Select(x => x.Key)
                    .ToList());

            return route;
        }

        private Guid? GetParentPageId(IPublishedContent content, string culture)
        {
            return _deliCulture.IsPublishedInCulture(content.Parent, culture) 
                ? content.Parent.Key 
                : null;
        }
    }
}
