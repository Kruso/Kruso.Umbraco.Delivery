using Kruso.Umbraco.Delivery.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliTemplates : IDeliTemplates
    {
        private readonly IDeliCache _deliCache;
        private readonly IFileService _fileService;
        private readonly ILogger<DeliTemplates> _log;

        public DeliTemplates(IDeliCache deliCache, IFileService fileService, ILogger<DeliTemplates> log)
        {
            _deliCache = deliCache;
            _fileService = fileService;
            _log = log;
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

        public IPublishedContent AssignJsonTemplate(IPublishedContent content)
        {
            var template = JsonTemplate();

            return template != null && content.GetTemplateAlias() != template.Alias
                ? new DeliPublishedContent(content, template.Id)
                : content;
        }

        public bool IsJsonTemplate(IPublishedContent content)
        {
            var template = JsonTemplate();
            return template != null && content != null && template.Id == (content.TemplateId ?? -1);
        }
    }
}
