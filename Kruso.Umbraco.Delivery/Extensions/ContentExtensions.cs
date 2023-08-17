using Kruso.Umbraco.Delivery.ModelGeneration;
using Kruso.Umbraco.Delivery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class ContentExtensions
    {
        public static string? Name(this IPublishedContent content, IModelFactoryContext context)
        {
            var name = string.Empty;

            if (!string.IsNullOrEmpty(context.Culture))
                name = content.Name(context.Culture);

            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(context.FallbackCulture))
                name = content.Name(context.FallbackCulture);

            if (string.IsNullOrEmpty(name))
                name = content.Name;

            return name;
        }

        public static string? Name(this IPublishedContent content, DeliDomainAndUri domain)
        {
            var name = string.Empty;

            if (!string.IsNullOrEmpty(domain.Culture))
                name = content.Name(domain.Culture);

            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(domain.FallbackCulture))
                name = content.Name(domain.FallbackCulture);

            if (string.IsNullOrEmpty(name))
                name = content.Name;

            return name;
        }
    }
}
