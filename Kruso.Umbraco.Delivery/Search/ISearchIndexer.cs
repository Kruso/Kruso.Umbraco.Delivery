using Examine;
using Kruso.Umbraco.Delivery.Json;
using System.Collections.Generic;

namespace Kruso.Umbraco.Delivery.Search
{
    public interface ISearchIndexer
    {
        void Index(IIndex index, Dictionary<string, JsonNode> modelNodesByCulture, Dictionary<string, List<object>> valueSet);
    }
}
