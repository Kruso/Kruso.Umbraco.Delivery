using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.Grid
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

        public virtual object? Create(IPublishedProperty property)
        {
            var culture = _modelFactory.Context.Culture;
            var contentItems = _modelFactory.CreateBlocks(_deliProperties.PublishedContentValue<IPublishedContent>(property, culture));

            var configuration = _deliDataTypes.EditorConfiguration<MultiNodePickerConfiguration>(property.PropertyType.DataType.Id);
            return (configuration?.MaxNumber ?? 0) == 1
                ? contentItems?.FirstOrDefault()
                : contentItems;
        }
    }
}
