using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Models
{
    internal class Pagination
    {
        public static int DefaultPageSize = 10;

        [JsonPropertyName("skip")]
        public int Skip { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public int Items { get; private set; }
        public int TotalItems { get; set; }

        public bool ShouldPaginate => Skip > 0 || PageSize > 0;

        public Pagination(int? skip, int? page, int? pageSize)
        {
            if (page != null && page.HasValue)
            {
                Page = page.Value;
                PageSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : DefaultPageSize;
            }

            if (skip != null && skip.HasValue)
            {
                Skip = skip.Value;
                PageSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : DefaultPageSize;
            }
        }

        public IEnumerable<T> Paginate<T>(IEnumerable<T> items, Func<T, bool> filter = null)
        {
            items = items.Where(x => filter != null && filter(x));

            TotalItems = items.Count();

            if (ShouldPaginate)
            {
                var skip = Skip + (Page * PageSize);
                var take = PageSize;

                items = items.Skip(skip);
                if (take > 0)
                    items = items.Take(take);
            }

            Items = items.Count();
            return items;
        }
    }
}
