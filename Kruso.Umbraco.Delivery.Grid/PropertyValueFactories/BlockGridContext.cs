using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Grid.PropertyValueFactories
{
    internal class BlockGridContext
    {
        private int Idx = 0;
        public readonly int DefaultGridColumns = 12;

        public BlockGridContext(int idx, int? defaultGridColumns = null)
        {
            Idx = idx;
            DefaultGridColumns = defaultGridColumns ?? 12;
        }

        public Guid GenerateUuid()
        {
            string id = Idx.ToString("X").PadRight(8, '0').ToLower();

            var uuid = id.Length > 8
                ? $"{id.Substring(0, 8)}-{id.Substring(8).PadRight(4, '0')}-0000-1000-8000-00805f9b34fb"
                : $"{id}-0000-1000-8000-00805f9b34fb";

            Idx++;

            return Guid.ParseExact(uuid, "d");
        }
    }
}
