using Kruso.Umbraco.Delivery.Routing;
using System;

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

        public DeliveryConfigValues Get(Uri callingUri = null)
        {
            callingUri ??= _deliRequestAccessor.Current?.CallingUri;
            return _deliveryConfig.GetSite(callingUri);
        }

        public bool IsMultiSite() => _deliveryConfig.IsMultiSite();
    }
}
