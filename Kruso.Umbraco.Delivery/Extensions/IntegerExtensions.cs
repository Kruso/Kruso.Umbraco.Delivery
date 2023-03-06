using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class IntegerExtensions
    {
        public static Guid GenerateUuid(this int idx, int seed)
        {
            return Guid.ParseExact($"{UuidComponent(idx, true)}-1000-2000-{UuidComponent(seed, false)}", "d");
        }

        private static string UuidComponent(int val, bool eightFour)
        {
            var code = val.ToString("X").PadRight(8, '0').ToLower();

            if (eightFour)
                return code.Length > 8
                    ? $"{code.Substring(0, 8)}-{code.Substring(8).PadRight(4, '0')}"
                    : $"{code}-0000";
            else
                return code.Length > 12
                    ? code.Substring(0, 12)
                    : code.PadRight(12, '0');
        }
    }
}
