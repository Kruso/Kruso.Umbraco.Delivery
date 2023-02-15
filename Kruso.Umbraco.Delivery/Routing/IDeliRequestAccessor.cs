using Kruso.Umbraco.Delivery.Security;
using Microsoft.AspNetCore.Http;
using System;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Routing
{
    public interface IDeliRequestAccessor
    {
        IUserIdentity Identity { get; }
        IDeliRequest Current { get; }

        void InitializeIndexing(IPublishedContent content, string culture, Uri callingUri);
        void Initialize(HttpRequest request, Uri originalUri, string jwtToken);
        IDeliRequest FinalizeForContent(IPublishedContent content, string culture);
        IDeliRequest FinalizeForPreview(IPublishedContent content, string culture, Uri callingUri);
        IDeliRequest FinalizeForSearch(string culture);
        IDeliRequest Unfinalize();
    }
}
