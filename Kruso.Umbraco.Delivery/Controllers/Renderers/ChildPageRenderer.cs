using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelConversion;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Routing.Implementation;
using Kruso.Umbraco.Delivery.Services;
using Kruso.Umbraco.Delivery.Services.Implementation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Kruso.Umbraco.Delivery.Controllers.Renderers
{
    public class ChildPageRenderer : PageRenderer
    {
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliContent _deliContent;
        private readonly IDeliCulture _deliCulture;
        private readonly ILogger<PageRenderer> _log;

        public ChildPageRenderer(
            IModelConverter modelConverter,
            IModelFactory modelFactory,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContent deliContent,
            IDeliCulture deliCulture,
            ILogger<PageRenderer> log)
            : base(modelConverter, modelFactory, deliRequestAccessor, deliContent, log)
        {
            _deliRequestAccessor = deliRequestAccessor;
            _deliContent = deliContent;
            _deliCulture = deliCulture;
            _log = log;
        }

        public RenderResponse<IEnumerable<JsonNode>> Render(Pagination pagination, string type = null)
        {
            var childPages = new List<JsonNode>();

            var res = CreatePage();
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var deliRequest = _deliRequestAccessor.Current;

                _deliCulture.WithCultureContext(deliRequest.Culture, () =>
                {
                    var children = pagination.Paginate(deliRequest.Content.Children,
                        (item) => string.IsNullOrEmpty(type) || item.ContentType.Alias.Equals(type, StringComparison.InvariantCultureIgnoreCase));

                    foreach (var child in children)
                    {
                        _deliRequestAccessor.FinalizeForContent(child, deliRequest.Culture);
                        var childRes = CreatePage();
                        if (childRes.Model != null)
                        {
                            if (deliRequest.ModelFactoryOptions.ModifyFields)
                                childRes.Model = childRes.Model.Clone(deliRequest.ModelFactoryOptions.IncludeFields, deliRequest.ModelFactoryOptions.ExcludeFields);

                            childPages.Add(childRes.Model);
                        }
                            
                    }
                });
            }

            return new RenderResponse<IEnumerable<JsonNode>>
            {
                StatusCode = childPages.Any() ? HttpStatusCode.OK : HttpStatusCode.NotFound,
                Model = childPages
            };
        }
    }
}
