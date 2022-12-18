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

        public BlockGridPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var context = _modelFactory.Context;

            var blocks = new List<JsonNode>();
            var blockGridItems = _deliProperties.Value(property, context.Culture) as IEnumerable<BlockGridItem>;

            var idx = context.Page.Id * 10000;

            var res = blockGridItems?
                .Select((x, i) => CreateBlockGridArea(ref idx, context, x))
                .ToList() 
                ?? new List<JsonNode>();

            return res;
        }

        private JsonNode CreateBlockGridArea(ref int idx, IModelFactoryContext2 context, BlockGridItem blockGridItem)
        {
            var uuid = UuidFromPartial(idx++);
            var subIdx = idx;
            var gridArea = _modelFactory.CreateCustomBlock(uuid, nameof(BlockGridArea), (block) =>
            {
                var gridItem = CreateBlockGridItem(ref subIdx, context, blockGridItem);

                block
                    .AddProp("alias", "root")
                    .AddProp("columnSpan", blockGridItem.ColumnSpan)
                    .AddProp("rowSpan", blockGridItem.RowSpan)
                    .AddProp("gridItems", new List<JsonNode> { gridItem });
            });

            idx = subIdx;

            return gridArea;
        }

        private JsonNode CreateBlockGridArea(ref int idx, IModelFactoryContext2 context, BlockGridArea blockGridArea)
        {
            var uuid = UuidFromPartial(idx++);

            if (context.ReachedMaxDepth)
            {
                return CreateBlockGridAreaRef(uuid, context.Page.Key, context.Culture);
            }
            else
            {
                if (context.IncrementDepth(uuid))
                {
                    try
                    {
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
                    }
                    finally
                    {
                        context.DecrementDepth();
                    }
                }
            }
                



            return gridArea;
        }

        private JsonNode CreateBlockGridItem(ref int idx, IModelFactoryContext2 context, BlockGridItem blockGridItem)
        {
            var block = _modelFactory.CreateBlock(blockGridItem.Content);
            if (block.Type == "Ref")
                return block;

            var settings = _modelFactory.CreateBlock(blockGridItem.Settings);
            var areas = new List<JsonNode>();
            foreach (var area in blockGridItem.Areas ?? new BlockGridArea[0])
            {
                var grid = CreateBlockGridArea(ref idx, context, area);
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

        private JsonNode CreateBlockGridAreaRef(Guid id, Guid pageId, string culture)
        {
            return new JsonNode(id, pageId, culture, "Ref")
                .AddProp("refType", nameof(BlockGridArea));
        }
    }
}
