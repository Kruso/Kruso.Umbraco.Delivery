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

        public void InitializeDeliRequest(HttpRequest request, Uri originalUri, string jwtToken)
        {
            var token = _deliSecurity.ValidateJwtPreviewToken(request, originalUri);
            var deliRequest = new DeliRequest(request, originalUri, token);

            _deliCache.AddToRequest(CacheKey, deliRequest);
        }

        public void FinalizeDeliRequest(IPublishedContent content, string culture, bool isPreviewPaneRequest = false)
        {
            var deliRequest = _deliCache.GetFromRequest<DeliRequest>(CacheKey);
            deliRequest?.Finalize(content, culture, isPreviewPaneRequest);
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
