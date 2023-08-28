using Kruso.Umbraco.Delivery.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
	[ModelPropertyValueFactory("Umbraco.MultiNodeTreePicker")]
    public class MultiNodeTreePickerPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliDataTypes _deliDataTypes;
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public MultiNodeTreePickerPropertyValueFactory(IDeliDataTypes deliDataTypes, IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliDataTypes = deliDataTypes;
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var contentItems = GetPublishedContent(_modelFactory.Context, property);
			var blocks = _modelFactory.CreateBlocks(contentItems);

            var configuration = _deliDataTypes.EditorConfiguration<MultiNodePickerConfiguration>(property.PropertyType.DataType.Id);
            return configuration?.MaxNumber == 1
                ? blocks.FirstOrDefault()
                : blocks;
        }

        private IEnumerable<IPublishedContent> GetPublishedContent(IModelFactoryContext context, IPublishedProperty property)
		{
            var content = _deliProperties.PublishedContentValue<IPublishedContent>(property, context.Culture);
            if (!content?.Any() ?? false)
                content = _deliProperties.PublishedContentValue<IPublishedContent>(property, context.FallbackCulture);

            return content
				.Where(x => x.IsPublished(context.Culture))
				.ToList();
		}
    }
}
