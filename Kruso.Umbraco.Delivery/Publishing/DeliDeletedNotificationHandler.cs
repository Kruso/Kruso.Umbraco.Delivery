using Kruso.Umbraco.Delivery.Services;
using System;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliDeletedNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentDeletingNotification>
    {
        public DeliDeletedNotificationHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Handle(ContentDeletingNotification notification) =>
            base.Handle(notification.DeletedEntities, EventType.Deleted, (content, culture) => true);
    }
}
