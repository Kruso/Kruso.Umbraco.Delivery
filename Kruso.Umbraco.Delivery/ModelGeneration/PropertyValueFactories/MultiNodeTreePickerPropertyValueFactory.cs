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
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public MultiNodeTreePickerPropertyValueFactory(IDeliDataTypes deliDataTypes, IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var context = _modelFactory.Context;

            if (!(_deliProperties.Value(property, context.Culture) is List<IPublishedContent> contentItems))
            {
                contentItems = new List<IPublishedContent>();
                var contentItem = _deliProperties.Value(property, context.Culture) as IPublishedContent;
                if (contentItem != null)
                    contentItems.Add(contentItem);
            }

            var res = _modelFactory.CreateBlocks(contentItems);

            var configuration = _deliDataTypes.EditorConfiguration<MultiNodePickerConfiguration>(property.PropertyType.DataType.Id);
            return configuration?.MaxNumber == 1
                ? res.FirstOrDefault()
                : res;
        }
    }
}
