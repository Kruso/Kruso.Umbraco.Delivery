using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory(new[] { "Umbraco.DropDown.Flexible", "Umbraco.DropDown" })]
    public class DropDownPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public DropDownPropertyValueFactory(IDeliDataTypes deliDataTypes, IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var preValues = _deliDataTypes.PreValues(property.PropertyType.DataType.Id);
            var selected = (_deliProperties.Value(property, _modelFactory.Context.Culture) ?? string.Empty).ToString()
                .Replace("[", "")
                .Replace("]", "")
                .Replace("\"", "")
                .Split(',')
                .Select(x => x.Trim());

            selected = selected
                .Where(x => preValues.Contains(x))
                .ToArray();

            return new JsonNode()
                .AddProp("selected", selected?.FirstOrDefault())
                .AddProp("values", preValues);
        }
    }
}
