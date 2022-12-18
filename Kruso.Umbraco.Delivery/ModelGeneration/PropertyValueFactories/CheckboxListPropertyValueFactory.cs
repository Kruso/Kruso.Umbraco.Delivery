using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using Newtonsoft.Json.Linq;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.CheckBoxList")]
    public class CheckboxListPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public CheckboxListPropertyValueFactory(IDeliDataTypes deliDataTypes, IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var preValues = _deliDataTypes.PreValues(property.PropertyType.DataType.Id);
            var val = _deliProperties.Value(property, _modelFactory.Context.Culture);
            string[] selected = null;
            if (val is string[])
            {
                selected = val as string[];
            } 
            else
            {
                var json = JArray.Parse(val.ToString());
                if (json != null)
                {
                    selected = json.Select(x => x.Value<string>()).ToArray();
                }
            }

            return new JsonNode()
                .AddProp("selected", selected.Where(x => preValues.Contains(x)).ToArray())
                .AddProp("values", preValues);
        }
    }
}
