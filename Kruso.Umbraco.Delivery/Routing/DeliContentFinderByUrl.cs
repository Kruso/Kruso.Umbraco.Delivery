using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Routing;

namespace Kruso.Umbraco.Delivery.Routing
{
    public class DeliContentFinderByUrl : IContentFinder
    {
        private readonly IDeliContentLoader _deliContentLoader;
        private readonly ILogger<ContentFinderByUrl> _logger;

        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliDomain _deliDomain;

        public DeliContentFinderByUrl(
            IDeliContentLoader deliContentLoader,
            ILogger<ContentFinderByUrl> logger,

            IDeliRequestAccessor deliRequestAccessor,
            IDeliDomain deliDomain)
        {
            _deliContentLoader = deliContentLoader;
            _logger = logger;

            _deliRequestAccessor = deliRequestAccessor;
            _deliDomain = deliDomain;
        }

        public Task<bool> TryFindContent(IPublishedRequestBuilder frequest)
        {
            var domain = _deliDomain.GetDomainByRequest(frequest.Uri, true);
            if (domain != null)
            {
                frequest.SetDomain(domain);
                frequest.SetCulture(domain.Culture);

                var domainSeg = domain.Uri.Segments.MatchableSegment().Trim('/');
                var requestSeg = frequest.AbsolutePathDecoded.Segments().First().Trim('/');

                var content = _deliContentLoader.FindContentByRoute(frequest, domainSeg, requestSeg);
                if (content != null)
                {
                    frequest.SetPublishedContent(content);
                    _deliRequestAccessor.Finalize(content, frequest.Domain.Culture);

                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}
