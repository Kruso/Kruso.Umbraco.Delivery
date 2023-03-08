namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public interface IModelConverterComponentSource
    {

        bool HasConverters();
        bool HasListConverters();
        IModelNodeConverter GetConverter(TemplateType templateType, string type, string context);
        IModelNodeListConverter GetListConverter(string documentAlias, string propertyName = null);
    }
}