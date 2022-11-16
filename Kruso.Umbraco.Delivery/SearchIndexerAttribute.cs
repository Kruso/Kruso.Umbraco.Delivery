using System.Linq;

namespace Kruso.Umbraco.Delivery
{
    /// <summary>
    /// Mark your custom class as a ModelFactory (also have the class implement the IModelFactory interface) using this attribute. 
    /// Your class will replace the default one used to generate Page, Block or Navigation objects
    /// </summary>
    public class SearchIndexerAttribute : IdentifiableAttribute
    {
        public string Index { get; private set; }

        public SearchIndexerAttribute(string index, params string[] contentTypeAliases)
            : base(contentTypeAliases.Select(x => $"{index}.{x}".ToLower()).ToArray())
        {
            Index = index;
        }
    }
}
