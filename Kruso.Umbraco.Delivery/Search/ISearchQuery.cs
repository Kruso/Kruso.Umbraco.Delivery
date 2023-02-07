using Examine;
using Kruso.Umbraco.Delivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Search
{
    public interface ISearchQuery
    {
        ISearchResults Execute(ISearcher searcher, SearchRequest searchRequest);
    }
}
