using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Models
{
    public class ApiResponse
    {
        public dynamic Payload { get; set; }
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public ApiResponse()
        {
        }

        public ApiResponse(dynamic payload)
        {
            Payload = payload;
        }
    }
}
