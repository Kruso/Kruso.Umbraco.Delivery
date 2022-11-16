using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class UriExtensions
    {
        public static string CleanPath(this Uri uri)
        {
            return uri != null
                ? string.Join("", uri.Segments)?.TrimStart('/')?.TrimEnd('/')?.ToLower() ?? string.Empty
                : null;
        }
    }
}
