using Kruso.Umbraco.Delivery.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
    internal static class ActionResultExtensions
    {
        internal static IActionResult ToJsonResult<T>(this T obj, HttpStatusCode statusCode = HttpStatusCode.OK)
            where T : class
        {
            if (obj == null)
                return new NotFoundResult();

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(obj, JsonSerializationConfig.DeliverySerializationSettings),
                ContentType = "application/json",
                StatusCode = (int)statusCode
            };
        }
    }
}
