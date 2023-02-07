using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Kruso.Umbraco.Delivery.Services.Implementation
{
    public class DeliDataTypes : IDeliDataTypes
    {
        private readonly IDeliCache _deliCache;
        private readonly IDataTypeService _dataTypeService;

        public DeliDataTypes(IDeliCache deliCache, IDataTypeService dataTypeService)
        {
            _deliCache = deliCache;
            _dataTypeService = dataTypeService;
        }

        public IDataType DataType(int id)
        {
            if (id > 0)
            {
                var key = $"Umb_DataType_{id}";
                var res = _deliCache.GetFromRequest<IDataType>(key);
                if (res == null)
                {
                    res = _dataTypeService.GetDataType(id);
                    if (res != null)
                        _deliCache.AddToRequest(key, res);
                }

                return res;
            }

            return null;
        }

        public IEnumerable<string> PreValues(int id)
        {
            var dataType = DataType(id);
            if (dataType != null)
            {
                var configuration = dataType.Configuration as ValueListConfiguration;
                var preValues = (configuration?.Items.Select(x => x.Value) ?? Enumerable.Empty<string>()).ToList();

                return preValues;
            }

            return Enumerable.Empty<string>();
        }

        public T EditorConfiguration<T>(int id)
            where T : class
        {
            var dataType = DataType(id);
            if (dataType != null)
            {
                var configuration = dataType.Configuration as T;
                return configuration;
            }

            return default(T);
        }
    }
}
