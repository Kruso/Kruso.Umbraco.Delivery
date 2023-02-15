using Kruso.Umbraco.Delivery.Security;
using System;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Routing
{
    public interface IDeliRequestAccessor
    {
        IUserIdentity Identity { get; }
        IDeliRequest Current { get; }

        IDeliRequest FinalizeForContent(IPublishedContent content, string culture);
        IDeliRequest FinalizeForPreview(IPublishedContent content, string culture, Uri callingUri);
        IDeliRequest Unfinalize();
    }
}
