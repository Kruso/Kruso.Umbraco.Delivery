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

        public ContentPickerPropertyValueFactory(IDeliProperties deliProperties)
        {
            _deliProperties = deliProperties;
        }

        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        {
            var blocks = new List<JsonNode>();

            var propertyContent = _deliProperties.Value(property, context.Culture) as IPublishedContent;
            if (propertyContent != null)
            {
                var block = context.ModelFactory.CreateBlock(propertyContent);
                if (block != null && block.AnyProps())
                    blocks.Add(block);
            }

            return blocks;
        }
    }
}
