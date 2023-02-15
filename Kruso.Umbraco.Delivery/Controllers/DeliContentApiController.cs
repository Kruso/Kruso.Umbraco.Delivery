using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Controllers
{
    public class DeliContentApiController : BaseController
    {
        private readonly PageRenderer _pageRenderer;
        private readonly ChildPageRenderer _childPageRenderer;
        private readonly BlockRenderer _blockRenderer;
        private readonly ManifestRenderer _manifestRenderer;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliContent _deliContent;

        public DeliContentApiController(
            PageRenderer pageRenderer,
            ChildPageRenderer childPageRenderer,
            BlockRenderer blockRenderer,
            ManifestRenderer manifestRenderer,
            IDeliCulture umbCulture,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContent deliContent,
            ILogger<DeliContentApiController> logger)
            : base(umbCulture, logger)
        {
            _pageRenderer = pageRenderer;
            _childPageRenderer = childPageRenderer;
            _blockRenderer = blockRenderer;
            _manifestRenderer = manifestRenderer;

            _deliRequestAccessor = deliRequestAccessor;
            _deliContent = deliContent;
        }

        [HttpGet]
        [Route("api/keepalive/ping")]
        public IActionResult Ping()
        {
            return Ok();
        }

        [HttpGet]
        [Route("api/{culture}/content/{pageId}")]
        public IActionResult Content(string culture, Guid pageId) => Execute(culture, () =>
        {
            var content = _deliContent.PublishedContent(pageId);
            _deliRequestAccessor.Finalize(content, culture);

            var renderResponse = _deliCulture.WithCultureContext(_deliRequestAccessor.Current.Culture, 
                () => _pageRenderer.Render());
            
            return renderResponse.ToActionResult();
        });

        [HttpGet]
        [Route("api/{culture}/content/{pageId}/{blockId}")]
        public IActionResult Content(string culture, Guid pageId, Guid blockId) => Execute(culture, () =>
        {
            var content = _deliContent.PublishedContent(pageId);
            _deliRequestAccessor.Finalize(content, culture);

            var renderResponse = _deliCulture.WithCultureContext(_deliRequestAccessor.Current.Culture, 
                () => _blockRenderer.Render(blockId));

            return renderResponse.ToActionResult();
        });

        [HttpGet]
        [Route("api/{culture}/content/{id}/children")]
        public IActionResult Children(string culture, Guid id, string type = null, int? skip = null, int? page = null, int? pageSize = null) => Execute(culture, () =>
        {
            var content = _deliContent.PublishedContent(id);
            _deliRequestAccessor.Finalize(content, culture);

            var pagination = new Pagination(skip, page, pageSize);
            var renderResponse = _deliCulture.WithCultureContext(_deliRequestAccessor.Current.Culture, 
                () => _childPageRenderer.Render(pagination, type));

            return renderResponse.ToActionResult();
        });

        [HttpGet]
        [Route("api/manifest/")]
        public IActionResult Manifest(string features = null) => Execute(() =>
        {
            return _manifestRenderer.Get(GetFeatures(features)).ToJsonResult();
        });

        [HttpGet]
        [Route("api/{culture}/manifest/")]
        public IActionResult Manifest(string culture, string features = null) => Execute(() =>
        {
            return _manifestRenderer.Get(GetFeatures(features), culture).ToJsonResult();
        });

        private string[] GetFeatures(string features)
        {
            return features
                ?.Split(',')
                ?.Select(x => x.Trim().ToLower())
                ?.ToArray() ?? new string[0];
        }
    }
}