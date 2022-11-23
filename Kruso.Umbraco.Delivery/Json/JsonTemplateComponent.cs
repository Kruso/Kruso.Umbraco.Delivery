using Kruso.Umbraco.Delivery.Services;
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
        private readonly IDeliContentTypes _deliContentTypes;

        public JsonTemplateComponent(IDeliContentTypes deliContentTypes)
        {
            _deliContentTypes = deliContentTypes;
        }

        public void Initialize()
        {
            //Ensures that the Json template is created on startup if it doesn't already exist
            var template = _deliContentTypes.JsonTemplate();
        }

        public void Terminate()
        {
        }
    }
}
