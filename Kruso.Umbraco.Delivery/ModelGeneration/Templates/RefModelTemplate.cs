using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.Templates
{
    [ModelTemplate(TemplateType.Ref)]
    public class RefModelTemplate : IModelTemplate
    {
        public virtual JsonNode Create(IModelFactoryContext context, JsonNode props, IPublishedContent content)
        {
            var nodeRef = new JsonNode
            {
                Id = content.Key,
                PageId = context.Page.Key,
                Culture = context.Culture,
                Type = DeliConstants.RefTypeAlias,
                CompositionTypes = content.ContentType?.CompositionAliases?.ToArray()
            };

            nodeRef.AddProp(DeliConstants.RefTypeProperty, content.ContentType.Alias.Capitalize());

            return nodeRef;
        }
    }
}
