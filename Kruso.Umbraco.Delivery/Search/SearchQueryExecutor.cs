using Examine;
using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Search
{
    public class SearchQueryExecutor : ISearchQueryExecutor
    {
        private readonly IDeliContent _deliContent;
        private readonly IModelFactory _modelFactory;
        private readonly IExamineManager _examineManager;

        private Dictionary<string, ISearchQuery> _searchQueries = null;

        public SearchQueryExecutor(IEnumerable<ISearchQuery> searchQueries, IDeliContent deliContent, IModelFactory modelFactory, IExamineManager examineManager)
        {
            _deliContent = deliContent;
            _modelFactory = modelFactory;
            _examineManager = examineManager;

            _searchQueries = searchQueries.ToFilteredDictionary<ISearchQuery, SearchQueryAttribute>();
        }

        public SearchResult Execute(SearchRequest searchRequest)
        {
            var searchQuery = GetSearchQuery(searchRequest.QueryName);
            var indexName = searchQuery.Index();

            var skip = (searchRequest.Page * searchRequest.PageSize) + searchRequest.Skip;
            var take = searchRequest.PageSize;

            var res = new SearchResult
            {
                skip = skip,
                pageNo = searchRequest.Page,
                pageSize = searchRequest.PageSize
            };

            if (searchQuery != null && indexName != null && _examineManager.TryGetIndex(indexName, out var index))
            {
                var searchResults = searchQuery?.Execute(index.Searcher, searchRequest);
                if (searchResults?.Any() ?? false)
                {
                    var pages = searchResults
                        .Select(x => _deliContent.PublishedContent(Convert.ToInt32(x.Id)))
                        .Skip(skip);

                    if (take > 0)
                        pages = pages.Take(take);

                    var pageModels = _modelFactory
                        .CreatePages(pages, searchRequest.Culture)
                        .ToList();

                    res.totalCount = searchResults.TotalItemCount;
                    res.pageResults = pageModels ?? Enumerable.Empty<JsonNode>();
                    res.success = true;

                    return res;
                }
            }

            return res;
        }

        private ISearchQuery GetSearchQuery(string queryName)
        {
            var key = queryName.ToLower();

            return _searchQueries.ContainsKey(key)
                ? _searchQueries[key]
                : null;
        }
    }
}
