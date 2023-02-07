using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Security
{
    public interface IUserIdentity
    {
        Claim[] Claims { get; }

        UserType UserType { get; }
        string UserName { get; }
        string Name { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        string[] Roles { get; }
        string[] Groups { get; }
        bool IsAuthenticated { get; }

        bool HasAccess(IPublishedContent content);
    }
}
