using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Webhooks
{
    internal class DeliWebhookService : IDeliWebhookService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDeliConfig _deliConfig;
        private readonly ILogger<DeliWebhookService> _log;

        public DeliWebhookService(IHttpClientFactory httpClientFactory, IDeliConfig deliConfig, ILogger<DeliWebhookService> log)
        {
            _httpClientFactory = httpClientFactory;
            _deliConfig = deliConfig;
            _log = log;
        }

        public async Task SendAsync(List<DeliWebhookNotification> notifications)
        {
            _log.LogDebug("Starting webhook send notifications...");

            try
            {
                var batches = GetWebhookBatches(notifications);
                _log.LogDebug($"Got {batches.Count} webhook batches.");

                foreach (var batch in batches)
                {
                    try
                    {
                        var httpClient = _httpClientFactory.CreateClient(batch.Webhook.Name);
                        _log.LogDebug($"Sending batch to webhook {batch.Webhook.Name}:{batch.Webhook.Url}...");

                        var json = JsonConvert.SerializeObject(batch.Notifications, Formatting.Indented);
                        var response = await httpClient.SendAsync(new HttpRequestMessage
                        {
                            Content = new StringContent(json, Encoding.UTF8, "application/json")
                        });

                        if (response.IsSuccessStatusCode)
                            _log.LogDebug($"Response {response.StatusCode} for webhook {batch.Webhook.Name}:{batch.Webhook.Url}.");
                        else
                            _log.LogError($"Response {response.StatusCode} for webhook {batch.Webhook.Name}:{batch.Webhook.Url}.");
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, $"Internal error sending webhook batch for webhook {batch.Webhook.Name}:{batch.Webhook.Url}.");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Internal error sending webhook batches.");
            }
        }

        private List<DeliWebhookBatch> GetWebhookBatches(List<DeliWebhookNotification> notifications)
        {
            var res = new List<DeliWebhookBatch>();

            _log.LogDebug("Creating webhook batches...");

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
                        _log.LogDebug($"Created batch of {notifications.Count} notifications for webhook {webhook.Name}:{webhook.Url}");
                    }
                }
            }

            return res;
        }
    }
}
