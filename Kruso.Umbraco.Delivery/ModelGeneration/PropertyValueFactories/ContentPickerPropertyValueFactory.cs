using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.ContentPicker")]
    public class ContentPickerPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public ContentPickerPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var blocks = new List<JsonNode>();

            var propertyContent = _deliProperties.Value(property, _modelFactory.Context.Culture) as IPublishedContent;
            if (propertyContent != null)
            {
                var block = _modelFactory.CreateBlock(propertyContent);
                if (block != null && block.AnyProps())
                    blocks.Add(block);
            }

            return blocks;
        }
    }
}
