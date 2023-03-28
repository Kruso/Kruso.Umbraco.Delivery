using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Models
{
    internal class DeliPublishedElement : IPublishedContent
    {
        private readonly IPublishedContent _page;
        private readonly IPublishedElement _element;

        public int Id => -1;

        public string Name => null;

        public string UrlSegment => _page?.UrlSegment ?? string.Empty;

        public int SortOrder => -1;

        public int Level => -1;

        public string Path => _page?.Path ?? string.Empty;

        public int? TemplateId => _page?.TemplateId ?? -1;

        public int CreatorId => _page?.CreatorId ?? -1;

        public DateTime CreateDate => _page?.CreateDate ?? DateTime.MinValue;

        public int WriterId => _page?.WriterId ?? -1;

        public DateTime UpdateDate => _page?.UpdateDate ?? DateTime.MinValue;

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => _page?.Cultures ?? new Dictionary<string, PublishedCultureInfo>();

        public PublishedItemType ItemType => PublishedItemType.Element;

        public IPublishedContent Parent => _page;

        public IEnumerable<IPublishedContent> Children => Enumerable.Empty<IPublishedContent>();

        public IEnumerable<IPublishedContent> ChildrenForAllCultures => Enumerable.Empty<IPublishedContent>();

        public IPublishedContentType ContentType => _element.ContentType;

        public Guid Key => _element.Key;

        public IEnumerable<IPublishedProperty> Properties => _element.Properties;

        public IPublishedProperty GetProperty(string alias)
        {
            return _element.GetProperty(alias);
        }

        public bool IsDraft(string culture = null)
        {
            return _page?.IsDraft(culture) ?? false;
        }

        public bool IsPublished(string culture = null)
        {
            return _page?.IsPublished(culture) ?? true;
        }

        public DeliPublishedElement(IPublishedContent page, IPublishedElement element)
        {
            _page = page;
            _element = element ?? throw new ArgumentException("element is null");
        }
    }
}
