using Kruso.Umbraco.Delivery.Grid.Json;

namespace Kruso.Umbraco.Delivery.Grid
{
    public static class Setup
    {
        public static void AddGridComponents(this UmbracoDeliveryOptions options)
        {
            options.JsonConverters.Add(new StylesGridJsonConverter());
            options.JsonConverters.Add(new StylesSpacingsJsonConverter());
            options.JsonConverters.Add(new StylesGridItemJsonConverter());
            options?.AddComponentsFrom(typeof(Setup).Assembly);
        }
    }
}
