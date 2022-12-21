using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public interface IModelFactory
    {
        IModelFactoryContext Context { get; }

        JsonNode CreateBlock(IMedia media, string culture = null, ModelFactoryOptions options = null);
        JsonNode CreateBlock(IPublishedContent block, string culture = null, ModelFactoryOptions options = null);
        JsonNode CreateBlock(IPublishedElement element, string culture = null, ModelFactoryOptions options = null);
        IEnumerable<JsonNode> CreateBlocks(IEnumerable<IPublishedContent> blocks, string culture = null, ModelFactoryOptions options = null);
        IEnumerable<JsonNode> CreateBlocks(IEnumerable<IPublishedElement> items);
        JsonNode CreateCustomBlock(Guid id, string type, Action<JsonNode> fillBlockAction);
        JsonNode CreatePage(IPublishedContent page, string culture = null, ModelFactoryOptions options = null);
        IEnumerable<JsonNode> CreatePages(IEnumerable<IPublishedContent> pages, string culture = null, ModelFactoryOptions options = null);
        IEnumerable<JsonNode> CreateRoutes(IEnumerable<IPublishedContent> items, string culture);
    }
}