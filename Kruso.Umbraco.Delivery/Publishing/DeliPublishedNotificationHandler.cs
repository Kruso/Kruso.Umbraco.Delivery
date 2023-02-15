using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliPublishedNotificationHandler : INotificationHandler<ContentPublishedNotification>
    {
        private readonly IDeliEventHandlerSource _deliEventHandlerSource;
        private readonly IDeliContent _deliContent;

        public DeliPublishedNotificationHandler(IDeliEventHandlerSource deliEventHandlerSource, IDeliContent deliContent)
        {
            _deliEventHandlerSource = deliEventHandlerSource;
            _deliContent = deliContent;
        }

        public void Handle(ContentPublishedNotification notification)
        {
            foreach (var content in notification.PublishedEntities)
            {
                var eventHandler = _deliEventHandlerSource.Get(EventType.Published, content.ContentType.Alias);
                if (eventHandler != null)
                {
                    var publishedContent = _deliContent.PublishedContent(content.Key);
                    eventHandler.Handle(publishedContent);
                }
            }
        }
    }
}
