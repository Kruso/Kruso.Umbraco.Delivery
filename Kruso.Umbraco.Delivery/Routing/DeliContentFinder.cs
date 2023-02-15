using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace Kruso.Umbraco.Delivery.Routing
{
    public class DeliContentFinder : IContentFinder
    {
        private readonly ILogger<ContentFinderByUrl> _logger;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliDomain _deliDomain;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliContent _deliContent;

        public DeliContentFinder(
            ILogger<ContentFinderByUrl> logger,
            IUmbracoContextAccessor umbracoContextAccessor,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliDomain deliDomain,
            IDeliCulture deliCulture,
            IDeliContent deliContent)
        {
            _logger = logger;
            _umbracoContextAccessor = umbracoContextAccessor;
            _deliRequestAccessor = deliRequestAccessor;
            _deliDomain = deliDomain;
            _deliCulture = deliCulture;
            _deliContent = deliContent;
        }

        public Task<bool> TryFindContent(IPublishedRequestBuilder frequest)
        {
            if (!_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                return Task.FromResult(false);
            }

            var domain = _deliDomain.GetDomainByRequest(frequest.Uri, true);
            if (domain != null)
            {
                frequest.SetDomain(domain);
                frequest.SetCulture(domain.Culture);

                var dseg = domain.Uri.Segments.MatchableSegment().Trim('/');
                var rseg = frequest.AbsolutePathDecoded.Segments().First().Trim('/');

                var absolutePathDecoded = dseg.Equals(rseg, StringComparison.InvariantCultureIgnoreCase)
                    ? frequest.AbsolutePathDecoded
                    : $"/{dseg}{frequest.AbsolutePathDecoded}";

                var route = frequest.Domain.ContentId + DomainUtilities.PathRelativeToDomain(frequest.Domain.Uri, absolutePathDecoded);
                var content = FindContent(frequest, route);
                if (content != null)
                {
                    frequest.SetPublishedContent(content);
                    _deliRequestAccessor.FinalizeForContent(content, frequest.Domain.Culture);

                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        protected IPublishedContent FindContent(IPublishedRequestBuilder docreq, string route)
        {
             if (!_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                return null;
            }

            if (docreq == null)
            {
                throw new System.ArgumentNullException(nameof(docreq));
            }

            _logger.LogDebug("Test route {Route}", route);

            var content = umbracoContext.Content.GetByRoute(umbracoContext.InPreviewMode, route, culture: docreq.Culture);
            if (_deliContent.IsPage(content) &&  _deliCulture.IsPublishedInCulture(content, docreq.Domain.Culture))
            {
                _logger.LogDebug("Got content, id={NodeId}", content.Id);
                return content;
            }
            else
            {
                _logger.LogDebug("No match.");
                return null;
            }
        }
    }
}
