using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using System;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public interface IModelFactoryContext
    {
        string Culture { get; }
        int CurrentDepth { get; }
        bool Initialized { get; }
        ModelFactoryOptions Options { get; }
        IPublishedContent Page { get; }
        bool ReachedMaxDepth { get; }

        JsonNode PageWithDepth(IPublishedContent page, string culture, ModelFactoryOptions options, Func<JsonNode> createPageFunc);
        JsonNode BlockWithDepth(IPublishedContent block, string culture, ModelFactoryOptions options, Func<JsonNode> createBlockFunc);
        JsonNode CustomBlockWithDepth(Guid key, string type, string culture, Func<JsonNode> createBlockFunc);
    }
}