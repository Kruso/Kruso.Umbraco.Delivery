using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net;
using Umbraco.Cms.Web.Common.Controllers;

namespace Kruso.Umbraco.Delivery.Controllers
{
    internal abstract class BaseController : UmbracoApiController
    {
        protected readonly IDeliCulture _deliCulture;

        protected readonly ILogger _logger;

        internal BaseController(IDeliCulture deliCulture, ILogger logger)
        {
            _deliCulture = deliCulture;
            _logger = logger;
        }

        protected bool IsStatusCode(ContentResult res, HttpStatusCode status)
        {
            return res != null 
                && res.StatusCode.HasValue 
                && res.StatusCode.Value == (int)status;
        }

        protected IActionResult Execute(Func<IActionResult> action)
        {
            return Execute(null, action);
        }

        protected IActionResult Execute(string culture, Func<IActionResult> action)
        {
            try
            {
                if (!_deliCulture.IsCultureSupported(culture))
                {
                    var msg = $"Unsupported language {culture}. Supported languages are {string.Join(", ", _deliCulture.SupportedCultures)}";

                    _logger.LogError(msg);
                    return BadRequest(msg);
                }

                var res = action != null
                    ? action.Invoke()
                    : BadRequest("Action content returned null");

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing request");

                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Content = ex.ToString()
                };
            }
        }
    }
}
