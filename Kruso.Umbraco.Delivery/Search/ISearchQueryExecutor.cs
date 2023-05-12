using Examine;

namespace Kruso.Umbraco.Delivery.Search
{
    public interface ISearchQueryExecutor
    {
        ISearchResults? ExecuteInternal(SearchRequest searchRequest);
        SearchResult Execute(SearchRequest searchRequest);
    }
}