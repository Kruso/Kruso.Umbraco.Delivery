using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliSecurity
    {
        string CreateJwtPreviewToken();
        JwtSecurityToken ValidateJwtPreviewToken(HttpRequest request, Uri originalUri);
    }
}