using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Search;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace Kruso.Umbraco.Delivery.Controllers
{
    internal class DeliSearchApiController : BaseController
    {
        private readonly ISearchQueryExecutor _searchQueryExecutor;
        private readonly IModelConverter _modelConverter;
        private readonly ActionResultRenderer _actionResultRenderer;

        internal DeliSearchApiController(
            ISearchQueryExecutor searchQueryExecutor, 
            IModelConverter modelConverter,
            ActionResultRenderer actionResultRenderer,
            IDeliCulture umbCulture,
            ILogger<DeliSearchApiController> logger)
            : base(umbCulture, logger)
        {
            _searchQueryExecutor = searchQueryExecutor;
            _modelConverter = modelConverter;
            _actionResultRenderer = actionResultRenderer;
        }

        [HttpGet]
        [Route("api/{culture}/search/{queryName}")]
        public IActionResult Search(string culture, string queryName)
        {
            return Execute(culture, () =>
            {
                var searchRequest = SearchRequest.Create(culture, queryName, HttpContext.Request.Query);
                var res = _searchQueryExecutor.Execute(searchRequest);

                return _actionResultRenderer.ToJsonResult(Response, res);
            });
        }
    }
}
