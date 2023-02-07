using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.ModelGeneration
{
    public class ModelProperty
    {
        public string Name { get; }
        public string Type { get; set; }
        public object Value { get; set; }
        public bool Error { get; set; }

        public ModelProperty(string propertyName)
        {
            Name = propertyName;
        }
    }
}
