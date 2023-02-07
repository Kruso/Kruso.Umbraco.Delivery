using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Security
{
    public static class ClaimsExtensions
    {
        public static string[] ValuesOfType(this IEnumerable<Claim> claims, string claimType)
        {
            return claims
                .Where(x => x.Type == claimType)
                .Select(x => x.Value)
                .ToArray();
        }

        public static string ValueOfType(this IEnumerable<Claim> claims, string claimType)
        {
            return claims.FirstOrDefault(x => x.Type == claimType)?.Value;
        }

        public static string Username(this IEnumerable<Claim> claims)
        {
            var res = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)
                ?? claims.FirstOrDefault(x => x.Type == Constants.Claims.PreferredUserName)
                ?? claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)
                ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return res?.Value;
        }

        public static string Username(this ClaimsPrincipal identity)
        {
            return identity?.Claims.Username();
        }

        public static string Username(this ClaimsIdentity identity)
        {
            return identity?.Claims.Username();
        }

        public static string FirstName(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value
                ?? claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
        }

        public static string LastName(this IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value
                ?? claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
        }

        public static string FullName(this IEnumerable<Claim> claims)
        {
            return $"{claims.FirstName()} {claims.LastName()}".Trim();
        }

        public static string FullName(this ClaimsPrincipal identity)
        {
            return identity?.Claims.FullName();
        }

        public static string FullName(this ClaimsIdentity identity)
        {
            return identity?.Claims.FullName();
        }

        public static string Email(this IEnumerable<Claim> claims)
        {
            return claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value
                ?? claims?.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value;
        }

        public static string Email(this ClaimsPrincipal identity)
        {
            return identity?.Claims.Email();
        }

        public static string Email(this ClaimsIdentity identity)
        {
            return identity?.Claims.Email();
        }

        public static string[] Roles(this IEnumerable<Claim> claims)
        {
            return claims?
                .Where(x => x.Type == ClaimTypes.Role || x.Type == JwtClaimTypes.Role)
                .Select(x => x.Value)
                .ToArray() ?? new string[0];
        }

        public static string[] Roles(this ClaimsPrincipal identity)
        {
            return identity?.Claims?.Roles() ?? new string[0];
        }

        public static string[] Roles(this ClaimsIdentity identity)
        {
            return identity?.Claims?.Roles() ?? new string[0];
        }

        public static Dictionary<string, object> AddClaim(this Dictionary<string, object> claims, string claimType, object value)
        {
            if (claims != null && !claims.ContainsKey(claimType) && value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                claims.Add(claimType, value);
            }

            return claims;
        }

        public static List<Claim> AddClaim(this List<Claim> claims, string claimType, object value)
        {
            if (claims != null && value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                claims.Add(new Claim(claimType, value.ToString()));
            }

            return claims;
        }
    }
}
