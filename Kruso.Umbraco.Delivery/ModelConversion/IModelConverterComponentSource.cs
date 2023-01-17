namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public interface IModelConverterComponentSource
    {
        IModelNodeConverter GetConverter(TemplateType templateType, string type);
        IModelNodeListConverter GetListConverter(string documentAlias, string propertyName = null);
    }
}