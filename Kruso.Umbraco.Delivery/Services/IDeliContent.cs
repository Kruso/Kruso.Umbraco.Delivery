using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliContent
    {
        bool IsNotFoundType(IPublishedContent content);
        bool IsRenderablePage(IPublishedContent content);
        bool IsPage(IPublishedContent content);
        bool IsJsonTemplate(IPublishedContent content);
        bool IsSettingsType(IPublishedContent content);
        string NameByCulture(IPublishedContent content, string culture);

        IEnumerable<IPublishedContent> PublishedDescendants(IPublishedContent content);
        IEnumerable<IPublishedContent> RootPages();
        IEnumerable<IPublishedContent> RootPages(string culture);
        IEnumerable<IPublishedContent> RootPublishedContent();
        IPublishedContent PublishedContent(Guid id);
        IPublishedContent PublishedContent(int id);
        IPublishedContent PublishedContentWithJsonTemplate(int id);
        IPublishedContent PublishedContent(string path, string culture);
        IEnumerable<IPublishedContent> PublishedChildren(int id);
        IPublishedContent UnpublishedContent(int id);
        IPublishedContent UnpublishedContent(Guid id);
    }
}