using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliDeletedNotificationHandler : INotificationHandler<ContentDeletingNotification>
    {
        private readonly IDeliEventHandlerSource _deliEventHandlerSource;
        private readonly IDeliContent _deliContent;

        public DeliDeletedNotificationHandler(IDeliEventHandlerSource deliEventHandlerSource, IDeliContent deliContent)
        {
            _deliEventHandlerSource = deliEventHandlerSource;
            _deliContent = deliContent;
        }

        public void Handle(ContentDeletingNotification notification)
        {
            foreach (var content in notification.DeletedEntities)
            {
                var eventHandler = _deliEventHandlerSource.Get(EventType.Deleted, content.ContentType.Alias);
                if (eventHandler != null)
                {
                    var publishedContent = _deliContent.PublishedContent(content.Key);
                    eventHandler.Handle(publishedContent);
                }
            }
        }
    }
}
