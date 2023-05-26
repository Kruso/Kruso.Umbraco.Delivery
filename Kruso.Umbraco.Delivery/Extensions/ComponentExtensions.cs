using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class ComponentExtensions
    {
        public static Dictionary<string, T> ToFilteredDictionary<T, TA>(this IEnumerable<T> components)
            where T : class
            where TA : IdentifiableAttribute
        {
            var thisNamespace = typeof(ComponentExtensions).Namespace;
            var res = new Dictionary<string, T>();

            foreach (var component in components.OrderBy(x => x.GetType().Namespace == thisNamespace ? 0 : 1))
            {
                foreach (var attr in component.GetType().GetCustomAttributes(typeof(TA), true).Cast<TA>())
                {
                    if (attr != null)
                    {
                        foreach (var c in attr.Components)
                        {
                            if (!res.ContainsKey(c))
                            {
                                res.Add(c, component);
                            }
                            else
                            {
                                res[c] = component;
                            }
                        }
                    }
                }
            }

            return res;
        }
    }
}
