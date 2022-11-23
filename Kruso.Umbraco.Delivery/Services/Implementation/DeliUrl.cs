using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliUrl : IDeliUrl
    {
        private readonly IDeliDomain _deliDomain;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IPublishedUrlProvider _urlProvider;
        private readonly IDeliContent _deliContent;

        public DeliUrl(IDeliDomain deliDomain, IDeliRequestAccessor deliRequestAccessor, IPublishedUrlProvider urlProvider, IDeliContent deliContent)
        {
            _deliDomain = deliDomain;
            _deliRequestAccessor = deliRequestAccessor;
            _urlProvider = urlProvider;
            _deliContent = deliContent;
        }

        public string GetAbsoluteDeliveryUrl(string relativePath, string culture)
        {
            var path = GetDeliveryUrl(relativePath, culture);
            return InternalAbsoluteDeliveryUrl(path, culture);
        }

        public string GetAbsoluteDeliveryUrl(IPublishedContent content, string culture)
        {
            var path = GetDeliveryUrl(content, culture);
            return InternalAbsoluteDeliveryUrl(path, culture);
        }

        public IEnumerable<UrlInfo> GetAlternativeDeliveryUrls(IPublishedContent content, string culture)
        {
            return _deliContent.IsPage(content)
                ? _urlProvider.GetOtherUrls(content.Id)
                : Enumerable.Empty<UrlInfo>();
        }

        public string GetDeliveryUrl(IPublishedContent content, string culture)
        {
            var path =  _deliContent.IsPage(content)
                ? content.Url(culture)
                : string.Empty;

            return GetDeliveryUrl(path, culture);
        }

        public string GetDeliveryUrl(string path, string culture)
        {
            path = path?.Trim() ?? string.Empty;

            if (path == "#")
                return string.Empty;

            if (path.EndsWith("#"))
                path = path.TrimEnd("#");

            if (!path.StartsWith("/") && Uri.TryCreate(path, UriKind.Absolute, out var url))
                path = url.AbsolutePath;

            if (!path.StartsWith("/"))
                path = "/" + path;

            return path;
        }

        public string RemoveDomainPrefixFromPath(string path)
        {
            path = (path ?? string.Empty).CleanPath();
            var domain = _deliDomain.GetDomainsByRequest()
                .FirstOrDefault(x => path.StartsWith(x.Name));

            return domain != null
                ? path.Substring(domain.Name.Length)
                : path;
        }

        private string InternalAbsoluteDeliveryUrl(string path, string culture)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var req = _deliRequestAccessor.Current?.CallingUri;
            if (req == null)
                return path;

            return $"{req.Scheme}://{req.Authority}{path}";
        }
    }
}
