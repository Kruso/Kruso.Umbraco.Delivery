using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Routing.Implementation;
using Kruso.Umbraco.Delivery.Security;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Routing
{
    public class XForwardedMiddleware : IMiddleware
    {
        private readonly string[] ExcludeRoutes = new string[]
        {
            "api/keepalive/ping",
            "umbraco",
            "media",
            "app_plugins"
        };

        private readonly string[] IncludeRoutes = new string[]
        {
            "umbraco/preview/",
            "umbraco/backoffice/umbracoapi/content/getbyid"
        };

        public const string DefaultForwardedHeader = "X-Forwarded-For";
        public const string HostHeader = "X-Forwarded-Host";
        public const string ProtoHeader = "X-Forwarded-Proto";
        public const string PrefixHeader = "X-Forwarded-Prefix";

        private readonly IDeliRequestModifier _deliRequestModifier;
        private readonly IDeliRequestAccessor _deliRequestAccessor;

        public XForwardedMiddleware(IDeliRequestModifier deliRequestModifier, IDeliRequestAccessor deliRequestAccessor)
        {
            _deliRequestModifier = deliRequestModifier;
            _deliRequestAccessor = deliRequestAccessor;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (_deliRequestModifier.ShouldModify(context.Request))
            {
                var originalUri = context.Request.AbsoluteUri();
                var callingHost = _deliRequestModifier.DetermineCallingHost(context.Request);
                if (callingHost != null)
                    _deliRequestModifier.Modify(context.Request, callingHost);

                var jwtToken = context.Request.GetJwtBearerToken();
                _deliRequestAccessor.Initialize(context.Request, originalUri, jwtToken);
            }

            await next(context);
        }
    }
}
