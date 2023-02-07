using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliMedia
    {
        IPublishedContent GetMedia(string udi);
    }
}