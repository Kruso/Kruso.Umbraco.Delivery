using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliDeletedNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentDeletingNotification>
    {
        public DeliDeletedNotificationHandler(
            IDeliEventHandlerSource deliEventHandlerSource,
            IDeliContent deliContent,
            IDeliCulture deliCulture
            )
            : base(deliEventHandlerSource, deliContent, deliCulture)
        {
        }

        public void Handle(ContentDeletingNotification notification) =>
            base.Handle(notification.DeletedEntities, (content, culture) => true);
    }
}
