namespace Kruso.Umbraco.Delivery
{
    /// <summary>
    /// Mark your class as a ModelPropertyValueFactory (with the IPropertyModelValueFactory interface) to replace
    /// the default property value factory for a given property's editorAlias. 
    /// </summary>
    public class PropertyValueFactoryAttribute : IdentifiableAttribute
    {
        public PropertyValueFactoryAttribute()
            : base("")
        {
        }

        public PropertyValueFactoryAttribute(string editorAlias)
            : base(editorAlias)
        {
        }

        public PropertyValueFactoryAttribute(string editorAlias, string documentType, string propertyName)
            : base($"{editorAlias}+{documentType}+{propertyName}")
        {
        }

        public PropertyValueFactoryAttribute(string editorAlias, string alias)
            : base($"{editorAlias}+{alias}")
        {
        }

        public PropertyValueFactoryAttribute(string[] editorAliases)
            : base(editorAliases)
        {
        }
    }
}
