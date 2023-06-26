using Kruso.Umbraco.Delivery.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Search
{
    public class SearchIndexerValueSet
    {
        private readonly Dictionary<string, List<object>> _valueSet;
        
        public IPublishedContent Content { get; private set; }

        public SearchIndexerValueSet(IReadOnlyDictionary<string, IReadOnlyList<object>> values, IPublishedContent content)
        {
            _valueSet = values.ToDictionary(x => x.Key, x => x.Value.ToList());
            Content = content;
        }

        public SearchIndexerValueSet Set<T>(string prop, T val, string culture = null)
        {
            prop = string.IsNullOrEmpty(culture)
                ? prop
                : $"{prop}_{culture}";

            Remove(prop);
            _valueSet.Add(prop, new List<object> { val });

            return this;
        }

        public SearchIndexerValueSet Copy<T>(JsonNode obj, string prop, string culture = null)
        {
            var val = obj.Val<T>(prop);
            var propExpr = new PropExpression(prop);

            var target = string.IsNullOrEmpty(culture)
                ? propExpr.Target.Name
                : $"{propExpr.Target.Name}_{culture}";

            return Set(target, val);
        }

        public SearchIndexerValueSet SetList<T>(string prop, IEnumerable<T> vals, string culture = null)
        {
            prop = string.IsNullOrEmpty(culture)
                ? prop
                : $"{prop}_{culture}";

            Remove(prop);
            if (vals != null && vals.Any())
               _valueSet.Add(prop, vals.Cast<object>().ToList());

            return this;
        }

        public void Remove(string prop)
        {
            if (_valueSet.ContainsKey(prop))
                _valueSet.Remove(prop);
        }

        public bool HasProp(string prop)
        {
            return _valueSet.ContainsKey(prop);
        }

        public T Val<T>(string prop)
        {
            if (HasProp(prop))
            {
                var val = _valueSet[prop].FirstOrDefault();
                if (val == null)
                {
                    return default;
                }
                if (val is T)
                {
                    return (T)val;
                }
                else if (typeof(T) == typeof(string))
                {
                    return (T)((val?.ToString() ?? string.Empty) as object);
                }
            }

            return default;
        }

        public IDictionary<string, IEnumerable<object>> Values()
        {
            return _valueSet.ToDictionary(x => x.Key, x => (IEnumerable<object>)x.Value);
        }
    }
}
