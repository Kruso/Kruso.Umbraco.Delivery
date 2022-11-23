using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliPages
    {
        IPublishedContent FindStartPageRelative(string contentType, string culture);
        IPublishedContent NotFoundPage(string culture);
        IPublishedContent SettingsPage(string culture);
        IPublishedContent StartPage(DomainAndUri domain);
        IPublishedContent StartPage(string culture);
        IPublishedContent StartPage(IPublishedContent page, string culture);
        Dictionary<string, IPublishedContent> StartPages();
    }
}