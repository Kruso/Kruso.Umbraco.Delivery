using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class DomainExtensions
    {
        public static bool IsAbsoluteDomain(this IDomain domain)
        {
            if (domain == null || string.IsNullOrEmpty(domain.DomainName))
                return false;

            if (domain.DomainName.StartsWith("http"))
                return true;

            var res = domain.DomainName.Split("/").Where(x => !string.IsNullOrEmpty(x)).Count() == 2;
            return res;
        }

        public static bool IsRelativeDomainForPath(this IDomain domain, string path)
        {
            if (domain == null)
                return false;

            path = path.CleanPath().ToLower();

            return !domain.IsAbsoluteDomain() && domain.DomainName.CleanPath().ToLower() == path;
        }

        public static bool IsForHost(this IDomain domain, string host)
        {
            return domain != null && !string.IsNullOrEmpty(domain.DomainName) && !string.IsNullOrEmpty(host)
                ? domain.DomainName.ToLower().StartsWith(host.ToLower())
                : false;
        }
    }
}
