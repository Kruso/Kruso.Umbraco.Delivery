using Kruso.Umbraco.Delivery.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Routing
{
    public interface IDeliRequest
    {
        IPublishedContent Content { get; }
        string Culture { get; }

        HttpRequest Request { get; }
        RequestType RequestType { get; }
        RequestOrigin RequestOrigin { get; }

        Uri CallingUri { get; }
        Uri OriginalUri { get; }
        IQueryCollection Query { get; }
        JwtSecurityToken Token { get; }

        ModelFactoryOptions ModelFactoryOptions { get; }

        string ResponseMessage { get; set; }

        bool IsPreviewForContent(int? id);
    }
}
