using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliProperties
    {
        object Value(IPublishedContent model, string alias, string culture);
        object Value(IPublishedProperty property, string culture);
        string Value(JObject jObject, string prop);
        IEnumerable<T> PublishedContentValue<T>(IPublishedProperty property, string culture) where T : IPublishedElement;
        bool AllEmptyProperties(IPublishedElement publishedContent, string? culture = null);
	}
}