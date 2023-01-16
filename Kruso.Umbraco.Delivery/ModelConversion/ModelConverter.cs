using Kruso.Umbraco.Delivery.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public class ModelConverter : IModelConverter
    {
        private readonly IModelConverterComponentSource _modelConverterComponentSource;
        private readonly ILogger<ModelConverter> _log;

        private static readonly string[] ExcludeTypes =
        {
            "Image"
        };

        public ModelConverter(IModelConverterComponentSource modelConverterComponentSource, ILogger<ModelConverter> log)
        {
            _modelConverterComponentSource = modelConverterComponentSource;
            _log = log;
        }

        public IEnumerable<JsonNode> Convert(IEnumerable<JsonNode> source, TemplateType converterType, string converterKey = null)
        {
            return source.Select(x => Convert(x, converterType, converterKey));
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
            var nodeType = source.Type;
            var contentType = converterType == TemplateType.Route
                ? ""
                : converterKey ?? nodeType;

            var converter = _modelConverterComponentSource.GetConverter(converterType, contentType);
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
                    _log.LogError(ex, $"{converter.GetType().Name ?? "[No Converter]"} threw an error for {converterType} {source.Id}:{nodeType}");
                    throw;
                }
            }

            var nodeListProps = target.AllPropsOf<IEnumerable<JsonNode>>();
            foreach (var kvp in nodeListProps.Where(x => x.Value.All(n => IsValidForListConverter(n))))
            {
                var listConverter = _modelConverterComponentSource.GetListConverter(nodeType, kvp.Key);
                if (listConverter != null)
                {
                    var res = new List<JsonNode>();
                    listConverter.Convert(target, kvp.Key, res, kvp.Value.ToArray());
                    target.AddProp(kvp.Key, res);
                }
            }

            return target;
        }

        private bool IsValidForListConverter(JsonNode node)
        {
            return node.Id != Guid.Empty 
                && !string.IsNullOrEmpty(node.Type) 
                && !ExcludeTypes.Contains(node.Type);
        }
    }
}
