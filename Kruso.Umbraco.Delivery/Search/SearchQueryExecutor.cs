using Examine;
using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.Logging;
using Examine.Search;

namespace Kruso.Umbraco.Delivery.Search
{
    public class SearchQueryExecutor : ISearchQueryExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDeliContent _deliContent;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IModelFactory _modelFactory;
        private readonly IModelConverter _modelConverter;
        private readonly IExamineManager _examineManager;
        private readonly ILogger<SearchQueryExecutor> _logger;

        public SearchQueryExecutor(
            IServiceProvider serviceProvider,
            IDeliContent deliContent,
            IDeliRequestAccessor deliRequestAccessor,
            IModelFactory modelFactory, 
            IModelConverter modelConverter, 
            IExamineManager examineManager,
            ILogger<SearchQueryExecutor> logger)
        {
            _serviceProvider = serviceProvider;
            _deliContent = deliContent;
            _deliRequestAccessor = deliRequestAccessor;
            _modelFactory = modelFactory;
            _modelConverter = modelConverter;
            _examineManager = examineManager;
            _logger = logger;
        }

        public ISearchResults? ExecuteInternal(SearchRequest searchRequest)
        {
            var searchQuery = GetSearchQuery(searchRequest.QueryName);
            var indexName = searchQuery.Index();

            if (searchQuery != null && indexName != null && _examineManager.TryGetIndex(indexName, out var index))
            {
                var searchResults = searchQuery?.Execute(index.Searcher, searchRequest);
                return searchResults;
            }

            return null;
        }

        public SearchResult Execute(SearchRequest searchRequest)
        {
            var searchResults = ExecuteInternal(searchRequest);
            var pages = GetFilteredContent(searchRequest, searchResults);
            var models = CreateResults(searchRequest, pages);

            return CreateSearchResults(searchRequest, searchResults, models);
        }

        private List<JsonNode> CreateResults(SearchRequest searchRequest, List<IPublishedContent> pages)
        {
            var deliRequest = _deliRequestAccessor.Current;
            List<JsonNode> models = new List<JsonNode>();
            foreach (var page in pages)
            {
                var model = _deliContent.IsPage(page)
                    ? _modelFactory.CreatePage(page, searchRequest.Culture)
                    : _modelFactory.CreateBlock(page, searchRequest.Culture);

                model = _modelConverter.Convert(model, TemplateType.Search)
                    .Clone(deliRequest?.ModelFactoryOptions?.IncludeFields, deliRequest?.ModelFactoryOptions?.ExcludeFields);

                if (model != null)
                    models.Add(model);
                else
                    _logger.LogWarning($"Could not convert page {page.Id} to json");
            }

            if (models.Any() && searchRequest.CustomSortOrderFunc != null)
                models = searchRequest.CustomSortOrderFunc.Invoke(models);

            return models;
        }

        private SearchResult CreateSearchResults(SearchRequest searchRequest, ISearchResults searchResults, List<JsonNode> models)
        {
            var res = new SearchResult();
            IEnumerable<JsonNode> results;
            if (searchRequest.ManualPaging)
            {
                res.skip = (searchRequest.Page * searchRequest.PageSize) + searchRequest.Skip;
                res.pageNo = searchRequest.Page;
                res.pageSize = searchRequest.PageSize == 0 ? int.MaxValue : searchRequest.PageSize;
                results = models.Skip(res.skip);
                if (searchRequest.PageSize > 0)
                    results = results.Take(searchRequest.PageSize);
            }
            else
            {
                var queryOptions = searchRequest.GetQueryOptions();
                res.skip = queryOptions.Skip;
                res.pageNo = searchRequest.Page;
                res.pageSize = queryOptions.Take;
                results = models;
            }

            res.totalCount = searchResults.TotalItemCount;
            res.pageResults = results.ToList();
            if (res.pageSize == int.MaxValue)
                res.pageSize = res.pageResults.Count();

            res.success = results.Count() > 0;

            return res;
        }

        private List<IPublishedContent> GetFilteredContent(SearchRequest searchRequest, ISearchResults? searchResults)
        {
            var res = new List<IPublishedContent>();
            if (searchResults == null)
                return res;

            foreach (var item in searchResults)
            {
                if (ShouldRemove(searchRequest, item))
                {
                    _logger.LogInformation($"Custom filter removed item {item.Id} from search results");
                }
                else
                {
                    var content = _deliContent.PublishedContent(Convert.ToInt32(item.Id));
                    if (content != null)
                        res.Add(content);
                    else
                        _logger.LogWarning($"Item {item.Id} found in index but not in published content");
                }
            }

            return res;
        }

        private static bool ShouldRemove(SearchRequest searchRequest, ISearchResult item)
        {
            return !(searchRequest.CustomFilterFunc?.Invoke(item) ?? true);
        }

        private ISearchQuery GetSearchQuery(string queryName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var componentSource = scope.ServiceProvider.GetService<ISearchQueryExecutorComponentSource>();
                return componentSource.GetSearchQuery(queryName);
            }
        }
    }
}
