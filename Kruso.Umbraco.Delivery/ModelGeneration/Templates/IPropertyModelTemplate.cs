using Kruso.Umbraco.Delivery.Json;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.Templates
{
    public interface IPropertyModelTemplate
    {
        object Create(IPublishedElement content, IPublishedProperty property, object val);
    }
}