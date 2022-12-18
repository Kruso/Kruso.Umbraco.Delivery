using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using static Umbraco.Cms.Core.PropertyEditors.ValueConverters.ColorPickerValueConverter;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.ColorPicker")]
    public class ColorPickerPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public ColorPickerPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var res = _deliProperties.Value(property, _modelFactory.Context.Culture);

            if (res is PickedColor color)
            {
                return new JsonNode()
                    .AddProp("color", ColorCode(color.Color))
                    .AddProp("label", color.Label);
            }
            else if (res == null)
            {
                return null;
            }
            else
            {
                return new JsonNode()
                    .AddProp("color", ColorCode(res.ToString()))
                    .AddProp("label", res.ToString());
            }
        }

        private string ColorCode(string color)
        {
            return !string.IsNullOrEmpty(color) ? $"#{color}" : null;
        }
    }
}