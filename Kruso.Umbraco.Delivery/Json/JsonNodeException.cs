using System;
using System.Runtime.Serialization;

namespace Kruso.Umbraco.Delivery.Json
{
    public class JsonNodeException : Exception
    {
        public JsonNodeException()
            : base()
        { }

        public JsonNodeException(string? msg)
            : base(msg)
        { }

        public JsonNodeException(string? msg, Exception? innerException)
            : base(msg, innerException)
        { }

        public JsonNodeException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        { }
    }
}
