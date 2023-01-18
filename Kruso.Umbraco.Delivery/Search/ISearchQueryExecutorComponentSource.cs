namespace Kruso.Umbraco.Delivery.Search
{
    public interface ISearchQueryExecutorComponentSource
    {
        ISearchQuery GetSearchQuery(string queryName);
    }
}