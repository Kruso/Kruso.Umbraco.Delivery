using Kruso.Umbraco.Delivery.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Search
{
    public class SearchQueryExecutorComponentSource : ISearchQueryExecutorComponentSource
    {
        private Dictionary<string, ISearchQuery> _searchQueries = null;

        public SearchQueryExecutorComponentSource(IEnumerable<ISearchQuery> searchQueries)
        {
            _searchQueries = searchQueries.ToFilteredDictionary<ISearchQuery, SearchQueryAttribute>();
        }

        public ISearchQuery GetSearchQuery(string queryName)
        {
            var key = queryName.ToLower();

            return _searchQueries.ContainsKey(key)
                ? _searchQueries[key]
                : null;
        }
    }
}
