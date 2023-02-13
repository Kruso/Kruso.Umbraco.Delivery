using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace Kruso.Umbraco.Delivery.Controllers
{
    internal class DeliRenderController : RenderController
    {
        private readonly PageRenderer _pageRenderer;
        private readonly ActionResultRenderer _actionResultRenderer;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliTemplates _deliTemplates;
        private readonly IDeliConfig _deliConfig;

        internal DeliRenderController(
            ILogger<RenderController> logger, 
            ICompositeViewEngine compositeViewEngine, 
            IUmbracoContextAccessor umbracoContextAccessor, 
            PageRenderer pageRenderer,
            ActionResultRenderer actionResultRenderer,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliTemplates deliTemplates,
            IDeliConfig deliConfig
            )
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _pageRenderer = pageRenderer;
            _actionResultRenderer = actionResultRenderer;
            _deliRequestAccessor = deliRequestAccessor;
            _deliTemplates = deliTemplates;
            _deliConfig = deliConfig;
        }

        public override IActionResult Index()
        {
            var deliRequest = _deliRequestAccessor.Current;
            if (_deliTemplates.IsJsonTemplate(deliRequest?.Content))
            {
                var renderResponse = _pageRenderer.Render();
                return _actionResultRenderer.ToResult(Response, renderResponse);
            }
            else
            {
                return base.Index();
            }
        }
    }
}
