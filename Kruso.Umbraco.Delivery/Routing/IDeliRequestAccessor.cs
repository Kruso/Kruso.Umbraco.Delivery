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
        void InitializeDeliRequest(HttpRequest request, Uri originalUri, string jwtToken);
        void FinalizeDeliRequest(IPublishedContent content, string culture, bool isPreviewPaneRequest = false);
    }
}
