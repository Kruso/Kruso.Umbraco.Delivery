using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.Slider")]
    public class SliderPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public SliderPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var value = _deliProperties.Value(property, _modelFactory.Context.Culture);
            if (value is decimal num)
                return num;

            if (value is Range<decimal> range)
                return new JsonNode()
                    .AddProp("type", "NumberRange")
                    .AddProp("maximum", range.Maximum)
                    .AddProp("minimum", range.Minimum);

            return null;
        }
    }
}
