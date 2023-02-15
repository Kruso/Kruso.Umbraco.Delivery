using Kruso.Umbraco.Delivery.Security;
using System;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Routing
{
    public interface IDeliRequestAccessor
    {
        IUserIdentity Identity { get; }
        IDeliRequest Current { get; }

        IDeliRequest Finalize(IPublishedContent content, string culture, Uri callingUri = null);
        IDeliRequest Unfinalize();
    }
}
