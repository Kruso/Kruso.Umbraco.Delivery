using Kruso.Umbraco.Delivery.Json;
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
}
