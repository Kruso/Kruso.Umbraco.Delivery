using Kruso.Umbraco.Delivery.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.NestedContent")]
    public class NestedContentPropertyValueFactory : IModelPropertyValueFactory
    {
        private readonly IDeliProperties _deliProperties;
        private readonly IModelFactory _modelFactory;

        public NestedContentPropertyValueFactory(IDeliProperties deliProperties, IModelFactory modelFactory)
        {
            _deliProperties = deliProperties;
            _modelFactory = modelFactory;
        }

        public virtual object Create(IPublishedProperty property)
        {
            var culture = _modelFactory.Context.Culture;
            var contentItems = _modelFactory.CreateBlocks(GetPublishedContent(_modelFactory.Context, property));

            return contentItems;
        }

		private IEnumerable<IPublishedContent> GetPublishedContent(IModelFactoryContext context, IPublishedProperty property)
		{
			var content = _deliProperties.PublishedContentValue<IPublishedContent>(property, context.Culture);
			if (!content?.Any() ?? false)
				content = _deliProperties.PublishedContentValue<IPublishedContent>(property, context.FallbackCulture);

			return content;
		}
	}
}