using Kruso.Umbraco.Delivery.Json;
using System.Collections.Generic;

namespace Kruso.Umbraco.Delivery.Search
{
    public class SearchResult
    {
        public bool success { get; set; }
        public long totalCount { get; set; }
        public int skip { get; set; }
        public int pageNo { get; set; }
        public int pageSize { get; set; }
        public IEnumerable<JsonNode> pageResults { get; set; } = new List<JsonNode>();
    }
}
