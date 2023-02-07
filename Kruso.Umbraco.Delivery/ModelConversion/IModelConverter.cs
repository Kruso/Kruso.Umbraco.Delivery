using Kruso.Umbraco.Delivery.Json;
using System.Collections.Generic;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public interface IModelConverter
    {
        IEnumerable<JsonNode> Convert(IEnumerable<JsonNode> source, TemplateType converterType, string converterKey = null);
        JsonNode Convert(JsonNode source, TemplateType converterType, string converterKey = null);
    }
}