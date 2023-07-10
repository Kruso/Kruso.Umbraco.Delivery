using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Search;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kruso.Umbraco.Delivery.Controllers
{
    public class DeliSearchApiController : BaseController
    {
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly ISearchQueryExecutor _searchQueryExecutor;

        public DeliSearchApiController(
            IDeliRequestAccessor deliRequestAccessor,
            ISearchQueryExecutor searchQueryExecutor, 
            IDeliCulture umbCulture,
            ILogger<DeliSearchApiController> logger)
            : base(umbCulture, logger)
        {
            _deliRequestAccessor = deliRequestAccessor;
            _searchQueryExecutor = searchQueryExecutor;
        }

        [HttpGet]
        [Route("api/{culture}/search/{queryName}")]
        public IActionResult Search(string culture, string queryName)
        {
            _deliRequestAccessor.Finalize(null, culture);
            return Execute(culture, () =>
            {
                var searchRequest = SearchRequest.Create(culture, queryName, HttpContext.Request.Query);
                return _searchQueryExecutor.Execute(searchRequest).ToJsonResult();
            });
        }

        [HttpGet]
        [Route("api/search/{queryName}")]
        public IActionResult SearchNoCulture(string queryName)
        {
            _deliRequestAccessor.Finalize(null, null);
            return Execute(() =>
            {
                var searchRequest = SearchRequest.Create(null, queryName, HttpContext.Request.Query);
                return _searchQueryExecutor.Execute(searchRequest).ToJsonResult();
            });
        }
    }
}
