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

                var value = property.PropertyType.Variations == ContentVariation.Nothing
                    ? property.GetValue(null, null)
                    : property.GetValue(culture, null);

                if (value == null || (value is string && string.IsNullOrEmpty(value?.ToString())))
                {
                    var fallbackCulture = _deliCulture.GetFallbackCulture(culture);
                    if (!string.IsNullOrEmpty(fallbackCulture))
                    {
                        value = property.PropertyType.Variations == ContentVariation.Nothing
                            ? property.GetValue(null, null)
                            : property.GetValue(fallbackCulture, null);
                    }
                }
                return value;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An unexpected error occurred");
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
