using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Webhooks
{
    internal class DeliWebhookNotification
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string EventType { get; set; }
        public string Url { get; set; }
        public DateTime Updated { get; set; }
    }
}
