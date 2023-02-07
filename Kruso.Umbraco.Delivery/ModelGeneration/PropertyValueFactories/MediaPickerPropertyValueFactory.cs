using Kruso.Umbraco.Delivery.Services;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.MediaPicker3")]
    public class MediaPickerPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public MediaPickerPropertyValueFactory(IDeliDataTypes deliDataTypes, IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var culture = _modelFactory.Context.Culture;
            var mediaItems = _modelFactory.CreateBlocks(_deliProperties.PublishedContentValue<IPublishedContent>(property, culture));

            var editorConfiguration = _deliDataTypes.EditorConfiguration<MediaPicker3Configuration>(property.PropertyType.DataType.Id);
            return editorConfiguration?.Multiple == true
                ? mediaItems
                : mediaItems.FirstOrDefault();
        }
    }
}
