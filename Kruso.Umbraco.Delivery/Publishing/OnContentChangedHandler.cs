using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using static Umbraco.Cms.Core.Constants.HttpContext;

namespace Kruso.Umbraco.Delivery.Publishing
{
    [DeliEvent(EventType.Published)]
    [DeliEvent(EventType.Deleted)]
    internal class OnContentChangedHandler : IDeliEventHandler
    {
        private readonly IDeliContent _deliContent;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliUrl _deliUrl;

        private static object _lock = new object();
        private static readonly BlockingCollection<JsonNode> _items = new BlockingCollection<JsonNode>();
        private static Task _notificationTask = null;
        private static int NotificationDelayMilliseconds = 1000;

        public OnContentChangedHandler(IDeliContent deliContent, IDeliCulture deliCulture, IDeliUrl deliUrl)
        {
            _deliContent = deliContent;
            _deliCulture = deliCulture;
            _deliUrl = deliUrl;
        }

        public bool Handle(EventType eventType, string culture, IPublishedContent publishedContent)
        {
            var updatedPages = _deliContent.IsRenderablePage(publishedContent)
                ? CreateNotifications(eventType, culture, publishedContent)
                : CreateNotifications(EventType.Published, culture, _deliContent.RelatedPages(publishedContent.Id));

            foreach (var updatedPage in updatedPages)
                _items.Add(updatedPage);

            InitializeNotificationTask();

            return true;
        }

        private void InitializeNotificationTask()
        {
            lock (_lock)
            {
                if (_notificationTask == null || _notificationTask.IsCompleted)
                {
                    if (_notificationTask != null && _notificationTask.IsCompleted)
                        _notificationTask.Dispose();

                    _notificationTask = Task.Run(async () =>
                    {
                        await Task.Delay(NotificationDelayMilliseconds);

                        var notificationItems = new List<JsonNode>();
                        while (_items.TryTake(out var item))
                        {
                            notificationItems.Add(item);
                            if (!_items.Any())
                            {
                                using (var client = new HttpClient())
                                {
                                    var response = await client.PostAsync("", null);
                                }
                            }
                        }

                        Debug.WriteLine(notificationItems.Any() ? $"Items: {notificationItems.Count()}" : "Items: 0");
                    });
                }
            }
        }

        private List<JsonNode> CreateNotifications(EventType eventType, string culture, IPublishedContent updatedPage)
            => CreateNotifications(eventType, culture, new List<IPublishedContent> { updatedPage });

        private List<JsonNode> CreateNotifications(EventType eventType, string culture, IEnumerable<IPublishedContent> updatedPages)
        {
            return updatedPages
                .Where(p => _deliCulture.IsPublishedInCulture(p, culture))
                .Select(p => new JsonNode()
                    .AddProp("id", p.Key)
                    .AddProp("name", p.Name)
                    .AddProp("type", p.ContentType.Alias.Capitalize())
                    .AddProp("eventType", eventType.ToString())
                    .AddProp("url", _deliUrl.GetAbsoluteDeliveryUrl(p, culture)))
                .ToList();
        }
    }
}
