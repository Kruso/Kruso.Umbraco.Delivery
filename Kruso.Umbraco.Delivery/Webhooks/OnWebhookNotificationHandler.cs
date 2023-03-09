using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Publishing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Webhooks
{
    [DeliEvent(EventType.Published)]
    [DeliEvent(EventType.Deleted)]
    internal class OnWebhookNotificationHandler : IDeliEventHandler
    {
        private readonly IDeliContent _deliContent;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliUrl _deliUrl;
        private readonly IDeliWebhookService _deliWebhookService;
        private readonly ILogger<OnWebhookNotificationHandler> _log;

        private static readonly BlockingCollection<DeliWebhookNotification> _items = new BlockingCollection<DeliWebhookNotification>();
        private static int NotificationDelayMilliseconds = 1000;

        private static ManualResetEvent _consumerResetEvent = new ManualResetEvent(false);
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public OnWebhookNotificationHandler(
            IDeliContent deliContent, 
            IDeliCulture deliCulture, 
            IDeliUrl deliUrl, 
            IDeliWebhookService deliWebhookService,
            ILogger<OnWebhookNotificationHandler> log
            )
        {
            _deliContent = deliContent;
            _deliCulture = deliCulture;
            _deliUrl = deliUrl;
            _deliWebhookService = deliWebhookService;
            _log = log;

            Task.Run(async () => await Consumer(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        public bool Handle(EventType eventType, string culture, IPublishedContent publishedContent)
        {
            var updatedContent = GetUpdatedContent(publishedContent);
            var notifications = CreateNotifications(EventType.Published, culture, updatedContent);

            foreach (var notification in notifications)
            {
                _items.Add(notification);
                _log.LogDebug($"Queued notification {notification.Id}:{notification.Type}");
            }

            _consumerResetEvent.Set();

            return true;
        }

        private async Task Consumer(CancellationToken cancellationToken)
        {
            var items = new List<DeliWebhookNotification>();

            while (!cancellationToken.IsCancellationRequested)
            {
                _log.LogDebug("Waiting to consume queued notifications...");
                _consumerResetEvent.WaitOne();

                await Task.Delay(NotificationDelayMilliseconds);
                _log.LogDebug("Starting to consume queued notifications...");

                bool consumed = true;
                while (consumed)
                {
                    consumed = _items.TryTake(out var notification, NotificationDelayMilliseconds, cancellationToken);
                    if (consumed && !items.Any(x => x.Id == notification.Id))
                        items.Add(notification);
                }

                _log.LogDebug($"Consumed {items.Count} queued notifications. Sending to webhook service...");
                await _deliWebhookService.SendAsync(items);

                items.Clear();

                _consumerResetEvent.Reset();
                _log.LogDebug("Completed consuming queued notifications.");
            } 
        }

        private List<IPublishedContent> GetUpdatedContent(IPublishedContent publishedContent)
        {
            var res = new List<IPublishedContent>();
            if (_deliContent.IsRenderablePage(publishedContent))
                res.Add(publishedContent);

            res.AddRange(_deliContent.RelatedPages(publishedContent.Id));

            return res
                .DistinctBy(x => x.Id)
                .ToList();
        }

        private List<DeliWebhookNotification> CreateNotifications(EventType eventType, string culture, IEnumerable<IPublishedContent> updatedPages)
        {
            return updatedPages
                .Where(p => _deliCulture.IsPublishedInCulture(p, culture))
                .Select(p => new DeliWebhookNotification
                {
                    Id = p.Key,
                    Name = p.Name,
                    Type = p.ContentType.Alias.Capitalize(),
                    EventType = eventType.ToString(),
                    Url = _deliUrl.GetAbsoluteDeliveryUrl(p, culture)
                })
                .ToList();
        }
    }
}
