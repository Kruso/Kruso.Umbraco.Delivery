using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public class ModelFactoryContext2 : IModelFactoryContext2
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

        public IPublishedContent Page => Peek()?.Page;
        public string Culture => Peek()?.Culture;
        public ModelFactoryOptions Options => Peek()?.Options;

        public bool Initialized => CurrentDepth > 0;
        public int CurrentDepth => _stack.Count;
        public bool ReachedMaxDepth => (Options?.MaxDepth ?? 0) > 0 && CurrentDepth >= (Options?.MaxDepth ?? 0);


        public ModelFactoryContext2(IDeliRequestAccessor deliRequestAccessor, IDeliCulture deliCulture)
        {
            _deliRequestAccessor = deliRequestAccessor;
            _deliCulture = deliCulture;

            InitializeDepth();
        }

        private bool InitializeDepth()
        {
            _stack.Clear();
            return Push(CreateStackItem(_deliRequestAccessor.Current?.Content));
        }

        public bool InitializeDepth(IPublishedContent content, string culture = null, ModelFactoryOptions options = null)
        {
            _stack.Clear();
            return Push(CreateStackItem(content, null, culture, options));
        }

        public bool IncrementDepth(Guid key, string culture = null, ModelFactoryOptions options = null)
        {
            return Initialized || InitializeDepth()
                ? Push(CreateStackItem(null, key, culture, options))
                : false;
        }

        public void DecrementDepth()
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

        private StackItem CreateStackItem(IPublishedContent content, Guid? key = null, string culture = null, ModelFactoryOptions options = null)
        {
            var stackItemKey = key != null ? key.Value : content?.Key;
            if (stackItemKey == null)
                return null;

            var prev = Peek();

            return new StackItem
            {
                Key = stackItemKey.Value,
                Page = content ?? prev?.Page,
                Culture = culture ?? prev?.Culture ?? _deliRequestAccessor.Current.Culture ?? _deliCulture.DefaultCulture,
                Options = options ?? prev?.Options ?? _deliRequestAccessor.Current.ModelFactoryOptions ?? new ModelFactoryOptions()
            };
        }
    }
}
