using Newtonsoft.Json.Linq;
using NPoco.fastJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Lucene.Net.Queries.Function.ValueSources.MultiFunction;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public abstract class Styles<T> where T : Settings, new()
    {
        [JsonIgnore]
        public T Small { get; private set; } = new T();
        [JsonIgnore]
        public T Medium { get; private set; } = new T();
        [JsonIgnore]
        public T Large { get; private set; } = new T();
        [JsonIgnore]
        public T ExtraLarge { get; private set; } = new T();

        public T? GetSettings(string propName)
        {
            var breakpoint = propName.Split('_').FirstOrDefault();
            switch (breakpoint)
            {
                case "small": return Small;
                case "medium": return Medium;
                case "large": return Large;
                case "extra": return ExtraLarge;
                default: return null;
            }
        }

        public JObject ToJObject()
        {
            var json = new JObject();

            Small.AddProps(json, "small");
            Medium.AddProps(json, "medium");
            Large.AddProps(json, "large");
            ExtraLarge.AddProps(json, "extra_large");

            return json;
        }
    }
}
