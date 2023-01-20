using Kruso.Umbraco.Delivery.Extensions;
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

        public void InitializeIndexing(IPublishedContent content, string culture)
        {
            var deliRequest = new DeliRequest();
            deliRequest.Finalize(content, culture);

            _deliCache.ReplaceOnRequest(CacheKey, deliRequest);
        }

        public void Initialize(HttpRequest request, Uri originalUri, string jwtToken)
        {
            var deliRequest = new DeliRequest(request, originalUri);
            deliRequest.Token = _deliSecurity.ValidateJwtPreviewToken(jwtToken, deliRequest.OriginalUri.Authority, deliRequest.CallingUri.Authority);

            _deliCache.AddToRequest(CacheKey, deliRequest);
        }

        public void Finalize(IPublishedContent content, string culture)
        {
            var deliRequest = _deliCache.GetFromRequest<DeliRequest>(CacheKey);
            deliRequest?.Finalize(content, culture);
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
