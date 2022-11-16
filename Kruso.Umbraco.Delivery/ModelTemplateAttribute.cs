using Kruso.Umbraco.Delivery.Extensions;
using System.Linq;

namespace Kruso.Umbraco.Delivery
{
    /// <summary>
    /// Mark your custom class as a ModelFactory (also have the class implement the IModelFactory interface) using this attribute. 
    /// Your class will replace the default one used to generate Page, Block or Navigation objects
    /// </summary>
    public class ModelTemplateAttribute : IdentifiableAttribute
    {
        public TemplateType TemplateType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateType">Specify whether the Factory replaces the Page, Block or Navigation default factory</param>
        public ModelTemplateAttribute(TemplateType templateType)
            : base(templateType.MakeKeys())
        {
            TemplateType = templateType;
        }

        public ModelTemplateAttribute(TemplateType templateType, params string[] documentTypes)
            : base(templateType.MakeKeys(documentTypes))
        {
            TemplateType = templateType;
        }
    }
}
