using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Search;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kruso.Umbraco.Delivery.Controllers
{
    public class DeliSearchApiController : BaseController
    {
        private readonly ISearchQueryExecutor _searchQueryExecutor;

        public DeliSearchApiController(
            ISearchQueryExecutor searchQueryExecutor, 
            IDeliCulture umbCulture,
            ILogger<DeliSearchApiController> logger)
            : base(umbCulture, logger)
        {
            _searchQueryExecutor = searchQueryExecutor;
        }

        [HttpGet]
        [Route("api/{culture}/search/{queryName}")]
        public IActionResult Search(string culture, string queryName)
        {
            return Execute(culture, () =>
            {
                var searchRequest = SearchRequest.Create(culture, queryName, HttpContext.Request.Query);
                return _searchQueryExecutor.Execute(searchRequest).ToJsonResult();
            });
        }
    }
}
