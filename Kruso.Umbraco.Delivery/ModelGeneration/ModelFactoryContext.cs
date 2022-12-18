//using Kruso.Umbraco.Delivery.Models;
//using Kruso.Umbraco.Delivery.Routing;
//using Kruso.Umbraco.Delivery.Security;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Umbraco.Cms.Core.Models.PublishedContent;
//using Umbraco.Extensions;

//namespace Kruso.Umbraco.Delivery.ModelGeneration
//{
//    internal class ModelFactoryContext : IModelFactoryContext
//    {
//        private readonly IDeliRequestAccessor _deliRequestAccessor;

//        private readonly Stack<Guid> _idStack = new Stack<Guid>();
//        private IEnumerable<IPublishedContent> _pages;

//        public IUserIdentity Identity { get; private set; }
//        public int MaxDepth { get; private set; }
//        public int CurrentDepth => _idStack.Count;
//        public bool LoadPreview { get; private set; }
//        public bool ApplyPublicAccessRights { get; private set; }
//        public IQueryCollection QueryString { get; private set; }
//        public IModelFactory ModelFactory { get; set; }
//        public bool Initialized => Page != null;

//        public bool ReachedMaxDepth
//        {
//            get
//            {
//                return MaxDepth > 0 && MaxDepth == CurrentDepth;
//            }
//        }

//        public IPublishedContent Page { get; private set; }
//        public string Culture { get; private set; }


//        public ModelFactoryContext(
//            IUserIdentity identity,
//            IDeliRequestAccessor deliRequestAccessor)
//        {
//            Identity = identity;
//            _deliRequestAccessor = deliRequestAccessor;
//        }

//        public void Init(string culture, ModelFactoryOptions options = null)
//        {
//            Culture = culture ?? _deliRequestAccessor.Current.Culture;
//            MaxDepth = options?.MaxDepth ?? 0;
//            LoadPreview = options?.LoadPreview ?? false;
//            ApplyPublicAccessRights = options?.ApplyPublicAccessRights ?? true;
//            QueryString = options?.QueryString;
//        }

//        public void Init(IPublishedContent page, string culture, ModelFactoryOptions options = null)
//        {
//            Page = page;
//            Init(culture, options);
//            _idStack.Clear();
//        }

//        public void Init(IEnumerable<IPublishedContent> pages, string culture, ModelFactoryOptions options = null)
//        {
//            Init(pages.FirstOrDefault(), culture, options);
//            _pages = pages;
//            _idStack.Clear();
//        }

//        public bool DetectedRecursion(Guid key)
//        {
//            return _idStack.Count(x => x == key) > 1;
//        }

//        public bool IncrementDepth(Guid key)
//        {
//            _idStack.Push(key);

//            if (MaxDepth == 0 && DetectedRecursion(key))
//            {
//                DecrementDepth();
//                return false;
//            }

//            if (MaxDepth > 0 && CurrentDepth > MaxDepth)
//            {
//                DecrementDepth();
//                return false;
//            }

//            return true;
//        }

//        public void DecrementDepth()
//        {
//            _idStack.Pop();
//        }

//        public bool NextPage()
//        {
//            if (Page != null && _pages != null && _pages.Any())
//            {
//                var idx = _pages.IndexOf(Page);
//                if (idx >= 0 && idx < _pages.Count() - 1)
//                {
//                    Page = _pages.ElementAt(idx + 1);
//                    _idStack.Clear();
//                    return true;
//                }
//            }

//            return false;
//        }
//    }
//}
