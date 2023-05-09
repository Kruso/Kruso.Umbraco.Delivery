using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Grid.Models;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Security;
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
        internal static JsonNode CreateGrid(this IModelFactory modelFactory, Guid id, Action<JsonNode>? fillBlockAction = null)
        {
            return modelFactory.CreateCustomBlock(id, "CoreGridSection", (block) =>
            {
                block
                    .AddStylesGrid()
                    .AddStylesGridItem()
                    .AddStylesSpacings();

                fillBlockAction?.Invoke(block);
            });
        }

        internal static JsonNode? CreateGrid(this IModelFactory modelFactory, IPublishedElement? grid, Action<JsonNode>? fillBlockAction = null)
            => modelFactory.CreateGridBlock(grid, (block) =>
            {
                block.AddToCompositionTypes("CoreGridSection");
                block.AddStylesGrid();

                fillBlockAction?.Invoke(block);
            });

        internal static JsonNode? CreateGridBlock<T>(this IModelFactory modelFactory, T? block, Action<JsonNode>? fillBlockAction = null)
            where T : IPublishedElement
        {
            if (block == null)
                return null;

            var res = modelFactory.CreateBlock(block);
            if (!res.IsRefType)
            {
                res
                    .AddToCompositionTypes("CoreGridItem")
                    .AddStylesGridItem()
                    .AddStylesSpacings();

                fillBlockAction?.Invoke(res);
            }

            return res;
        }

        internal static IEnumerable<JsonNode> CreateGridBlocks<T>(this IModelFactory modelFactory, IEnumerable<T> blocks)
            where T : IPublishedElement
        {
            return blocks
                .Select(x => modelFactory.CreateGridBlock(x))
                .Where(x => x != null)
                .ToList()!;
        }
    }
}
