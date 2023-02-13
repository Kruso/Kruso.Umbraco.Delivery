using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NPoco.fastJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Controllers.Renderers
{
    internal class ActionResultRenderer
    {
        private readonly IDeliConfig _deliConfig;

        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        internal ActionResultRenderer(IDeliConfig delConfig)
        {
            _deliConfig = delConfig;
        }

        internal IActionResult ToJsonResult(HttpResponse httpResponse, object obj, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var json = obj != null
                ? JsonConvert.SerializeObject(obj, _serializerSettings)
                : null;

            var res = new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = json == null
                    ? (int)HttpStatusCode.NotFound
                    : (int)statusCode
            };

            SetResponseCacheControl(httpResponse, res);
            
            return res;
        }

        internal IActionResult ToResult<T>(HttpResponse httpResponse, string content, string contentType) where T : class
        {
            var res = new ContentResult
            {
                Content = content,
                ContentType = "application/json",
                StatusCode = content == null
                    ? (int)HttpStatusCode.NotFound
                    : (int)HttpStatusCode.OK
            };

            SetResponseCacheControl(httpResponse, res);

            return res;
        }

        internal IActionResult ToResult<T>(HttpResponse httpResponse, RenderResponse<T> renderResponse) where T : class
        {
            return renderResponse != null
                ? ToJsonResult(httpResponse, renderResponse.Model, renderResponse.StatusCode)
                : new NotFoundResult();
        }

        private void SetResponseCacheControl(HttpResponse httpResponse, IActionResult actionResult)
        {
            if (actionResult is ContentResult contentResult)
            {
                if (contentResult.StatusCode == (int)HttpStatusCode.OK)
                {
                    var cacheControl = _deliConfig.Get().GetCacheControl(contentResult.ContentType);
                    if (!string.IsNullOrEmpty(cacheControl))
                    {
                        httpResponse.Headers.Add("cache-control", cacheControl);
                    }
                }
            }
        }
    }
}
