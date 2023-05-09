using Kruso.Umbraco.Delivery.Grid.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.Grid.PropertyValueFactories
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
            var blockGridContext = new BlockGridContext(_modelFactory.Context.Page.Id * 10000);
            return _modelFactory.CreateGrid(blockGridContext.GenerateUuid(), (grid) =>
            {
                var blocks = _modelFactory.CreateGridBlocks(_deliProperties.PublishedContentValue<IPublishedContent>(property, culture));
                grid.AddProp("content", IsMaxOneBlock(property) && blocks.Any() ? blocks.First() : blocks);
            });
        }

        private bool IsMaxOneBlock(IPublishedProperty property)
        {
            var configuration = _deliDataTypes.EditorConfiguration<MultiNodePickerConfiguration>(property.PropertyType.DataType.Id);
            return (configuration?.MaxNumber ?? 0) == 1;
        }
    }
}
