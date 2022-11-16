using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliDomain
    {
        DomainAndUri GetDomainByContent(IPublishedContent content, string culture);
        IEnumerable<DomainAndUri> GetDomainsByRequest(Uri uri = null);
        DomainAndUri GetDomainByRequest(string culture);
        DomainAndUri GetDomainByRequest(Uri uri = null, bool allowDefault = false);
        DomainAndUri GetDefaultDomainByRequest(Uri uri = null);
        string GetDomainCulture(string path);
    }
}