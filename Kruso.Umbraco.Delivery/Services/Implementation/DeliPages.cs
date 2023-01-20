using Kruso.Umbraco.Delivery.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliPages : IDeliPages
    {
        private readonly DeliveryConfig _deliveryConfig;
        private readonly IDeliContent _deliContent;
        private readonly IDeliDomain _deliDomain;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliRequestAccessor _deliRequestAccessor;

        public DeliPages(DeliveryConfig deliveryConfig, IDeliContent deliContent, IDeliDomain deliDomain, IDeliCulture deliCulture, IDeliRequestAccessor deliRequestAccessor)
        {
            _deliveryConfig = deliveryConfig;
            _deliContent = deliContent;
            _deliDomain = deliDomain;
            _deliCulture = deliCulture;
            _deliRequestAccessor = deliRequestAccessor;
        }

        public IPublishedContent StartPage(DomainAndUri domain)
        {
                var startPage = _deliContent.PublishedContent(domain.ContentId);
                return _deliCulture.IsPublishedInCulture(startPage, domain.Culture)
                    ? startPage
                    : null;
        }

        public IPublishedContent StartPage(string culture)
        {
            var domain = _deliDomain.GetDomainByRequest(culture);
            return StartPage(domain);
        }

        public IPublishedContent StartPage(IPublishedContent page, string culture)
        {
            var startPage = page?.Root();
            return _deliCulture.IsPublishedInCulture(startPage, culture)
                ? startPage 
                : null;
        }

        public IEnumerable<IPublishedContent> StartPages(string culture)
        {
            var res = new List<IPublishedContent>();

            return _deliContent.RootPages(culture);
        }

        public Dictionary<string, IPublishedContent> StartPages()
        {
            var res = new Dictionary<string, IPublishedContent>();

            var domains = _deliDomain.GetDomainsByRequest();
            var startPages = domains
                .Select(x => _deliContent.PublishedContent(x.ContentId))
                .Where(x => _deliContent.IsPage(x));

            foreach (var startPage in startPages)
            {
                foreach (var cultures in _deliCulture.GetCultures(startPage))
                {
                    //totod: Handle multiple sites with multiple languages
                    if (!res.ContainsKey(cultures))
                        res.Add(cultures, startPage);
                }
            }

            return res;
        }

        public IPublishedContent SettingsPage(string culture)
        {
            var config = _deliveryConfig.GetConfigValues(_deliRequestAccessor.Current?.CallingUri);
            return FindStartPageRelative(config.SettingsType, culture);
        }

        public IPublishedContent NotFoundPage(string culture)
        {
            var config = _deliveryConfig.GetConfigValues(_deliRequestAccessor.Current?.CallingUri);
            return FindStartPageRelative(config.NotFoundType, culture) ?? FindStartPageRelative(config.NotFoundType, _deliCulture.DefaultCulture);
        }

        public IPublishedContent FindStartPageRelative(string contentType, string culture)
        {
            IPublishedContent page = null;

            _deliCulture.WithCultureContext(culture, () =>
            {
                if (!string.IsNullOrEmpty(contentType))
                {
                    var startPage = StartPage(culture);
                    if (startPage != null)
                    {
                        Func<IPublishedContent, bool> isPageOfType = (content) => 
                            content.ContentType.Alias.Equals(contentType, StringComparison.InvariantCultureIgnoreCase)
                            && _deliCulture.IsPublishedInCulture(content, culture);

                        page = startPage.Children?.FirstOrDefault(x => isPageOfType(x));
                        
                        if (page == null)
                        {
                            page = startPage.Parent != null
                                ? startPage.Parent.Children?.FirstOrDefault(x => isPageOfType(x))
                                : _deliContent.RootPublishedContent()?.FirstOrDefault(x => isPageOfType(x));
                        }
                    }
                }

            });

            return page;
        }
    }
}
