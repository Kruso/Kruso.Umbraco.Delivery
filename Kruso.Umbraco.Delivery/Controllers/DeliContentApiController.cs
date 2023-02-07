using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Routing.Implementation;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Kruso.Umbraco.Delivery.Controllers
{
    public class DeliContentApiController : BaseController
    {
        private readonly PageRenderer _pageRenderer;
        private readonly ManifestRenderer _manifestRenderer;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliUrl _deliUrl;
        private readonly IDeliDomain _deliDomain;
        private readonly IDeliContent _deliContent;

        public DeliContentApiController(
            PageRenderer pageRenderer, 
            ManifestRenderer manifestRenderer, 
            IDeliCulture umbCulture,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliUrl deliUrl,
            IDeliDomain deliDomain,
            IDeliContent deliContent,
            ILogger<DeliContentApiController> logger)
            : base(umbCulture, logger)
        {
            _pageRenderer = pageRenderer;
            _manifestRenderer = manifestRenderer;
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
        public IActionResult Content(string culture, Guid pageId)
        {
            return Execute(culture, () =>
            {
                var content = _deliContent.PublishedContent(pageId);
                _deliRequestAccessor.Finalize(content, culture);
                return GetPageContent();
            });
        }

        [HttpGet]
        [Route("api/{culture}/content/{pageId}/{blockId}")]
        public IActionResult Content(string culture, Guid pageId, Guid blockId)
        {
            return Execute(culture, () =>
            {
                var content = _deliContent.PublishedContent(pageId);
                _deliRequestAccessor.Finalize(content, culture);
                return GetBlockContent(blockId);
            });
        }

        [HttpGet]
        [Route("api/{culture}/content/{id}/children")]
        public IActionResult Children(string culture, Guid id, string type = null, int? skip = null, int? page = null, int? pageSize = null)
        {
            return Execute(culture, () =>
            {
                var content = _deliContent.PublishedContent(id);
                _deliRequestAccessor.Finalize(content, culture);
                return CreateChildContent(type, skip, page, pageSize);
            });
        }

        [HttpGet]
        [Route("api/content")]
        public new IActionResult Content(string path)
        {
            return Execute(() =>
            {
                var newPath = _deliUrl.RemoveDomainPrefixFromPath(path);
                var culture = _deliDomain.GetDomainCulture(path);
                var content = _deliContent.PublishedContent(newPath, culture);

                _deliRequestAccessor.Finalize(content, culture);
                return GetPageContent();
            });
        }

        [HttpGet]
        [Route("api/manifest/")]
        public IActionResult Manifest(string features = null)
        {
            return Execute(() =>
            {
                JsonNode res = _manifestRenderer.Get(GetFeatures(features));

                return new ApiResponse(res);
            });
        }

        [HttpGet]
        [Route("api/{culture}/manifest/")]
        public IActionResult Manifest(string culture, string features = null)
        {
            return Execute(() =>
            {
                JsonNode res = _manifestRenderer.Get(GetFeatures(features), culture);

                return new ApiResponse(res);
            });
        }

        private ApiResponse GetPageContent()
        {
            var res = new ApiResponse();
            var deliRequest = _deliRequestAccessor.Current;

            _deliCulture.WithCultureContext(deliRequest.Culture, () =>
            {
                var renderModel = _pageRenderer.Render();

                var page = renderModel?.Model.Clone(deliRequest.ModelFactoryOptions.IncludeFields, deliRequest.ModelFactoryOptions.ExcludeFields);

                res.Payload = page;
                res.StatusCode = renderModel?.StatusCode ?? HttpStatusCode.NotFound;
            });

            return res;
        }

        private ApiResponse GetBlockContent(Guid blockId)
        {
            var res = new ApiResponse();

            if (blockId != Guid.Empty)
            {
                var deliRequest = _deliRequestAccessor.Current;
                _deliCulture.WithCultureContext(deliRequest.Culture, () =>
                {
                    var renderModel = _pageRenderer.Render();

                    var block = renderModel.StatusCode == HttpStatusCode.OK
                        ? renderModel.Model.Blocks().FirstOrDefault(x => x.Id == blockId)
                    : null;

                    block = block.Clone(deliRequest.ModelFactoryOptions.IncludeFields, deliRequest.ModelFactoryOptions.ExcludeFields);

                    var statusCode = block != null
                        ? HttpStatusCode.OK
                        : (renderModel?.StatusCode ?? HttpStatusCode.NotFound);

                    res.Payload = block;
                    res.StatusCode = statusCode;
                });
            }

            return res;
        }

        private ApiResponse CreateChildContent(string type, int? skip, int? page, int? pageSize)
        {
            var response = GetPageContent();
            if (response.StatusCode != HttpStatusCode.OK)
                return response;

            var pagination = new Pagination(skip, page, pageSize);
            var childPages = new List<JsonNode>();

            var deliRequest = _deliRequestAccessor.Current;
            _deliCulture.WithCultureContext(deliRequest.Culture, () =>
            {
                var children = pagination.Paginate(deliRequest.Content.Children, 
                    (item) => string.IsNullOrEmpty(type) || item.ContentType.Alias.Equals(type, StringComparison.InvariantCultureIgnoreCase));

                foreach (var child in children)
                {
                    _deliRequestAccessor.Finalize(child, deliRequest.Culture);
                    response = GetPageContent();
                    if (response.StatusCode == HttpStatusCode.OK && response.Payload is JsonNode payload)
                    {
                        childPages.Add(payload);
                    }
                }
            });

            response = new ApiResponse
            {
                Payload = new JsonNode()
                    .AddProp("statusCode", HttpStatusCode.OK)
                    .AddProp("message", "")
                    .AddProp("pagination", pagination)
                    .AddProp("items", childPages),
                StatusCode = HttpStatusCode.OK
            };

            return response;
        }

        private string[] GetFeatures(string features)
        {
            return features
                ?.Split(',')
                ?.Select(x => x.Trim().ToLower())
                ?.ToArray() ?? new string[0];
        }
    }
}