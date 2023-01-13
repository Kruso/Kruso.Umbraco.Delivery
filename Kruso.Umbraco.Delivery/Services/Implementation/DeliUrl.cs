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
        private readonly IDeliConfig _deliConfig;

        public DeliUrl(IDeliDomain deliDomain, IDeliRequestAccessor deliRequestAccessor, IPublishedUrlProvider urlProvider, IDeliContent deliContent, IDeliConfig deliConfig)
        {
            _deliDomain = deliDomain;
            _deliRequestAccessor = deliRequestAccessor;
            _urlProvider = urlProvider;
            _deliContent = deliContent;
            _deliConfig = deliConfig;
        }

        public string GetAbsoluteDeliveryUrl(string relativePath)
        {
            return GetAbsoluteDeliveryUrl(null, null, relativePath)?.ToString();
        }

        public string GetAbsoluteDeliveryUrl(IPublishedContent content, string culture)
        {
            var path = GetDeliveryUrl(content, culture);
            return GetAbsoluteDeliveryUrl(content, culture, path)?.ToString();
        }

        public string GetPreviewPaneUrl(string jwtToken)
        {
            var deliRequest = _deliRequestAccessor.Current;
            return $"{GetAbsoluteDeliveryUrl(deliRequest.Content, deliRequest.Culture)}?token={jwtToken}";
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

            return GetDeliveryUrl(path);
        }

        public string GetDeliveryUrl(string path)
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

        private Uri GetAbsoluteDeliveryUrl(IPublishedContent content = null, string culture = null, string path = null)
        {
            var deliRequest = _deliRequestAccessor.Current;
            var frontendHostUri = deliRequest?.CallingUri;

            if (frontendHostUri == null && !_deliConfig.IsMultiSite())
                Uri.TryCreate(_deliConfig.Get().FrontendHost, UriKind.Absolute, out frontendHostUri);

            if (frontendHostUri == null)
            {
                var domain = content == null || culture == null
                    ? _deliDomain.GetDefaultDomainByRequest()
                    : _deliDomain.GetDomainByContent(content, culture);

                if (Uri.TryCreate(domain.Name, UriKind.Absolute, out var domainUri))
                    frontendHostUri = new Uri($"{domainUri.Scheme}://{domainUri.Authority}");
            }

            if (content != null)
                path = GetDeliveryUrl(content, culture);

            return !string.IsNullOrEmpty(path)
                ? new Uri(frontendHostUri, path)
                : frontendHostUri;
        }
    }
}
