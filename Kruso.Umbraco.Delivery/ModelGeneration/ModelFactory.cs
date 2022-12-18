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
        private readonly IDeliRequestAccessor _deliRequestAccessor;
        private readonly IDeliContent _deliContent;
        private readonly IDeliCache _deliCache;
        private readonly ILogger<ModelFactory> _log;

        public IModelFactoryContext2 Context => GetContext();

        public ModelFactory(
            IServiceProvider serviceProvider,
            IDeliRequestAccessor deliRequestAccessor,
            IDeliContent deliContent,
            IDeliCache deliCache,
            ILogger<ModelFactory> log)
        {
            _serviceProvider = serviceProvider;
            _deliRequestAccessor = deliRequestAccessor;
            _deliContent = deliContent;
            _deliCache = deliCache;

            _log = log;
        }

        public JsonNode CreatePage(IPublishedContent page, string culture = null, ModelFactoryOptions options = null)
        {
            JsonNode jsonNode = null;

            if (page != null)
            {
                var context = GetContext();
                var componentSource = GetModelFactoryComponentSource();

                context.InitializeDepth(page, culture, options);

                if (!CanRender(page, context))
                    return null;

                try
                {
                    var props = CreateProperties(page, componentSource);
                    var template = componentSource.GetTemplate(TemplateType.Page, page);
                    jsonNode = template.Create(context, props, page);
                }
                finally
                {
                    context.DecrementDepth();
                }
            }

            return jsonNode?.AnyProps() ?? false
                ? jsonNode
                : null;
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
            JsonNode jsonNode = null;

            if (fillBlockAction != null)
            {
                var context = GetContext();
                if (context.IncrementDepth(id))
                {
                    var block = new JsonNode(id, context.Page?.Key, context.Culture, type);
                    fillBlockAction(block);
                    context.DecrementDepth();
                }
                else
                {
                    var refContent = new DeliRefContent(id, type);
                    var refTemplate = GetModelFactoryComponentSource().GetTemplate(TemplateType.Ref, refContent);
                    jsonNode = refTemplate.Create(context, new JsonNode(), refContent);
                }
            }

            return jsonNode;
        }

        public JsonNode CreateBlock(IPublishedContent block, string culture = null, ModelFactoryOptions options = null)
        {
            JsonNode jsonNode = null;

            if (block != null)
            {
                var context = GetContext();
                if (CanRender(block, context))
                {
                    var componentSource = GetModelFactoryComponentSource();

                    if (context.IncrementDepth(block.Key))
                    {
                        var props = CreateProperties(block, componentSource);
                        var template = componentSource.GetTemplate(TemplateType.Block, block);
                        jsonNode = template.Create(context, props, block);

                        context.DecrementDepth();
                    }
                    else
                    {
                        var refTemplate = componentSource.GetTemplate(TemplateType.Ref, block);
                        jsonNode = refTemplate.Create(context, new JsonNode(), block);
                    }
                }
            }

            return jsonNode?.AnyProps() ?? false
                ? jsonNode
                : null;
        }

        public JsonNode CreateBlock(IPublishedElement element, string culture = null, ModelFactoryOptions options = null)
        {
            return element != null
                ? CreateBlock(new DeliPublishedElement(_deliRequestAccessor.Current.Content, element), culture, options)
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
            return items?.Any() ?? false
                ? CreateBlocks(items.Select(x => new DeliPublishedElement(_deliRequestAccessor.Current.Content, x)), null, null)
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
                    context.InitializeDepth(item, culture);

                    try
                    {
                        var template = GetModelFactoryComponentSource().GetTemplate(TemplateType.Route, item);
                        var jsonNode = template.Create(context, new JsonNode(), item);
                        if (jsonNode != null)
                            res.Add(jsonNode);
                    }
                    finally
                    {
                        context.DecrementDepth();
                    }
                }
            }

            return res;
        }

        private JsonNode CreateProperties(IPublishedElement content, IModelFactoryComponentSource componentSource)
        {
            var props = new JsonNode();

            if (content?.Properties != null)
            {
                foreach (var property in content.Properties)
                {
                    var modelProperty = GetModelProperty(content, property, componentSource);
                    props.AddProp(modelProperty.Name, modelProperty.Value);
                }
            }

            return props;
        }

        private ModelProperty GetModelProperty(IPublishedElement content, IPublishedProperty property, IModelFactoryComponentSource componentSource)
        {
            var res = new ModelProperty(property.Alias);
            IModelPropertyValueFactory resolver = null;

            try
            {
                resolver = componentSource.GetPropertyValueFactory(content, property);
                var val = resolver?.Create(property);
                res.Value = componentSource.GetPropertyModelTemplate().Create(content, property, val);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"{resolver?.GetType()?.Name ?? "[No PropertyResolver]"} threw an error for property {property.Alias}:{property.PropertyType.EditorAlias}");
                res.Error = true;
            }

            return res;
        }

        private bool CanRender(IPublishedContent content, IModelFactoryContext2 context)
        {
            if (_deliContent.IsPage(content))
            {
                return _deliRequestAccessor.Identity.HasAccess(content)
                    && content.IsPublished(context.Culture);
            }

            return true;
        }

        private IModelFactoryContext2 GetContext()
        {
            return _deliCache.GetFromRequest("deli_ModelFactory_Context", _serviceProvider.GetService<IModelFactoryContext2>());
        }

        private IModelFactoryComponentSource GetModelFactoryComponentSource()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetService<IModelFactoryComponentSource>();
            }
        }
    }
}
