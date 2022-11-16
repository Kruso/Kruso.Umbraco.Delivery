using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliSecurity : IDeliSecurity
    {
        private readonly IAuthTokenHandler _authTokenHandler;
        private readonly IDeliConfig _deliConfig;
        private readonly IDeliRequestAccessor _deliRequestAccessor;

        public DeliSecurity(IDeliRequestAccessor deliRequestAccessor, IDeliConfig deliConfig, IAuthTokenHandler authTokenHandler)
        {
            _authTokenHandler = authTokenHandler;
            _deliConfig = deliConfig;
            _deliRequestAccessor = deliRequestAccessor;
        }

        public string CreateJwtPreviewToken()
        {
            var deliRequest = _deliRequestAccessor.Current;
            var identity = _deliRequestAccessor.Identity;

            var claims = new List<Claim>(identity.Claims)
                .AddClaim(DeliveryClaimTypes.PreviewId, deliRequest.Content.Id)
                .AddClaim(DeliveryClaimTypes.PreviewCulture, deliRequest.Culture);

            var config = _deliConfig.Get();
            var jwt = _authTokenHandler.CreateSingleUseJwtToken(deliRequest.OriginalUri.Authority, deliRequest.CallingUri.Authority, 60, claims.ToArray(), config.CertificateThumbprint);

            return jwt;
        }

        public JwtSecurityToken ValidateJwtPreviewToken(HttpRequest request, Uri originalUri)
        {
            var jwtToken = request.GetJwtBearerToken();
            var tokenResponse = _authTokenHandler.ValidateSingleUseJwtToken(jwtToken, originalUri.Authority, request.AbsoluteUri().Authority);

            return tokenResponse?.ValidatedToken;
        }
    }
}
