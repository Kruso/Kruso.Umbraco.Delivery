using Kruso.Umbraco.Delivery.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.MultiNodeTreePicker")]
    public class MultiNodeTreePickerPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliContent _deliContent;
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IDeliProperties _deliProperties;

        public MultiNodeTreePickerPropertyValueFactory(IDeliContent deliContent, IDeliDataTypes deliDataTypes, IDeliProperties deliProperties)
        {
            _deliContent = deliContent;
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
        }

        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        {
            var contentItems = _deliProperties.Value(property, context.Culture) as List<IPublishedContent>
                ?? new List<IPublishedContent>();

            if (!contentItems.Any())
            {
                var contentItem = _deliProperties.Value(property, context.Culture) as IPublishedContent;
                if (contentItem != null)
                    contentItems.Add(contentItem);
            }

            var res = context.ModelFactory.CreateBlocks(contentItems);

            var configuration = _deliDataTypes.EditorConfiguration<MultiNodePickerConfiguration>(property.PropertyType.DataType.Id);
            return configuration?.MaxNumber == 1
                ? res.FirstOrDefault()
                : res;
        }
    }
}
