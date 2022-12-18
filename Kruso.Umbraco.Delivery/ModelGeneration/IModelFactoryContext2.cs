using Kruso.Umbraco.Delivery.Models;
using System;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public interface IModelFactoryContext2
    {
        string Culture { get; }
        int CurrentDepth { get; }
        bool Initialized { get; }
        ModelFactoryOptions Options { get; }
        IPublishedContent Page { get; }
        bool ReachedMaxDepth { get; }

        void DecrementDepth();
        bool IncrementDepth(Guid key, string culture = null, ModelFactoryOptions options = null);
        bool InitializeDepth(IPublishedContent content, string culture = null, ModelFactoryOptions options = null);
    }
}