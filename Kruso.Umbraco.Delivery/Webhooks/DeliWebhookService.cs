using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Webhooks
{
    internal class DeliWebhookService : IDeliWebhookService
    {
        private readonly IDeliConfig _deliConfig;

        public DeliWebhookService(IDeliConfig deliConfig)
        {
            _deliConfig = deliConfig;
        }

        public async Task SendAsync(List<DeliWebhookNotification> notifications)
        {
            var batches = GetWebhookBatches(notifications);

        }

        private List<DeliWebhookBatch> GetWebhookBatches(List<DeliWebhookNotification> notifications)
        {
            var res = new List<DeliWebhookBatch>();

            var notificationsByUri = notifications.GroupBy(x => new Uri(x.Url).HostUri());
            foreach (var group in notificationsByUri)
            {
                if (group.Any())
                {
                    var config = _deliConfig.Get(group.Key);
                    foreach (var webhook in config.Webhooks)
                    {
                        var batch = res.FirstOrDefault(x => x.Webhook.Url == webhook.Url);
                        if (batch == null)
                        {
                            batch = new DeliWebhookBatch { Webhook = webhook };
                            res.Add(batch);
                        }

                        batch.Notifications.AddRange(notifications);
                    }
                }
            }

            return res;
        }
    }
}
