using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.Logging;
using System;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace Kruso.Umbraco.Delivery.Routing.Implementation
{
    internal class DeliContentLoader : IDeliContentLoader
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliContent _deliContent;
        private readonly IDeliCulture _deliCulture;
        private readonly ILogger<DeliContentLoader> _logger;


        public DeliContentLoader(
            IUmbracoContextAccessor umbracoContextAccessor,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContent deliContent,
            IDeliCulture deliCulture,
            ILogger<DeliContentLoader> logger)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _deliRequestAccessor = deliRequestAccessor;
            _deliContent = deliContent;
            _deliCulture = deliCulture;
            _logger = logger;
        }

        public IPublishedContent FindContentById(int id, string culture, bool preview = false)
        {
            _logger.LogDebug("Trying to load content {id}:{culture}. Preview={preview}", id, culture, preview);

            var content = preview
                ? _deliContent.UnpublishedContent(id)
                : _deliContent.PublishedContent(id);

            var res = IsValidContentPage(content, culture)
                ? content
                : null;

            if (res == null)
                _logger.LogDebug("No match for content {id}:{culture}. Preview={preview}", id, culture, preview);

            return res;
        }

        public IPublishedContent FindContentByRoute(IPublishedRequestBuilder requestBuilder, string domainSeg, string requestSeg, bool preview = false)
        {
            IPublishedContent res = null;

            var route = BuildRoute(requestBuilder, domainSeg, requestSeg);

            _logger.LogDebug("Trying to load content by route {route}. Preview={preview}", route, preview);

            if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                var content = umbracoContext.Content.GetByRoute(preview, route, culture: requestBuilder.Culture);

                res = IsValidContentPage(content, requestBuilder.Culture)
                    ? content
                    : null;
            }

            if (res == null)
                _logger.LogDebug("No match for content by route {route}. Preview={preview}", route, preview);

            return res;
        }

        private string BuildRoute(IPublishedRequestBuilder requestBuilder, string domainSeg, string requestSeg)
        {
            var absolutePathDecoded = domainSeg.Equals(requestSeg, StringComparison.InvariantCultureIgnoreCase)
                ? requestBuilder.AbsolutePathDecoded
                : $"/{domainSeg}{requestBuilder.AbsolutePathDecoded}";

            var route = requestBuilder.Domain.ContentId + DomainUtilities.PathRelativeToDomain(requestBuilder.Domain.Uri, absolutePathDecoded);
            return route;
        }

        private bool IsValidContentPage(IPublishedContent content, string culture)
        {
            return !string.IsNullOrEmpty(culture)
                && _deliContent.IsPage(content)
                && _deliCulture.IsPublishedInCulture(content, culture)
                && _deliRequestAccessor.Identity.HasAccess(content);
        }
    }
}
