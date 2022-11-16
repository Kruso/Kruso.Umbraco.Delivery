using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;

namespace Kruso.Umbraco.Delivery.Json
{
    public class JsonTemplateComponent : IComponent
    {
        private readonly IFileService _fileService;
        private readonly ILogger<JsonTemplateComposer> _log;

        public JsonTemplateComponent(IFileService fileService, ILogger<JsonTemplateComposer> log)
        {
            _fileService = fileService;
            _log = log;
        }

        public void Initialize()
        {
            var template = _fileService.GetTemplate(DeliConstants.JsonTemplateAlias);
            if (template != null)
                _log.LogInformation("Json template found. Not creating.");

            if (template == null)
            {
                template = _fileService.CreateTemplateWithIdentity(DeliConstants.JsonTemplateName, DeliConstants.JsonTemplateAlias, string.Empty);
                if (template != null)
                {
                    _log.LogInformation("Json template not found. Created.");
                }
                else
                {
                    _log.LogError("Json template not found. Faile to create.");
                }
                //_fileService.SaveTemplate(template);
            }
        }

        public void Terminate()
        {
        }
    }
}
