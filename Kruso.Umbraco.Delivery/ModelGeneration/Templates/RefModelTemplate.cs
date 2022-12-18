using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.Templates
{
    [ModelTemplate(TemplateType.Ref)]
    public class RefModelTemplate : IModelTemplate
    {
        public virtual JsonNode Create(IModelFactoryContext2 context, JsonNode props, IPublishedContent content)
        {
            var page = new JsonNode(content.Key, context.Page.Key, context.Culture, "Ref")
                .AddProp("refType", content.ContentType.Alias.Capitalize());

            return page;
        }
    }
}
