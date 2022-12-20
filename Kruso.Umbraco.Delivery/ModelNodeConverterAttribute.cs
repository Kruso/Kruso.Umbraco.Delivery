using Kruso.Umbraco.Delivery.Extensions;
using System.Linq;

namespace Kruso.Umbraco.Delivery
{
    /// <summary>
    /// Mark a class as a ModelConverter (also have the class implement the IModelConverter interface) using this attribute. 
    /// </summary>
    public class ModelNodeConverterAttribute : IdentifiableAttribute
    {
        public TemplateType ConverterType { get; private set; }

        public ModelNodeConverterAttribute(TemplateType converterType = TemplateType.Block)
            : base(converterType.MakeKeys())
        {
            ConverterType = converterType;
        }

        public ModelNodeConverterAttribute(TemplateType converterType, params string[] components)
            : base(converterType.MakeKeys(components))
        {
            
            ConverterType = converterType;
        }
    }
}
