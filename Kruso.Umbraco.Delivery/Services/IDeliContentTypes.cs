using Umbraco.Cms.Core.Models;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliContentTypes
    {
        IContentType ContentType(string alias);
    }
}