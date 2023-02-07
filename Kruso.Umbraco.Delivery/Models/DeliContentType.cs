using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Models
{
    internal class DeliContentType : IPublishedContentType
    {
        private readonly IContentType _contentType;
        private readonly ISimpleContentType _simpleContentType;
        private readonly List<IPublishedPropertyType> _propertyTypes;


        internal DeliContentType(IContentType contentType, List<IPublishedPropertyType> propertyTypes)
        {
            _contentType = contentType;
            _propertyTypes = propertyTypes;
        }

        internal DeliContentType(ISimpleContentType contentType, List<IPublishedPropertyType> propertyTypes)
        {
            _simpleContentType = contentType;
            _propertyTypes = propertyTypes;
        }

        public Guid Key => _contentType?.Key ?? _simpleContentType.Key;

        public int Id => _contentType?.Id ?? _simpleContentType.Id;

        public string Alias => _contentType?.Alias ?? _simpleContentType.Alias;

        public PublishedItemType ItemType => throw new NotImplementedException();

        public HashSet<string> CompositionAliases => new HashSet<string>();

        public ContentVariation Variations => _contentType?.Variations ?? _simpleContentType.Variations;

        public bool IsElement => _contentType?.IsElement ?? _simpleContentType.IsElement;

        public IEnumerable<IPublishedPropertyType> PropertyTypes => _propertyTypes;

        public int GetPropertyIndex(string alias)
        {
            throw new NotImplementedException();
        }

        public IPublishedPropertyType GetPropertyType(string alias)
        {
            return PropertyTypes.FirstOrDefault(x => x.Alias == alias);
        }

        public IPublishedPropertyType GetPropertyType(int index)
        {
            return (PropertyTypes as List<IPublishedPropertyType>)[index];
        }
    }

}
