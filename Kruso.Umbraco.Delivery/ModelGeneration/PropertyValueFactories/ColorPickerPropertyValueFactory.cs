using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Services;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [PropertyValueFactory("Umbraco.ColorPicker")]
    public class ColorPickerPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;

        public ColorPickerPropertyValueFactory(IDeliProperties deliProperties)
        {
            _deliProperties = deliProperties;
        }

        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        {
            var value = _deliProperties.Value(property, context.Culture)?.ToString();
            var json = value.ToJson();
            var color = json != null
                ? _deliProperties.Value(json.ToObject<JObject>(), "value")
                : value;

            return !string.IsNullOrEmpty(color) ? $"#{color}" : null;
        }
    }
}