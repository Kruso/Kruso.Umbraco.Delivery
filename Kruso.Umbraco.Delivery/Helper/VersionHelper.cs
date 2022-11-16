using Kruso.Umbraco.Delivery.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Kruso.Umbraco.Delivery.Helper
{
    public static class VersionHelper
    {
        public static List<AssemblyVersion> _assemblyVersions = new List<AssemblyVersion>();

        public static AssemblyVersion[] GetVersions()
        {
            return _assemblyVersions.ToArray();
        }

        public static void RegisterVersion(string assemblyName)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = allAssemblies.FirstOrDefault(x => x.GetName().Name.ToLower() == assemblyName.ToLower());

            RegisterVersion(assembly);
        }

        public static void RegisterVersion(Assembly assembly)
        {
            if (assembly != null && !_assemblyVersions.Any(x => x.Assembly == assembly.GetName().Name))
            {
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                var av = new AssemblyVersion
                {
                    Assembly = assembly.GetName().Name,
                    Version = fvi.FileVersion,
                    BuildTime = GetBuildDate(assembly)
                };

                _assemblyVersions.Add(av);
            }
        }

        private static DateTime GetBuildDate(Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value.Substring(index + BuildVersionMetadataPrefix.Length);
                    if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }
            }

            return DateTime.MinValue;
        }
    }
}
