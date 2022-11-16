using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliConfig
    {
        DeliveryConfigValues Get(string authority = null);
    }
}
