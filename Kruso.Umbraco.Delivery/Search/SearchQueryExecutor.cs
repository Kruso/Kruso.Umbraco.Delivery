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

        public SearchQueryExecutor(
            IServiceProvider serviceProvider,
            IDeliContent deliContent,
            IDeliRequestAccessor deliRequestAccessor,
            IModelFactory modelFactory, 
            IModelConverter modelConverter, 
            IExamineManager examineManager)
        {
            _serviceProvider = serviceProvider;
            _deliContent = deliContent;
            _deliRequestAccessor = deliRequestAccessor;
            _modelFactory = modelFactory;
            _modelConverter = modelConverter;
            _examineManager = examineManager;
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
            var res = new SearchResult
            {
                skip = (searchRequest.Page * searchRequest.PageSize) + searchRequest.Skip,
                pageNo = searchRequest.Page,
                pageSize = searchRequest.PageSize
            };

            var searchResults = ExecuteInternal(searchRequest);
            if (searchResults?.Any() ?? false)
            {
                var pages = searchResults
                    .Where(x => searchRequest.CustomFilterFunc?.Invoke(x) ?? true)
                    .Select(x => _deliContent.PublishedContent(Convert.ToInt32(x.Id)));

                var deliRequest = _deliRequestAccessor.Current;

                var pageModels = _modelConverter.Convert(_modelFactory.CreatePages(pages, searchRequest.Culture), TemplateType.Search)
                    .Select(x => x.Clone(deliRequest?.ModelFactoryOptions?.IncludeFields, deliRequest?.ModelFactoryOptions?.ExcludeFields));

                if (searchRequest.CustomSortOrderFunc != null)
                    pageModels = searchRequest.CustomSortOrderFunc.Invoke(pageModels);

                pageModels = pageModels.Skip(res.skip);
                if (searchRequest.PageSize > 0)
                    pageModels = pageModels.Take(searchRequest.PageSize);

                res.totalCount = pages.Count();
                res.pageResults = pageModels.ToList();
                res.success = true;
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
