using Microsoft.AspNetCore.Http;
using System;

namespace Kruso.Umbraco.Delivery.Routing
{
    public interface IDeliRequestModifier
    {
        bool IsBackendRequest(HttpRequest request);
        bool ShouldModify(HttpRequest request);
        Uri DetermineCallingHost(HttpRequest request);
        void Modify(HttpRequest request, Uri callingAuthority);
    }
}