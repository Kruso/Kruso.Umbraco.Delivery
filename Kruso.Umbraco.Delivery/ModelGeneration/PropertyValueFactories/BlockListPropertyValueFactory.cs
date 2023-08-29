using Kruso.Umbraco.Delivery.Services;
using System.Linq;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
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

        public virtual object Create(IPublishedProperty property)
        {
            var value = GetValueWithFallback(_modelFactory.Context, property, out bool isFallback);

			if (value is BlockListModel blockListModel)
            {
				var res = _modelFactory.CreateBlocks(blockListModel.Select(x => x.Content));
                return IsMaxOneBlock(property)
                    ? res.FirstOrDefault()
                    : res;
            }
            else if (value is BlockListItem blockListItem)
            {
                return _modelFactory.CreateBlock(blockListItem.Content);
            }

            return null;
        }

        private bool IsMaxOneBlock(IPublishedProperty property)
        {
            var configuration = _deliDataTypes.EditorConfiguration<BlockListConfiguration>(property.PropertyType.DataType.Id);
            return configuration?.ValidationLimit?.Max == 1;
        }

        
        private object GetValueWithFallback(IModelFactoryContext context, IPublishedProperty property, out bool isFallback)
        {
            isFallback = false;
			var value = _deliProperties.Value(property, context.Culture);
			if (value == null)
			{
				value = _deliProperties.Value(property, context.FallbackCulture);
				isFallback = true;
			}

            if (value is BlockListModel blockListModel && !blockListModel.Any() && !isFallback)
            {
                value = _deliProperties.Value(property, context.FallbackCulture);
                isFallback = true;
            }

            return value;
		}
    }
}
