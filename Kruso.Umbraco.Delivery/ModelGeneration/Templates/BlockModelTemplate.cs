using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
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

        public virtual JsonNode Create(IModelFactoryContext2 context, JsonNode props, IPublishedContent block)
        {
           return new JsonNode(block.Key, context.Page?.Key, context.Culture, block.ContentType.Alias)
                .AddPropIfNotNull("name", block.Name)
                .AddPropIfNotNull("urls", CreateUrls(context, block))
                .AddProp("sortOrder", block.SortOrder)
                .CopyAllProps(props);
        }

        public JsonNode CreateUrls(IModelFactoryContext2 context, IPublishedContent block)
        {
            return _deliContent.IsPage(block)
                ? new JsonNode()
                    .AddProp("slug", block.UrlSegment(context.Culture))
                    .AddProp("url", _deliUrl.GetDeliveryUrl(block, context.Culture))
                : null;
        }
    }
}
