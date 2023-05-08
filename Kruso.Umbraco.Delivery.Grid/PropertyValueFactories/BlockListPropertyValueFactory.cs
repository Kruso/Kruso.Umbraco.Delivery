using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Grid.Extensions;
using Kruso.Umbraco.Delivery.Grid.Models;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.Grid.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.BlockList")]
    public class BlockListPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IModelFactory _modelFactory;

        public BlockListPropertyValueFactory(IDeliProperties deliProperties, IDeliDataTypes deliDataTypes, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _deliDataTypes = deliDataTypes;
            _modelFactory = modelFactory;
        }

        public virtual object? Create(IPublishedProperty property)
        {
            return _modelFactory.CreateGrid((grid) =>
            {
                var blocks = new List<JsonNode>();

                var value = _deliProperties.Value(property, _modelFactory.Context.Culture);
                if (value is BlockListModel blockListModel)
                {
                    var res = _modelFactory.CreateGridBlocks(blockListModel?.Select(x => x.Content));
                    blocks.AddRange(res);
                }
                else if (value is BlockListItem blockListItem)
                {
                    var block = _modelFactory.CreateGridBlock(blockListItem.Content);
                    if (block != null)
                        blocks.Add(block);
                }

                grid.AddProp("content", IsMaxOneBlock(property) && blocks.Any() ? blocks.First() : blocks);
            });
        }

        private bool IsMaxOneBlock(IPublishedProperty property)
        {
            var configuration = _deliDataTypes.EditorConfiguration<BlockListConfiguration>(property.PropertyType.DataType.Id);
            return (configuration?.ValidationLimit?.Max ?? 0) == 1;
        }
    }
}
