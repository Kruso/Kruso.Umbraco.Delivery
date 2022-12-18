using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.TrueFalse")]
    public class TrueFalsePropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public TrueFalsePropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var val = _deliProperties.Value(property, _modelFactory.Context.Culture);

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
