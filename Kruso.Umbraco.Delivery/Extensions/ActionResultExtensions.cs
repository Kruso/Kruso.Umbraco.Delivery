using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class ActionResultExtensions
    {
        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            NullValueHandling = NullValueHandling.Include
        };
    

        public static IActionResult ToJsonResult<T>(this T obj, HttpStatusCode statusCode = HttpStatusCode.OK)
            where T : class
        {
            if (obj == null)
                return new NotFoundResult();

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(obj, _serializerSettings),
                ContentType = "application/json",
                StatusCode = (int)statusCode
            };
        }
    }
}
