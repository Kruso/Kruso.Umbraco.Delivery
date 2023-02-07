using Kruso.Umbraco.Delivery.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class ISearchExtensions
    {
        public static string Index(this ISearchQuery searchQuery)
        {
            var attr = searchQuery?.GetType().GetCustomAttributes(typeof(SearchQueryAttribute), true).FirstOrDefault() as SearchQueryAttribute;
            return attr?.Index;
        }

        public static string Index(this ISearchIndexer searchIndexer)
        {
            var attr = searchIndexer?.GetType().GetCustomAttributes(typeof(SearchIndexerAttribute), true).FirstOrDefault() as SearchIndexerAttribute;
            return attr?.Index;
        }
    }
}
