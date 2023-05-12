using Kruso.Umbraco.Delivery.Grid.Extensions;
using Kruso.Umbraco.Delivery.Grid.Models;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Grid.PropertyValueFactories
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
            var blockGridModel = _deliProperties.Value(property, context.Culture) as BlockGridModel;
            var blockGridContext = new BlockGridContext(context.Page.Id * 10000, blockGridModel?.GridColumns);
            return _modelFactory.CreateGrid(blockGridContext.GenerateUuid(), (grid) =>
            {
                grid
                    .StylesGrid()
                    .SetColumns(StylesConstants.Breakpoint.Medium, blockGridContext.DefaultGridColumns);

                var blocks = CreateBlocks(blockGridContext, blockGridModel);
                grid.AddProp("content", blocks);
            });
        }

        private JsonNode[] CreateBlocks(BlockGridContext context, IEnumerable<BlockGridItem>? items)
        {
            if (items == null)
                return new JsonNode[0];

            var res = new List<JsonNode>();
            foreach (var item in items)
            {
                JsonNode? block = item.Areas.Any()
                    ? CreateGrid(context, item)
                    : _modelFactory.CreateGridBlock(item.Content, (block) =>
                    {
                        block
                            .AddProp("settings", _modelFactory.CreateBlock(item?.Settings))
                            .StylesGridItem()
                                .SetColSpans(StylesConstants.Breakpoint.Medium, item.ColumnSpan)
                                .SetRowSpans(StylesConstants.Breakpoint.Medium, item.RowSpan);
                    });

                if (block != null)
                    res.Add(block);
            }

            return res.ToArray();
        }

        private JsonNode? CreateGrid(BlockGridContext context, BlockGridItem? item)
        {
            if (item?.Areas.Any() ?? false)
            {
                return _modelFactory.CreateGrid(item?.Content, (grid) =>
                {
                    grid
                        .AddProp("content", CreateGrids(context, item?.Areas))
                        .AddProp("settings", _modelFactory.CreateBlock(item?.Settings));

                    grid.StylesGrid()
                        .SetColumns(StylesConstants.Breakpoint.Medium, item?.AreaGridColumns ?? context.DefaultGridColumns);

                    grid.StylesGridItem()
                        .SetColSpans(StylesConstants.Breakpoint.Medium, item?.ColumnSpan ?? context.DefaultGridColumns)
                        .SetRowSpans(StylesConstants.Breakpoint.Medium, item?.RowSpan ?? 1);
                });
            }

            return null;
        }

        private JsonNode[] CreateGrids(BlockGridContext context, IEnumerable<BlockGridArea>? areas)
        {
            if (areas == null)
                return new JsonNode[0];

            return areas
                .Select(x => CreateGrid(context, x))
                .Where(x => x != null)
                .ToArray()!;
        }

        private JsonNode? CreateGrid(BlockGridContext context, BlockGridArea area)
        {
            if (area == null)
                return null;

            var grid = _modelFactory.CreateGrid(context.GenerateUuid(), (grid) =>
            {
                grid
                    .AddProp("content", CreateBlocks(context, area));

                grid.StylesGrid()
                    .SetColumns(StylesConstants.Breakpoint.Medium, context.DefaultGridColumns);
                    
                grid.StylesGridItem()
                    .SetColSpans(StylesConstants.Breakpoint.Medium, area.ColumnSpan)
                    .SetRowSpans(StylesConstants.Breakpoint.Medium, area.RowSpan); //TODO: We can set row spans here I think...
            });

            return grid;
        }
    }
}