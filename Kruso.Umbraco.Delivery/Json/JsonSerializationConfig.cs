using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Json
{
    public static class JsonSerializationConfig
    {
        private static List<JsonConverter> _jsonConverters = new List<JsonConverter>();

        public static JsonSerializerSettings DeliverySerializationSettings = null;

        public static void AddJsonConverter(JsonConverter jsonConverter)
        {
            if (jsonConverter != null)
                _jsonConverters.Add(jsonConverter);
        }

        public static void AddJsonConverters(List<JsonConverter> jsonConverters)
        {
            if (jsonConverters != null)
                _jsonConverters.AddRange(jsonConverters);
        }

        public static void Init()
        {
            DeliverySerializationSettings = new JsonSerializerSettings
            {
                Converters = _jsonConverters,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                NullValueHandling = NullValueHandling.Include
            };
        }
    }
}
