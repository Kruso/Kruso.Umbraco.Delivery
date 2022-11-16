using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery
{
    public interface IModelTemplate
    {
        JsonNode Create(IModelFactoryContext context, JsonNode props, IPublishedContent content);
    }
}
