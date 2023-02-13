﻿using Examine;
using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Search
{
    public class SearchQueryExecutor : ISearchQueryExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDeliContent _deliContent;
        private readonly IModelFactory _modelFactory;
        private readonly IModelConverter _modelConverter;
        private readonly IExamineManager _examineManager;
        private readonly IDeliRequestAccessor _delRequestAccessor;

        public SearchQueryExecutor(
            IServiceProvider serviceProvider,
            IDeliContent deliContent, 
            IModelFactory modelFactory, 
            IModelConverter modelConverter, 
            IExamineManager examineManager,
            IDeliRequestAccessor delRequestAccessor)
        {
            _serviceProvider = serviceProvider;
            _deliContent = deliContent;
            _modelFactory = modelFactory;
            _modelConverter = modelConverter;
            _examineManager = examineManager;
            _delRequestAccessor = delRequestAccessor;
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
                    var results = searchResults.Where(x => searchRequest.CustomFilterFunc?.Invoke(x) ?? true);

                    results = searchRequest.CustomSortOrderFunc != null
                        ? results.OrderBy(x => searchRequest.CustomSortOrderFunc(x)).Skip(skip)
                        : results.Skip(skip);

                    if (take > 0)
                        results = results.Take(take);

                    var pages = results
                        .Select(x => _deliContent.PublishedContent(Convert.ToInt32(x.Id)));

                    var pageModels = _modelConverter.Convert(_modelFactory.CreatePages(pages, searchRequest.Culture), TemplateType.Search)
                        .ToList();

                    var modelFactoryOptions = _delRequestAccessor.Current?.ModelFactoryOptions;
                    if ((modelFactoryOptions?.IncludeFields?.Any() ?? false) || (modelFactoryOptions?.ExcludeFields?.Any() ?? false))
                        pageModels = pageModels.Select(x => x.Clone(modelFactoryOptions.IncludeFields, modelFactoryOptions.ExcludeFields)).ToList();

                    res.totalCount = searchResults.TotalItemCount;
                    res.pageResults = pageModels;
                    res.success = true;
                }
            }

            return res;
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
