using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.RadioButtonList")]
    public class RadioButtonListPropertyValueFactory : CheckboxListPropertyValueFactory, IModelPropertyValueFactory
    {
        public RadioButtonListPropertyValueFactory(IDeliDataTypes deliDataTypes, IDeliProperties deliProperties, IModelFactory modelFactory)
            : base(deliDataTypes, deliProperties, modelFactory)
        {
        }

        public override object Create(IPublishedProperty property)
        {
            var res = base.Create(property) as JsonNode;
            var selected = res.Val<string[]>("selected")?.FirstOrDefault();

            res.AddProp("selected", selected);
            return res;
        }
    }
}
