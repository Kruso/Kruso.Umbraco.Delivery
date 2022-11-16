using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Security
{
    public class ValidateTokenResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public JwtSecurityToken ValidatedToken { get; set; }
    }
}
