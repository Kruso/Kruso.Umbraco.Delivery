using Kruso.Umbraco.Delivery.Json;
using System.Net;

namespace Kruso.Umbraco.Delivery.Models
{
    public class RenderResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Data { get; set; }
        public JsonNode Model { get; set; }
        public string ContentType { get; set; }
        public string Message { get; set; }

        public RenderResponse()
        {
            StatusCode = HttpStatusCode.OK;
            ContentType = "application/json";
        }
    }
}
