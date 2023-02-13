using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Controllers
{
    internal class DeliContentApiController : BaseController
    {
        private readonly PageRenderer _pageRenderer;
        private readonly ChildPageRenderer _childPageRenderer;
        private readonly BlockRenderer _blockRenderer;
        private readonly ManifestRenderer _manifestRenderer;
        private readonly ActionResultRenderer _actionResultRenderer;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliUrl _deliUrl;
        private readonly IDeliDomain _deliDomain;
        private readonly IDeliContent _deliContent;

        internal DeliContentApiController(
            PageRenderer pageRenderer,
            ChildPageRenderer childPageRenderer,
            BlockRenderer blockRenderer,
            ManifestRenderer manifestRenderer,
            ActionResultRenderer actionResultRenderer,
            IDeliCulture umbCulture,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliUrl deliUrl,
            IDeliDomain deliDomain,
            IDeliContent deliContent,
            ILogger<DeliContentApiController> logger)
            : base(umbCulture, logger)
        {
            _pageRenderer = pageRenderer;
            _childPageRenderer = childPageRenderer;
            _blockRenderer = blockRenderer;
            _manifestRenderer = manifestRenderer;
            _actionResultRenderer = actionResultRenderer;

            _deliRequestAccessor = deliRequestAccessor;
            _deliUrl = deliUrl;
            _deliDomain = deliDomain;
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

            IActionResult result = null;
            _deliCulture.WithCultureContext(_deliRequestAccessor.Current.Culture, () =>
            {
                var res = _pageRenderer.Render();
                result = _actionResultRenderer.ToResult(Response, res);
            });

            return result;
        });

        [HttpGet]
        [Route("api/{culture}/content/{pageId}/{blockId}")]
        public IActionResult Content(string culture, Guid pageId, Guid blockId) => Execute(culture, () =>
        {
            var content = _deliContent.PublishedContent(pageId);
            _deliRequestAccessor.Finalize(content, culture);

            IActionResult result = null;
            _deliCulture.WithCultureContext(_deliRequestAccessor.Current.Culture, () =>
            {
                var res = _blockRenderer.Render(blockId);
                result = _actionResultRenderer.ToResult(Response, res);
            });

            return result;
        });

        [HttpGet]
        [Route("api/{culture}/content/{id}/children")]
        public IActionResult Children(string culture, Guid id, string type = null, int? skip = null, int? page = null, int? pageSize = null) => Execute(culture, () =>
        {
            var content = _deliContent.PublishedContent(id);
            _deliRequestAccessor.Finalize(content, culture);

            IActionResult result = null;
            _deliCulture.WithCultureContext(_deliRequestAccessor.Current.Culture, () =>
            {
                var pagination = new Pagination(skip, page, pageSize);
                var res = _childPageRenderer.Render(pagination, type);
                result = _actionResultRenderer.ToResult(Response, res);
            });

            return result;
        });

        //[HttpGet]
        //[Route("api/content")]
        //public override ContentResult Content(string path)
        //{
        //    var newPath = _deliUrl.RemoveDomainPrefixFromPath(path);
        //    var culture = _deliDomain.GetDomainCulture(path);

        //    var res = Execute(culture, () =>
        //    {
        //        var content = _deliContent.PublishedContent(newPath, culture);

        //        _deliRequestAccessor.Finalize(content, culture);

        //        IActionResult result = null;
        //        _deliCulture.WithCultureContext(_deliRequestAccessor.Current.Culture, () =>
        //        {
        //            var res = _pageRenderer.Render();
        //            result = _actionResultRenderer.ToResult(Response, res);
        //        });

        //        return result;
        //    });

        //    return res as ContentResult;
        //}

        [HttpGet]
        [Route("api/manifest/")]
        public IActionResult Manifest(string features = null) => Execute(() =>
        {
            JsonNode res = _manifestRenderer.Get(GetFeatures(features));
            return _actionResultRenderer.ToJsonResult(Response, res);
        });

        [HttpGet]
        [Route("api/{culture}/manifest/")]
        public IActionResult Manifest(string culture, string features = null) => Execute(() =>
        {
            JsonNode res = _manifestRenderer.Get(GetFeatures(features), culture);
            return _actionResultRenderer.ToJsonResult(Response, res);
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