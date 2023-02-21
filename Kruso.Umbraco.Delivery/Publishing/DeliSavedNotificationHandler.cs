using Kruso.Umbraco.Delivery.Services;
using System;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliSavedNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentSavedNotification>
    {
        public DeliSavedNotificationHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Handle(ContentSavedNotification notification) =>
            base.Handle(notification.SavedEntities, EventType.Saved, notification.HasSavedCulture);
    }
}
