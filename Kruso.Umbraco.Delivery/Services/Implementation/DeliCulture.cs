using Kruso.Umbraco.Delivery.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliCulture : IDeliCulture
    {
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly ILocalizationService _localizationService;

        public DeliCulture(
            IVariationContextAccessor variationContextAccessor, 
            ILocalizationService localizationService)
        {
            _variationContextAccessor = variationContextAccessor;
            _localizationService = localizationService;
        }

        public string[] SupportedCultures
        {
            get
            {
                var languages = GetAllLanguages();
                return languages
                    .Select(x => x.CultureInfo.Name)
                    .ToArray();
            }
        }

        public string DefaultCulture
        {
            get
            {
                var lang = GetAllLanguages()
                    .FirstOrDefault(x => x.IsDefault)?.CultureInfo.Name;

                return lang;
            }
        }

        public string DefaultFallbackCulture
        {
            get
            {
                return GetFallbackCulture(DefaultCulture);
            }
        }

        public bool IsCultureSupported(string culture)
        {
            return SupportedCultures.Contains(culture);
        }

        public string GetFallbackCulture(string culture)
        {
            var fallbackLanguage = _localizationService.GetLanguageByIsoCode(culture);
            return fallbackLanguage?.CultureInfo?.Name;
        }

        public bool IsPublishedInCulture(IPublishedContent content, string culture)
        {
            return GetCultures(content).Any(x => x.Equals(culture, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<string> GetCultures(IPublishedContent content)
        {
            if (content == null)
            {
                return Enumerable.Empty<string>();
            }

            var res = content.Cultures.Values
                .Where(x => !string.IsNullOrEmpty(x.Culture))
                .Select(x => x.Culture)
                .ToList()
                ?? new List<string>();

            if (!res.Any())
            {
                var all = GetAllLanguages();
                var lang = all.FirstOrDefault(x => x.IsDefault) ?? all.FirstOrDefault();

                if (lang != null)
                {
                    res.Add(lang.CultureInfo.Name);
                }
            }

            return res.ToList();
        }


        public void WithCultureContext(string culture, Action action)
        {
            if (action != null)
            {
                var oldCulture = _variationContextAccessor.VariationContext?.Culture;

                try
                {
                    if (!string.IsNullOrEmpty(culture) && culture != oldCulture)
                    {
                        _variationContextAccessor.VariationContext = new VariationContext(culture);
                    }
                    
                    action();
                }
                finally
                {
                    if (!string.IsNullOrEmpty(culture) && culture != oldCulture)
                    {
                        _variationContextAccessor.VariationContext = new VariationContext(oldCulture);
                    }
                }
            }
        }

        public JsonNode GetCultureInfo(string culture)
        {
            var allLanguages = _localizationService.GetAllLanguages();
            var lang = allLanguages.FirstOrDefault(x => x.CultureInfo.Name == culture);
            if (lang != null)
            {
                var fallbackCulture = string.Empty;
                if (lang.FallbackLanguageId.HasValue)
                {
                    var fallbackLanguage = allLanguages.FirstOrDefault(x => x.Id == lang.FallbackLanguageId);
                    if (fallbackLanguage != null)
                        fallbackCulture = fallbackLanguage.CultureInfo.Name;
                }

                return new JsonNode()
                    .AddProp("isDefault", lang.IsDefault)
                    .AddProp("culture", lang.CultureInfo.Name)
                    .AddProp("nameNative", lang.CultureInfo.NativeName)
                    .AddProp("nameEnglish", lang.CultureInfo.EnglishName)
                    .AddProp("fallbackCulture", fallbackCulture);
            }

            return null;
        }

        private IEnumerable<ILanguage> GetAllLanguages()
        {
            return _localizationService.GetAllLanguages();
        }
    }
}
