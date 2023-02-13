using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using UmbCore = Umbraco.Cms.Core;

namespace Kruso.Umbraco.Delivery.Security
{
    public class DefaultIdentity : IUserIdentity
    {
        private readonly IPublicAccessService _publicAccessService;
        private readonly IOptionsSnapshot<CookieAuthenticationOptions> _cookieOptionsSnapshot;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Claim[] Claims { get; private set; }

        public string UserName => Claims.Username();

        public string Email => Claims.Email();

        public bool IsAuthenticated => !string.IsNullOrEmpty(Claims.Username());

        public virtual string[] Roles => new string[0];

        public virtual string[] Groups => Claims.Roles();

        public UserType UserType { get; private set; }

        public string Name => Claims.FullName();

        public string FirstName => Claims.FirstName();

        public string LastName => Claims.LastName();

        public DefaultIdentity(IHttpContextAccessor httpContextAccessor, IMemberManager memberManager, IOptionsSnapshot<CookieAuthenticationOptions> cookieOptionsSnapshot, IPublicAccessService publicAccessService)
        {
            UserType = UserType.Visitor;

            _httpContextAccessor = httpContextAccessor;
            _cookieOptionsSnapshot = cookieOptionsSnapshot;
            _publicAccessService = publicAccessService;

            var claims = new List<Claim>();
            var umbUser = GetBackOfficeUser();
            if (umbUser != null)
            {
                UserType = UserType.BackOffice;
                claims
                    .AddClaim(Constants.Claims.PreferredUserName, umbUser.GetUsername())
                    .AddClaim(JwtClaimTypes.Email, umbUser.GetEmail())
                    .AddClaim(JwtClaimTypes.GivenName, umbUser.GetRealName()?.Split(' ').FirstOrDefault())
                    .AddClaim(JwtClaimTypes.FamilyName, umbUser.GetRealName()?.Split(' ').LastOrDefault());

                if (umbUser.GetRoles()?.Any() ?? false)
                {
                    foreach (var role in umbUser.GetRoles())
                    {
                        claims.AddClaim(JwtClaimTypes.Role, role);
                    }
                }
            }

            var umbMember = memberManager.GetCurrentMemberAsync().GetAwaiter().GetResult();
            if (umbMember != null)
            {
                claims
                    .AddClaim(Constants.Claims.PreferredUserName, umbMember.UserName)
                    .AddClaim(JwtClaimTypes.Email, umbMember.Email)
                    .AddClaim(JwtClaimTypes.GivenName, umbMember.Name?.Split(' ').FirstOrDefault())
                    .AddClaim(JwtClaimTypes.FamilyName, umbMember.Name?.Split(' ').LastOrDefault());

                var roles = memberManager.GetRolesAsync(umbMember).GetAwaiter().GetResult();
                if (roles?.Any() ?? false)
                {
                    foreach (var role in roles)
                    {
                        claims.AddClaim(JwtClaimTypes.Role, role);
                    }
                }
            }

            Claims = claims.ToArray();
        }

        public bool HasAccess(IPublishedContent content)
        {
            if (content == null)
                return false;

            if (!_publicAccessService.IsProtected(content.Path))
                return true;

            if (!IsAuthenticated)
                return false;

            var t = _publicAccessService.HasAccessAsync(content.Path, UserName, () => Task.FromResult(Roles as IEnumerable<string>));
            Task.WaitAll(t);

            return t.Result;
        }

        private ClaimsIdentity GetBackOfficeUser()
        {
            var cookieOptions = _cookieOptionsSnapshot.Get(UmbCore.Constants.Security.BackOfficeAuthenticationType);

            string backOfficeCookie = _httpContextAccessor.HttpContext?.Request?.Cookies[cookieOptions.Cookie.Name!];
            if (backOfficeCookie != null)
            {
                var unprotected = cookieOptions.TicketDataFormat.Unprotect(backOfficeCookie!);
                return unprotected?.Principal?.GetUmbracoIdentity();
            }

            return null;
        }
    }
}
