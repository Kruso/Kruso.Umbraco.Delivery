using System;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliMovedNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentMovedNotification>
    {
        public DeliMovedNotificationHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Handle(ContentMovedNotification notification) => Handle(notification.MoveInfoCollection.Select(x => x.Entity), EventType.Moved);
    }
}
