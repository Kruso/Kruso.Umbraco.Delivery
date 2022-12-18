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
            public Guid Key => throw new NotImplementedException();

            public int Id => throw new NotImplementedException();

            public string Alias { get; private set; }

            public PublishedItemType ItemType => throw new NotImplementedException();

            public HashSet<string> CompositionAliases => throw new NotImplementedException();

            public ContentVariation Variations => throw new NotImplementedException();

            public bool IsElement => throw new NotImplementedException();

            public IEnumerable<IPublishedPropertyType> PropertyTypes => throw new NotImplementedException();

            public int GetPropertyIndex(string alias)
            {
                throw new NotImplementedException();
            }

            public IPublishedPropertyType GetPropertyType(string alias)
            {
                throw new NotImplementedException();
            }

            public IPublishedPropertyType GetPropertyType(int index)
            {
                throw new NotImplementedException();
            }

            internal DeliRefContentType(string alias)
            {
                Alias = alias;
            }
        }
        public int Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

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
