using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.Models
{
    public class DeliPublishedContent : IPublishedContent
    {
        private readonly IPublishedContent _content;

        public int Id => _content.Id;

        public string Name => _content.Name;

        public string UrlSegment => _content.UrlSegment;

        public int SortOrder => _content.SortOrder;

        public int Level => _content.Level;

        public string Path => _content.Path;

        public int? TemplateId { get; private set; }

        public int CreatorId => _content.CreatorId;

        public DateTime CreateDate => _content.CreateDate;

        public int WriterId => _content.WriterId;

        public DateTime UpdateDate => _content.UpdateDate;

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => _content.Cultures;

        public PublishedItemType ItemType => PublishedItemType.Media;

        public IPublishedContent Parent => null;

        public IEnumerable<IPublishedContent> Children => _content.Children;

        public IEnumerable<IPublishedContent> ChildrenForAllCultures => _content.ChildrenForAllCultures;

        public IPublishedContentType ContentType => _content.ContentType;

        public Guid Key => _content.Key;

        public IEnumerable<IPublishedProperty> Properties => _content.Properties;

        public IPublishedProperty GetProperty(string alias)
        {
            return _content.GetProperty(alias);
        }

        public bool IsDraft(string culture = null)
        {
            return false;
        }

        public bool IsPublished(string culture = null)
        {
            return true;
        }

        public DeliPublishedContent(IPublishedContent content)
        {
            _content = content;
            TemplateId = content.TemplateId;
        }

        public DeliPublishedContent(IPublishedContent content, int templateId)
        {
            _content = content;
            TemplateId = templateId;
        }
    }
}
