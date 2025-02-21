using System;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.Models
{
    internal class DeliPropertyType : IPublishedPropertyType
    {
        private readonly IPropertyType _propertyType;

        internal DeliPropertyType(IPropertyType propertyType)
        {
            _propertyType = propertyType;
        }

        public IPublishedContentType ContentType => throw new NotImplementedException();

        public PublishedDataType DataType { get; set; }

        public int DataTypeId
        {
            get
            {
                return _propertyType.DataTypeId;
            }
        }

        public string Alias => _propertyType.Alias;

        public string EditorAlias => _propertyType.PropertyEditorAlias;

        public bool IsUserProperty => true;

        public ContentVariation Variations => _propertyType.Variations;

        public PropertyCacheLevel CacheLevel => throw new NotImplementedException();
        public PropertyCacheLevel DeliveryApiCacheLevel { get; }

        public Type ModelClrType => throw new NotImplementedException();

        public Type ClrType => throw new NotImplementedException();

        public object ConvertInterToObject(IPublishedElement owner, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            throw new NotImplementedException();
        }

        public object ConvertInterToXPath(IPublishedElement owner, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            throw new NotImplementedException();
        }

        public object ConvertInterToDeliveryApiObject(IPublishedElement owner, PropertyCacheLevel referenceCacheLevel, object inter,
            bool preview, bool expanding)
        {
            throw new NotImplementedException();
        }

        public object ConvertSourceToInter(IPublishedElement owner, object source, bool preview)
        {
            throw new NotImplementedException();
        }

        public bool? IsValue(object value, PropertyValueLevel level)
        {
            return true;
        }
    }
}
