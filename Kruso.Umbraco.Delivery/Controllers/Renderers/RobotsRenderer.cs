using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Controllers.Renderers
{
    public class RobotsRenderer
    {
        private readonly IDeliConfig _deliConfig;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliPages _deliPages;

        public RobotsRenderer(IDeliConfig deliConfig, IDeliCulture deliCulture, IDeliRequestAccessor deliRequestAccessor, IDeliPages deliPages)
        {
            _deliConfig = deliConfig;
            _deliCulture = deliCulture;
            _deliRequestAccessor = deliRequestAccessor;
            _deliPages = deliPages;
        }

        public string Create(string culture)
        {
            var deliRequest = _deliRequestAccessor.Current;
            var robotsTxt = string.Empty;
            culture ??= deliRequest.Culture ?? _deliCulture.DefaultCulture;

            var config = _deliConfig.Get();
            if (!string.IsNullOrWhiteSpace(config.RobotsTxtProperty))
            {
                var parts = config.RobotsTxtProperty.Split('.');
                if (parts.Length == 2)
                {
                    var documentType = parts[0];
                    var property = parts[1];

                    _deliCulture.WithCultureContext(culture, () =>
                    {
                        var page = _deliPages.FindStartPageRelative(documentType, culture);
                        if (page != null && page.HasProperty(property))
                        {
                            robotsTxt = page.Value<string>(property);
                        }
                    });
                }
            }

            return robotsTxt;
        }
    }
}
