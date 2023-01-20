using Kruso.Umbraco.Delivery.Extensions;
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
            if (ShouldHandleRequest(context.Request))
            {
                var originalUri = context.Request.AbsoluteUri();
                var callingAuthority = GetCallingAuthority(context);
                if (callingAuthority != null)
                {
                    context.Request.Host = new HostString(callingAuthority.Authority);
                    context.Request.Scheme = callingAuthority.Scheme;
                    context.Request.PathBase = callingAuthority.CleanPath();
                }

                var jwtToken = context.Request.GetJwtBearerToken();
                _deliRequestAccessor.Initialize(context.Request, originalUri, jwtToken);
            }

            await next(context);
        }

        private bool ShouldHandleRequest(HttpRequest request)
        {
            if (request == null)
                return false;

            var path = request.AbsoluteUri().CleanPath();

            return !ExcludeRoutes.Any(r => path.InvariantStartsWith(r))
                || IncludeRoutes.Any(r => path.InvariantStartsWith(r));
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

            _logger.LogInformation($"No calling authority found in header for {context.Request.AbsoluteUri()}. Trying settings...");

            if (string.IsNullOrEmpty(callingAuthority) && !_deliConfig.IsMultiSite())
            {
                callingAuthority = config.FrontendHost;

            }

            Uri.TryCreate(callingAuthority, UriKind.Absolute, out var baseUri);

            if (baseUri == null)
                _logger.LogInformation($"No calling authority found in settings for {context.Request.AbsoluteUri()}.");

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
