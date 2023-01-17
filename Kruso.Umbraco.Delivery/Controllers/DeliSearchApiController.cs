using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.Models;
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
        private readonly IModelConverter _modelConverter;

        public DeliSearchApiController(
            ISearchQueryExecutor searchQueryExecutor, 
            IModelConverter modelConverter, 
            IDeliCulture umbCulture,
            ILogger<DeliSearchApiController> logger)
            : base(umbCulture, logger)
        {
            _searchQueryExecutor = searchQueryExecutor;
            _modelConverter = modelConverter;
        }

        [HttpGet]
        [Route("api/{culture}/search/{queryName}")]
        public IActionResult Search(string culture, string queryName)
        {
            return Execute(culture, () =>
            {
                var searchRequest = SearchRequest.Create(culture, queryName, HttpContext.Request.Query);
                var res = _searchQueryExecutor.Execute(searchRequest);

                res.pageResults = _modelConverter.Convert(res.pageResults, TemplateType.Search).ToArray();

                return new ApiResponse(res);
            });
        }
    }
}
