using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;

namespace Kruso.Umbraco.Delivery.Controllers.Renderers
{
    public class PageRenderer
    {
        private readonly IModelConverter _modelConverter;
        private readonly IModelFactory _modelFactory;
        private readonly IDeliRequest _deliRequest;
        private readonly IDeliContent _deliContent;
        private readonly ILogger<PageRenderer> _log;

        public PageRenderer(
            IModelConverter modelConverter,
            IModelFactory modelFactory,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContent deliContent,
            ILogger<PageRenderer> log)
        {
            _modelConverter = modelConverter;
            _modelFactory = modelFactory;
            _deliRequest = deliRequestAccessor.Current;
            _deliContent = deliContent;
            _log = log;
        }

        public RenderResponse<JsonNode> Render()
        {
            var res = CreatePage();

            if (_deliRequest.ModelFactoryOptions.ModifyFields)
                res.Model = res.Model.Clone(_deliRequest.ModelFactoryOptions.IncludeFields, _deliRequest.ModelFactoryOptions.ExcludeFields);

            return res;
        }

        protected RenderResponse<JsonNode> CreatePage()
        {
            var statusCode = HttpStatusCode.OK;
            JsonNode jsonNode = null;
            var errorMessage = string.Empty;

            try
            {
                var page = _modelFactory.CreatePage(_deliRequest.Content, _deliRequest.Culture, _deliRequest.ModelFactoryOptions);
                jsonNode = _deliRequest.ModelFactoryOptions.Convert
                    ? _modelConverter.Convert(page, TemplateType.Page)
                    : page;

                statusCode = _deliContent.IsNotFoundType(_deliRequest.Content)
                    ? HttpStatusCode.NotFound
                    : HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _log.LogError(ex, "An unexpected error occurred when rendering a json document");

                statusCode = HttpStatusCode.InternalServerError;
            }

            var message = !string.IsNullOrEmpty(errorMessage)
                ? errorMessage
                : _deliRequest.ResponseMessage;

            return new RenderResponse<JsonNode>
            {
                StatusCode = statusCode,
                Model = jsonNode,
                Message = message
            };
        }
    }
}
