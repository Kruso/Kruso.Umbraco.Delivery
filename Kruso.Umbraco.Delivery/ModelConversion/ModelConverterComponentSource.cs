﻿using Kruso.Umbraco.Delivery.Extensions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public class ModelConverterComponentSource : IModelConverterComponentSource
    {
        private readonly Dictionary<string, IModelNodeConverter> _modelNodeConverters = null;
        private readonly Dictionary<string, IModelNodeListConverter> _modelNodeListConverters = null;

        private readonly ILogger<ModelConverter> _log;

        public ModelConverterComponentSource(IEnumerable<IModelNodeConverter> modelNodeConverters, IEnumerable<IModelNodeListConverter> modelNodeListConverters, ILogger<ModelConverter> log)
        {
            _modelNodeConverters = modelNodeConverters.ToFilteredDictionary<IModelNodeConverter, ModelNodeConverterAttribute>();
            _modelNodeListConverters = modelNodeListConverters.ToFilteredDictionary<IModelNodeListConverter, ModelNodeListConverterAttribute>();

            _log = log;
        }


        public bool HasConverters() => _modelNodeConverters.Any();
        public bool HasListConverters() => _modelNodeListConverters.Any();

        public IModelNodeListConverter GetListConverter(string documentAlias, string propertyName = null)
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

        public IModelNodeConverter GetConverter(TemplateType templateType, string type, string context)
        {
            if (templateType == TemplateType.Route)
                type = string.Empty;

            var key = templateType.MakeKey(type);

            if (!string.IsNullOrEmpty(context))
            {
                var contextKey = $"{key}@{context.ToLower()}";
                if (_modelNodeConverters.ContainsKey(contextKey))
                    return _modelNodeConverters[contextKey];
            }

            if (_modelNodeConverters.ContainsKey(key))
                return _modelNodeConverters[key];

            var defaultKey = templateType.MakeKey();
            if (_modelNodeConverters.ContainsKey(defaultKey))
                return _modelNodeConverters[defaultKey];

            return null;
        }
    }
}
