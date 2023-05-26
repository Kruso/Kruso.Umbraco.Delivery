using System;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliPublishedNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentPublishedNotification>
    {
        public DeliPublishedNotificationHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Handle(ContentPublishedNotification notification) => Handle(notification.PublishedEntities, EventType.Published);
    }
}
