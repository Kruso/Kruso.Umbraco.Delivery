using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using Kruso.Umbraco.Delivery.Services.Implementation;
using System;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.ModelGeneration.Templates
{
    [ModelTemplate(TemplateType.Block, "Image", "File", "UmbracoMediaVideo", "UmbracoMediaAudio", "UmbracoMediaArticle", "UmbracoMediaVectorGraphics")]
    public class MediaModelTemplate : IModelTemplate
    {
        private readonly IDeliConfig _deliConfig;

        public MediaModelTemplate(IDeliConfig deliConfig)
        {
            _deliConfig = deliConfig;
        }

        public virtual JsonNode Create(IModelFactoryContext context, JsonNode props, IPublishedContent block)
        {
            var jsonNode = new JsonNode
            {
                Id = block.Key,
                PageId = context.Page?.Key,
                Type = block.ContentType.Alias,
                CompositionTypes = block.ContentType.CompositionAliases?.ToArray()
            };

            jsonNode
                .AddPropIfNotNull("name", block.Name)
                .CopyAllProps(props)
                .RenameProp("umbracoHeight", "originalHeight")
                .RenameProp("umbracoWidth", "originalWidth")
                .RenameProp("umbracoExtension", "extension")
                .RenameProp("umbracoBytes", "bytes")
                .AddProp("src", Src(props))
                .AddProp("alt", Alt(props, block.Name))
                .AddPropIfNotNull("imageCropping", ImageCropping(props, jsonNode.Type))
                .RemoveProp("umbracoFile")
                .RemoveProp("umbracoBytes");

            return jsonNode;
        }

        private JsonNode ImageCropping(JsonNode props, string mediaType)
        {
            return mediaType == "Image"
                ? props.Node("umbracoFile").RemoveProp("url")
                : null;
        }

        private string Alt(JsonNode props, string blockName)
        {
            return !string.IsNullOrEmpty(props.Val<string>("alt"))
                ? props.Val<string>("alt")
                : blockName;
        }

        private string Src(JsonNode block)
        {
            var mediaPath = block.PropIs<string>("umbracoFile")
                ? block.Val<string>("umbracoFile")
                : block.Node("umbracoFile")?.Val<string>("url");

            var mediaCdnUrl = _deliConfig.Get().MediaCdnUrl;
            if (!string.IsNullOrEmpty(mediaPath) && !string.IsNullOrEmpty(mediaCdnUrl))
            {
                var uri = new Uri(new Uri(mediaCdnUrl), mediaPath);
                return uri.AbsoluteUri;
            }

            return mediaPath;
        }
    }
}
