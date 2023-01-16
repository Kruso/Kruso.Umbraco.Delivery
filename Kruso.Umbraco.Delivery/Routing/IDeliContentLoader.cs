using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;

namespace Kruso.Umbraco.Delivery.Routing
{
    public interface IDeliContentLoader
    {
        IPublishedContent FindContentById(int id, string culture, bool preview = false);
        IPublishedContent FindContentByRoute(IPublishedRequestBuilder requestBuilder, string domainSeg, string requestSeg, bool preview = false);
    }
}