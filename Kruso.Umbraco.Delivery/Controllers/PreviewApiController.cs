﻿using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Routing.Implementation;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Controllers;
using UmbCore = Umbraco.Cms.Core;

namespace Kruso.Umbraco.Delivery.Controllers
{
    public class PreviewApiController : UmbracoApiController
    {
        private readonly IDeliContent _deliContent;
        private readonly IDeliSecurity _deliSecurity;
        private readonly IDeliUrl _deliUrl;
        private readonly IDeliRequestAccessor _deliRequestAccessor;

        private readonly IBackOfficeSecurityAccessor _backofficeSecurityAccessor;
        private readonly UmbCore.Web.ICookieManager _cookieManager;

        public PreviewApiController(
            IDeliContent deliContent, 
            IDeliSecurity deliSecurity, 
            IDeliUrl deliUrl, 
            IDeliRequestAccessor deliRequestAccessor,

            IBackOfficeSecurityAccessor backofficeSecurityAccessor,
            UmbCore.Web.ICookieManager cookieManager) 
        {
            _deliContent = deliContent;
            _deliSecurity = deliSecurity;
            _deliUrl = deliUrl;
            _deliRequestAccessor = deliRequestAccessor;

            _backofficeSecurityAccessor = backofficeSecurityAccessor;
            _cookieManager = cookieManager;
        }

        /// <summary>
        ///     The endpoint that is loaded within the preview iframe
        /// </summary>
        [HttpGet]
        [Route("umbraco/preview/frame")]
        [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        public ActionResult Frame(int id, string culture)
        {
            EnterPreview(id);

            var content = _deliContent.PublishedContent(id);
            _deliRequestAccessor.Finalize(content, culture);

            var deliRequest = _deliRequestAccessor.Current;

            var jwt = _deliSecurity.CreateJwtPreviewToken(deliRequest.OriginalUri.Authority, deliRequest.CallingUri.Authority);
            var url = _deliUrl.GetPreviewPaneUrl(jwt);

            return Redirect(url);
        }

        public ActionResult? EnterPreview(int id)
        {
            IUser? user = _backofficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            _cookieManager.SetCookieValue(UmbCore.Constants.Web.PreviewCookieName, "preview");

            return new EmptyResult();
        }
    }
}
