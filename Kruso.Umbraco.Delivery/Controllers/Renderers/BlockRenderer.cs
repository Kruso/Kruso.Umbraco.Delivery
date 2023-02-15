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
    public class BlockRenderer : PageRenderer
    {
        private readonly IDeliRequest _deliRequest;
        
        public BlockRenderer(
            IModelConverter modelConverter,
            IModelFactory modelFactory,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContent deliContent,
            ILogger<PageRenderer> log)
            : base(modelConverter, modelFactory, deliRequestAccessor, deliContent, log)
        {
            _deliRequest = deliRequestAccessor.Current;
        }

        public RenderResponse<JsonNode> Render(Guid blockId)
        {
            var res = CreatePage();

            var block = res.StatusCode == HttpStatusCode.OK
                ? res.Model.Blocks().FirstOrDefault(x => x.Id == blockId)
                : null;

            res.Model = _deliRequest.ModelFactoryOptions.ModifyFields
                ? block.Clone(_deliRequest.ModelFactoryOptions.IncludeFields, _deliRequest.ModelFactoryOptions.ExcludeFields)
                : block;

            if (res.Model == null)
                res.StatusCode = HttpStatusCode.NotFound;

            return res;
        }
    }
}
