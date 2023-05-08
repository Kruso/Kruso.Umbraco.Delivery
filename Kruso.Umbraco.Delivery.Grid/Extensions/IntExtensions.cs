using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Grid.Extensions
{
    public static class IntExtensions
    {
        public static Guid GenerateUuid(this int idx)
        {
            string id = idx.ToString("X").PadRight(8, '0').ToLower();

            var uuid = id.Length > 8
                ? $"{id.Substring(0, 8)}-{id.Substring(8).PadRight(4, '0')}-0000-1000-8000-00805f9b34fb"
                : $"{id}-0000-1000-8000-00805f9b34fb";

            return Guid.ParseExact(uuid, "d");
        }
    }
}
