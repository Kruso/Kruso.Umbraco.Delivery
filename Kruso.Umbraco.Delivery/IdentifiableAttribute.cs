using System;
using System.Linq;

namespace Kruso.Umbraco.Delivery
{
    public abstract class IdentifiableAttribute : Attribute
    {
        public string[] Components { get; protected set; }

        public IdentifiableAttribute(string component)
        {
            Components = new[] { component.ToLower() };
        }

        public IdentifiableAttribute(string[] components)
        {
            Components = components.Select(x => x.ToLower()).ToArray();
        }
    }

}
