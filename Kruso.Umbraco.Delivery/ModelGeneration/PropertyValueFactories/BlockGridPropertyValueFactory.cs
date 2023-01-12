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
            var blockGridItems = _deliProperties.Value(property, context.Culture) as IEnumerable<BlockGridItem>;
            var idx = context.Page.Id * 10000;

            var res = blockGridItems?
                .Select((x, i) => CreateBlockGridArea(ref idx, x))
                .ToList() 
                ?? new List<JsonNode>();

            return res;
        }

        private JsonNode CreateBlockGridArea(ref int idx, BlockGridItem blockGridItem)
        {
            var blockGridArea = new BlockGridArea(new List<BlockGridItem> { blockGridItem }, "root", blockGridItem.RowSpan, blockGridItem.ColumnSpan);
            return CreateBlockGridArea(ref idx, blockGridArea);
        }


        private JsonNode CreateBlockGridArea(ref int idx, BlockGridArea blockGridArea)
        {
            if (blockGridArea == null)
                return null;

            var uuid = UuidFromPartial(idx++);

            var gridItemBlocks = new List<JsonNode>();

            foreach (var gridItem in blockGridArea)
            {
                var subIdx = idx;
                var gridItemUuid = UuidFromPartial(idx++);
                var gridItemBlock = _modelFactory.CreateCustomBlock(gridItemUuid, nameof(BlockGridItem), (block) =>
                {
                    var settings = _modelFactory.CreateBlock(gridItem.Settings);
                    var areas = new List<JsonNode>();

                    foreach (var area in (gridItem.Areas ?? new BlockGridArea[0]))
                    {
                        var grid = CreateBlockGridArea(ref subIdx, area);
                        if (grid != null)
                        {
                            areas.Add(grid);
                            subIdx++;
                        }
                    }

                    block
                        .AddProp("columnSpan", gridItem.ColumnSpan)
                        .AddProp("rowSpan", gridItem.RowSpan)
                        .AddProp("gridColumns", gridItem.GridColumns)
                        .AddProp("areaGridColumns", gridItem.AreaGridColumns)
                        .AddProp("settings", settings)
                        .AddProp("block", block)
                        .AddProp("areas", areas);
                });

                idx = subIdx;
            }

            var gridArea = _modelFactory.CreateCustomBlock(uuid, nameof(BlockGridArea), (block) =>
            {
                block
                    .AddProp("alias", blockGridArea.Alias)
                    .AddProp("columnSpan", blockGridArea.ColumnSpan)
                    .AddProp("rowSpan", blockGridArea.RowSpan)
                    .AddProp("gridItems", gridItemBlocks);
            });

            return gridArea;
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
