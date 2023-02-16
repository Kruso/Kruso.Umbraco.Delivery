using Kruso.Umbraco.Delivery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public abstract class DeliNotificationHandler
    {
        private readonly IDeliEventHandlerSource _deliEventHandlerSource;
        private readonly IDeliContent _deliContent;
        private readonly IDeliCulture _deliCulture;

        public DeliNotificationHandler(
            IDeliEventHandlerSource deliEventHandlerSource,
            IDeliContent deliContent,
            IDeliCulture deliCulture
            )
        {
            _deliEventHandlerSource = deliEventHandlerSource;
            _deliContent = deliContent;
            _deliCulture = deliCulture;
        }

        protected bool Handle(IEnumerable<IContent> entities, Func<IContent, string, bool> hasCultureFunc)
        {
            var cancel = false;

            var cultures = _deliCulture.SupportedCultures;
            foreach (var content in entities)
            {
                var publishedCultures = cultures
                    .Where(x => hasCultureFunc(content, x))
                    .ToArray();

                var eventHandler = _deliEventHandlerSource.Get(EventType.Published, content.ContentType.Alias);
                if (eventHandler != null)
                {
                    var publishedContent = _deliContent.PublishedContent(content.Key);
                    cancel |= !eventHandler.Handle(EventType.Published, publishedCultures, publishedContent);
                }
            }

            return cancel;
        }
    }
}
