using Kruso.Umbraco.Delivery.Controllers.Renderers;
using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
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
        private readonly IDeliContent _deliContent;

        public DeliRenderController(
            ILogger<RenderController> logger, 
            ICompositeViewEngine compositeViewEngine, 
            IUmbracoContextAccessor umbracoContextAccessor, 
            PageRenderer pageRenderer,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContent deliContent
            )
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _pageRenderer = pageRenderer;
            _deliRequestAccessor = deliRequestAccessor;
            _deliContent = deliContent;
        }

        public override IActionResult Index()
        {
            var deliRequest = _deliRequestAccessor.Current;
            if (_deliContent.IsJsonTemplate(deliRequest?.Content))
            {
                var renderModel = _pageRenderer.Render();

                if (renderModel == null)
                    return NotFound();

                Response.StatusCode = (int)renderModel.StatusCode;
                var response = renderModel.Model != null
                    ? renderModel.Model.Clone(deliRequest?.ModelFactoryOptions.IncludeFields, deliRequest?.ModelFactoryOptions.ExcludeFields).ToString()
                    : renderModel.Data;
                
                return Content(response, renderModel.ContentType);
            }
            else
            {
                return base.Index();
            }
        }
    }
}
