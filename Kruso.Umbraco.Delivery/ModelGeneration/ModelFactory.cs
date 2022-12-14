using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Models;
using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly IModelFactoryContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDeliContent _deliContent;
        private readonly ILogger<ModelFactory> _log;

        public string Culture =>_context.Culture;

        public ModelFactory(
            IModelFactoryContext context,
            IServiceProvider serviceProvider,
            IDeliContent deliContent,
            ILogger<ModelFactory> log)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _deliContent = deliContent;

            _log = log;
        }

        public void Init(IPublishedContent page)
        {
            GetModelFactoryContext().Init(this, page);
        }

        public void Init(IPublishedContent page, string culture, ModelFactoryOptions options = null)
        {
            GetModelFactoryContext().Init(this, page, culture, options);
        }

        public void Init(IEnumerable<IPublishedContent> pages, string culture, ModelFactoryOptions options = null)
        {
            GetModelFactoryContext().Init(this, pages, culture, options);
        }

        public JsonNode CreatePage()
        {
            var context = GetModelFactoryContext();
            var modelFactoryComponentSource = GetModelFactoryComponentSource();
            return CreatePage(context, modelFactoryComponentSource);
        }

        public IEnumerable<JsonNode> CreatePages()
        {
            var res = new List<JsonNode>();

            var context = GetModelFactoryContext();
            if (context.Page != null)
            {
                do
                {
                    var modelFactoryComponentSource = GetModelFactoryComponentSource();
                    var page = CreatePage(context, modelFactoryComponentSource);
                    if (page != null)
                    {
                        res.Add(page);
                    }
                }
                while (context.NextPage());
            }

            return res;
        }

        public IEnumerable<JsonNode> CreateBlocks(IEnumerable<IPublishedElement> items)
        {
            var context = GetModelFactoryContext();
            return CreateBlocks(items.Select(x => new DeliPublishedElement(context.Page, x)));
        }

        public IEnumerable<JsonNode> CreateBlocks(IEnumerable<IPublishedContent> items)
        {
            var context = GetModelFactoryContext();
            var modelFactoryComponentSource = GetModelFactoryComponentSource();

            return items?
                .Select(x => CreateBlock(context, modelFactoryComponentSource, x))
                .Where(x => x != null)
                .ToList()
                ?? new List<JsonNode>();
        }

        public JsonNode CreateBlock()
        {
            var context = GetModelFactoryContext();
            var modelFactoryComponentSource = GetModelFactoryComponentSource();

            return CreateBlock(context, modelFactoryComponentSource, context.Page);
        }

        public JsonNode CreateBlock(IPublishedElement element)
        {
            var context = GetModelFactoryContext();
            var modelFactoryComponentSource = GetModelFactoryComponentSource();

            var publishedElement = new DeliPublishedElement(context.Page, element);
            return CreateBlock(context, modelFactoryComponentSource, publishedElement);
        }

        public JsonNode CreateBlock(IMedia media)
        {
            var context = GetModelFactoryContext();
            var modelFactoryComponentSource = GetModelFactoryComponentSource();

            var publishedMedia = new DeliPublishedMedia(media);
            return CreateBlock(context, modelFactoryComponentSource, publishedMedia);
        }

        public JsonNode CreateBlock(IPublishedContent content)
        {
            var context = GetModelFactoryContext();
            var modelFactoryComponentSource = GetModelFactoryComponentSource();

            return CreateBlock(context, modelFactoryComponentSource, content);
        }

        public IEnumerable<JsonNode> CreateRoutes()
        {
            var context = GetModelFactoryContext();
            var modelFactoryComponentSource = GetModelFactoryComponentSource();
            var template = modelFactoryComponentSource.GetTemplate(TemplateType.Route);

            var res = new List<JsonNode>();

            if (context.Page != null)
            {
                do
                {
                    if (IsRenderablePageForIdentity(context, context.Page))
                    {
                        var props = CreateProperties(context, modelFactoryComponentSource, context.Page);
                        var dataNode = template?.Create(context, props, context.Page);
                        if (dataNode != null)
                        {
                            res.Add(dataNode);
                        }
                    }
                }
                while (context.NextPage());
            }

            return res;
        }

        private JsonNode CreatePage(IModelFactoryContext context, IModelFactoryComponentSource modelFactoryComponentSource)
        {
            var page = context.Page;
            if (!IsRenderablePageForIdentity(context, page))
                return null;

            JsonNode pageModel;

            if (!context.IncrementDepth(page.Key))
            {
                var template = modelFactoryComponentSource.GetTemplate(TemplateType.Ref, page);
                pageModel = template.Create(context, new JsonNode(), page);
            }
            else
            {
                var template = modelFactoryComponentSource.GetTemplate(TemplateType.Page, page);
                var props = CreateProperties(context, modelFactoryComponentSource, page);
                pageModel = template.Create(context, props, page) ?? new JsonNode();

                context.DecrementDepth();
            }

            return pageModel?.AnyProps() ?? false
                ? pageModel
                : null;
        }

        private JsonNode CreateBlock(IModelFactoryContext context, IModelFactoryComponentSource modelFactoryComponentSource, IPublishedContent content)
        {
            if (!IsRenderablePageForIdentity(context, content))
            {
                return null;
            }

            if (context.Page == null)
                context.Init(this, content);

            JsonNode block = null;

            if (!context.IncrementDepth(content.Key))
            {
                var template = modelFactoryComponentSource.GetTemplate(TemplateType.Ref, content);
                block = template.Create(context, new JsonNode(), content);
            }
            else
            {
                var template = modelFactoryComponentSource.GetTemplate(TemplateType.Block, content);
                var props = CreateProperties(context, modelFactoryComponentSource, content);
                block = template.Create(context, props, content);

                context.DecrementDepth();
            }

            return block?.AnyProps() ?? false
                ? block
                : null;
        }

        private JsonNode CreateProperties(IModelFactoryContext context, IModelFactoryComponentSource modelFactoryComponentSource, IPublishedElement content)
        {
            var props = new JsonNode();
            
            if (content?.Properties != null)
            {
                foreach (var prop in content.Properties)
                {
                    var modelProperty = GetModelProperty(context, modelFactoryComponentSource, content, prop);
                    props.AddProp(modelProperty.Name, modelProperty.Value);
                }
            }

            return props;
        }

        private ModelProperty GetModelProperty(IModelFactoryContext context, IModelFactoryComponentSource modelFactoryComponentSource, IPublishedElement content, IPublishedProperty property)
        {
            var res = new ModelProperty(property.Alias);
            IModelPropertyValueFactory resolver = null;

            try
            {
                resolver = modelFactoryComponentSource.GetPropertyValueFactory(content, property);
                var val = resolver?.Create(context, property);
                res.Value = modelFactoryComponentSource.GetPropertyModelTemplate().Create(content, property, val);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"{resolver?.GetType()?.Name ?? "[No PropertyResolver]"} threw an error for property {property.Alias}:{property.PropertyType.EditorAlias}");
                res.Error = true;
            }

            return res;
        }


        private bool IsRenderablePageForIdentity(IModelFactoryContext context, IPublishedContent content)
        {
            if (_deliContent.IsPage(content))
            {
                return context.Identity.HasAccess(content)
                    && content.IsPublished(context.Culture);
            }

            return true;
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
