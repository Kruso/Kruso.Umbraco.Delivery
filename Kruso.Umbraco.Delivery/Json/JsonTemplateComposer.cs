using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Kruso.Umbraco.Delivery.Json
{
    public class JsonTemplateComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddComponent<JsonTemplateComponent>();
        }
    }
}
