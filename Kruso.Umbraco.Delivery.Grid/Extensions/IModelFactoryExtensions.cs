using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Grid.Models;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Grid.Extensions
{
    internal static class IModelFactoryExtensions
    {
        internal static JsonNode CreateGrid(this IModelFactory modelFactory, Action<JsonNode> fillBlockAction)
        {
            var grid = modelFactory.CreateCustomBlock(1.GenerateUuid(1), "CoreGridSection", (block) =>
            {

                block
                    .AddProp("styles_grid", new StylesGrid())
                    .AddProp("styles_spacings", new StylesSpacings());

                fillBlockAction?.Invoke(block);
            });

            return grid;

        }

        internal static JsonNode CreateGridBlock(this IModelFactory modelFactory, IPublishedElement block)
        {
            return modelFactory
                .CreateBlock(block)
                    .AddProp("styles_grid_item", new StylesGridItem())
                    .AddProp("styles_spacings", new StylesSpacings());
        }

        internal static IEnumerable<JsonNode> CreateGridBlocks(this IModelFactory modelFactory, IEnumerable<IPublishedElement> blocks)
        {
            return modelFactory
                .CreateBlocks(blocks)
                .Select(x => x
                    .AddProp("styles_grid_item", new StylesGridItem())
                    .AddProp("styles_spacings", new StylesSpacings()));
        }

        internal static IEnumerable<JsonNode> CreateGridBlocks(this IModelFactory modelFactory, IEnumerable<IPublishedContent> blocks)
        {
            return modelFactory
                .CreateBlocks(blocks)
                .Select(x => x
                    .AddProp("styles_grid_item", new StylesGridItem())
                    .AddProp("styles_spacings", new StylesSpacings()));
        }
    }
}
