using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;

namespace Kruso.Umbraco.Delivery.Models
{
    public class RenderResponse<T> where T : class
    {
        public HttpStatusCode StatusCode { get; set; }
        public T Model { get; set; }
        public string ContentType { get; set; }
        public string Message { get; set; }

        public RenderResponse()
        {
            StatusCode = HttpStatusCode.OK;
            ContentType = "application/json";
        }
    }

    public static class RenderResponseExtensions
    {
        public static IActionResult ToActionResult<T>(this RenderResponse<T> renderResponse)
            where T : class
        {
            if (renderResponse == null)
                return new NotFoundResult();

            if (renderResponse.Model == null)
                return new ContentResult
                {
                    ContentType = renderResponse.ContentType,
                    StatusCode = (int)renderResponse.StatusCode
                };

            if (renderResponse.Model is string content)
                return new ContentResult
                {
                    Content = content,
                    ContentType = renderResponse.ContentType,
                    StatusCode = (int)renderResponse.StatusCode
                };

            if (renderResponse.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
                return renderResponse.Model.ToJsonResult(renderResponse.StatusCode);

            return new NotFoundResult();
        }
    }
}
