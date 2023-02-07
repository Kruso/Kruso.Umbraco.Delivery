using Microsoft.AspNetCore.Http;

namespace Kruso.Umbraco.Delivery.Security
{
    public static class RequestExtensions
    {
        public static string GetJwtBearerToken(this HttpRequest request)
        {
            const string BearerPrefix = "Bearer ";
            var authHeader = request.Headers["Authorization"].ToString();

            return !string.IsNullOrEmpty(authHeader) && authHeader.StartsWith(BearerPrefix)
                ? authHeader.Substring(BearerPrefix.Length)
                : null;
        }
    }
}
