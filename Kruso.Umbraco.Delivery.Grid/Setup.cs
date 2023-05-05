using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Grid
{
    public static class Setup
    {
        public static void AddGridComponents(this UmbracoDeliveryOptions options)
        {
            options?.AddComponentsFrom(typeof(Setup).Assembly);
        }
    }
}
