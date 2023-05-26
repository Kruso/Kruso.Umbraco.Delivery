using Kruso.Umbraco.Delivery.Services;
using System;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliDeletedNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentDeletedNotification>
    {
        public DeliDeletedNotificationHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Handle(ContentDeletedNotification notification) => Handle(notification.DeletedEntities, EventType.Deleted);
    }
}
