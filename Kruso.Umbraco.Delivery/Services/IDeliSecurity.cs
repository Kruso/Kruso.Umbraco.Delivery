using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliSecurity
    {
        string CreateJwtPreviewToken(string issuer, string audience = null);
        JwtSecurityToken ValidateJwtPreviewToken(string jwtToken, string issuer, string audience = null);
    }
}