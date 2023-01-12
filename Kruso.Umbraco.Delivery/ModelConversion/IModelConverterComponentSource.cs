namespace Kruso.Umbraco.Delivery.ModelConversion
{
    internal interface IModelConverterComponentSource
    {
        IModelNodeConverter GetConverter(TemplateType templateType, string type);
        IModelNodeListConverter GetListConverter(string documentAlias, string propertyName = null);
    }
}