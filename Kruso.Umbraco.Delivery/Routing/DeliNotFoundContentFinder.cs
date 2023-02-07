using Kruso.Umbraco.Delivery.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;

namespace Kruso.Umbraco.Delivery.Routing
{
    public class DeliNotFoundContentFinder : IContentLastChanceFinder
    {
        private readonly IDeliConfig _deliConfig;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliContent _deliContent;
        private readonly IDeliDomain _deliDomain;

        public DeliNotFoundContentFinder(IDeliConfig deliConfig, IDeliRequestAccessor deliRequestAccessor, IDeliCulture deliCulture, IDeliContent deliContent, IDeliDomain deliDomain)
        {
            _deliConfig = deliConfig;
            _deliRequestAccessor = deliRequestAccessor;
            _deliCulture = deliCulture;
            _deliContent = deliContent;
            _deliDomain = deliDomain;
        }

        public Task<bool> TryFindContent(IPublishedRequestBuilder request)
        {
            var domain = _deliDomain.GetDomainByRequest(request.Uri, allowDefault: true)
                ?? _deliDomain.GetDefaultDomainByRequest(request.Uri);

            if (domain == null)
                return Task.FromResult(false);

            var res = false;
            if (!string.IsNullOrEmpty(_deliConfig.Get().NotFoundType))
            {
                IPublishedContent notFoundPage = null;
                string culture = string.Empty;

                if (!string.IsNullOrEmpty(domain.Culture))
                {
                    _deliCulture.WithCultureContext(domain.Culture, () =>
                    {
                        notFoundPage = GetNotFoundPage(domain.ContentId);
                        if (notFoundPage != null)
                            culture = domain.Culture;
                    });
                }

                if (notFoundPage == null)
                {
                    _deliCulture.WithCultureContext(_deliCulture.DefaultCulture, () =>
                    {
                        notFoundPage = GetNotFoundPage(domain.ContentId);
                        if (notFoundPage != null)
                            culture = _deliCulture.DefaultCulture;
                    });
                }

                _deliRequestAccessor.Finalize(notFoundPage, culture);

                request.SetDomain(domain);
                request.SetPublishedContent(notFoundPage);
                request.SetCulture(culture);
                request.SetIs404();

                res = notFoundPage != null;
            }

            return Task.FromResult(res);
        }

        private IPublishedContent GetNotFoundPage(int? startPageId)
        {
            if (startPageId != null)
            {
                var startPage = _deliContent.PublishedContent(startPageId.Value);
                if (startPage != null)
                {
                    Func<IPublishedContent, bool> isPageOfType = (content) =>
                        content.ContentType.Alias.Equals(_deliConfig.Get().NotFoundType, StringComparison.InvariantCultureIgnoreCase);

                    return startPage.Children?.FirstOrDefault(x => isPageOfType(x));

                }
            }

            return null;
        }

        private IPublishedContent Get404ByStartPageId(string culture, int? startPageId)
        {
            IPublishedContent notFoundPage = null;

            if (startPageId != null)
            {
                var startPage = _deliContent.PublishedContent(startPageId.Value);
                if (startPage != null)
                {
                    Func<IPublishedContent, bool> isPageOfType = (content) =>
                        content.ContentType.Alias.Equals(_deliConfig.Get().NotFoundType, StringComparison.InvariantCultureIgnoreCase)
                        && _deliCulture.IsPublishedInCulture(content, culture);

                    notFoundPage = startPage.Children?.FirstOrDefault(x => isPageOfType(x));
                    if (notFoundPage == null)
                    {
                        notFoundPage = startPage.Parent != null
                            ? startPage.Parent.Children?.FirstOrDefault(x => isPageOfType(x))
                            : _deliContent.RootPublishedContent()?.FirstOrDefault(x => isPageOfType(x));
                    }
                }
            }

            return notFoundPage;
        }
    }
}
