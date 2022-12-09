﻿using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.NestedContent")]
    public class NestedContentPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;

        public NestedContentPropertyValueFactory(IDeliProperties deliProperties)
        {
            _deliProperties = deliProperties;
        }

        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        {
            IEnumerable<JsonNode> blocks = null;

            var val = _deliProperties.Value(property, context.Culture);
            if (val is IPublishedElement item)
            {
                var block = context.ModelFactory.CreateBlock(item);
                if (block != null)
                    blocks = new List<JsonNode>() { block };
            }
            else if (val is IEnumerable<IPublishedElement> items)
            {
                blocks = context.ModelFactory.CreateBlocks(items).ToList();
            }

            return blocks ?? Enumerable.Empty<JsonNode>();
        }
    }
}