using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.MultiUrlPicker")]
    public class MultiUrlPickerPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliUrl _deliUrl;
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public MultiUrlPickerPropertyValueFactory(IDeliUrl deliUrl, IDeliDataTypes deliDataTypes, IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliUrl = deliUrl;
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var links = new List<Link>();

            var val = _deliProperties.Value(property, _modelFactory.Context.Culture);
            if (val == null)
                return null;

            if (val is Link)
            {
                links.Add(val as Link);
            }
            else if (val is IEnumerable<Link>)
            {
                links.AddRange(val as IEnumerable<Link>);
            }

            var res = links
                .Select(x => new JsonNode()
                    .AddProp("url", x.Type == LinkType.External ? x.Url : _deliUrl.GetDeliveryUrl(x.Url))
                    .AddProp("label", x.Name)
                    .AddProp("target", x.Target)
                    .AddProp("linkType", x.Type.ToString()))
                .ToList();

            var configuration = _deliDataTypes.EditorConfiguration<MultiUrlPickerConfiguration>(property.PropertyType.DataType.Id);
            return configuration?.MaxNumber == 1
                ? (object)res.FirstOrDefault()
                : res;
        }
    }
}