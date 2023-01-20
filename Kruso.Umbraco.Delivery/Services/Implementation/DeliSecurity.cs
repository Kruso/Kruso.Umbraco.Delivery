using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliSecurity : IDeliSecurity
    {
        private readonly IAuthTokenHandler _authTokenHandler;
        private readonly IDeliConfig _deliConfig;
        private readonly IServiceProvider _serviceProvider;

        public DeliSecurity(IDeliConfig deliConfig, IAuthTokenHandler authTokenHandler, IServiceProvider serviceProvider)
        {
            _authTokenHandler = authTokenHandler;
            _deliConfig = deliConfig;
            _serviceProvider = serviceProvider;
        }

        public string CreateJwtPreviewToken(string issuer, string audience = null)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var deliRequestAccessor = scope.ServiceProvider.GetService<IDeliRequestAccessor>();
                var deliRequest = deliRequestAccessor.Current;
                var identity = deliRequestAccessor.Identity;

                var claims = new List<Claim>(identity.Claims)
                    .AddClaim(DeliveryClaimTypes.PreviewId, deliRequest.Content.Id)
                    .AddClaim(DeliveryClaimTypes.PreviewCulture, deliRequest.Culture);

                var config = _deliConfig.Get();
                var jwt = _authTokenHandler.CreateSingleUseJwtToken(deliRequest.OriginalUri.Authority, deliRequest.CallingUri.Authority, 60, claims.ToArray());

                return jwt;
            }
        }

        public JwtSecurityToken ValidateJwtPreviewToken(string jwtToken, string issuer, string audience = null)
        {
            var tokenResponse = _authTokenHandler.ValidateSingleUseJwtToken(jwtToken, issuer, audience);

            return tokenResponse?.ValidatedToken;
        }
    }
}
