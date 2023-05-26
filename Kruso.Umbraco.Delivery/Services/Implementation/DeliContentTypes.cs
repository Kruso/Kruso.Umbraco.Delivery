using J2N.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliContentTypes : IDeliContentTypes
    {
        private readonly IDeliCache _deliCache;
        private readonly IContentTypeService _contentTypeService;

        public DeliContentTypes(IDeliCache deliCache, IContentTypeService contentTypeService)
        {
            _deliCache = deliCache;
            _contentTypeService = contentTypeService;
        }

        public IContentType ContentType(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
            {
                var key = $"Umb_ContentType_{alias}";
                var res = _deliCache.GetFromRequest<IContentType>(key);
                if (res == null)
                {
                    res = _contentTypeService.Get(alias);

                    if (res != null)
                        _deliCache.AddToRequest(key, res);

                    return res;
                }
            }

            return null;
        }

        public string[] ContentTypeAliases(string alias)
        {
            var types = new List<string>();

            var contentType = ContentType(alias);
            if (contentType != null)
            {
                types.Add(contentType.Alias);
                if (contentType.CompositionAliases()?.Any() ?? false)
                    types.AddRange(contentType.CompositionAliases());
            }

            return types.ToArray();
        }
    }
}
