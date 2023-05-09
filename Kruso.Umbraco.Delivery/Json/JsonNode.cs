using Kruso.Umbraco.Delivery.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using static Umbraco.Cms.Core.PropertyEditors.ImageCropperConfiguration;

namespace Kruso.Umbraco.Delivery.Json
{
    public class JsonNode : DynamicObject, ISerializable
    {
        internal static class ReservedProps
        {
            public const string Id = "id";
            public const string PageId = "pageId";
            public const string ParentPageId = "parentPageId";
            public const string Culture = "culture";
            public const string Type = "type";
            public const string CompositionTypes = "compositionTypes";

            public static readonly string[] All = new[]
            {
                Id,
                PageId,
                ParentPageId,
                Culture,
                Type,
                CompositionTypes
            };
        }

        internal static bool KeepEmptyProperties = true;

        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        public JsonNode()
            : base()
        {
        }

        public object this[string key]
        {
            get
            {
                return _properties.ContainsKey(key)
                    ? _properties[key]
                    : null;
            }
            set
            {
                if (!string.IsNullOrEmpty(key))
                {
                    if (ReservedProps.All.Contains(key) && _properties.ContainsKey(key) && _properties[key] != null)
                    {
                        throw new InvalidOperationException($"Cannot update value of existing reserved property {key}");
                    }

                    if (!KeepEmptyProperties && IsValueEmpty(value))
                    {
                        Remove(key);
                    }
                    else if (_properties.ContainsKey(key))
                    {
                        _properties[key] = value;
                    }
                    else
                    {
                        _properties.Add(key, value);
                    }
                }
            }
        }

        public Guid Id
        {
            get { return GetReserved<Guid>(ReservedProps.Id); }
            set { SetReserved(ReservedProps.Id, value); }
        }

        public Guid? PageId
        {
            get { return GetReserved<Guid?>(ReservedProps.PageId); }
            set { SetReserved(ReservedProps.PageId, value); }
        }

        public Guid? ParentPageId
        {
            get { return GetReserved<Guid?>(ReservedProps.ParentPageId); }
            set { SetReserved(ReservedProps.ParentPageId, value); }
        }

        public string Culture
        {
            get { return GetReserved<string>(ReservedProps.Culture); }
            set { SetReserved(ReservedProps.Culture, value.ToLower()); }
        }

        public string Type
        {
            get { return GetReserved<string>(ReservedProps.Type); }
            set { SetReserved(ReservedProps.Type, value.Capitalize()); }
        }

        public bool IsRefType => Type == DeliConstants.RefTypeAlias;

        public string[] CompositionTypes
        {
            get { return GetReserved<string[]>(ReservedProps.CompositionTypes); }
            set { SetReserved(ReservedProps.CompositionTypes, value?.Select(x => x.Capitalize()).ToArray() ?? new string[0] ); }
        }

        public JsonNode AddToCompositionTypes(string compositionType)
        {
            if (!CompositionTypes.Contains(compositionType))
            {
                var compositionTypes = CompositionTypes.ToList();
                compositionTypes.Add(compositionType);

                if (_properties.ContainsKey(ReservedProps.CompositionTypes))
                    _properties[ReservedProps.CompositionTypes] = compositionTypes.ToArray();
                else
                    _properties.Add(ReservedProps.CompositionTypes, compositionTypes.ToArray());
            }

            return this;
        }

        public static bool IsValueEmpty(object value)
        {
            if (value == null)
                return true;

            if (value is string && string.IsNullOrEmpty(value.ToString()))
                return true;

            if (value is IEnumerable<JsonNode> && !(value as IEnumerable<JsonNode>).Any())
                return true;

            return false;
        }

        public bool AnyProps()
        {
            return _properties.Keys.Any();
        }

        public bool HasProp(string prop)
        {
            return !string.IsNullOrEmpty(prop) && _properties.ContainsKey(prop);
        }

        public bool HasProps(params string[] props)
        {
            return props.All(prop => HasProp(prop));
        }

        public string[] AllPropNames()
        {
            return _properties.Keys.ToArray();
        }

        public Dictionary<string, T> AllPropsOf<T>()
        {
            var allProps = _properties
                .Where(x => x.Value != null && x.Value is T)
                .ToDictionary(kvp => kvp.Key, kvp => (T)kvp.Value);

            return allProps;
        }

        public IEnumerable<JsonNode> AllNodes()
        {
            var res = new List<JsonNode>();
            foreach (var prop in _properties.Where(x => x.Value != null && (x.Value is JsonNode || x.Value is IEnumerable<JsonNode>)))
            {
                if (prop.Value is JsonNode)
                {
                    var dataNode = prop.Value as JsonNode;
                    res.AddRange(dataNode.AllNodes());
                    res.Add(dataNode);
                }
                else
                {
                    foreach (var dataNode in prop.Value as IEnumerable<JsonNode>)
                    {
                        res.AddRange(dataNode.AllNodes());
                        res.Add(dataNode);
                    }
                }
            }

            return res;
        }

        public void Remove(params string[] props)
        {
            foreach (var prop in props)
            {
                if (_properties.ContainsKey(prop))
                    _properties.Remove(prop);
            }
        }

        #region Interfaces and Overrides

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _properties.TryGetValue(binder.Name, out result);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _properties.TryGetValue(key, out value);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return base.GetDynamicMemberNames().Concat(_properties.Keys);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                NullValueHandling = NullValueHandling.Include
            });
        }

        #endregion Interfaces and Overrides

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var kvp in _properties)
            {
                info.AddValue(kvp.Key, kvp.Value);
            }
        }

        private T GetReserved<T>(string prop)
        {
            var val = _properties.ContainsKey(prop)
                ? _properties[prop]
                : default(T);

            return val is T
                ? (T)val
                : default;
        }

        private void SetReserved(string prop, object val)
        {
            if (!_properties.ContainsKey(prop))
            {
                _properties.Add(prop, val);
            }
        }

        private bool CanAddProp(string prop)
        {
            return !ReservedProps.All.Contains(prop);
        }
    }
}
