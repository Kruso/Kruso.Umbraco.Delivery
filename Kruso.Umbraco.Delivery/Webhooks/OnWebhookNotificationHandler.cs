using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Publishing;
using Kruso.Umbraco.Delivery.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using static Umbraco.Cms.Core.Constants.HttpContext;

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

        private static readonly BlockingCollection<DeliWebhookNotification> _items = new BlockingCollection<DeliWebhookNotification>();
        private static int NotificationDelayMilliseconds = 1000;

        private static ManualResetEvent _consumerResetEvent = new ManualResetEvent(false);
        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public OnWebhookNotificationHandler(IDeliContent deliContent, IDeliCulture deliCulture, IDeliUrl deliUrl, IDeliWebhookService deliWebhookService)
        {
            _deliContent = deliContent;
            _deliCulture = deliCulture;
            _deliUrl = deliUrl;
            _deliWebhookService = deliWebhookService;

            Task.Run(async () => await Consumer(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        public bool Handle(EventType eventType, string culture, IPublishedContent publishedContent)
        {
            //NOT CORRECT. It can be a renderable page and still need to get related pages if it is a page used
            // as a block.
            var updatedPages = _deliContent.IsRenderablePage(publishedContent)
                ? CreateNotifications(eventType, culture, publishedContent)
                : CreateNotifications(EventType.Published, culture, _deliContent.RelatedPages(publishedContent.Id));

            foreach (var updatedPage in updatedPages)
                _items.Add(updatedPage);

            _consumerResetEvent.Set();

            return true;
        }

        private async Task Consumer(CancellationToken cancellationToken)
        {
            var items = new List<DeliWebhookNotification>();

            while (!cancellationToken.IsCancellationRequested)
            {
                _consumerResetEvent.WaitOne();

                await Task.Delay(NotificationDelayMilliseconds);

                bool consumed = true;
                while (consumed)
                {
                    consumed = _items.TryTake(out var notification, NotificationDelayMilliseconds, cancellationToken);
                    if (consumed)
                        items.Add(notification);
                }

                await _deliWebhookService.SendAsync(items);

                items.Clear();

                _consumerResetEvent.Reset();
            } 
        }

        private List<DeliWebhookNotification> CreateNotifications(EventType eventType, string culture, IPublishedContent updatedPage)
            => CreateNotifications(eventType, culture, new List<IPublishedContent> { updatedPage });

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
