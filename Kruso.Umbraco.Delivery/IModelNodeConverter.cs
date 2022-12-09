using Kruso.Umbraco.Delivery.Json;

namespace Kruso.Umbraco.Delivery
{
    public interface IModelNodeConverter
    {
        void Convert(JsonNode target, JsonNode source);
    }
}
