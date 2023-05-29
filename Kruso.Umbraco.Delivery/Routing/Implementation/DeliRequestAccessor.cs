using Kruso.Umbraco.Delivery.Security;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Routing.Implementation
{
    public class DeliRequestAccessor : IDeliRequestAccessor
    {
        private const string CacheKey = "delivery-umb-request";

        private readonly IDeliCache _deliCache;
        private readonly IDeliSecurity _deliSecurity;
        private readonly IServiceProvider _serviceProvider;

        public IUserIdentity Identity => GetUserIdentity();
        public IDeliRequest Current => _deliCache.GetFromRequest<IDeliRequest>(CacheKey);

        public DeliRequestAccessor(
            IDeliCache deliCache,
            IDeliSecurity deliSecurity,
            IServiceProvider serviceProvider)
        {
            _deliCache = deliCache;
            _deliSecurity = deliSecurity;
            _serviceProvider = serviceProvider;
        }

        internal void Initialize(HttpRequest request, Uri originalUri)
        {
            var deliRequest = new DeliRequest(request, originalUri);
            _deliCache.AddToRequest(CacheKey, deliRequest);
        }

        internal void InitializeIndexing(IPublishedContent content, string culture, Uri callingUri)
        {
            var deliRequest = new DeliRequest();
            deliRequest.Finalize(content, culture, callingUri);

            _deliCache.ReplaceOnRequest(CacheKey, deliRequest);
        }

        public IDeliRequest Finalize(string culture, Uri callingUri = null)
        {
            var deliRequest = _deliCache.GetFromRequest<DeliRequest>(CacheKey);
            if (deliRequest != null)
            {
                deliRequest.Finalize(null, culture, callingUri);

                if (!string.IsNullOrEmpty(deliRequest.JwtToken))
                    deliRequest.Token = _deliSecurity.ValidateJwtPreviewToken(deliRequest.JwtToken, deliRequest.OriginalUri.Authority, deliRequest.CallingUri.Authority);
            }

            return deliRequest;
        }

        public IDeliRequest Finalize(IPublishedContent content, string culture, Uri callingUri = null)
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
    }
}
