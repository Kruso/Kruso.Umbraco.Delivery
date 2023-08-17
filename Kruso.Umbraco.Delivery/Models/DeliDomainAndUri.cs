using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Routing;

namespace Kruso.Umbraco.Delivery.Models
{
    public class DeliDomainAndUri : DomainAndUri
    {
        public string FallbackCulture { get; private set; }

        public DeliDomainAndUri(Domain domain, Uri currentUri, string fallbackCulture)
            : base(domain, currentUri)
        {
            FallbackCulture = fallbackCulture;
        }
    }
}
