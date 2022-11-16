using IdentityModel;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Security
{
    public class DefaultIdentity : IUserIdentity
    {
        private readonly IPublicAccessService _publicAccessService;

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

        public DefaultIdentity(IMemberManager memberManager, IBackOfficeSecurityAccessor backOfficeSecurityAccessor, IPublicAccessService publicAccessService)
        {
            UserType = UserType.Visitor;
            _publicAccessService = publicAccessService;

            var claims = new List<Claim>();
            var umbUser = backOfficeSecurityAccessor?.BackOfficeSecurity?.CurrentUser;
            if (umbUser != null)
            {
                UserType = UserType.BackOffice;
                claims
                    .AddClaim(Constants.Claims.PreferredUserName, umbUser.Username)
                    .AddClaim(JwtClaimTypes.Email, umbUser.Email)
                    .AddClaim(JwtClaimTypes.GivenName, umbUser.Name?.Split(' ').FirstOrDefault())
                    .AddClaim(JwtClaimTypes.FamilyName, umbUser.Name?.Split(' ').LastOrDefault());

                if (umbUser.Groups?.Any() ?? false)
                {
                    var roles = umbUser.Groups.Select(x => x.Name).Distinct();
                    foreach (var role in roles)
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
    }
}
