using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Kruso.Umbraco.Delivery.Security
{
    public class AuthTokenHandler : IAuthTokenHandler
    {
        private readonly IDeliCache _deliCache;
        private readonly ICertificateHandler _certificateHandler;
        private readonly ILogger<AuthTokenHandler> _logger;

        public AuthTokenHandler(IDeliCache deliCache, ICertificateHandler certificateHandler, ILogger<AuthTokenHandler> logger)
        {
            _deliCache = deliCache;
            _certificateHandler = certificateHandler;
            _logger = logger;
        }

        public string CreateSingleUseJwtToken(string issuer, string audience, int expires, Claim[] claims, string certificateThumbprint = null)
        {
            var jwtToken = CreateJwtToken(issuer, audience, expires, claims, certificateThumbprint);
            _deliCache.AddToMemory(jwtToken, jwtToken);

            return jwtToken;
        }

        public string CreateJwtToken(string issuer, string audience, int expires, Claim[] claims, string certificateThumbprint = null)
        {
            using (var certificate = _certificateHandler.GetCertificate(certificateThumbprint))
            {
                var key = new X509SecurityKey(certificate);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddSeconds(expires),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                _logger.LogInformation($"Jwt token {ObfuscateToken(jwtToken)} created");
                
                return jwtToken;
            }
        }

        public ValidateTokenResponse ValidateSingleUseJwtToken(string jwtToken, string issuer, string audience = null, string certificateThumbprint = null)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                return new ValidateTokenResponse { Message = "No single use access token" };
            }

            if (!_deliCache.RemoveFromMemory(jwtToken))
            {
                _logger.LogInformation($"Jwt single use token {ObfuscateToken(jwtToken)} already used or doesn't exist");
                return new ValidateTokenResponse { Message = "Jwt single use token already used or doesn't exist" };
            }

            return ValidateJwtToken(jwtToken, issuer, audience, certificateThumbprint);
        }

        public ValidateTokenResponse ValidateJwtToken(string jwtToken, string issuer, string audience = null, string certificateThumbprint = null)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                return new ValidateTokenResponse { Message = "No access token" };
            }

            JwtSecurityToken validatedToken = null;
            try
            {
                using (var certificate = _certificateHandler.GetCertificate())
                {
                    var key = new X509SecurityKey(certificate);
                    var tokenHandler = new JwtSecurityTokenHandler();

                    tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = audience != null,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = key,
                    }, out SecurityToken securityToken);

                    validatedToken = securityToken as JwtSecurityToken;
                    if (validatedToken == null)
                    {
                        _logger.LogError($"Access token {ObfuscateToken(jwtToken)} is invalid");
                        return new ValidateTokenResponse { Message = "Access token is invalid" };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred when decrypting access token {ObfuscateToken(jwtToken)}");
                return new ValidateTokenResponse { Message = "An error occurred when decrypting the access token" };
            }

            return new ValidateTokenResponse
            {
                Succeeded = true,
                ValidatedToken = validatedToken
            };
        }

        private string ObfuscateToken(string jwtToken)
        {
            return !string.IsNullOrEmpty(jwtToken)
                ? $"{jwtToken.Substring(0, 6)}****{jwtToken.Substring(jwtToken.Length - 6)}"
                : null;
        }
    }
}
