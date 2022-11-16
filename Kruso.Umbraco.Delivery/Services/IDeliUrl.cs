using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliUrl
    {
        string GetAbsoluteDeliveryUrl(string relativePath, string culture);
        string GetAbsoluteDeliveryUrl(IPublishedContent content, string culture);
        string GetDeliveryUrl(IPublishedContent content, string culture);
        string GetDeliveryUrl(string path, string culture);
        IEnumerable<UrlInfo> GetAlternativeDeliveryUrls(IPublishedContent content, string culture);
        string RemoveDomainPrefixFromPath(string path);
    }
}