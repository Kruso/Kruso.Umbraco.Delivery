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
        private readonly IDeliContent _deliContent;
        private readonly IDeliUrl _deliUrl;
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IDeliProperties _deliProperties;

        public MultiUrlPickerPropertyValueFactory(IDeliContent deliContent, IDeliUrl deliUrl, IDeliDataTypes deliDataTypes, IDeliProperties deliProperties)
        {
            _deliContent = deliContent;
            _deliUrl = deliUrl;
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
        }

        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        {
            var links = new List<Link>();

            var val = _deliProperties.Value(property, context.Culture);
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
                    .AddProp("url", x.Type == LinkType.External ? x.Url : _deliUrl.GetDeliveryUrl(x.Url, context.Culture))
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