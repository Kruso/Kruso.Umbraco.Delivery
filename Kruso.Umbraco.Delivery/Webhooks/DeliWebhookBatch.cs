using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Webhooks
{
    internal class DeliWebhookBatch
    {
        public DeliveryWebhook Webhook { get; set; }
        public List<DeliWebhookNotification> Notifications { get; } = new List<DeliWebhookNotification>();
    }
}
