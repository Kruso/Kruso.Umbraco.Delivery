using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Extensions
{
    public static class TemplateTypeExtensions
    {
        public static string MakeKey(this TemplateType templateType, string componentType = null)
        {
            return string.IsNullOrEmpty(componentType)
                ? templateType.ToString().ToLower()
                : $"{templateType}+{componentType}".ToLower();
        }

        public static string[] MakeKeys(this TemplateType templateType, params string[] componentTypes)
        {
            return componentTypes?.Any() == true
                ? componentTypes.Select(c => $"{templateType}+{c}".ToLower()).ToArray()
                : new string[] { templateType.ToString().ToLower() };
        }
    }
}
