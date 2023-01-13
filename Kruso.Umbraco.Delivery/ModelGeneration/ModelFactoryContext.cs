using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public class ModelFactoryContext : IModelFactoryContext
    {
        private class StackItem
        {
            public Guid Key { get; set; }
            public IPublishedContent Page { get; set; }
            public string Culture { get; set; }
            public ModelFactoryOptions Options { get; set; }
        }

        private readonly Stack<StackItem> _stack = new Stack<StackItem>();

        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliContent _deliContent;
        private readonly IServiceProvider _serviceProvider;

        public IPublishedContent Page => Peek()?.Page;
        public string Culture => Peek()?.Culture;
        public ModelFactoryOptions Options => Peek()?.Options;

        public bool Initialized => CurrentDepth > 0;
        public int CurrentDepth => _stack.Count;
        public bool ReachedMaxDepth => (Options?.MaxDepth ?? 0) > 0 && CurrentDepth >= (Options?.MaxDepth ?? 0);

        public ModelFactoryContext(IDeliRequestAccessor deliRequestAccessor, IDeliCulture deliCulture, IDeliContent deliContent, IServiceProvider serviceProvider)
        {
            _deliRequestAccessor = deliRequestAccessor;
            _deliCulture = deliCulture;
            _deliContent = deliContent;
            _serviceProvider = serviceProvider;
        }

        public JsonNode PageWithDepth(IPublishedContent page, string culture, ModelFactoryOptions options, Func<JsonNode> createPageFunc)
        {
            if (!CanRender(page))
                return null;

            var didIncrement = IncrementDepth(page, culture, options);
            return WithDepth(page, didIncrement, createPageFunc);
        }

        public JsonNode BlockWithDepth(IPublishedContent block, string culture, ModelFactoryOptions options, Func<JsonNode> createBlockFunc)
        {
            if (!CanRender(block))
                return null;

            var didIncrement = IncrementDepth(block.Key, culture, options);
            return WithDepth(block, didIncrement, createBlockFunc);
        }

        public JsonNode CustomBlockWithDepth(Guid key, string type, string culture, Func<JsonNode> createBlockFunc)
        {
            var didIncrement = IncrementDepth(key, culture, null);
            var jsonNode = WithDepth(null, didIncrement, createBlockFunc);
            if (jsonNode == null && didIncrement)
            {
                var refContent = new DeliRefContent(key, type);
                var refTemplate = GetModelFactoryComponentSource().GetTemplate(TemplateType.Ref, refContent);
                jsonNode = refTemplate.Create(this, new JsonNode(), refContent);
            }

            return jsonNode;
        }

        private JsonNode WithDepth(IPublishedContent content, bool didIncrement, Func<JsonNode> createBlockFunc)
        {
            if (didIncrement)
            {
                try
                {
                    if (createBlockFunc != null)
                    {
                        var jsonNode = createBlockFunc();
                        return jsonNode?.AnyProps() ?? false
                            ? jsonNode
                            : null;
                    }
                }
                finally
                {
                    DecrementDepth();
                }
            }
            else if (content != null)
            {
                var refTemplate = GetModelFactoryComponentSource().GetTemplate(TemplateType.Ref, content);
                return refTemplate.Create(this, new JsonNode(), content);
            }


            return null;
        }

        private bool IncrementDepth(IPublishedContent page, string culture = null, ModelFactoryOptions options = null)
        {
            return Push(CreateStackItem(page, null, culture, options));
        }

        private bool IncrementDepth(Guid key, string culture = null, ModelFactoryOptions options = null)
        {
            if (!Initialized)
                Push(CreateStackItem(_deliRequestAccessor.Current?.Content, null, culture, options));

            return Push(CreateStackItem(null, key, culture, options));
        }

        private void DecrementDepth()
        {
            Pop();
        }

        private bool Push(StackItem stackItem)
        {
            if (stackItem == null)
                return false;

            _stack.Push(stackItem);

            if (stackItem.Options.MaxDepth == 0 && DetectedRecursion(stackItem.Key))
            {
                DecrementDepth();
                return false;
            }

            if (stackItem.Options.MaxDepth > 0 && CurrentDepth > stackItem.Options.MaxDepth)
            {
                DecrementDepth();
                return false;
            }

            return true;
        }

        private bool DetectedRecursion(Guid key)
        {
            return _stack.Count(x => x.Key == key) > 1;
        }

        private StackItem Peek()
        {
            return _stack.Any()
                ? _stack.Peek()
                : null;
        }

        private StackItem Pop()
        {
            return _stack.Any()
                ? _stack.Pop()
                : null;
        }

        private StackItem CreateStackItem(IPublishedContent page, Guid? key = null, string culture = null, ModelFactoryOptions options = null)
        {
            var stackItemKey = key != null ? key.Value : page?.Key;
            if (stackItemKey == null)
                return null;

            var prev = Peek();

            return new StackItem
            {
                Key = stackItemKey.Value,
                Page = page ?? prev?.Page,
                Culture = culture ?? prev?.Culture ?? _deliRequestAccessor.Current?.Culture ?? _deliCulture.DefaultCulture,
                Options = options ?? prev?.Options ?? _deliRequestAccessor.Current?.ModelFactoryOptions ?? new ModelFactoryOptions()
            };
        }

        private IModelFactoryComponentSource GetModelFactoryComponentSource()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetService<IModelFactoryComponentSource>();
            }
        }

        private bool CanRender(IPublishedContent content)
        {
            if (content == null)
                return false;

            //TODO: This is not correct, some blocks are access controlled.
            if (_deliContent.IsPage(content))
            {
                return _deliRequestAccessor.Identity.HasAccess(content)
                    && content.IsPublished(Culture);
            }

            return true;
        }
    }
}
