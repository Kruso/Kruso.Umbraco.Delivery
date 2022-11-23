using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliContentTypes : IDeliContentTypes
    {
        private readonly IDeliCache _deliCache;
        private readonly IContentTypeService _contentTypeService;
        private readonly IFileService _fileService;
        private readonly ILogger<DeliContentTypes> _log;

        public DeliContentTypes(IDeliCache deliCache, IContentTypeService contentTypeService, IFileService fileService, ILogger<DeliContentTypes> log)
        {
            _deliCache = deliCache;
            _contentTypeService = contentTypeService;
            _fileService = fileService;
            _log = log;
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

        public ITemplate JsonTemplate()
        {
            const string key = $"Umb_Template_{DeliConstants.JsonTemplateAlias}";

            var template = _deliCache.GetFromMemory<ITemplate>(key);
            if (template == null)
            {
                template = _fileService.GetTemplate(DeliConstants.JsonTemplateAlias);
                if (template == null)
                {
                    _log.LogInformation("Json template not found. Creating...");
                    template = _fileService.CreateTemplateWithIdentity(DeliConstants.JsonTemplateName, DeliConstants.JsonTemplateAlias, string.Empty);
                    if (template != null)
                    {
                        _log.LogInformation("Json template created");
                    }
                    else
                        _log.LogError("Json template could not be created");
                }

                if (template != null)
                    _deliCache.AddToMemory(key, template);
            }

            return template;
        }
    }
}
