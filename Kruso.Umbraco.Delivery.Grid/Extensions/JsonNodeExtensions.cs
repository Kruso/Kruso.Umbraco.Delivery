using Kruso.Umbraco.Delivery.Grid.Models;
using Kruso.Umbraco.Delivery.Json;

namespace Kruso.Umbraco.Delivery.Grid.Extensions
{
    public static class JsonNodeExtensions
    {
        public static StylesGrid StylesGrid(this JsonNode grid) => grid.Val<StylesGrid>("styles_grid");
        public static StylesGridItem StylesGridItem(this JsonNode grid) => grid.Val<StylesGridItem>("styles_grid_item");
        public static StylesSpacings StylesSpacings(this JsonNode grid) => grid.Val<StylesSpacings>("styles_spacings");

        public static JsonNode AddStylesGrid(this JsonNode grid) => grid.AddPropIfNotExists("styles_grid", new StylesGrid());
        public static JsonNode AddStylesGridItem(this JsonNode grid) => grid.AddPropIfNotExists("styles_grid_item", new StylesGridItem());
        public static JsonNode AddStylesSpacings(this JsonNode grid) => grid.AddPropIfNotExists("styles_spacings", new StylesSpacings());
    }
}
