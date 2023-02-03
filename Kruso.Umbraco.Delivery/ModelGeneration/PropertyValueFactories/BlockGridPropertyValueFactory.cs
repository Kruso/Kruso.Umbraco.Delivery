using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.BlockGrid")]
    public class BlockGridPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        private class BlockGridContext
        {
            private int Idx = 0;

            public BlockGridContext(int idx) => Idx = idx;

            public Guid GenerateUuid()
            {
                string id = Idx.ToString("X").PadRight(8, '0').ToLower();

                var uuid = id.Length > 8
                    ? $"{id.Substring(0, 8)}-{id.Substring(8).PadRight(4, '0')}-0000-1000-8000-00805f9b34fb"
                    : $"{id}-0000-1000-8000-00805f9b34fb";

                Idx++;

                return Guid.ParseExact(uuid, "d");
            }
        }

        public BlockGridPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var context = _modelFactory.Context;

            var blockGridModel = _deliProperties.Value(property, context.Culture) as BlockGridModel;
            return CreateBlockGridModel(new BlockGridContext(context.Page.Id * 10000), blockGridModel);
        }

        private JsonNode CreateBlockGridModel(BlockGridContext context, BlockGridModel blockGridModel)
        {
            if (blockGridModel == null)
                return null;

            return _modelFactory.CreateCustomBlock(context.GenerateUuid(), nameof(BlockGridModel), (block) =>
            {
                block
                    .AddProp("gridColumnr", blockGridModel.GridColumns)
                    .AddProp("items", CreateBlockGridItems(context, blockGridModel));
            });
        }

        private IEnumerable<JsonNode> CreateBlockGridItems(BlockGridContext context, IEnumerable<BlockGridItem> blockGridItems)
        {
            return blockGridItems?
                .Select(gridItem => CreateBlockGridItem(context, gridItem))
                .Where(gridItem => gridItem != null)
                .ToList() ?? new List<JsonNode>();
        }

        private JsonNode CreateBlockGridItem(BlockGridContext context, BlockGridItem gridItem)
        {
            var gridItemBlock = _modelFactory.CreateCustomBlock(context.GenerateUuid(), nameof(BlockGridItem), (block) =>
            {
                var settings = _modelFactory.CreateBlock(gridItem.Settings);
                var content = _modelFactory.CreateBlock(gridItem.Content);

                block
                    .AddProp("columnSpan", gridItem.ColumnSpan)
                    .AddProp("rowSpan", gridItem.RowSpan)
                    .AddProp("gridColumns", gridItem.GridColumns)
                    .AddProp("areaGridColumns", gridItem.AreaGridColumns)
                    .AddProp("settings", settings)
                    .AddProp("block", content)
                    .AddProp("areas", CreateBlockGridAreas(context, gridItem));
            });
            return gridItemBlock;
        }

        private List<JsonNode> CreateBlockGridAreas(BlockGridContext context, BlockGridItem gridItem)
        {
            var areas = new List<JsonNode>();

            foreach (var area in (gridItem.Areas ?? new BlockGridArea[0]))
            {
                var grid = CreateBlockGridArea(context, area);
                if (grid != null)
                    areas.Add(grid);
            }

            return areas;
        }

        private JsonNode CreateBlockGridArea(BlockGridContext context, BlockGridArea blockGridArea)
        {
            if (blockGridArea == null)
                return null;

            var gridArea = _modelFactory.CreateCustomBlock(context.GenerateUuid(), nameof(BlockGridArea), (block) =>
            {
                block
                    .AddProp("alias", blockGridArea.Alias)
                    .AddProp("columnSpan", blockGridArea.ColumnSpan)
                    .AddProp("rowSpan", blockGridArea.RowSpan)
                    .AddProp("gridItems", CreateBlockGridItems(context, blockGridArea));
            });

            return gridArea;
        }
    }
}
