using Kruso.Umbraco.Delivery.Models;
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
        IEnumerable<DeliDomainAndUri> GetDomains();
        DeliDomainAndUri GetDomainByContent(IPublishedContent content, string culture);
        IEnumerable<DeliDomainAndUri> GetDomainsByRequest(Uri uri = null);
        DeliDomainAndUri GetDomainByRequest(string culture);
        DeliDomainAndUri GetDomainByRequest(Uri uri = null, bool allowDefault = false);
        DeliDomainAndUri GetDefaultDomainByRequest(Uri uri = null);
        string GetDomainCulture(string path);
    }
}