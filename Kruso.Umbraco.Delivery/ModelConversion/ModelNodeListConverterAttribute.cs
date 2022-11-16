﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public class ModelNodeListConverterAttribute : IdentifiableAttribute
    {
        public ModelNodeListConverterAttribute(params string[] propertyNames)
            : base(propertyNames)
        {
            propertyNames = propertyNames.Select(x => x.ToLower()).ToArray();
            if (!propertyNames.Any())
                propertyNames = new[] { "." };

            Components = propertyNames;
        }
    }
}
