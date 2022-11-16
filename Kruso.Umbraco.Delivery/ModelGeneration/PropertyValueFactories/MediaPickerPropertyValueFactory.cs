using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [PropertyValueFactory("Umbraco.MediaPicker3")]
    public class MediaPickerPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IDeliProperties _deliProperties;
        private readonly IDeliMedia _deliMedia;
        private readonly IDeliConfig _deliConfig;

        public MediaPickerPropertyValueFactory(IDeliDataTypes deliDataTypes, IDeliProperties deliProperties, IDeliMedia deliMedia, IDeliConfig deliConfig)
        {
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
            _deliMedia = deliMedia;
            _deliConfig = deliConfig;
        }

        public virtual object Create(IModelFactoryContext context, IPublishedProperty property)
        
        {
            IEnumerable<JsonNode> res = null;
            var value = _deliProperties.Value(property, context.Culture);
            if (value is string)
            {
                res = CreateFromUnpublishedReferences(context, value.ToString());
            }
            else if (value != null)
            {
                res = CreateFromPublishedContent(context, value) ?? CreateFromMedia(context, value);
            }

            res ??= Enumerable.Empty<JsonNode>();

            var editorConfiguration = _deliDataTypes.EditorConfiguration<MediaPicker3Configuration>(property.PropertyType.DataType.Id);
            return editorConfiguration?.Multiple == true
                ? res
                : res.FirstOrDefault();
        }

        private IEnumerable<JsonNode> CreateFromUnpublishedReferences(IModelFactoryContext context, string val)
        {
            var mediaItems = new List<JsonNode>();

            if (!string.IsNullOrEmpty(val))
            {
                var refs = val.ToString().Split(',');
                foreach (var r in refs)
                {
                    var media = _deliMedia.GetMedia(r);
                    var block = ConvertBlock(context.ModelFactory.CreateBlock(media));
                    if (block != null)
                    {
                        mediaItems.Add(block);
                    }
                }
            }

            return mediaItems;
        }

        private IEnumerable<JsonNode> CreateFromPublishedContent(IModelFactoryContext context, object value)
        {
            var mediaItems = new List<JsonNode>();

            foreach (var content in GetValueAsList<IPublishedContent>(value))
            {
                var block = ConvertBlock(context.ModelFactory.CreateBlock(content));
                if (block != null)
                {
                    mediaItems.Add(block);
                }
            }

            return mediaItems.Any()
                ? mediaItems
                : null;
        }

        private IEnumerable<JsonNode> CreateFromMedia(IModelFactoryContext context, object value)
        {
            var mediaItems = new List<JsonNode>();

            foreach (var media in GetValueAsList<IMedia>(value))
            {
                var block = ConvertBlock(context.ModelFactory.CreateBlock(media));
                if (block != null)
                {
                    mediaItems.Add(block);
                }
            }

            return mediaItems;
        }

        private JsonNode ConvertBlock(JsonNode block)
        {
            if (block != null && !block.Val<bool>("isRef"))
            {
                var props = block.Node("props");
                if (props != null)
                {
                    block
                        .CopyAllProps(props)
                        .RemoveProp("props");
                }

                block
                    .RenameProp("umbracoHeight", "originalHeight")
                    .RenameProp("umbracoWidth", "originalWidth")
                    .RenameProp("umbracoExtension", "extension")
                    .RenameProp("umbracoBytes", "bytes")
                    .AddProp("src", MediaUrl(block))
                    .AddProp("imageCropping", block.Node("umbracoFile").RemoveProp("url"))
                    .AddPropIfNotExists("alt", block.Val<string>("name"))
                    .RemoveProp("umbracoFile")
                    .RemoveProp("culture")
                    .RemoveProp("sortOrder")
                    .RemoveProp("slug")
                    .RemoveProp("url")
                    .RemoveProp("bytes");
            }

            return block;
        }

        private IEnumerable<T> GetValueAsList<T>(object value)
            where T: class
        {
            var res = new List<T>();
            if (value != null)
            {
                if (value is T)
                {
                    res.Add(value as T);
                }
                else if (value is IEnumerable<T>)
                {
                    res.AddRange(value as IEnumerable<T>);
                }
            }
            return res;
        }

        private string MediaUrl(JsonNode block)
        {
            var uFile = block.PropIs<string>("umbracoFile") ? block.Val<string>("umbracoFile") : string.Empty;

            var mediaPath = block.Node("umbracoFile")?.Val<string>("url");
            if (string.IsNullOrEmpty(mediaPath))
                mediaPath = block.PropIs<string>("umbracoFile")
                    ? block.Val<string>("umbracoFile")
                    : string.Empty;

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
