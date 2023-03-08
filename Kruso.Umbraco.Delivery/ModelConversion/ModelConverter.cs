using Kruso.Umbraco.Delivery.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public class ModelConverter : IModelConverter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ModelConverter> _log;

        public ModelConverter(IServiceProvider serviceProvider, ILogger<ModelConverter> log)
        {
            _serviceProvider = serviceProvider;
            _log = log;
        }

        public IEnumerable<JsonNode> Convert(IEnumerable<JsonNode> source, TemplateType converterType)
        {
            if (source == null || !source.Any())
                return Enumerable.Empty<JsonNode>();

            var componentSource = GetModelConverterSource();
            if (componentSource == null)
                return source;

            return source
                .Select(x => Convert(componentSource, x, converterType, null))
                .Where(x => x != null);
        }

        public JsonNode Convert(JsonNode source, TemplateType converterType)
        {
            var componentSource = GetModelConverterSource();
            if (componentSource == null)
                return source;

            return Convert(componentSource, source, converterType, null);
        }

        private JsonNode Convert(IModelConverterComponentSource componentSource, JsonNode source, TemplateType converterType, string context)
        {
            if (!source.IsBlock() || source.IsRefType)
                return source;

            if (converterType == TemplateType.Route)
                return RunConverter(componentSource, source, TemplateType.Route, context);

            var target = new JsonNode();
            foreach (var propName in source.AllPropNames())
            {
                var convertedBlock = ConvertBlock(componentSource, source, propName);
                if (convertedBlock != null)
                {
                    target.AddProp(propName, convertedBlock);
                    continue;
                }

                var convertedBlocks = ConvertBlockList(componentSource, source, propName, ConvertBlocks(componentSource, source, propName));
                if (convertedBlocks != null)
                {
                    target.AddProp(propName, convertedBlocks);
                    continue;
                }

                target.CopyProp(source, propName);
            }

            return RunConverter(componentSource, target, converterType, context);
        }

        private JsonNode ConvertBlock(IModelConverterComponentSource componentSource, JsonNode source, string propName)
        {
            var block = source.Node(propName);

            return block != null
                ? Convert(componentSource, block, TemplateType.Block, $"{source.Type}.{propName}")
                : null;
        }

        private IEnumerable<JsonNode> ConvertBlocks(IModelConverterComponentSource componentSource, JsonNode source, string propName)
        {
            var blocks = source.Nodes(propName);

            return blocks.Any()
                ? blocks.Select(x => Convert(componentSource, x, TemplateType.Block, $"{source.Type}.{propName}"))
                : null;
        }

        private IEnumerable<JsonNode> ConvertBlockList(IModelConverterComponentSource componentSource, JsonNode source, string propName, IEnumerable<JsonNode> sourceList) 
        {
            if (source == null || sourceList == null)
                return null;

            if (!componentSource.HasListConverters())
                return sourceList;

            if (!sourceList.All(x => x.IsBlock()))
                return sourceList;

            var converter = componentSource.GetListConverter(source.Type, propName);
            if (converter == null)
                return sourceList;

            try
            {
                var target = new List<JsonNode>();
                converter.Convert(source, propName, target, sourceList.ToArray());
                return target;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"{converter.GetType().Name ?? "[No List Converter]"} threw an error for {source.Id}:{source.Type}:{propName}");
                throw;
            }
        }

        private JsonNode RunConverter(IModelConverterComponentSource componentSource, JsonNode source, TemplateType converterType, string context)
        {
            if (source == null)
                return null;

            var types = GetJsonNodeTypes(source);
            foreach (var type in types)
            {

                var converter = componentSource.GetConverter(converterType, type, context);
                if (converter != null)
                {
                    try
                    {
                        var target = new JsonNode();
                        converter.Convert(target, source);
                        source = target;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, $"{converter.GetType().Name ?? "[No Converter]"} threw an error for {converterType} {source.Id}:{source.Type}");
                        throw;
                    }
                }
            }

            return source;
        }

        private string[] GetJsonNodeTypes(JsonNode source)
        {
            List<string> types = new();
            types.Add(source.Type);
            types.AddRange(source.CompositionTypes ?? new string[0]);

            return types.ToArray();
        }

        private IModelConverterComponentSource GetModelConverterSource()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var componentSource = scope.ServiceProvider.GetService<IModelConverterComponentSource>();

                return componentSource.HasConverters() || componentSource.HasListConverters()
                    ? componentSource
                    : null;
            }
        }
    }
}
