using Kruso.Umbraco.Delivery.ModelGeneration.Templates;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public interface IModelFactoryComponentSource
    {
        IPropertyModelTemplate GetPropertyModelTemplate();
        IModelPropertyValueFactory GetPropertyValueFactory(IPublishedElement content, IPublishedProperty property);
        IModelTemplate GetTemplate(TemplateType templateType, IPublishedElement content = null);
    }
}