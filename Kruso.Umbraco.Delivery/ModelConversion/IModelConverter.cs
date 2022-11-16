using Kruso.Umbraco.Delivery.Json;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public interface IModelConverter
    {
        JsonNode Convert(JsonNode source, TemplateType converterType, string converterKey = null);
    }
}