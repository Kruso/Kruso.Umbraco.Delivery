using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class HttpRequestExtensions
    {
        public static Uri AbsoluteUri(this HttpRequest req)
        {
            if (req != null)
            {
                Uri.TryCreate(new Uri(req.Host()), req.Path.ToString(), out var requestUri);
                return requestUri;
            }

            return null;
        }

        public static string Host(this HttpRequest req)
        {
            return req != null
                ? $"{req.Scheme}://{req.Host}"
                : string.Empty;
        }
    }
}
