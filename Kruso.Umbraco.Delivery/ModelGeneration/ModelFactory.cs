using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.Logging;
using NPoco.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public class ModelFactory : IModelFactory
    {
        private Dictionary<string, IModelTemplate> _modelTemplates = null;
        private Dictionary<string, IModelPropertyValueFactory> _propertyValueFactories = null;

        private readonly IDeliContent _deliContent;
        private readonly IModelFactoryContext _context = null;
        private readonly ILogger<ModelFactory> _log;

        public string Culture => _context?.Culture;

        public ModelFactory(
            IEnumerable<IModelTemplate> modelTemplates,
            IEnumerable<IModelPropertyValueFactory> modelPropertyValueFactories,
            IDeliContent deliContent,
            IModelFactoryContext context, 
            ILogger<ModelFactory> log)
        {
            _context = context;
            _context.Init(this);

            _deliContent = deliContent;
            _modelTemplates = modelTemplates.ToFilteredDictionary<IModelTemplate, ModelTemplateAttribute>();
            _propertyValueFactories = modelPropertyValueFactories.ToFilteredDictionary<IModelPropertyValueFactory, PropertyValueFactoryAttribute>();

            _log = log;
        }

        public void Init(IPublishedContent page)
        {
            _context.Init(this, page);
        }

        public void Init(IPublishedContent page, string culture, ModelFactoryOptions options = null)
        {
            _context.Init(this, page, culture, options);
        }

        public void Init(IEnumerable<IPublishedContent> pages, string culture, ModelFactoryOptions options = null)
        {
            _context.Init(this, pages, culture, options);
        }

        public JsonNode CreatePage()
        {
            if (!IsRenderablePageForIdentity(_context.Page))
                return null;

            JsonNode page;

            if (!_context.IncrementDepth(_context.Page.Key))
            {
                var template = GetTemplate(TemplateType.Ref, _context.Page);
                page = template.Create(_context, new JsonNode(), _context.Page);
            }
            else
            {
                var template = GetTemplate(TemplateType.Page, _context.Page);
                var props = CreateProperties(_context.Page);
                page = template.Create(_context, props, _context.Page) ?? new JsonNode();

                _context.DecrementDepth();
            }

            return page?.AnyProps() ?? false
                ? page
                : null;
        }

        public IEnumerable<JsonNode> CreatePages()
        {
            var res = new List<JsonNode>();

            if (_context.Page != null)
            {
                do
                {
                    var page = CreatePage();
                    if (page != null)
                    {
                        res.Add(page);
                    }
                }
                while (_context.NextPage());
            }

            return res;
        }

        public IEnumerable<JsonNode> CreateBlocks(IEnumerable<IPublishedElement> items)
        {
            return items?
                .Select(x => CreateBlock(x))
                .Where(x => x != null)
                .ToList()
                ?? new List<JsonNode>();
        }

        public IEnumerable<JsonNode> CreateBlocks(IEnumerable<IPublishedContent> items)
        {
            return items?
                .Select(x => CreateBlock(x))
                .Where(x => x != null)
                .ToList()
                ?? new List<JsonNode>();
        }

        public JsonNode CreateBlock()
        {
            return CreateBlock(_context.Page);
        }

        public JsonNode CreateBlock(IPublishedElement element)
        {
            if (element == null)
                return null;

            var publishedElement = new DeliPublishedElement(_context.Page, element);
            return CreateBlock(publishedElement);
        }

        public JsonNode CreateBlock(IMedia media)
        {
            if (media == null) 
                return null;

            var publishedMedia = new DeliPublishedMedia(media);
            return CreateBlock(publishedMedia);
        }

        public JsonNode CreateBlock(IPublishedContent content)
        {
            if (!IsRenderablePageForIdentity(content))
            {
                return null;
            }

            if (_context.Page == null)
                _context.Init(this, content);

            JsonNode block = null;

            if (!_context.IncrementDepth(content.Key))
            {
                var template = GetTemplate(TemplateType.Ref, content);
                block = template.Create(_context, new JsonNode(), content);
            }
            else
            {
                var template = GetTemplate(TemplateType.Block, content);
                var props = CreateProperties(content);
                block = template.Create(_context, props, content);

                _context.DecrementDepth();
            }

            return block?.AnyProps() ?? false
                ? block
                : null;
        }

        public IEnumerable<JsonNode> CreateRoutes()
        {
            var res = new List<JsonNode>();
            var template = GetTemplate(TemplateType.Route);
            if (_context.Page != null)
            {
                do
                {
                    if (IsRenderablePageForIdentity(_context.Page))
                    {
                        var props = CreateProperties(_context.Page);
                        var dataNode = template?.Create(_context, props, _context.Page);
                        if (dataNode != null)
                        {
                            res.Add(dataNode);
                        }
                    }
                }
                while (_context.NextPage());
            }

            return res;
        }

        private JsonNode CreateProperties(IPublishedElement content, params string[] includeProps)
        {
            if (content != null)
            {
                var allProps = content.Properties
                    .Where(x => includeProps == null || !includeProps.Any() || includeProps.Contains(x.Alias))
                    .Select(p => ResolveProperty(p, content))
                    .Where(x => x != null)
                    .ToList();

                var props = new JsonNode();
                allProps.ForEach(x =>
                {
                    props.AddProp(x.Val<string>("name"), x.Val("value"));
                });

                return props;
            }

            return null;
        }

        private JsonNode ResolveProperty(IPublishedProperty property, IPublishedElement content)
        {
            var resolver = GetPropertyValueFactory(property.PropertyType.EditorAlias, property.PropertyType.Alias, property.Alias, content.ContentType.Alias);

            try
            {
                return new JsonNode()
                    .AddProp("name", property.Alias)
                    .AddProp("fieldType", property.PropertyType.EditorAlias)
                    .AddProp("value", resolver?.Create(_context, property));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"{resolver.GetType().Name ?? "[No PropertyResolver]"} threw an error for property {property.Alias}:{property.PropertyType.EditorAlias}");
                throw;
            }
        }

        private IModelTemplate GetTemplate(TemplateType templateType, IPublishedElement content = null)
        {
            var key = templateType.MakeKey(content?.ContentType?.Alias);

            if (_modelTemplates.ContainsKey(key))
            {
                return _modelTemplates[key];
            }

            var defaultKey = templateType.MakeKey();
            if (_modelTemplates.ContainsKey(defaultKey))
            {
                return _modelTemplates[defaultKey];
            }

            return null;
        }

        private IModelPropertyValueFactory GetPropertyValueFactory(string propertyTypeEditorAlias, string propertyTypeAlias, string propertyAlias, string contentTypeAlias)
        {
            return GetPropertyResolverIfExists($"{propertyTypeEditorAlias}+{contentTypeAlias}+{propertyAlias}")
                ?? GetPropertyResolverIfExists($"{propertyTypeEditorAlias}+{propertyTypeAlias}")
                ?? GetPropertyResolverIfExists(propertyTypeEditorAlias)
                ?? GetPropertyResolverIfExists("");
        }

        private IModelPropertyValueFactory GetPropertyResolverIfExists(string key)
        {
            key = key.ToLower();
            return _propertyValueFactories.ContainsKey(key)
                ? _propertyValueFactories[key]
                : null;
        }

        private bool IsRenderablePageForIdentity(IPublishedContent content)
        {
            if (_deliContent.IsPage(content))
            {
                return _context.Identity.HasAccess(content)
                    && content.IsPublished(_context.Culture);
            }

            return true;
        }
    }
}
