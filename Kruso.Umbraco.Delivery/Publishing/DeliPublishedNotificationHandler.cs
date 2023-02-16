using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliPublishedNotificationHandler : DeliNotificationHandler, INotificationHandler<ContentPublishedNotification>
    {
        public DeliPublishedNotificationHandler(
            IDeliEventHandlerSource deliEventHandlerSource,
            IDeliContent deliContent,
            IDeliCulture deliCulture
            )
            : base(deliEventHandlerSource, deliContent, deliCulture)
        {
        }

        public void Handle(ContentPublishedNotification notification) =>
            base.Handle(notification.PublishedEntities, notification.HasPublishedCulture);
    }
}
