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
        private IPublishedContent _content;
        public IPublishedContent Content
        {
            get
            {
                return _content;
            }
            private set
            {
                _content = value;
                SetRequestType();
            }
        }

        public Uri CallingUri { get; private set; }
        public RequestType RequestType { get; private set; } = RequestType.Initialized;
        public Uri OriginalUri { get; private set; }
        public IQueryCollection Query { get; private set; }
        public JwtSecurityToken Token { get; private set; }

        public string Culture { get; private set; }

        public string ResponseMessage { get; set; }

        public ModelFactoryOptions ModelFactoryOptions { get; private set; }

        internal DeliRequest(HttpRequest request, Uri originalUri, JwtSecurityToken token)
        {
            CallingUri = request.AbsoluteUri();
            OriginalUri = originalUri;
            Query = request.Query;
            Token = token;
            ModelFactoryOptions = CreateModelFactoryOptions();
        }

        private ModelFactoryOptions CreateModelFactoryOptions()
        {
            var res = new ModelFactoryOptions
            {
                LoadPreview = IsPreviewContentRequest(),
                QueryString = Query,
                IncludeFields = Query.Strs("include"),
                ExcludeFields = Query.Strs("exclude")
            };

            var depth = Query.Int("depth");
            if (depth > 0)
                res.MaxDepth = depth;

            return res;
        }

        internal void Finalize(IPublishedContent content, string culture, bool isPreviewPaneRequest = false)
        {
            if (isPreviewPaneRequest)
            {
                CallingUri = OriginalUri;
            }

            Content = content;
            Culture = culture;
        }

        private void SetRequestType()
        {
            if (IsPreviewPaneRequest())
            {
                RequestType = RequestType.PreviewPane;
            }
            else if (IsPreviewContentRequest())
            {
                RequestType = RequestType.PreviewContent;
            }
            else if (Content != null)
            {
                RequestType = RequestType.Content;
            }
            else
            {
                RequestType = RequestType.Failed;
            }
        }

        private bool IsPreviewContentRequest()
        {
            return Token != null
                && int.TryParse(Token.Claims.ValueOfType(DeliveryClaimTypes.PreviewId), out var tokenContentId)
                && tokenContentId == (Content?.Id ?? -1);
        }

        private bool IsPreviewPaneRequest()
        {
            return CallingUri.Authority.Equals(OriginalUri.Authority, StringComparison.InvariantCultureIgnoreCase)
                ? CallingUri.CleanPath() == (Content?.Id.ToString() ?? "-1")
                : false;
        }
    }
}
