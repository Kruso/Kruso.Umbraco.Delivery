using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Search;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Controllers
{
    public class DeliSearchApiController : BaseController
    {
        private readonly ISearchQueryExecutor _searchQueryExecutor;
        private readonly IDeliRequestAccessor _deliRequestAccessor;

        public DeliSearchApiController(
            ISearchQueryExecutor searchQueryExecutor,
            IDeliRequestAccessor deliRequestAccessor,
            IModelConverter modelConverter, 
            IDeliCulture umbCulture,
            ILogger<DeliSearchApiController> logger)
            : base(umbCulture, logger)
        {
            _searchQueryExecutor = searchQueryExecutor;
            _deliRequestAccessor = deliRequestAccessor;
        }

        [HttpGet]
        [Route("api/{culture}/search/{queryName}")]
        public IActionResult Search(string culture, string queryName)
        {
            return Execute(culture, () =>
            {
                _deliRequestAccessor.FinalizeForSearch(culture);
                var searchRequest = SearchRequest.Create(culture, queryName, HttpContext.Request.Query);
                var res = _searchQueryExecutor.Execute(searchRequest);

                return new ApiResponse(res);
            });
        }
    }
}
