using System;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliMovedToTrashNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentMovedToRecycleBinNotification>
    {
        public DeliMovedToTrashNotificationHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Handle(ContentMovedToRecycleBinNotification notification) => Handle(notification.MoveInfoCollection.Select(x => x.Entity), EventType.Deleted);
    }
}
