using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliProperties : IDeliProperties
    {
        private readonly IDeliCulture _deliCulture;
        private readonly ILogger<DeliProperties> _log;

        public DeliProperties(IDeliCulture deliCulture, ILogger<DeliProperties> log)
        {
            _deliCulture = deliCulture;
            _log = log;
        }

        public object Value(IPublishedContent model, string alias, string culture)
        {
            var prop = model.GetProperty(alias);
            return prop != null ? Value(prop, culture) : null;
        }

        public object Value(IPublishedProperty property, string culture)
        {
            try
            {
                if (property == null)
                    return null;

                var value = InternalValue(property, culture);
                if (IsEmptyValue(value))
                {
                    var fallbackCulture = _deliCulture.GetFallbackCulture(culture);
                    if (!string.IsNullOrEmpty(fallbackCulture))
                        value = InternalValue(property, fallbackCulture);
                }

                if (IsEmptyValue(value))
                    value = InternalValue(property);

                return value;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An unexpected error occurred");
                return null;
            }
        }

        public bool AllEmptyProperties(IPublishedElement publishedContent, string? culture = null)
        {
            if (publishedContent == null)
                return true;

            foreach (var property in publishedContent.Properties)
            {
                var val = Value(property, culture);
                if (!IsEmptyValue(val))
                    return false;
            }

            return true;
        }

        private bool IsEmptyValue(object? value) => value == null || (value is string && string.IsNullOrEmpty(value?.ToString()));

        private object? InternalValue(IPublishedProperty property, string? culture = null)
        {
            try
            {
                if (property == null)
                    return null;

                return property.PropertyType.Variations == ContentVariation.Nothing || string.IsNullOrEmpty(culture)
                    ? property.GetValue()
                    : property.GetValue(culture, null);
			}
            catch (Exception ex)
            {
                _log.LogError(ex, $"An unexpected error occurred getting property value {property.Alias} with culture {culture}");
                return null;
            }
        }

        public string Value(JObject jObject, string prop)
        {
            return jObject?.Properties().FirstOrDefault(x => x.Name == prop)?.Value.ToString();
        }

        public IEnumerable<T> PublishedContentValue<T>(IPublishedProperty property, string culture)
            where T : IPublishedElement
        {
            var res = new List<T>();
            if (Value(property, culture) is T item)
            {
                res.Add(item);
            }
            else if (Value(property, culture) is IEnumerable<T> items)
            {
                res.AddRange(items);
            }

            return res;
        }
    }
}
