using System;

namespace Kruso.Umbraco.Delivery
{
    /// <summary>
    /// Mark your class as a ModelPropertyValueFactory (with the IPropertyModelValueFactory interface) to replace
    /// the default property value factory for a given property's editorAlias. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModelPropertyValueFactoryAttribute : IdentifiableAttribute
    {
        public ModelPropertyValueFactoryAttribute()
            : base("")
        {
        }

        public ModelPropertyValueFactoryAttribute(string editorAlias)
            : base(editorAlias)
        {
        }

        public ModelPropertyValueFactoryAttribute(string editorAlias, string documentType, string propertyName)
            : base($"{editorAlias}+{documentType}+{propertyName}")
        {
        }

        public ModelPropertyValueFactoryAttribute(string editorAlias, string alias)
            : base($"{editorAlias}+{alias}")
        {
        }

        public ModelPropertyValueFactoryAttribute(string[] editorAliases)
            : base(editorAliases)
        {
        }
    }
}
