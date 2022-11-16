using Kruso.Umbraco.Delivery.Json;
using System.Collections.Generic;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public interface IModelNodeListConverter
    {
        void Convert(JsonNode parentNode, string propName, List<JsonNode> target, JsonNode[] source);
    }
}
