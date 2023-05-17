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

        public void Add<T>(string prop, T val)
        {
            Remove(prop);
            _valueSet.Add(prop, new List<object> { val });
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
