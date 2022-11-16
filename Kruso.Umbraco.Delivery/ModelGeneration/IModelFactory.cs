using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public interface IModelFactory
    {
        string Culture { get; }

        void Init(IPublishedContent page);
        void Init(IPublishedContent page, string culture, ModelFactoryOptions options = null);
        void Init(IEnumerable<IPublishedContent> pages, string culture, ModelFactoryOptions options = null);

        IEnumerable<JsonNode> CreateBlocks(IEnumerable<IPublishedContent> items);
        JsonNode CreateBlock();
        JsonNode CreateBlock(IMedia media);
        JsonNode CreateBlock(IPublishedContent content);
        JsonNode CreateBlock(IPublishedElement element);
        IEnumerable<JsonNode> CreateRoutes();
        JsonNode CreatePage();
        IEnumerable<JsonNode> CreatePages();
    }
}