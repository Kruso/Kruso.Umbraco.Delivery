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
    public class DeliContentFinderByPreviewUrl : IContentFinder
    {
        private readonly ILogger<ContentFinderByIdPath> _logger;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliContentLoader _deliContentLoader;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliUrl _deliUrl;
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFinderByIdPath"/> class.
        /// </summary>
        public DeliContentFinderByPreviewUrl(
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContentLoader deliContentLoader,
            IDeliCulture deliCulture,
            IDeliUrl deliUrl,
            ILogger<ContentFinderByIdPath> logger)
        {
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
            if (!TryGetContent(path, out var id, out var culture))
            {
                _logger.LogDebug("Path {path} is not a valid format for a preview request", path);
                return Task.FromResult(false);
            }

            if (_deliRequestAccessor.Current == null)
            {
                _logger.LogDebug("There is no valid delivery request object initialized", path);
                return Task.FromResult(false);
            }

            var content = _deliContentLoader.FindContentById(id, culture, true);
            if (content == null)
            {
                _logger.LogDebug("Request {path} did not provide a valid content id", path);
                return Task.FromResult(false);
            }

            var callingUrl = _deliUrl.GetAbsoluteDeliveryUrl(content, culture);
            if (string.IsNullOrEmpty(callingUrl))
            {
                _logger.LogDebug("Request {path} provided content did not have a valid url", path);
                return Task.FromResult(false);
            }

            var deliRequest = _deliRequestAccessor.Finalize(content, culture, new Uri(callingUrl));
            if (!deliRequest.IsValidPreviewRequest())
            {
                _logger.LogDebug("Request {path} is not a valid preview request", path);
                _deliRequestAccessor.Unfinalize();
                return Task.FromResult(false);
            }

            frequest.SetCulture(culture);
            frequest.SetPublishedContent(content);

            return Task.FromResult(true);
        }

        private bool TryGetContent(string path, out int contentId, out string culture)
        {
            contentId = -1;
            culture = null;

            var parts = path.Split('/');

            if (parts.Length != 2)
                return false;

            if (!_deliCulture.IsCultureSupported(parts[0]))
                return false;

            if (!int.TryParse(parts[1], out var id))
                return false;

            contentId = id;
            culture = parts[0];

            return true;
        }
    }
}
