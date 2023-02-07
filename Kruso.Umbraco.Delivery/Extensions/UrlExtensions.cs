using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Routing;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class UrlExtensions
    {
        public static Uri HostUri(this Uri uri)
        {
            return uri != null && uri.IsAbsoluteUri
                ? new Uri($"{uri.Scheme}://{uri.Authority}")
                : null;
        }

        public static string CleanPath(this Uri uri)
        {
            return uri != null
                ? string.Join("", uri.Segments)?.TrimStart('/')?.TrimEnd('/')?.ToLower() ?? string.Empty
                : null;
        }

        public static string CleanPath(this IPublishedRequest request)
        {
            if (request == null)
                return null;

            return request.Uri.AbsolutePath.CleanPath();
        }

        public static Uri AbsoluteUri(this HttpRequest req)
        {
            if (req != null)
            {
                Uri.TryCreate(new Uri(req.Host()), req.Path.ToString(), out var requestUri);
                return requestUri;
            }

            return null;
        }

        public static Uri AbsoluteUri(this string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var requestUri)
                ? requestUri
                : null;
        }

        public static string Host(this HttpRequest req)
        {
            return req != null
                ? $"{req.Scheme}://{req.Host}"
                : string.Empty;
        }


        public static string CleanPath(this string path)
        {
            if (path == null)
                path = string.Empty;

            path = path.TrimStart('#');

            if (path.Contains('?'))
                path = path.Substring(0, path.IndexOf('?'));

            if (path.Contains('#'))
                path = path.Substring(0, path.IndexOf('#'));

            if (!path.EndsWith("/"))
                path += "/";

            if (!path.StartsWith("/"))
                path = "/" + path;

            return path.ToLower();
        }

        public static string[] Segments(this string path)
        {
            var res = path.CleanPath().Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => $"/{x}")
                .ToArray();

            return res.Length == 0
                ? new string[] { "/" }
                : res;
        }

        public static string MatchableSegment(this string[] segments)
        {
            if (segments == null || segments.Length == 0)
                return "/";

            var res = segments.Length == 1
                ? segments[0]
                : segments[1];

            if (!res.StartsWith("/"))
                res = $"/{res}";

            if (!res.EndsWith("/"))
                res = $"{res}/";

            return res;
        }
    }
}
