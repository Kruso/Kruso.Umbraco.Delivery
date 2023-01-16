using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Routing.Implementation
{
    public sealed class DeliRequest : IDeliRequest
    {
        private bool _finalized = false;

        public IPublishedContent Content { get; private set; }
        public string Culture { get; private set; }

        public HttpRequest Request { get; private set; }
        public RequestType RequestType => GetRequestType();
        public RequestOrigin RequestOrigin => GetRequestOrigin();

        public Uri CallingUri => Request.AbsoluteUri();

        public Uri OriginalUri { get; private set; }
        public IQueryCollection Query => Request?.Query;
        public JwtSecurityToken Token { get; private set; }

        public string ResponseMessage { get; set; }

        public ModelFactoryOptions ModelFactoryOptions { get; private set; }

        internal DeliRequest()
        {
            ModelFactoryOptions = CreateModelFactoryOptions();
        }

        internal DeliRequest(HttpRequest request)
        {
            Request = request;
            OriginalUri = request.AbsoluteUri();
        }

        internal DeliRequest(HttpRequest request, Uri originalUri, JwtSecurityToken token)
        {
            Request = request;
            OriginalUri = originalUri;
            Token = token;
        }

        private ModelFactoryOptions CreateModelFactoryOptions()
        {
            var res = new ModelFactoryOptions
            {
                LoadPreview = RequestType == RequestType.PreviewContent,
                QueryString = Query,
                IncludeFields = Query.Strs("include"),
                ExcludeFields = Query.Strs("exclude")
            };

            var depth = Query.Int("depth");
            if (depth > 0)
                res.MaxDepth = depth;

            return res;
        }

        internal void Finalize(IPublishedContent content, string culture)
        {
            Content = content;
            Culture = culture;
            _finalized = true;

            ModelFactoryOptions = CreateModelFactoryOptions();
        }

        private RequestType GetRequestType()
        {
            if (IsPreviewContentRequest())
                return RequestType.PreviewContent;

            if (!_finalized)
                return RequestType.Initialized;

            if (Content != null)
                return RequestType.Content;

            return RequestType.Failed;
        }

        private RequestOrigin GetRequestOrigin()
        {
            if (CallingUri == null || OriginalUri == null)
                return RequestOrigin.Indexer;
            
            return CallingUri.Authority.Equals(OriginalUri.Authority, StringComparison.InvariantCultureIgnoreCase)
                ? RequestOrigin.Backend
                : RequestOrigin.Frontend;
        }

        private bool IsPreviewContentRequest()
        {
            return Token != null
                && int.TryParse(Token.Claims.ValueOfType(DeliveryClaimTypes.PreviewId), out var tokenContentId)
                && tokenContentId == (Content?.Id ?? -1);
        }
    }
}
