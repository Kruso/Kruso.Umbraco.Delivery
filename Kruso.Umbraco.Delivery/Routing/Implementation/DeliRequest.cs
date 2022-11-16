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
        public RequestType RequestType { get; private set; }
        public Uri CallingUri { get; private set; }
        public Uri OriginalUri { get; private set; }
        public IQueryCollection Query { get; private set; }
        public JwtSecurityToken Token { get; private set; }

        public IPublishedContent Content { get; private set; }
        public string Culture { get; private set; }

        public string ResponseMessage { get; set; }

        public ModelFactoryOptions ModelFactoryOptions { get; private set; }

        internal DeliRequest(HttpRequest request, Uri originalUri, JwtSecurityToken token)
        {
            CallingUri = request.AbsoluteUri();
            OriginalUri = originalUri;
            Query = request.Query;
            Token = token;

            RequestType = IsPreviewPaneRequest()
                ? RequestType.PreviewPane
                : RequestType.Initialized;

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

        internal void Finalize(IPublishedContent content, string culture, Uri callingUri = null)
        {
            if (RequestType != RequestType.PreviewPane)
            {
                Content = content;
                Culture = culture;

                if (Content != null)
                {
                    RequestType = IsPreviewContentRequest()
                        ? RequestType.PreviewContent
                        : RequestType.Content;

                    if (callingUri != null)
                    {
                        CallingUri = callingUri;
                    }
                }
                else
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
