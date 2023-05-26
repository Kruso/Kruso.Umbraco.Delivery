using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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

            return new JsonNode()
                .AddProp("selected", GetSelectedValues(val, preValues))
                .AddProp("values", preValues);
        }

        private string[] GetSelectedValues(object val, IEnumerable<string> preValues)
        {
            List<string> selected = new();

            if (val != null)
            {
                if (val is string[])
                    selected.AddRange(val as string[]);
                else if (val is string)
                {
                    var valStr = val.ToString();
                    if (preValues.Contains(valStr))
                        selected.Add(valStr);
                    else if (valStr.IsJson())
                    {
                        var res = JArray.Parse(valStr).Select(x => x.Value<string>());
                        selected.AddRange(res);
                    }

                }
                   
            }

            return selected
                .Where(x => preValues.Contains(x))
                .ToArray();
        }
    }
}
