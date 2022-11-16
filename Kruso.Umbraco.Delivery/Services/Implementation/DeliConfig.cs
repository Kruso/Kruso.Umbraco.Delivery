using Kruso.Umbraco.Delivery.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliConfig : IDeliConfig
    {
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly DeliveryConfig _deliveryConfig;

        public DeliConfig(IDeliRequestAccessor deliRequestAccessor, DeliveryConfig deliveryConfig)
        {
            _deliRequestAccessor = deliRequestAccessor;
            _deliveryConfig = deliveryConfig;
        }

        public DeliveryConfigValues Get(string authority = null)
        {
            authority ??= _deliRequestAccessor.Current?.CallingUri?.Authority;
            return _deliveryConfig.GetConfigValues(authority);
        }
    }
}
