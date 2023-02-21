using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using System;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliPublishedNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentPublishedNotification>
    {
        public DeliPublishedNotificationHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Handle(ContentPublishedNotification notification) =>
            base.Handle(notification.PublishedEntities, EventType.Published, notification.HasPublishedCulture);
    }
}
