using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class QueryCollectionExtensions
    {
        public static string[] Strs(this IQueryCollection query, string param)
        {
            if (query != null && query.ContainsKey(param))
            {
                return query[param]
                    .SelectMany(x => x.Split(','))
                    .Select(x => x.Trim())
                    .Where(x => x != null)
                    .ToArray();
            }

            return new string[0];
        }

        public static string Str(this IQueryCollection query, string param)
        {
            var vals = Strs(query, param);
            return string.Join(", ", vals);
        }

        public static int Int(this IQueryCollection query, string param)
        {
            int.TryParse(Strs(query, param).FirstOrDefault(), out var res);
            return res;
        }
    }
}
