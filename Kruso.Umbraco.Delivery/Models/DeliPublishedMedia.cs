using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Models
{
    internal class DeliPublishedMedia : IPublishedContent
    {
        private readonly IMedia _media;

        public int Id => _media.Id;

        public string Name => _media.Name;

        public string UrlSegment => String.Empty;

        public int SortOrder => -1;

        public int Level => -1;

        public string Path => _media.Path;

        public int? TemplateId => null;

        public int CreatorId => _media.CreatorId;

        public DateTime CreateDate => _media.CreateDate;

        public int WriterId => _media.WriterId;

        public DateTime UpdateDate => _media.UpdateDate;

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => throw new NotImplementedException("Cultures not implemented");

        public PublishedItemType ItemType => PublishedItemType.Media;

        public IPublishedContent Parent => null;

        public IEnumerable<IPublishedContent> Children => Enumerable.Empty<IPublishedContent>();

        public IEnumerable<IPublishedContent> ChildrenForAllCultures => Enumerable.Empty<IPublishedContent>();

        public IPublishedContentType ContentType => new DeliContentType(_media.ContentType, Properties.Select(x => x.PropertyType).ToList());

        public Guid Key => _media.Key;

        public IEnumerable<IPublishedProperty> Properties => _media.Properties.Select(x => new DeliProperty(x));

        public IPublishedProperty GetProperty(string alias)
        {
            throw new NotImplementedException();
        }

        public bool IsDraft(string culture = null)
        {
            return false;
        }

        public bool IsPublished(string culture = null)
        {
            return true;
        }

        public DeliPublishedMedia(IMedia media)
        {
            _media = media;
        }
    }
}
