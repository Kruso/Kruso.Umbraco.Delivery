using Kruso.Umbraco.Delivery.Grid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kruso.Umbraco.Delivery.Grid.Json
{
    public abstract class StylesJsonConverter<T, TS> : JsonConverter<T>
        where T : Styles<TS>, new()
        where TS : Settings, new()
    {
        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var json = reader.ReadAsString();
            if (!string.IsNullOrEmpty(json))
            {
                var styles = new T();
                var obj = JObject.Parse(json);

                foreach (var prop in obj.Properties())
                {
                    var settings = styles.GetSettings(prop.Name);
                    if (settings != null)
                        settings.SetProp(prop.Name, prop.Value<string>());
                }

                return styles;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, T? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value != null)
                value.ToJObject().WriteTo(writer);
        }
    }
}
