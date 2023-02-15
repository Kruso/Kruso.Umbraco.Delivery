using Examine;
using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Routing.Implementation;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Search
{
    public class SearchIndexerComponent : IComponent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IExamineManager _examineManager;
        private readonly ILogger<SearchIndexerComponent> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SearchIndexerComponent(
            IUmbracoContextFactory umbracoContextFactory, 
            IExamineManager examineManager, 
            ILogger<SearchIndexerComponent> logger,
            IServiceProvider serviceProvider)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _examineManager = examineManager ?? throw new ArgumentNullException(nameof(examineManager));
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void Initialize()
        {
            try
            {
                var indexNames = GetIndexers().Values
                    .Select(x => x.Index())
                    .Distinct()
                    .ToArray();

                foreach (var indexName in indexNames)
                {
                    if (!_examineManager.TryGetIndex(indexName, out IIndex index))
                        throw new InvalidOperationException($"No index found by name {indexName}");

                    if (!(index is BaseIndexProvider indexProvider))
                        throw new InvalidOperationException("Could not cast");

                    indexProvider.TransformingIndexValues += IndexProviderTransformingIndexValues;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Initialize: Exception: {0} | Message: {1} | Stack Trace: {2}", e.InnerException != null ? e.InnerException.ToString() : "", e.Message != null ? e.Message.ToString() : "", e.StackTrace);
            }
        }

        private void IndexProviderTransformingIndexValues(object sender, IndexingItemEventArgs args)
        {
            try
            {
                if (int.TryParse(args.ValueSet.Id, out var pageId))
                {
                    var contentType = args.ValueSet.ItemType;
                    var indexer = GetIndexer(args.Index.Name, contentType);
                    if (indexer != null)
                    {
                        using (UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
                        {
                            var content = umbracoContextReference.UmbracoContext.Content.GetById(pageId);
                            if (content != null)
                            {
                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var umbCulture = scope.ServiceProvider.GetService<IDeliCulture>();
                                    var deliRequestAccessor = scope.ServiceProvider.GetService<IDeliRequestAccessor>() as DeliRequestAccessor;
                                    var modelConverter = scope.ServiceProvider.GetService<IModelConverter>();
                                    var modelFactory = scope.ServiceProvider.GetService<IModelFactory>();
                                    var deliContent = scope.ServiceProvider.GetService<IDeliContent>();
                                    var deliUrl = scope.ServiceProvider.GetService<IDeliUrl>();
                                    try
                                    {
                                        var modelNodesByCulture = new Dictionary<string, JsonNode>();
                                        foreach (var culture in umbCulture.SupportedCultures)
                                        {
                                            umbCulture.WithCultureContext(culture, () =>
                                            {
                                                var callingUri = deliUrl.GetFrontendHostUri(content, culture);
                                                deliRequestAccessor.InitializeIndexing(content, culture, callingUri);

                                                var modelNode = deliContent.IsPage(content)
                                                    ? modelConverter.Convert(modelFactory.CreatePage(content, culture), TemplateType.Page)
                                                    : modelConverter.Convert(modelFactory.CreateBlock(content, culture), TemplateType.Block);

                                                if (!umbCulture.IsPublishedInCulture(content, culture))
                                                    modelNode = null;

                                                modelNodesByCulture.Add(culture, modelNode);
                                            });
                                        }

                                        var valueSet = new SearchIndexerValueSet(args.ValueSet.Values, content);
                                        indexer.Index(args.Index, modelNodesByCulture, valueSet);
                                        args.SetValues(valueSet.Values());
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogError(e, "Indexing: Exception: {0} | Message: {1} | Stack Trace: {2}", e.InnerException != null ? e.InnerException.ToString() : "", e.Message != null ? e.Message.ToString() : "", e.StackTrace);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "IndexProviderTransformingIndexValues: Exception: {0} | Message: {1} | Stack Trace: {2}", e.InnerException != null ? e.InnerException.ToString() : "", e.Message != null ? e.Message.ToString() : "", e.StackTrace);
            }
        }

        public void Terminate() { }

        private ISearchIndexer GetIndexer(string index, string contentTypeAlias)
        {
            var key = $"{index}.{contentTypeAlias}".ToLower();

            var indexers = GetIndexers();
            return indexers.ContainsKey(key)
                ? indexers[key]
                : null;
        }

        private Dictionary<string, ISearchIndexer> GetIndexers()
        {
            using (var scope = _serviceProvider.CreateScope())
            {

                var indexers = scope.ServiceProvider
                    .GetServices<ISearchIndexer>()
                    .ToFilteredDictionary<ISearchIndexer, SearchIndexerAttribute>();

                return indexers;
            }
        }
    }
}
