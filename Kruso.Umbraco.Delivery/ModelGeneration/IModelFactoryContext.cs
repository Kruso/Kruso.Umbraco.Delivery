//using Kruso.Umbraco.Delivery.Models;
//using Kruso.Umbraco.Delivery.Security;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using Umbraco.Cms.Core.Models.PublishedContent;

//namespace Kruso.Umbraco.Delivery.ModelGeneration
//{
//    public interface IModelFactoryContext
//    {
//        IUserIdentity Identity { get; }
//        string Culture { get; }
//        int CurrentDepth { get; }
//        bool LoadPreview { get; }
//        int MaxDepth { get; }
//        IPublishedContent Page { get; }
//        bool ReachedMaxDepth { get; }
//        IQueryCollection QueryString { get; }
//        public IModelFactory ModelFactory { get; set; }
//        bool Initialized { get; }

//        void Init(string culture, ModelFactoryOptions options = null);
//        void Init(IPublishedContent page, string culture, ModelFactoryOptions options = null);
//        void Init(IEnumerable<IPublishedContent> pages, string culture, ModelFactoryOptions options = null);

//        bool DetectedRecursion(Guid key);
//        bool IncrementDepth(Guid key);
//        void DecrementDepth();

//        bool NextPage();
//    }
//}