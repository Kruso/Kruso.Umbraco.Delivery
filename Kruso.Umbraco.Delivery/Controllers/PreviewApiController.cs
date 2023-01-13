using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Web.Common.Controllers;

namespace Kruso.Umbraco.Delivery.Controllers
{
    public class PreviewApiController : UmbracoApiController
    {
        private readonly IDeliContent _deliContent;
        private readonly IDeliCulture _deliCulture;
        private readonly IDeliSecurity _deliSecurity;
        private readonly IDeliUrl _deliUrl;
        private readonly IDeliRequestAccessor _deliRequestAccessor;

        public PreviewApiController(IDeliContent deliContent, IDeliCulture deliCulture, IDeliSecurity deliSecurity, IDeliUrl deliUrl, IDeliRequestAccessor deliRequestAccessor) 
        {
            _deliContent = deliContent;
            _deliCulture = deliCulture;
            _deliSecurity = deliSecurity;
            _deliUrl = deliUrl;
            _deliRequestAccessor = deliRequestAccessor;
        }

        [HttpGet]
        [Route("umbraco/preview")]
        public IActionResult Preview(int? id, bool? init)
        {
            if (init != null && init.HasValue && init == true) 
                return Ok();

            if (id == null || id <= 0)
                return NotFound();

            var content = _deliContent.PublishedContent(id.Value);
            _deliRequestAccessor.Finalize(content, _deliCulture.CurrentCulture);

            var jwt = _deliSecurity.CreateJwtPreviewToken();
            var url = _deliUrl.GetPreviewPaneUrl(jwt);

            return Redirect(url);
        }
    }
}
