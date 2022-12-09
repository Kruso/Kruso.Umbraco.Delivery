using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.TrueFalse")]
    public class TrueFalsePropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;

        public TrueFalsePropertyValueFactory(IDeliProperties deliProperties)
        {
            _deliProperties = deliProperties;
        }

        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        {
            var val = _deliProperties.Value(property, context.Culture);

            bool value = false;
            if (val is string)
                value = (val as string) != "0";
            else if (val is int)
                value = ((int)val) > 0;
            else if (val is bool)
                value = (bool)val;

            return value;
        }
    }
}
