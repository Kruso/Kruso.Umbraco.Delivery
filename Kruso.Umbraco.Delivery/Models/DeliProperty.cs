using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Models
{
    internal class DeliProperty : IPublishedProperty
    {
        private readonly IProperty _property;
        private readonly IPropertyType _propertyType;

        private readonly string _alias;
        private readonly object _value;

        internal DeliProperty(IProperty property)
        {
            _property = property;
            _propertyType = property.PropertyType;
        }

        internal DeliProperty(IPropertyType propertyType, string alias, object value)
        {
            _propertyType = propertyType;
            _alias = alias;
            _value = value;
        }

        public IPublishedPropertyType PropertyType => new DeliPropertyType(_propertyType);

        public string Alias => _alias ?? _property.Alias;

        public object GetSourceValue(string culture = null, string segment = null)
        {
            return _value != null
                ? _value
                : _property.GetValue(culture, segment);
        }

        public object GetValue(string culture = null, string segment = null)
        {
            if (_value != null)
                return _value;

            var val = _property.GetValue(culture, segment)?.ToString() ?? string.Empty;
            return val;
        }

        public object GetXPathValue(string culture = null, string segment = null)
        {
            return _value != null
                ? _value
                : _property.GetValue(culture, segment);
        }

        public bool HasValue(string culture = null, string segment = null)
        {
            var val = GetValue(culture, segment);
            return val != null;
        }
    }

}
