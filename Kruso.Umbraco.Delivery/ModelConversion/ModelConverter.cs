using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public class ModelConverter : IModelConverter
    {
        private readonly ILogger<ModelConverter> _log;

        private readonly Dictionary<string, IModelNodeConverter> _modelNodeConverters = null;
        private readonly Dictionary<string, IModelNodeListConverter> _modelNodeListConverters = null;

        public ModelConverter(IEnumerable<IModelNodeConverter> modelNodeConverters, IEnumerable<IModelNodeListConverter> modelNodeListConverters, ILogger<ModelConverter> log)
        {
            _modelNodeConverters = modelNodeConverters.ToFilteredDictionary<IModelNodeConverter, ModelNodeConverterAttribute>();
            _modelNodeListConverters = modelNodeListConverters.ToFilteredDictionary<IModelNodeListConverter, ModelNodeListConverterAttribute>();

            _log = log;
        }

        public JsonNode Convert(JsonNode source, TemplateType converterType, string converterKey = null)
        {
            if (source.Val<bool>("isRef"))
                return source;

            var target = converterType != TemplateType.Route
                ? new JsonNode()
                : source;

            if (converterType != TemplateType.Route)
            {
                if (source != null)
                {
                    foreach (var propName in source.AllPropNames())
                    {
                        var dataNode = source.Node(propName);
                        if (dataNode != null)
                        {
                            var res = Convert(dataNode, TemplateType.Block);
                            target.AddProp(propName, res);
                        }
                        else
                        {
                            if (source.PropIs<IEnumerable<JsonNode>>(propName))
                            {
                                var blocks = source.Nodes(propName);
                                var res = blocks.Select(x => Convert(x, TemplateType.Block));

                                target.AddProp(propName, res);
                            }
                            else
                            {
                                target.CopyProp(propName, source, propName);
                            }
                        }
                    }
                }
            }

            return RunConverter(target, converterType, converterKey);
        }

        private JsonNode RunConverter(JsonNode source, TemplateType converterType, string converterKey = null)
        {
            if (source == null || !source.HasProp("type"))
                return source;

            JsonNode target = null;
            var nodeType = source.Val<string>("type");
            var contentType = converterType == TemplateType.Route
                ? ""
                : converterKey ?? nodeType;

            var converter = GetConverter(converterType, contentType);
            if (converter == null)
            {
                target = source;
            }
            else
            {
                try
                {
                    target = new JsonNode();
                    converter.Convert(target, source);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"{converter.GetType().Name ?? "[No Converter]"} threw an error for {converterType} {source.Val<string>("id")}:{nodeType}");
                    throw;
                }
            }

            var nodeListProps = target.AllPropsOf<IEnumerable<JsonNode>>();
            foreach (var kvp in nodeListProps.Where(x => x.Value.All(n => IsValidForListConverter(n))))
            {
                var listConverter = GetListConverter(nodeType, kvp.Key);
                if (listConverter != null)
                {
                    var res = new List<JsonNode>();
                    listConverter.Convert(target, kvp.Key, res, kvp.Value.ToArray());
                    target.AddProp(kvp.Key, res);
                }
            }

            return target;
        }

        private static readonly string[] ExcludeTypes =
        {
            "Image"
        };

        private bool IsValidForListConverter(JsonNode node)
        {
            return node.Id != Guid.Empty 
                && !string.IsNullOrEmpty(node.Type) 
                && !ExcludeTypes.Contains(node.Type);
        }

        private IModelNodeListConverter GetListConverter(string documentAlias, string propertyName = null)
        {
            if (!string.IsNullOrEmpty(documentAlias))
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    var key = $"{documentAlias}.{propertyName}".ToLower();
                    if (_modelNodeListConverters.ContainsKey(key))
                        return _modelNodeListConverters[key];
                }

                if (_modelNodeListConverters.ContainsKey(documentAlias))
                    return _modelNodeListConverters[documentAlias];
            }

            if (_modelNodeListConverters.ContainsKey("."))
            {
                return _modelNodeListConverters["."];
            }

            return null;
        }

        private IModelNodeConverter GetConverter(TemplateType templateType, string type)
        {
            var key = templateType.MakeKey(type);

            if (_modelNodeConverters.ContainsKey(key))
            {
                return _modelNodeConverters[key];
            }

            var defaultKey = templateType.MakeKey();
            if (_modelNodeConverters.ContainsKey(defaultKey))
            {
                return _modelNodeConverters[defaultKey];
            }

            return null;
        }

    }
}
