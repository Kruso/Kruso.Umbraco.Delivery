using Kruso.Umbraco.Delivery.Extensions;
using Kruso.Umbraco.Delivery.ModelGeneration.Templates;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public class ModelFactoryComponentSource : IModelFactoryComponentSource
    {
        private Dictionary<string, IModelTemplate> _modelTemplates = null;
        private Dictionary<string, IModelPropertyValueFactory> _propertyValueFactories = null;
        private readonly IPropertyModelTemplate _propertyModelTemplate;

        public ModelFactoryComponentSource(
            IEnumerable<IModelTemplate> modelTemplates,
            IEnumerable<IModelPropertyValueFactory> modelPropertyValueFactories,
            IPropertyModelTemplate propertyModelTemplate)
        {
            _modelTemplates = modelTemplates.ToFilteredDictionary<IModelTemplate, ModelTemplateAttribute>();
            _propertyValueFactories = modelPropertyValueFactories.ToFilteredDictionary<IModelPropertyValueFactory, ModelPropertyValueFactoryAttribute>();
            _propertyModelTemplate = propertyModelTemplate;
        }

        public IModelTemplate GetTemplate(TemplateType templateType, IPublishedElement content = null)
        {
            var key = templateType.MakeKey(content?.ContentType?.Alias);

            if (_modelTemplates.ContainsKey(key))
            {
                return _modelTemplates[key];
            }

            var defaultKey = templateType.MakeKey();
            if (_modelTemplates.ContainsKey(defaultKey))
            {
                return _modelTemplates[defaultKey];
            }

            return null;
        }

        public IModelPropertyValueFactory GetPropertyValueFactory(IPublishedElement content, IPublishedProperty property)
        {
            if (content == null || property == null)
                return null;

            return GetPropertyResolverIfExists($"{property.PropertyType.EditorAlias}+{content.ContentType.Alias}+{property.Alias}")
                ?? GetPropertyResolverIfExists($"{property.PropertyType.EditorAlias}+{property.PropertyType.Alias}")
                ?? GetPropertyResolverIfExists(property.PropertyType.EditorAlias)
                ?? GetPropertyResolverIfExists("");
        }

        public IPropertyModelTemplate GetPropertyModelTemplate()
        {
            return _propertyModelTemplate;
        }

        private IModelPropertyValueFactory GetPropertyResolverIfExists(string key)
        {
            key = key.ToLower();
            return _propertyValueFactories.ContainsKey(key)
                ? _propertyValueFactories[key]
                : null;
        }
    }
}
