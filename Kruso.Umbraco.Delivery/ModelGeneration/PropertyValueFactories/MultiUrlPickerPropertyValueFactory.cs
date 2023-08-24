using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
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
        private readonly IModelFactory _modelFactory;

        public MultiUrlPickerPropertyValueFactory(IDeliContent deliContent, IDeliUrl deliUrl, IDeliDataTypes deliDataTypes, IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliContent = deliContent;
            _deliUrl = deliUrl;
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var links = GetLinks(_modelFactory.Context, property);
            foreach (var link in links.Where(x => x.Type == LinkType.Content && x.Udi != null))
            {
                link.Url = GetUrl(link, _modelFactory.Context.Culture, _modelFactory.Context.FallbackCulture); 
            }

            var res = links
                .Where(x => !string.IsNullOrEmpty(x.Url))
                .Select(x => new JsonNode()
                    .AddProp("url", x.Url)
                    .AddProp("label", x.Name)
                    .AddProp("target", x.Target)
                    .AddProp("linkType", x.Type.ToString()))
                .ToList();

            var configuration = _deliDataTypes.EditorConfiguration<MultiUrlPickerConfiguration>(property.PropertyType.DataType.Id);
            return configuration?.MaxNumber == 1
                ? (object)res.FirstOrDefault()
                : res;
        }

        private List<Link> GetLinks(IModelFactoryContext context, IPublishedProperty property)
        {
			var links = new List<Link>();

			var val = _deliProperties.Value(property, context.Culture);
			if (val == null)
                val = _deliProperties.Value(property, context.FallbackCulture);

            if (val == null)
                return links;

			if (val is Link)
				links.Add(val as Link);

			else if (val is IEnumerable<Link>)
				links.AddRange(val as IEnumerable<Link>);

            return links;
		}

        private string GetQueryString(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var idx = url.IndexOf("?");
                if (idx == -1)
                    idx = url.IndexOf("#");
                if (idx > -1)
                    return url.Substring(idx);
			}

            return null;
        }

        private string GetUrl(Link link, string culture, string fallbackCulture)
        {
			var publishedContent = _deliContent.PublishedContent(link.Udi);
			if (publishedContent != null)
			{
				var queryString = GetQueryString(link.Url);
                var url = _deliUrl.GetDeliveryUrl(publishedContent, culture);
                if (string.IsNullOrEmpty(url))
                    url = _deliUrl.GetDeliveryUrl(publishedContent, fallbackCulture); 
                
                url += queryString;

                return url;
			}

            return link.Url;
		}
    }
}