using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace Kruso.Umbraco.Delivery.Controllers
{
    public class DeliRenderController : RenderController
    {
        private readonly PageRenderer _pageRenderer;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliTemplates _deliTemplates;

        public DeliRenderController(
            ILogger<RenderController> logger, 
            ICompositeViewEngine compositeViewEngine, 
            IUmbracoContextAccessor umbracoContextAccessor, 
            PageRenderer pageRenderer,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliTemplates deliTemplates
            )
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _pageRenderer = pageRenderer;
            _deliRequestAccessor = deliRequestAccessor;
            _deliTemplates = deliTemplates;
        }

        public override IActionResult Index()
        {
            var deliRequest = _deliRequestAccessor.Current;

            return _deliTemplates.IsJsonTemplate(deliRequest?.Content)
                ? _pageRenderer.Render().ToActionResult()
                : base.Index();
        }
    }
}
