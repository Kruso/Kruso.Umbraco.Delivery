using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliSavedNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentSavedNotification>
    {
        public DeliSavedNotificationHandler(
            IDeliEventHandlerSource deliEventHandlerSource,
            IDeliContent deliContent,
            IDeliCulture deliCulture
            )
            : base(deliEventHandlerSource, deliContent, deliCulture)
        {
        }

        public void Handle(ContentSavedNotification notification) =>
            base.Handle(notification.SavedEntities, notification.HasSavedCulture);
    }
}
