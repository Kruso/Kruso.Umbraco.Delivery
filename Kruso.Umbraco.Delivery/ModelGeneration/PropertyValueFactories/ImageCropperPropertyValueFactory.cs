using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.Services;
using Newtonsoft.Json;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.ImageCropper")]
    public class ImageCropperPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public ImageCropperPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var imageCropper = _deliProperties.Value(property, _modelFactory.Context.Culture) as ImageCropperValue;
            if (imageCropper != null)
            {
                return new JsonNode()
                    .AddProp("url", imageCropper.Src)
                    .AddProp("focalPoint", new JsonNode()
                        .AddProp("top", imageCropper.FocalPoint?.Top)
                        .AddProp("left", imageCropper.FocalPoint?.Left))
                    .AddProp("crops", imageCropper.Crops?
                        .Select(x => new JsonNode()
                            .AddProp("alias", x.Alias)
                            .AddProp("height", x.Height)
                            .AddProp("width", x.Width)
                            .AddProp("coordinates", new JsonNode()
                                .AddProp("x1", x.Coordinates?.X1)
                                .AddProp("x2", x.Coordinates?.X2)
                                .AddProp("y1", x.Coordinates?.Y1)
                                .AddProp("y2", x.Coordinates?.Y2))));
            }

            return null;
        }
    }
}
