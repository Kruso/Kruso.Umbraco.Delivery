using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Publishing
{
    public class DeliEventHandlerSource : IDeliEventHandlerSource
    {
        private Dictionary<string, IDeliEventHandler> _eventHandlers = null;

        public DeliEventHandlerSource(IEnumerable<IDeliEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers.ToFilteredDictionary<IDeliEventHandler, DeliEventAttribute>();
        }

        public IDeliEventHandler Get(EventType eventType, string documentType)
        {

            var key = eventType.MakeKey(documentType);

            if (_eventHandlers.ContainsKey(key))
            {
                return _eventHandlers[key];
            }

            var defaultKey = eventType.MakeKey();
            if (_eventHandlers.ContainsKey(defaultKey))
            {
                return _eventHandlers[defaultKey];
            }

            return null;
        }
    }
}
