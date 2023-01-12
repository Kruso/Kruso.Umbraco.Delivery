using Kruso.Umbraco.Delivery.Json;
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
        private readonly IModelFactory _modelFactory;

        public NestedContentPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            IEnumerable<JsonNode> blocks = null;

            var val = _deliProperties.Value(property, _modelFactory.Context.Culture);
            if (val is IPublishedElement item)
            {
                var block = _modelFactory.CreateBlock(item);
                if (block != null)
                    blocks = new List<JsonNode>() { block };
            }
            else if (val is IEnumerable<IPublishedElement> items)
            {
                blocks = _modelFactory.CreateBlocks(items).ToList();
            }

            return blocks ?? Enumerable.Empty<JsonNode>();
        }
    }
}