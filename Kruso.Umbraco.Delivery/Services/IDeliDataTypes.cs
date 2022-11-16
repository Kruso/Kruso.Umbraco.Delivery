using System.Collections.Generic;
using Umbraco.Cms.Core.Models;

namespace Kruso.Umbraco.Delivery.Services
{
    public interface IDeliDataTypes
    {
        IDataType DataType(int id);
        IEnumerable<string> PreValues(int id);
        T EditorConfiguration<T>(int id) where T : class;
    }
}