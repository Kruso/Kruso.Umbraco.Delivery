using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Security;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Routing
{
    public class XForwardedMiddleware : IMiddleware
    {
        private readonly string[] ExcludeRoutes = new string[]
        {
            "umbraco",
            "media",
            "app_plugins"
        };

        public const string DefaultForwardedHeader = "X-Forwarded-For";
        public const string HostHeader = "X-Forwarded-Host";
        public const string ProtoHeader = "X-Forwarded-Proto";
        public const string PrefixHeader = "X-Forwarded-Prefix";

        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliConfig _deliConfig;
        private readonly ILogger<XForwardedMiddleware> _logger;

        public XForwardedMiddleware(IDeliRequestAccessor deliRequestAccessor, IDeliConfig deliConfig, ILogger<XForwardedMiddleware> logger)
        {
            _deliRequestAccessor = deliRequestAccessor;
            _deliConfig = deliConfig;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var originalUri = context.Request.AbsoluteUri();

            if (ShouldHandleRequest(context.Request))
            {
                var callingAuthority = GetCallingAuthority(context);
                if (callingAuthority != null)
                {
                    context.Request.Host = new HostString(callingAuthority.Authority);
                    context.Request.Scheme = callingAuthority.Scheme;
                    context.Request.PathBase = callingAuthority.CleanPath();
                }

                var jwtToken = context.Request.GetJwtBearerToken();

                _deliRequestAccessor.InitializeDeliRequest(context.Request, originalUri, jwtToken);
            }

            await next(context);
        }

        private bool ShouldHandleRequest(HttpRequest request)
        {
            if (request == null)
                return false;

            var requestPath = request.AbsoluteUri().CleanPath();
            return !ExcludeRoutes.Any(r => requestPath.StartsWith(r));
        }

        private Uri GetCallingAuthority(HttpContext context)
        {
            var config = _deliConfig.Get();

            var callingAuthority = 
                GetCallingAuthorityFromHeader(context, config.ForwardedHeader)
                ?? GetCallingAuthorityFromHeader(context, DefaultForwardedHeader);

            if (string.IsNullOrEmpty(callingAuthority))
            {
                if (context.Request.Headers.TryGetValue(ProtoHeader, out var scheme)
                    && context.Request.Headers.TryGetValue(HostHeader, out var host)
                    && context.Request.Headers.TryGetValue(PrefixHeader, out var path))
                { 
                   callingAuthority = $"{scheme.Last().Trim(' ')}://{host.Last().Trim(' ')}/{path.Last().Trim('/').Trim(' ')}".ToLower();
                }
            }

            if (string.IsNullOrEmpty(callingAuthority) && !_deliConfig.IsMultiSite())
            {
                callingAuthority = config.FrontendHost;
            }

            Uri.TryCreate(callingAuthority, UriKind.Absolute, out var baseUri);

            return baseUri;
        }

        private string GetCallingAuthorityFromHeader(HttpContext context, string forwardedHeader)
        {
            if (string.IsNullOrEmpty(forwardedHeader))
                return null;

            context.Request.Headers.TryGetValue(forwardedHeader, out var callingAuthority);
            return callingAuthority;
        }
    }
}
