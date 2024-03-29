﻿using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliDomain : IDeliDomain
    {
        private readonly IDeliContent _deliContent;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        
        public DeliDomain(IDeliContent deliContent, IDeliCulture deliCulture, IDeliRequestAccessor deliRequestAccessor, IUmbracoContextAccessor umbracoContextAccessor)
        {
            _deliContent = deliContent;
            _deliCulture = deliCulture;
            _deliRequestAccessor = deliRequestAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public DeliDomainAndUri GetDomainByContent(IPublishedContent content, string culture)
        {
            var domain = GetAllDomains(_deliRequestAccessor.Current?.CallingUri)
                .FirstOrDefault(d => d.ContentId == content.Root().Id && d.Culture == culture);

            return domain;
        }

        public IEnumerable<DeliDomainAndUri> GetDomains()
        {
            var uri = _deliRequestAccessor.Current?.CallingUri;
            return GetAllDomains(uri);
        }

        public IEnumerable<DeliDomainAndUri> GetDomainsByRequest(Uri uri = null)
        {
            uri ??= _deliRequestAccessor.Current?.CallingUri;

            var domains = GetAllDomains(uri);

            var candidates = domains
                .Where(d => d.Name.StartsWith($"{uri.Scheme}://") && d.Name.Contains(uri.Authority));

            if (!candidates.Any())
                candidates = domains
                    .Where(d => d.Name.StartsWith(uri.Authority));

            if (!candidates.Any())
                candidates = domains
                    .Where(x => !x.Name.Contains(uri.Authority) && x.Uri.Scheme == uri.Scheme && x.Uri.Authority == uri.Authority);

            return candidates.ToList();
        }

        public DeliDomainAndUri GetDefaultDomainByRequest(Uri uri = null)
        {
            uri ??= _deliRequestAccessor.Current.CallingUri;

            var candidates = GetDomainsByRequest(uri);
            var res = candidates.FirstOrDefault(x => x.Culture == _deliCulture.DefaultCulture);

            return res;
        }

        public DeliDomainAndUri GetDomainByRequest(string culture)
        {
            var candidates = GetDomainsByRequest();

            var res = candidates.FirstOrDefault(x => x.Culture == culture);
            return res;
        }

        public DeliDomainAndUri GetDomainByRequest(Uri uri = null, bool allowDefault = false)
        {
            uri ??= _deliRequestAccessor.Current.CallingUri;
            var candidates = GetDomainsByRequest(uri);

            var res = candidates.FirstOrDefault(x =>
            {
                var canSeg = x.Uri.Segments.MatchableSegment();
                var uriSeg = uri.Segments.MatchableSegment();

                return canSeg == uriSeg;
            });

            if (allowDefault && res == null)
                res = candidates.FirstOrDefault(x => x.Culture == _deliCulture.DefaultCulture);

            return res;
        }

        public string GetDomainCulture(string path)
        {
            path = path.CleanPath();

            var domains = GetDomainsByRequest();
            var culture = domains.FirstOrDefault(x => path.StartsWith($"{x.Name}/"))?.Culture;
            return culture ?? _deliCulture.DefaultCulture;
        }

        private IEnumerable<DeliDomainAndUri> GetAllDomains(Uri uri)
        {
            IEnumerable<DeliDomainAndUri> domains = null;

            if (uri != null && _umbracoContextAccessor.TryGetUmbracoContext(out var context))
            {
                domains = context.PublishedSnapshot.Domains.GetAll(false)
                    .Where(d => d.IsWildcard == false)
                    .Select(d => CreateDomain(d, uri))
                    .OrderByDescending(d => d.Uri.ToString());
            }

            if (domains == null || !domains.Any())
            {
                var virtualDomainAndUri = CreateVirtualDomain(uri);
                if (virtualDomainAndUri != null)
                {
                    domains = new List<DeliDomainAndUri>
                    {
                        virtualDomainAndUri
                    };
                }
            }

            return domains ?? Enumerable.Empty<DeliDomainAndUri>();
        }

        private DeliDomainAndUri CreateVirtualDomain(Uri requestUri)
        {
            var startPages = _deliContent.RootPublishedContent()
                .Where(x => _deliContent.IsPage(x) && _deliCulture.IsPublishedInCulture(x, _deliCulture.DefaultCulture));

            if (startPages.Count() == 1 && requestUri != null)
            {
                var host = new Uri($"{requestUri.Scheme}://{requestUri.Authority}/");
                var startPage = startPages.First();
                var virtualDomain = new Domain(-1, host.AbsoluteUri, startPage.Id, _deliCulture.DefaultCulture, false);
                return CreateDomain(virtualDomain, host);
            }

            return null;
        }

        private DeliDomainAndUri CreateDomain(Domain domain, Uri currentUri)
        {
            var fallbackCulture = _deliCulture.GetFallbackCulture(domain.Culture);
            return new DeliDomainAndUri(domain, currentUri, fallbackCulture);
        }
    }
}
