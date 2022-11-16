﻿using Kruso.Umbraco.Delivery.ModelConversion;
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
        [ProducesResponseType(typeof(SearchResult), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Route("api/{culture}/search/{queryName}")]
        public IActionResult Search(string culture, string queryName)
        {
            return Execute(culture, () =>
            {
                var searchRequest = SearchRequest.Create(culture, queryName, HttpContext.Request.Query);
                var res = _searchQueryExecutor.Execute(searchRequest);

                if (res.pageResults != null)
                {
                    res.pageResults = res.pageResults
                        .Select(x => _modelConverter.Convert(x, TemplateType.Search))
                        .Where(x => x != null)
                        .ToArray();
                }

                return new ApiResponse(res);
            });
        }
    }
}
