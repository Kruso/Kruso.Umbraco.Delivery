using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace Kruso.Umbraco.Delivery.Controllers.Renderers
{
    public class PageRenderer
    {
        private readonly IModelConverter _modelConverter;
        private readonly IModelFactory _modelFactory;
        private readonly IDeliRequest _deliRequest;
        private readonly IDeliContent _deliContent;
        private readonly IDeliUrl _deliUrl;
        private readonly IDeliSecurity _deliSecurity;
        private readonly ILogger<PageRenderer> _log;

        public PageRenderer(
            IModelConverter modelConverter,
            IModelFactory modelFactory,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContent deliContent,
            IDeliUrl deliUrl,
            IDeliSecurity deliSecurity,
            ILogger<PageRenderer> log)
        {
            _modelConverter = modelConverter;
            _modelFactory = modelFactory;
            _deliRequest = deliRequestAccessor.Current;
            _deliContent = deliContent;
            _deliUrl = deliUrl;
            _deliSecurity = deliSecurity;
            _log = log;
        }

        public RenderResponse Render()
        {
            return _deliRequest.RequestType == RequestType.PreviewPane
                ? CreatePreviewResponse()
                : CreateJsonResponse();
        }

        private RenderResponse CreatePreviewResponse()
        {
            using (var client = new HttpClient())
            {
                var jwt = _deliSecurity.CreateJwtPreviewToken();
                var url = _deliUrl.GetPreviewPaneUrl(_deliRequest.Content, _deliRequest.Culture, jwt);
                var previewHtml = LoadPreviewHtml();
                var newHtml = previewHtml.Replace("{{url}}", url);

                var res = new RenderResponse
                {
                    Data = newHtml,
                    ContentType = "text/html",
                    Message = _deliRequest.ResponseMessage
                };

                return res;
            }
        }

        private RenderResponse CreateJsonResponse()
        {
            var dataNode = CreateJsonNode(out var errorMessage);

            var statusCode = dataNode == null
                ? HttpStatusCode.InternalServerError
                : (_deliContent.IsNotFoundType(_deliRequest.Content) ? HttpStatusCode.NotFound : HttpStatusCode.OK);

            var message = !string.IsNullOrEmpty(errorMessage)
                ? errorMessage
                : _deliRequest.ResponseMessage;

            return new RenderResponse
            {
                StatusCode = statusCode,
                Model = dataNode,
                Message = message
            };
        }

        private JsonNode CreateJsonNode(out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var page = _modelFactory.CreatePage(_deliRequest.Content, _deliRequest.Culture, _deliRequest.ModelFactoryOptions);
                return _modelConverter.Convert(page, TemplateType.Page);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _log.LogError(ex, "An unexpected error occurred when rendering a json document");
                return null;
            }
        }

        private string LoadPreviewHtml()
        {
            const string resourceName = "Kruso.Umbraco.Delivery.Controllers.Resources.previewTemplate.html";
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var template = reader.ReadToEnd();
                return template;
            }
        }

    }
}
