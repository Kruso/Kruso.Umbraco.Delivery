using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Models
{
    internal class DeliRefContent : IPublishedContent
    {
        private class DeliRefContentType : IPublishedContentType
        {
            public Guid Key { get; private set; }

            public int Id => throw new NotImplementedException();

            public string Alias { get; private set; }

            public PublishedItemType ItemType => PublishedItemType.Element;

            public HashSet<string> CompositionAliases => new HashSet<string>();

            public ContentVariation Variations => throw new NotImplementedException();

            public bool IsElement => true;

            public IEnumerable<IPublishedPropertyType> PropertyTypes => Enumerable.Empty<IPublishedPropertyType>();

            public int GetPropertyIndex(string alias)
            {
                return -1;
            }

            public IPublishedPropertyType GetPropertyType(string alias)
            {
                return null;
            }

            public IPublishedPropertyType GetPropertyType(int index)
            {
                return null;
            }

            internal DeliRefContentType(string alias)
            {
                Alias = alias;
            }
        }

        public int Id { get; private set; }

        public string Name => string.Empty;

        public string UrlSegment => throw new NotImplementedException();

        public int SortOrder => throw new NotImplementedException();

        public int Level => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public int? TemplateId => throw new NotImplementedException();

        public int CreatorId => throw new NotImplementedException();

        public DateTime CreateDate => throw new NotImplementedException();

        public int WriterId => throw new NotImplementedException();

        public DateTime UpdateDate => throw new NotImplementedException();

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => throw new NotImplementedException();

        public PublishedItemType ItemType => throw new NotImplementedException();

        public IPublishedContent Parent => throw new NotImplementedException();

        public IEnumerable<IPublishedContent> Children => throw new NotImplementedException();

        public IEnumerable<IPublishedContent> ChildrenForAllCultures => throw new NotImplementedException();

        public IPublishedContentType ContentType { get; private set; }

        public Guid Key { get; private set; }

        public IEnumerable<IPublishedProperty> Properties => throw new NotImplementedException();

        public IPublishedProperty GetProperty(string alias)
        {
            throw new NotImplementedException();
        }

        public bool IsDraft(string culture = null)
        {
            throw new NotImplementedException();
        }

        public bool IsPublished(string culture = null)
        {
            throw new NotImplementedException();
        }

        public DeliRefContent(Guid key, string contentTypeAlias)
        {
            Key = key;
            ContentType = new DeliRefContentType(contentTypeAlias);
        }
    }
}
