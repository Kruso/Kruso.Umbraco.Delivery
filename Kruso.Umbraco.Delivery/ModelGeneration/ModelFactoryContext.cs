﻿using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
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
            public string? FallbackCulture { get; set; }
            public ModelFactoryOptions Options { get; set; }
        }

        private readonly Stack<StackItem> _stack = new Stack<StackItem>();

        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliCulture _deliCulture;

        public IPublishedContent Page => Peek()?.Page;
        public string Culture => Peek()?.Culture ?? _deliCulture.DefaultCulture;
        public string? FallbackCulture => Peek()?.FallbackCulture;

        public ModelFactoryOptions Options => Peek()?.Options;

        public bool Initialized => CurrentDepth > 0;
        public int CurrentDepth => _stack.Count;
        public bool ReachedMaxDepth => (Options?.MaxDepth ?? 0) > 0 && CurrentDepth >= (Options?.MaxDepth ?? 0);

        public Func<IPublishedContent, JsonNode> CreateRef { get; set; }

        public ModelFactoryContext(IDeliRequestAccessor deliRequestAccessor, IDeliCulture deliCulture)
        {
            _deliRequestAccessor = deliRequestAccessor;
            _deliCulture = deliCulture;
        }

        public JsonNode PageWithDepth(IPublishedContent page, string culture, ModelFactoryOptions options, Func<JsonNode> createPageFunc)
        {
            if (!CanRender(page, options, Culture))
                return null;

            var didIncrement = IncrementDepth(page, culture, options);
            return WithDepth(page, didIncrement, createPageFunc);
        }

        public JsonNode BlockWithDepth(IPublishedContent block, string culture, ModelFactoryOptions options, Func<JsonNode> createBlockFunc)
        {
            culture ??= Culture;
            var fallbackCulture = _deliCulture.GetFallbackCulture(culture);

            if (CanRender(block, options, culture))
            {
				var didIncrement = IncrementDepth(block.Key, culture, options);
				return WithDepth(block, didIncrement, createBlockFunc);
			}
            else if (fallbackCulture != null && CanRender(block, options, fallbackCulture))
			{
				var didIncrement = IncrementDepth(block.Key, culture, options);
				return WithDepth(block, didIncrement, createBlockFunc);
			}

			return null;
        }

        public JsonNode CustomBlockWithDepth(Guid key, string type, string culture, Func<JsonNode> createBlockFunc)
        {
            var didIncrement = IncrementDepth(key, culture, null);
            var refContent = new DeliRefContent(key, type);

            return WithDepth(refContent, didIncrement, createBlockFunc);
        }

        private JsonNode WithDepth(IPublishedContent content, bool didIncrement, Func<JsonNode> createBlockFunc)
        {
            if (didIncrement)
            {
                try
                {
                    if (createBlockFunc != null)
                    {
                        var stackItem = Peek();
                        var jsonNode = stackItem != null && !string.IsNullOrEmpty(stackItem.Culture)
                            ? _deliCulture.WithCultureContext(stackItem.Culture, createBlockFunc)
                            : createBlockFunc();

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
            else if (content != null && CreateRef != null)
            {
                return CreateRef(content);
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

        private StackItem? Peek()
        {
            return _stack.Any()
                ? _stack.Peek()
                : null;
        }

        private bool DetectedRecursion(Guid key)
        {
            return _stack.Count(x => x.Key == key) > 1;
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

            culture = culture ?? prev?.Culture ?? _deliRequestAccessor.Current?.Culture ?? _deliCulture.DefaultCulture;
            var fallbackCulture = _deliCulture.GetFallbackCulture(culture);

            return new StackItem
            {
                Key = stackItemKey.Value,
                Page = page ?? prev?.Page,
                Culture = culture,
                FallbackCulture = fallbackCulture,
                Options = options ?? prev?.Options ?? _deliRequestAccessor.Current?.ModelFactoryOptions ?? new ModelFactoryOptions()
            };
        }

        private bool CanRender(IPublishedContent content, ModelFactoryOptions options, string culture)
        {
            if (content == null)
                return false;

            if (content.ItemType == PublishedItemType.Element)
                return true;

            if (options == null || !options.ApplyPublicAccessRights)
                return true;

            return content.IsPublished(culture) && _deliRequestAccessor.Identity.HasAccess(content);
        }
    }
}
