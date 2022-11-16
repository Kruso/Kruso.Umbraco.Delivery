using Kruso.Umbraco.Delivery.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Routing
{
    public interface IDeliRequest
    {
        RequestType RequestType { get; }
        Uri CallingUri { get; }
        Uri OriginalUri { get; }
        IQueryCollection Query { get; }
        JwtSecurityToken Token { get; }

        IPublishedContent Content { get; }
        string Culture { get; }

        ModelFactoryOptions ModelFactoryOptions { get; }

        string ResponseMessage { get; set; }
    }
}
