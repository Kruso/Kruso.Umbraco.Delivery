using Kruso.Umbraco.Delivery.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.PropertyValueFactories
{
    [ModelPropertyValueFactory("Umbraco.NestedContent")]
    public class NestedContentPropertyValueFactory : IModelPropertyValueFactory
    {
        protected readonly IDeliProperties _deliProperties;
		protected readonly IModelFactory _modelFactory;

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

		protected virtual IEnumerable<IPublishedElement> GetPublishedContent(IModelFactoryContext context, IPublishedProperty property)
		{
			var items = _deliProperties.PublishedContentValue<IPublishedElement>(property, context.Culture);
			if (!items?.Any() ?? false)
				items = _deliProperties.PublishedContentValue<IPublishedElement>(property, context.FallbackCulture);

			return items;
		}
	}
}