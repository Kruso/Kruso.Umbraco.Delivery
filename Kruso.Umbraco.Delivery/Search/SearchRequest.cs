using Examine;
using Kruso.Umbraco.Delivery.Json;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Search
{
    public class SearchRequest
    {
        public string Culture { get; set; }
        public string QueryName { get; set; }
        public int Skip { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public JsonNode Params { get; private set; } = new JsonNode();

        public Func<IEnumerable<JsonNode>, List<JsonNode>> CustomSortOrderFunc;
        public Func<ISearchResult, bool> CustomFilterFunc;

        public int IntParam(string parm)
        {
            int.TryParse(Params.Val<string>(parm), out var val);
            return val;
        }

        public int[] IntParams(string parm)
        {
            var vals = StringParams(parm)
                .Select(x => int.TryParse(x.Trim(), out var i) ? (int?)i : null)
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToArray();

            return vals;
        }

        public string StringParam(string parm)
        {
            return Params.Val<string[]>(parm)?.First();
        }

        public string[] StringParams(string parm)
        {
            return Params.Val<string[]>(parm) ?? new string[0];
        }

        public static SearchRequest Create(string culture, string queryName, IQueryCollection query)
        {
            var searchRequest = new SearchRequest
            {
                Culture = culture,
                QueryName = queryName,
            };

            foreach (var group in query.GroupBy(x => x.Key))
            {
                if (group.Key.Equals("page", StringComparison.InvariantCultureIgnoreCase) && int.TryParse(group.First().Value, out var page))
                {
                    searchRequest.Page = page;
                }
                else if (group.Key.Equals("pagesize", StringComparison.InvariantCultureIgnoreCase) && int.TryParse(group.First().Value, out var pageSize))
                {
                    searchRequest.PageSize = pageSize;
                }
                else if (group.Key.Equals("skip", StringComparison.InvariantCultureIgnoreCase) && int.TryParse(group.First().Value, out var skip))
                {
                    searchRequest.Skip = skip;
                }
                else
                {
                    var vals = CreateStringArray(group.SelectMany(kvp => kvp.Value));
                    searchRequest.Params.AddProp(group.Key, vals);
                }
            }

            return searchRequest;
        }

        private static string[] CreateStringArray(IEnumerable<string> strs)
        {
            return strs
                .SelectMany(s => s.Split(",", StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Trim())
                .ToArray();
        }
    }
}
