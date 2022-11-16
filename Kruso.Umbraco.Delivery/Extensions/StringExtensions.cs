using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class StringExtensions
    {
        public static JToken ToJson(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) 
            { 
                return null; 
            }
            text = text.Trim();
            if ((text.StartsWith("{") && text.EndsWith("}")) || //For object
                (text.StartsWith("[") && text.EndsWith("]"))) //For array
            {
                try
                {
                    return JToken.Parse(text);
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return null;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string Capitalize(this string text)
        {
            return string.IsNullOrEmpty(text)
                ? text
                : $"{text.Substring(0, 1).ToUpper()}{(text.Length > 1 ? text.Substring(1) : string.Empty)}";
        }

        public static string CamelCase(this string text)
        {
            return string.IsNullOrEmpty(text)
                ? text
                : $"{text.Substring(0, 1).ToLower()}{(text.Length > 1 ? text.Substring(1) : string.Empty)}";
        }

        public static NameValueCollection ToNvc(this string queryString)
        {
            var nvc = new NameValueCollection();
            if (!string.IsNullOrEmpty(queryString))
            {
                foreach (var parm in queryString.TrimStart('?').Split('&'))
                {
                    if (!string.IsNullOrEmpty(parm))
                    {
                        var parts = parm.Split('=');
                        if (parts.Length == 2)
                        {
                            nvc.Add(parts[0], parts[1]);
                        }
                    }

                }
            }

            return nvc;
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