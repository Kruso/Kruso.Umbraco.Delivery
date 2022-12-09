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

        public BlockGridPropertyValueFactory(IDeliProperties deliProperties)
        {
            _deliProperties = deliProperties;
        }

        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        {
            var blocks = new List<JsonNode>();
            var blockGridItems = _deliProperties.Value(property, context.Culture) as IEnumerable<BlockGridItem>;

            var idx = context.Page.Id * 10000;

            var res = blockGridItems?
                .Select((x, i) => CreateBlockGrid(ref idx, context, x))
                .ToList() 
                ?? new List<JsonNode>();

            return res;
        }

        private JsonNode CreateBlockGrid(ref int idx, IModelFactoryContext context, BlockGridItem blockGridItem)
        {
            var uuid = UuidFromPartial(idx++);

            var gridItem = CreateBlockGridItem(ref idx, context, blockGridItem);

            var gridArea = new JsonNode(uuid, context.Page.Key, context.Culture, nameof(BlockGridArea))
                .AddProp("alias", "root")
                .AddProp("columnSpan", blockGridItem.ColumnSpan)
                .AddProp("rowSpan", blockGridItem.RowSpan)
                .AddProp("gridItems", new List<JsonNode> { gridItem });

            return gridArea;
        }

        private JsonNode CreateBlockGrid(ref int idx, IModelFactoryContext context, BlockGridArea blockGridArea)
        {
            var uuid = UuidFromPartial(idx++);

            var gridItems = new List<JsonNode>();
            foreach (var area in blockGridArea)
            {
                var gridItem = CreateBlockGridItem(ref idx, context, area);
                if (gridItem != null)
                    gridItems.Add(gridItem);
            }

            var gridArea = new JsonNode(uuid, context.Page.Key, context.Culture, nameof(BlockGridArea))
                .AddProp("alias", blockGridArea.Alias)
                .AddProp("columnSpan", blockGridArea.ColumnSpan)
                .AddProp("rowSpan", blockGridArea.RowSpan)
                .AddProp("gridItems", gridItems);

            return gridArea;
        }

        private JsonNode CreateBlockGridItem(ref int idx, IModelFactoryContext context, BlockGridItem blockGridItem)
        {
            var block = context.ModelFactory.CreateBlock(blockGridItem.Content);
            var settings = context.ModelFactory.CreateBlock(blockGridItem.Settings);
            var areas = new List<JsonNode>();
            foreach (var area in blockGridItem.Areas ?? new BlockGridArea[0])
            {
                var grid = CreateBlockGrid(ref idx, context, area);
                if (grid != null)
                    areas.Add(grid);
            }

            var uuid = UuidFromPartial(idx++);
            var gridItem = new JsonNode(uuid, context.Page.Key, context.Culture, nameof(BlockGridItem))
                .AddProp("columnSpan", blockGridItem.ColumnSpan)
                .AddProp("rowSpan", blockGridItem.RowSpan)
                .AddProp("gridColumns", blockGridItem.GridColumns)
                .AddProp("areaGridColumns", blockGridItem.AreaGridColumns)
                .AddProp("settings", settings)
                .AddProp("block", block)
                .AddProp("areas", areas);

            return gridItem;
        }

        private Guid UuidFromPartial(Int32 partial)
        {
            string id = partial.ToString("X").PadRight(8, '0').ToLower();

            var uuid = id.Length > 8
                ? $"{id.Substring(0, 8)}-{id.Substring(8).PadRight(4, '0')}-0000-1000-8000-00805f9b34fb"
                : $"{id}-0000-1000-8000-00805f9b34fb";

            return Guid.ParseExact(uuid, "d");
        }
    }
}
