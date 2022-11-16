using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using Umbraco.Cms.Web.Common.Controllers;

namespace Kruso.Umbraco.Delivery.Controllers
{
    public abstract class BaseController : UmbracoApiController
    {
        protected readonly IDeliCulture _deliCulture;
        protected readonly ILogger _logger;
        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public BaseController(IDeliCulture deliCulture, ILogger logger)
        {
            _deliCulture = deliCulture;
            _logger = logger;
        }

        protected IActionResult Execute(Func<ApiResponse> action)
        {
            return Execute(null, action);
        }

        protected IActionResult Execute(string culture, Func<ApiResponse> action)
        {
            ApiResponse res = null;

            try
            {
                if (!string.IsNullOrEmpty(culture))
                {
                    if (!_deliCulture.IsCultureSupported(culture))
                    {
                        res = new ApiResponse
                        {
                            StatusCode = System.Net.HttpStatusCode.BadRequest,
                            Payload = new
                            {
                                message = $"Unsupported language {culture}. Supported languages are {string.Join(", ", _deliCulture.SupportedCultures)}"
                            }
                        };
                    }
                }

                res ??= action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing request");
                res = new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Payload = ex.ToString()
                };
            }

            var formatter = new JsonSerializerSettings
            {

                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(res.Payload, formatter);
            return new ContentResult
            {
                Content = json,
                StatusCode = (int)res.StatusCode,
                ContentType = "application/json"
            };
        }
    }
}
