using HtmlAgilityPack;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.TinyMCE")]
    public class TinyMCEPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public TinyMCEPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var val = _deliProperties.Value(property, _modelFactory.Context.Culture);
            var value = (val is IHtmlEncodedString)
                ? (IHtmlEncodedString)val
                : new HtmlEncodedString(val?.ToString());

            return CleanHtml(value);
        }

        private string CleanHtml(IHtmlEncodedString html)
        {
            var htmlString = html?.ToHtmlString();
            if (!string.IsNullOrEmpty(htmlString))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlString);

                var links = doc.DocumentNode?.SelectNodes("//a[@href]");
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        // Get the value of the HREF attribute
                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        if (hrefValue.StartsWith("mailto:") || hrefValue.StartsWith("tel:"))
                        {
                            hrefValue = hrefValue.TrimEnd('/');
                        }

                        link.SetAttributeValue("href", hrefValue);
                    }

                    return doc.DocumentNode?.WriteTo();
                }
            }

            return htmlString;
        }
    }
}
