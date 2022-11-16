using Umbraco.Cms.Core.Routing;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class PublishedRequestExtensions
    {
        public static string CleanPath(this IPublishedRequest request)
        {
            if (request == null)
                return null;

            return request.Uri.AbsolutePath.CleanPath();
        }
    }
}
