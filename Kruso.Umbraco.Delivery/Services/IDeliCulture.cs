using Kruso.Umbraco.Delivery.Json;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliCulture
    {
        string[] SupportedCultures { get; }
        string DefaultFallbackCulture { get; }
        string DefaultCulture { get; }

        IEnumerable<string> GetCultures(IPublishedContent content);
        bool IsCultureSupported(string culture);
        string GetFallbackCulture(string culture);
        bool IsPublishedInCulture(IPublishedContent content, string culture);
        void WithCultureContext(string culture, Action action);
        JsonNode GetCultureInfo(string culture);
    }
}