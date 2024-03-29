﻿using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliContent
    {
        bool IsNotFoundType(IPublishedContent content);
        bool IsRenderablePage(IPublishedContent content);
        bool IsRenderablePage(IContent content);
        bool IsPage(IPublishedContent content);
        bool IsSettingsType(IPublishedContent content);

        IEnumerable<IPublishedContent> PublishedDescendants(IPublishedContent content);
        IEnumerable<IPublishedContent> RootPages();
        IEnumerable<IPublishedContent> RootPages(string culture);
        IEnumerable<IPublishedContent> RootPublishedContent();
        IPublishedContent PublishedContent(Guid id);
        IPublishedContent PublishedContent(int id);
        IPublishedContent PublishedContent(Udi id);
        IPublishedContent PublishedContent(string path, string culture);
        IEnumerable<IPublishedContent> PublishedChildren(Guid id, params string[] documentTypeAliases);
        IEnumerable<IPublishedContent> PublishedChildren(int id, params string[] documentTypeAliases);
        IEnumerable<IPublishedContent> PublishedChildren(IPublishedContent content, params string[] documentTypeAliases);
        IPublishedContent UnpublishedContent(int id);
        IPublishedContent UnpublishedContent(Guid id);
        List<IPublishedContent> RelatedPages(int id);
    }
}