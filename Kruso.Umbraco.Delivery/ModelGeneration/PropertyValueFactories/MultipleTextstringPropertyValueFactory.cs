using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [PropertyValueFactory("Umbraco.MultipleTextstring")]
    public class MultipleTextstringPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;

        public MultipleTextstringPropertyValueFactory(IDeliProperties deliProperties)
        {
            _deliProperties = deliProperties;
        }

        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        {
            return _deliProperties.Value(property, context.Culture) as string[];
        }
    }
}
