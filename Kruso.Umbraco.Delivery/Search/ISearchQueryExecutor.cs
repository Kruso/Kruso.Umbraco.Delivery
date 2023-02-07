namespace Kruso.Umbraco.Delivery.Search
{
    public interface ISearchQueryExecutor
    {
        SearchResult Execute(SearchRequest searchRequest);
    }
}