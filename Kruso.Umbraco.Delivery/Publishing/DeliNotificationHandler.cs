using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public abstract class DeliNotificationHandler
    {
        private readonly IDeliEventHandlerSource _deliEventHandlerSource;
        private readonly IDeliContentTypes _deliContentTypes;

        protected DeliNotificationHandler(IServiceProvider serviceProvider)
        {
            _deliEventHandlerSource = serviceProvider.GetService<IDeliEventHandlerSource>();
            _deliContentTypes = serviceProvider.GetService<IDeliContentTypes>();
        }

        protected PublishEventResponse Handle(IEnumerable<IContent> entities, EventType eventType)
        {
            foreach (var entity in entities)
            {
                var response = _deliEventHandlerSource.Get(eventType, _deliContentTypes.ContentTypeAliases(entity.ContentType.Alias))?.Handle(eventType, entity) ?? PublishEventResponse.Continue;
                if (response != PublishEventResponse.Continue)
                    return response;
            }
            
            return PublishEventResponse.Continue;
        }
    }
}
