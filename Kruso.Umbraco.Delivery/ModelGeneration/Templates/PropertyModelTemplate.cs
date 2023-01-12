using Kruso.Umbraco.Delivery.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Kruso.Umbraco.Delivery.ModelGeneration.Templates
{
    public class PropertyModelTemplate : IPropertyModelTemplate
    {
        public object Create(IPublishedElement content, IPublishedProperty property, object val)
        {
            return val;
        }
    }
}
