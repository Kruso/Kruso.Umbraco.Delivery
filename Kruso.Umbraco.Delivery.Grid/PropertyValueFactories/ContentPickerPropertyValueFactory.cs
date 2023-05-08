using Kruso.Umbraco.Delivery.Grid.Extensions;
using Kruso.Umbraco.Delivery.Json;
using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Services;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Grid.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.ContentPicker")]
    public class ContentPickerPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public ContentPickerPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var culture = _modelFactory.Context.Culture;
            return _modelFactory.CreateGrid((grid) =>
            {
                var blocks = _modelFactory.CreateGridBlocks(_deliProperties.PublishedContentValue<IPublishedContent>(property, culture));
                grid.AddProp("content", blocks);
            });
        }
    }
}
