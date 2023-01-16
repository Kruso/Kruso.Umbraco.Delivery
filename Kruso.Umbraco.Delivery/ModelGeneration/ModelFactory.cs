using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Routing;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public class ModelFactory : IModelFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IModelFactoryComponentSource _modelFactoryComponentSource;
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliCache _deliCache;
        private readonly ILogger<ModelFactory> _log;

        public IModelFactoryContext Context => GetContext();

        public ModelFactory(
            IServiceProvider serviceProvider,
            IModelFactoryComponentSource modelFactoryComponentSource,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliCache deliCache,
            ILogger<ModelFactory> log)
        {
            _serviceProvider = serviceProvider;
            _modelFactoryComponentSource = modelFactoryComponentSource;
            _deliRequestAccessor = deliRequestAccessor;
            _deliCache = deliCache;

            _log = log;
        }

        public JsonNode CreatePage(IPublishedContent page, string culture = null, ModelFactoryOptions options = null)
        {
            var context = GetContext();

            return context.PageWithDepth(page, culture, options, () =>
            {
                var props = CreateProperties(page);
                var template = _modelFactoryComponentSource.GetTemplate(TemplateType.Page, page);

                return template.Create(context, props, page);
            });
        }

        public IEnumerable<JsonNode> CreatePages(IEnumerable<IPublishedContent> pages, string culture = null, ModelFactoryOptions options = null)
        {
            var res = new List<JsonNode>();

            if (pages != null)
            {
                foreach (var page in pages)
                {
                    var item = CreatePage(page, culture, options);
                    if (item != null)
                        res.Add(item);
                }
            }

            return res;
        }

        public JsonNode CreateCustomBlock(Guid id, string type, Action<JsonNode> fillBlockAction)
        {
            var context = GetContext();

            return context.CustomBlockWithDepth(id, type, context.Culture, () =>
            {
                var block = new JsonNode(id, context.Page?.Key, context.Culture, type);
                fillBlockAction?.Invoke(block);
                return block;
            });
        }

        public JsonNode CreateBlock(IPublishedContent block, string culture = null, ModelFactoryOptions options = null)
        {
            var context = GetContext();

            return context.BlockWithDepth(block, culture, options, () =>
            {
                var props = CreateProperties(block);
                var template = _modelFactoryComponentSource.GetTemplate(TemplateType.Block, block);

                return template.Create(context, props, block);
            });
        }

        public JsonNode CreateBlock(IPublishedElement element, string culture = null, ModelFactoryOptions options = null)
        {
            return element != null
                ? CreateBlock(new DeliPublishedElement(GetBlockPage(), element), culture, options)
                : null;
        }

        public JsonNode CreateBlock(IMedia media, string culture = null, ModelFactoryOptions options = null)
        {
            return media != null
                ? CreateBlock(new DeliPublishedMedia(media), culture, options)
                : null;
        }

        public IEnumerable<JsonNode> CreateBlocks(IEnumerable<IPublishedContent> blocks, string culture = null, ModelFactoryOptions options = null)
        {
            var res = new List<JsonNode>();

            if (blocks != null)
            {
                foreach (var block in blocks)
                {
                    var item = CreateBlock(block, culture, options);
                    if (item != null)
                        res.Add(item);
                }
            }

            return res;
        }

        public IEnumerable<JsonNode> CreateBlocks(IEnumerable<IPublishedElement> items)
        {
            var page = GetBlockPage();

            return items?.Any() ?? false
                ? CreateBlocks(items.Select(x => new DeliPublishedElement(page, x)), null, null)
                : Enumerable.Empty<JsonNode>();
        }


        public IEnumerable<JsonNode> CreateRoutes(IEnumerable<IPublishedContent> items, string culture)
        {
            var res = new List<JsonNode>();

            if (items != null)
            {
                var context = GetContext();

                foreach (var item in items)
                {
                    var template = _modelFactoryComponentSource.GetTemplate(TemplateType.Route, item);
                    var jsonNode = template.Create(context, new JsonNode(), item);

                    if (jsonNode != null)
                        res.Add(jsonNode);
                }
            }

            return res;
        }

        private JsonNode CreateProperties(IPublishedElement content)
        {
            var props = new JsonNode();

            if (content?.Properties != null)
            {
                foreach (var property in content.Properties)
                {
                    var modelProperty = GetModelProperty(content, property);
                    props.AddProp(modelProperty.Name, modelProperty.Value);
                }
            }

            return props;
        }

        private ModelProperty GetModelProperty(IPublishedElement content, IPublishedProperty property)
        {
            var res = new ModelProperty(property.Alias);
            IModelPropertyValueFactory resolver = null;

            try
            {
                resolver = _modelFactoryComponentSource.GetPropertyValueFactory(content, property);
                var val = resolver?.Create(property);
                res.Value = _modelFactoryComponentSource.GetPropertyModelTemplate().Create(content, property, val);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"{resolver?.GetType()?.Name ?? "[No PropertyResolver]"} threw an error for property {property.Alias}:{property.PropertyType.EditorAlias}");
                res.Error = true;
            }

            return res;
        }

        private IModelFactoryContext GetContext()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return _deliCache.GetFromRequest("deli_ModelFactory_Context", _serviceProvider.GetService<IModelFactoryContext>());
            }
        }

        private IPublishedContent GetBlockPage()
        {
            var context = GetContext();
            return context.Page ?? _deliRequestAccessor.Current?.Content;
        }
    }
}
