using Kruso.Umbraco.Delivery.Security;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace Kruso.Umbraco.Delivery.Routing
{
    public class DeliContentFinderByIdPath : IContentFinder
    {
        private readonly ILogger<ContentFinderByIdPath> _logger;
        private readonly IRequestAccessor _requestAccessor;
        private readonly WebRoutingSettings _webRoutingSettings;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliContentLoader _deliContentLoader;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliUrl _deliUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFinderByIdPath"/> class.
        /// </summary>
        public DeliContentFinderByIdPath(
            IOptions<WebRoutingSettings> webRoutingSettings,
            IRequestAccessor requestAccessor,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContentLoader deliContentLoader,
            IDeliCulture deliCulture,
            IDeliUrl deliUrl,
            ILogger<ContentFinderByIdPath> logger)
        {
            _webRoutingSettings = webRoutingSettings.Value ?? throw new System.ArgumentNullException(nameof(webRoutingSettings));
            _requestAccessor = requestAccessor ?? throw new System.ArgumentNullException(nameof(requestAccessor));
            _deliRequestAccessor = deliRequestAccessor ?? throw new System.ArgumentNullException(nameof(deliRequestAccessor));
            _deliContentLoader = deliContentLoader ?? throw new System.ArgumentNullException(nameof(deliContentLoader));
            _deliCulture = deliCulture ?? throw new System.ArgumentNullException(nameof(deliCulture));
            _deliUrl = deliUrl ?? throw new System.ArgumentNullException(nameof(deliUrl));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Tries to find and assign an Umbraco document to a <c>PublishedRequest</c>.
        /// </summary>
        /// <param name="frequest">The <c>PublishedRequest</c>.</param>
        /// <returns>A value indicating whether an Umbraco document was found and assigned.</returns>
        public Task<bool> TryFindContent(IPublishedRequestBuilder frequest)
        {
            var path = frequest.AbsolutePathDecoded.Trim('/');
            if (!int.TryParse(path, out var id))
            {
                _logger.LogDebug("Path {path} is not a valid content id", path);
                return Task.FromResult(false);
            }

            if (_webRoutingSettings.DisableFindContentByIdPath)
            { 
                return Task.FromResult(false);
            }

            var culture = _requestAccessor.GetQueryStringValue("culture");
            if (string.IsNullOrEmpty(culture))
                culture = _deliCulture.CurrentCulture;

            var content = _deliContentLoader.FindContentById(id, culture);
            if (content != null && _deliRequestAccessor.Identity.UserType == UserType.BackOffice)
            {
                frequest.SetCulture(culture);
                frequest.SetPublishedContent(content);

                _deliRequestAccessor.Finalize(content, culture);

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
