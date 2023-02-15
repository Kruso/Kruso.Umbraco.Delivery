using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Security;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Routing.Implementation
{
    public class DeliRequestAccessor : IDeliRequestAccessor
    {
        private const string CacheKey = "delivery-umb-request";

        public const string CacheControlHeader = "Cache-Control";
        public const string ETagHeader = "ETag";

        private readonly DeliveryConfig _deliveryConfig;
        private readonly IDeliCache _deliCache;
        private readonly IDeliSecurity _deliSecurity;
        private readonly IServiceProvider _serviceProvider;

        public IUserIdentity Identity => GetUserIdentity();
        public IDeliRequest Current => _deliCache.GetFromRequest<IDeliRequest>(CacheKey);

        public DeliRequestAccessor(
            DeliveryConfig deliveryConfig,
            IDeliCache deliCache,
            IDeliSecurity deliSecurity,
            IServiceProvider serviceProvider)
        {
            _deliveryConfig = deliveryConfig;
            _deliCache = deliCache;
            _deliSecurity = deliSecurity;
            _serviceProvider = serviceProvider;
        }

        internal async Task Initialize(HttpContext context, RequestDelegate next, Uri callingHost)
        {
            var originalUri = context.Request.AbsoluteUri();
            var jwtToken = context.Request.GetJwtBearerToken();

            if (callingHost != null) 
            {
                context.Request.Host = new HostString(callingHost.Authority);
                context.Request.Scheme = callingHost.Scheme;
                context.Request.PathBase = callingHost.CleanPath();
            }

            var deliRequest = new DeliRequest(context.Request, originalUri, jwtToken);
            _deliCache.AddToRequest(CacheKey, deliRequest);

            await WithResponseContent(context, next, (content) =>
            {
                var cacheControl = _deliveryConfig.GetConfigValues(callingHost ?? originalUri)?.GetCacheControl(context.Response.ContentType);
                if (!string.IsNullOrEmpty(cacheControl) && !string.IsNullOrEmpty(content))
                {
                    context.Response.Headers.Add(CacheControlHeader, cacheControl);

                    var etag = content.ToHashString();

                    if (context.Request.Headers.TryGetValue(ETagHeader, out var etagReq) && etag.Equals(etagReq, StringComparison.InvariantCultureIgnoreCase))
                        context.Response.StatusCode = (int)HttpStatusCode.NotModified;

                    context.Response.Headers.Add("ETag", etag);
                }
            });
        }

        internal void InitializeIndexing(IPublishedContent content, string culture, Uri callingUri)
        {
            var deliRequest = new DeliRequest();
            deliRequest.Finalize(content, culture, callingUri);

            _deliCache.ReplaceOnRequest(CacheKey, deliRequest);
        }

        public IDeliRequest FinalizeForContent(IPublishedContent content, string culture)
        {
            var deliRequest = _deliCache.GetFromRequest<DeliRequest>(CacheKey);
            if (deliRequest != null)
            {
                deliRequest.Finalize(content, culture);

                if (!string.IsNullOrEmpty(deliRequest.JwtToken))
                    deliRequest.Token = _deliSecurity.ValidateJwtPreviewToken(deliRequest.JwtToken, deliRequest.OriginalUri.Authority, deliRequest.CallingUri.Authority);
            }

            return deliRequest;
        }

        public IDeliRequest FinalizeForPreview(IPublishedContent content, string culture, Uri callingUri)
        {
            var deliRequest = _deliCache.GetFromRequest<DeliRequest>(CacheKey);
            if (deliRequest != null)
            {
                deliRequest.Finalize(content, culture, callingUri);

                if (!string.IsNullOrEmpty(deliRequest.JwtToken))
                    deliRequest.Token = _deliSecurity.ValidateJwtPreviewToken(deliRequest.JwtToken, deliRequest.OriginalUri.Authority, deliRequest.CallingUri.Authority);
            }

            return deliRequest;
        }

        public IDeliRequest Unfinalize()
        {
            var deliRequest = _deliCache.GetFromRequest<DeliRequest>(CacheKey);
            deliRequest?.UnFinalize();

            return deliRequest;
        }

        private IUserIdentity GetUserIdentity()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetService<IUserIdentity>();
            }
        }

        private async Task WithResponseContent(HttpContext context, RequestDelegate next, Action<string> action)
        {
            if (action == null)
                await next(context);

            Stream originalBody = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await next(context);

                    memStream.Position = 0;
                    string content = new StreamReader(memStream).ReadToEnd();

                    action(content);

                    if (context.Response.StatusCode != (int)HttpStatusCode.NotModified)
                    {
                        memStream.Position = 0;
                        await memStream.CopyToAsync(originalBody);
                    }
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}
