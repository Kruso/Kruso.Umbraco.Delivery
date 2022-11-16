using Kruso.Umbraco.Delivery.ModelGeneration;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery
{
    public interface IModelPropertyValueFactory
    {
        object Create(IModelFactoryContext context, IPublishedProperty property);
    }
}
