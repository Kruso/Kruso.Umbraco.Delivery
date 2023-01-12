using Kruso.Umbraco.Delivery.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliConfig : IDeliConfig
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DeliveryConfig _deliveryConfig;

        public DeliConfig(IServiceProvider serviceProvider, DeliveryConfig deliveryConfig)
        {
            _serviceProvider = serviceProvider;
            _deliveryConfig = deliveryConfig;
        }

        public DeliveryConfigValues Get(string authority = null)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var deliRequestAccessor = scope.ServiceProvider.GetService<IDeliRequestAccessor>();
                authority ??= deliRequestAccessor.Current?.CallingUri?.Authority;
                
                return _deliveryConfig.GetConfigValues(authority);
            }
        }

        public bool IsMultiSite()
        {
            return (_deliveryConfig.Sites?.Count() ?? 0) > 1;
        }
    }
}
