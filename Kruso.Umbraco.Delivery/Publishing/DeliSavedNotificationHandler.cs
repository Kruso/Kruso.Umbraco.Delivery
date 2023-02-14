using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliSavedNotificationHandler : INotificationHandler<ContentSavedNotification>
    {
        private readonly IDeliEventHandlerSource _deliEventHandlerSource;
        private readonly IDeliContent _deliContent;

        public DeliSavedNotificationHandler(IDeliEventHandlerSource deliEventHandlerSource, IDeliContent deliContent)
        {
            _deliEventHandlerSource = deliEventHandlerSource;
            _deliContent = deliContent;
        }

        public void Handle(ContentSavedNotification notification)
        {
            foreach (var content in notification.SavedEntities)
            {
                var eventHandler = _deliEventHandlerSource.Get(EventType.Saved, content.ContentType.Alias);
                if (eventHandler != null)
                {
                    var publishedContent = _deliContent.UnpublishedContent(content.Key);
                    eventHandler.Handle(publishedContent);
                }
            }
        }
    }
}
