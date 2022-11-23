using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Composing;

namespace Kruso.Umbraco.Delivery.Json
{
    public class JsonTemplateComponent : IComponent
    {
        private readonly IDeliTemplates _deliTemplates;

        public JsonTemplateComponent(IDeliTemplates deliTemplates)
        {
            _deliTemplates = deliTemplates;
        }

        public void Initialize()
        {
            //Ensures that the Json template is created on startup if it doesn't already exist
            var template = _deliTemplates.JsonTemplate();
        }

        public void Terminate()
        {
        }
    }
}
