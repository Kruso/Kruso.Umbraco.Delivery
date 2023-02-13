﻿using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kruso.Umbraco.Delivery.Controllers
{
    internal class DeliSeoApiController : BaseController
    {
        private readonly SitemapRenderer _sitemapRenderer;
        private readonly RobotsRenderer _robotsRenderer;
        private readonly ActionResultRenderer _actionResultRenderer;

        internal DeliSeoApiController(
            SitemapRenderer sitemapRenderer,
            RobotsRenderer robotsRenderer,
            ActionResultRenderer actionResultRenderer,
            IDeliCulture umbCulture,
            ILogger<DeliSeoApiController> logger)
            : base(umbCulture, logger)
        {
            _sitemapRenderer = sitemapRenderer;
            _robotsRenderer = robotsRenderer;
            _actionResultRenderer = actionResultRenderer;
        }

        [HttpGet]
        [Route("api/sitemap/")]
        public IActionResult Sitemap()
        {
            var sitemapXml = _sitemapRenderer.Create(null);
            return new ContentResult
            {
                Content = sitemapXml,
                ContentType = "application/xml"
            };
        }

        [HttpGet]
        [Route("api/{culture}/sitemap/")]
        public IActionResult Sitemap(string culture)
        {
            var sitemapXml = _sitemapRenderer.Create(culture);
            return new ContentResult
            {
                Content = sitemapXml, 
                ContentType = "application/xml"
            };
        }

        [HttpGet]
        [Route("api/{culture}/robots/")]
        public IActionResult Robots(string culture = null)
        {
            var robotsTxt = _robotsRenderer.Create(culture);
            return new ContentResult
            {
                Content = robotsTxt, 
                ContentType = "text/plain"
            };
        }
    }
}
