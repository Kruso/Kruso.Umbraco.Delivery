using Kruso.Umbraco.Delivery.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Json
{
    public class JsonNode : JObject
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

        public new object this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public Guid Id
        {
            get { return Get<Guid>(ReservedProps.Id); }
            set { this[ReservedProps.Id] = value; }
        }

        public Guid? PageId
        {
            get { return Get<Guid?>(ReservedProps.PageId); }
            set { this[ReservedProps.PageId] = value; }
        }

        public Guid? ParentPageId
        {
            get { return Get<Guid?>(ReservedProps.ParentPageId); }
            set { this[ReservedProps.ParentPageId] = value; }
        }

        public string Culture
        {
            get { return Get<string>(ReservedProps.Culture); }
            set { this[ReservedProps.Culture] = value?.ToLower(); }
        }

        public new string Type
        {
            get { return Get<string>(ReservedProps.Type); }
            set { this[ReservedProps.Type] = value?.Capitalize(); }
        }

        public bool IsRefType => Type == DeliConstants.RefTypeAlias;

        public string[] CompositionTypes
        {
            get { return GetArray<string>(ReservedProps.CompositionTypes).ToArray(); }
            set { this[ReservedProps.CompositionTypes] = value?.Select(x => x.Capitalize()).ToArray() ?? new string[0]; }
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

        public bool HasProp(string prop)
        {
            return !string.IsNullOrEmpty(prop) && ContainsKey(prop);
        }

        public bool HasProps(params string[] props)
        {
            return props.All(prop => HasProp(prop));
        }

        //public Dictionary<string, T> AllPropsOf<T>()
        //{
        //    var allProps = Properties()
        //        .Where(x => x.Value<object>() != null && x.Value<object>() is T)
        //        .ToDictionary(kvp => kvp.Name, kvp => (T)kvp.Value<object>());

        //    return allProps;
        //}

        public void Remove(params string[] props)
        {
            foreach (var prop in props)
            {
                if (ContainsKey(prop))
                    base.Remove(prop);
            }
        }

        //#region Interfaces and Overrides

        //public override bool TryGetMember(GetMemberBinder binder, out object result)
        //{
        //    return _properties.TryGetValue(binder.Name, out result);
        //}

        //public bool TryGetValue(string key, out object value)
        //{
        //    return _properties.TryGetValue(key, out value);
        //}

        //public override bool TrySetMember(SetMemberBinder binder, object value)
        //{
        //    _properties[binder.Name] = value;
        //    return true;
        //}

        //public override IEnumerable<string> GetDynamicMemberNames()
        //{
        //    return base.GetDynamicMemberNames().Concat(_properties.Keys);
        //}

        //public override string ToString()
        //{
        //    return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
        //    {
        //        Formatting = Formatting.Indented,
        //        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //        ObjectCreationHandling = ObjectCreationHandling.Replace,
        //        NullValueHandling = NullValueHandling.Include
        //    });
        //}

        //#endregion Interfaces and Overrides

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    foreach (var kvp in Properties)
        //    {
        //        info.AddValue(kvp.Key, kvp.Value);
        //    }
        //}

        internal object Get(string prop)
        {
            if (!ContainsKey(prop))
                return null;

            if (base[prop].Type == JTokenType.Array)
                return Values<object>();

            return (object)base[prop];
        }

        internal T Get<T>(string prop)
        {
            var val = Get(prop);
            if (val == null)
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)val.ToString();

            return val is T
                ? (T)val
                : default;
        }

        internal IEnumerable<T> GetArray<T>(string prop)
        {
            var jVal = ContainsKey(prop)
                ? base[prop]
                : null;

            return jVal != null && jVal.Type == JTokenType.Array
                ? jVal.Values<T>()
                : Enumerable.Empty<T>();
        }

        private void Set(string prop, object value)
        {
            if (!string.IsNullOrEmpty(prop))
            {
                if (ReservedProps.All.Contains(prop) && ContainsKey(prop) && GetValue(prop) != null)
                {
                    throw new InvalidOperationException($"Cannot update value of existing reserved property {prop}");
                }

                if (!KeepEmptyProperties && IsValueEmpty(value))
                {
                    base.Remove(prop);
                }
                else
                {
                    if (ContainsKey(prop))
                        base.Remove(prop);

                    if (value is string)
                        Add(prop, new JValue(value));
                    else if (value == null)
                        Add(prop, null);
                    else if (value is JToken)
                        Add(prop, (JToken)value);
                    else if (value.GetType().IsEnumerable())
                        Add(prop, new JArray(value));
                    else
                        Add(prop, new JValue(value));
                }
            }
        }
        private bool CanAddProp(string prop)
        {
            return !ReservedProps.All.Contains(prop);
        }
    }
}
