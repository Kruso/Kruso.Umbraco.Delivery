using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.ModelGeneration.Templates
{
    [ModelTemplate(TemplateType.Block)]
    public class BlockModelTemplate : IModelTemplate
    {
        private readonly IDeliContent _deliContent;
        private readonly IDeliUrl _deliUrl;

        public BlockModelTemplate(IDeliContent deliContent, IDeliUrl deliUrl)
        {
            _deliContent = deliContent;
            _deliUrl = deliUrl;
        }

        public virtual JsonNode Create(IModelFactoryContext context, JsonNode props, IPublishedContent block)
        {
            var jsonNode = new JsonNode
            {
                Id = block.Key,
                PageId = context.Page?.Key,
                ParentPageId = context.Page?.Parent?.Key,
                Type = block.ContentType.Alias,
                Culture = context.Culture,
                CompositionTypes = block.ContentType.CompositionAliases?.ToArray()
            };

            jsonNode
                .AddPropIfNotNull("name", block.Name)
                .AddPropIfNotNull("urls", CreateUrls(context, block))
                .AddProp("sortOrder", block.SortOrder)
                .CopyAllProps(props);

            return jsonNode;
        }

        public JsonNode CreateUrls(IModelFactoryContext context, IPublishedContent block)
        {
            return _deliContent.IsPage(block)
                ? new JsonNode()
                    .AddProp("slug", block.UrlSegment(context.Culture))
                    .AddProp("url", _deliUrl.GetDeliveryUrl(block, context.Culture))
                : null;
        }
    }
}
