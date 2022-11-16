using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Security
{
    public interface IAuthTokenHandler
    {
        ValidateTokenResponse ValidateSingleUseJwtToken(string jwtToken, string issuer, string audience = null, string certificateThumbprint = null);
        ValidateTokenResponse ValidateJwtToken(string jwtToken, string issuer, string audience = null, string certificateThumbprint = null);
        string CreateSingleUseJwtToken(string issuer, string audience, int expires, Claim[] claims, string certificateThumbprint = null);
        string CreateJwtToken(string issuer, string audience, int expires, Claim[] claims, string certificateThumbprint = null);
    }
}
