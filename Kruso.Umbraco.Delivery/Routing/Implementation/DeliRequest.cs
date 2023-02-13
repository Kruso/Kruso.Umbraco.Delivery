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
        private Uri _originalCallingUri;

        public IPublishedContent Content { get; private set; }
        public string Culture { get; private set; }

        public HttpRequest Request { get; private set; }
        public RequestType RequestType => GetRequestType();
        public RequestOrigin RequestOrigin { get; private set; } = RequestOrigin.Backend;

        public Uri CallingUri { get; private set; }

        public Uri OriginalUri { get; private set; }
        public IQueryCollection Query => Request?.Query;
        public string JwtToken { get; private set; }
        public JwtSecurityToken Token { get; internal set; }

        public string ResponseMessage { get; set; }

        public ModelFactoryOptions ModelFactoryOptions { get; private set; }

        internal DeliRequest()
        {
            RequestOrigin = GetRequestOrigin();
            ModelFactoryOptions = CreateModelFactoryOptions();
        }

        internal DeliRequest(HttpRequest request)
        {
            Request = request;
            CallingUri =  request.AbsoluteUri();
            OriginalUri = request.AbsoluteUri();
            RequestOrigin = GetRequestOrigin();
            ModelFactoryOptions = CreateModelFactoryOptions();
        }

        internal DeliRequest(HttpRequest request, Uri originalUri, string jwtToken)
        {
            Request = request;
            JwtToken = jwtToken;
            CallingUri = request.AbsoluteUri();
            OriginalUri = originalUri;
            RequestOrigin = GetRequestOrigin();
            ModelFactoryOptions = CreateModelFactoryOptions();
        }

        public bool IsValidPreviewRequest()
        {
            return Token != null && Content != null
                && int.TryParse(Token.Claims.ValueOfType(DeliveryClaimTypes.PreviewId), out var tokenContentId)
                && tokenContentId == Content.Id;
        }

        private ModelFactoryOptions CreateModelFactoryOptions()
        {
            var res = new ModelFactoryOptions
            {
                LoadPreview = IsValidPreviewRequest(),
                QueryString = Query,
                IncludeFields = Query.Strs("include"),
                ExcludeFields = Query.Strs("exclude")
            };

            var depth = Query.Int("depth");
            if (depth > 0)
                res.MaxDepth = depth;

            return res;
        }

        internal void UnFinalize()
        {
            Content = null;
            Culture = null;
            _finalized = false;

            if (_originalCallingUri != null)
            {
                CallingUri = _originalCallingUri;
                _originalCallingUri = null;
            }

            ModelFactoryOptions = CreateModelFactoryOptions();
        }

        internal void Finalize(IPublishedContent content, string culture, Uri callingUri = null)
        {
            Content = content;
            Culture = culture;
            _finalized = true;

            if (callingUri != null)
            {
                _originalCallingUri = CallingUri;
                CallingUri = callingUri;
            }

            ModelFactoryOptions = CreateModelFactoryOptions();
        }

        private RequestType GetRequestType()
        {
            if (IsValidPreviewRequest())
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
    }
}
