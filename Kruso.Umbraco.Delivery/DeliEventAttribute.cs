using Kruso.Umbraco.Delivery.Extensions;
using System;
using System.Linq;

namespace Kruso.Umbraco.Delivery
{
    /// <summary>
    /// Mark your custom class as a ModelFactory (also have the class implement the IModelFactory interface) using this attribute. 
    /// Your class will replace the default one used to generate Page, Block or Navigation objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DeliEventAttribute : IdentifiableAttribute
    {
        public EventType EventType { get; private set; }

        public DeliEventAttribute(EventType eventType)
            : base(eventType.MakeKeys())
        {
            EventType = eventType;
        }

        public DeliEventAttribute(EventType eventType, params string[] documentTypes)
            : base(eventType.MakeKeys(documentTypes))
        {
            EventType = eventType;
        }
    }
}
