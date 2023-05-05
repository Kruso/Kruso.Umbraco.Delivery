using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.Blocks;

namespace Kruso.Umbraco.Delivery.Grid
{
    [ModelPropertyValueFactory("Umbraco.BlockGrid")]
    public class BlockGridPropertyValueFactory : Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories.BlockGridPropertyValueFactory, IModelPropertyValueFactory
    {
        public BlockGridPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
            : base(deliProperties, modelFactory)
        {
        }

        protected override JsonNode CreateBlockGridModel(BlockGridContext context, BlockGridModel blockGridModel)
        {
            return base.CreateBlockGridModel(context, blockGridModel);
        }
    }
}