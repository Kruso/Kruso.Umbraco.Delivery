﻿using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Routing.Implementation;
using Kruso.Umbraco.Delivery.Security;
using Kruso.Umbraco.Delivery.Services;
using Kruso.Umbraco.Delivery.Services.Implementation;
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
    public class DeliRequestMiddleware : IMiddleware
    {
        private readonly UmbracoRequestPaths _umbracoRequestPaths;
        private readonly IDeliConfig _deliConfig;
        private readonly DeliRequestAccessor _deliRequestAccessor;
        private readonly ILogger<DeliRequestMiddleware> _logger;

        public const string HostHeader = "X-Forwarded-Host";
        public const string ProtoHeader = "X-Forwarded-Proto";
        public const string PrefixHeader = "X-Forwarded-Prefix";

        private readonly string[] ExcludeRoutes = new string[]
        {
            "api/keepalive/ping",
            "media",
            "app_plugins"
        };

        private readonly string[] IncludeRoutes = new string[]
        {
            "umbraco/preview/",
            "umbraco/backoffice/umbracoapi/content/getbyid"
        };

        private readonly string[] ExcludeExtensions = new string[]
        {
            ".js",
            ".css",
            ".jpg",
            ".jpeg",
            ".png"
        };

        public DeliRequestMiddleware(UmbracoRequestPaths umbracoRequestPaths, IDeliConfig deliConfig, IDeliRequestAccessor deliRequestAccessor, ILogger<DeliRequestMiddleware> logger)
        {
            _umbracoRequestPaths = umbracoRequestPaths;
            _deliConfig = deliConfig;
            _deliRequestAccessor = deliRequestAccessor as DeliRequestAccessor;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (ShouldInitializeDeliRequest(context.Request))
            {
                var callingHost = DetermineCallingHost(context.Request);
                await _deliRequestAccessor.Initialize(context, next, callingHost);
            }
            else
            {
                await next(context);
            }
        }

        public Uri DetermineCallingHost(HttpRequest request)
        {
            var config = _deliConfig.Get();

            var callingHost = string.IsNullOrEmpty(config.ForwardedHeader)
                ? GetCallingHostFromHeader(request, config.ForwardedHeader)
                : null;

            if (string.IsNullOrEmpty(callingHost))
            {
                if (request.Headers.TryGetValue(ProtoHeader, out var scheme)
                    && request.Headers.TryGetValue(HostHeader, out var host)
                    && request.Headers.TryGetValue(PrefixHeader, out var path))
                {
                    callingHost = $"{scheme.Last().Trim(' ')}://{host.Last().Trim(' ')}/{path.Last().Trim('/').Trim(' ')}".ToLower();
                }
            }

            if (string.IsNullOrEmpty(callingHost))
            {
                _logger.LogInformation($"Could not determine calling host from {request.AbsoluteUri()} header.");

                //If this is a request from the Umbraco backend then we don't need to determine the calling host
                // even if it is a preview request. We will deal with it later.
                if (IsBackendRequest(request))
                    return null;

                if (_deliConfig.IsMultiSite())
                {
                    //Multi-site front end requests are required to include a header value specifying the origin of the client request.
                    _logger.LogWarning($"Could not determine calling host from {request.AbsoluteUri()} in multi-site solution.");
                    return null;
                }
                else
                {
                    //Single-site installations are not required to include a header value specifying the origin of the client request
                    // so try get the origin from settings.
                    callingHost = config.FrontendHost;
                    if (string.IsNullOrEmpty(callingHost))
                    {
                        _logger.LogWarning($"Could not determine calling host from {request.AbsoluteUri()} in single-site solution.");
                        return null;
                    }
                }
            }

            //We have determined the callingHost, let's hope it's a valid absolute url
            Uri.TryCreate(callingHost, UriKind.Absolute, out var baseUri);
            if (baseUri == null)
                _logger.LogWarning($"Invalid host {callingHost} was found. Cannot use this.");

            return baseUri.HostUri();
        }

        private string GetCallingHostFromHeader(HttpRequest request, string forwardedHeader)
        {
            if (string.IsNullOrEmpty(forwardedHeader))
                return null;

            request.Headers.TryGetValue(forwardedHeader, out var callingAuthority);

            return callingAuthority;
        }

        private bool ShouldInitializeDeliRequest(HttpRequest request)
        {
            if (request == null)
                return false;

            var path = request.AbsoluteUri().CleanPath();

            if (ExcludeExtensions.Any(ext => path.EndsWith(ext)))
                return false;

            if (IncludeRoutes.Any(r => path.InvariantStartsWith(r)))
                return true;

            if (IsBackendRequest(request))
                return false;

            if (ExcludeRoutes.Any(r => path.InvariantStartsWith(r)))
                return false;

            return true;
        }

        private bool IsBackendRequest(HttpRequest request)
        {
            var path = request.AbsoluteUri().CleanPath();
            return _umbracoRequestPaths.IsBackOfficeRequest(path);
        }
    }
}