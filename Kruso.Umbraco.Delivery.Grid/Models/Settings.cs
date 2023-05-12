using Newtonsoft.Json.Linq;
using NPoco.fastJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lucene.Net.Queries.Function.ValueSources.MultiFunction;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public abstract class Settings
    {
        public abstract void SetProp(string propName, string? propVal);
        public abstract void AddProps(JObject obj, string prefix);

        protected void AddPropIfNotNull(JObject json, string prop, string? val)
        {
            if (!string.IsNullOrEmpty(val))
                json.Add(prop, val);
        }
    }

}
