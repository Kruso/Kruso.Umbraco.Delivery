using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Umbraco.Cms.Core.PropertyEditors.ImageCropperConfiguration;

namespace Kruso.Umbraco.Delivery.Json
{
    internal class JsonNodePropertyParser
    {
        private JsonNode _node;
        internal string Target { get; private set; }
        internal string[] TargetPath { get; private set; } = new string[0];
        internal string Source { get; private set; }
        internal string[] SourcePath { get; private set; } = new string[0];

        internal bool IsValidForGet => _node != null && !string.IsNullOrEmpty(Source);
        internal bool IsValidForUpdate => _node != null && !string.IsNullOrEmpty(Target);
        internal bool IsValidForCopy => IsValidForGet && IsValidForUpdate;

        internal static JsonNodePropertyParser Create(JsonNode node, string source)
        {
            var res = new JsonNodePropertyParser();

            var propParts = ParseTargetSource(source);
            if (propParts.Length == 1)
            {
                res.SourcePath = Parse(propParts[0], out var sourceProp);
                res.Source = sourceProp;
                res.Target = sourceProp;
            }
            else if (propParts.Length == 2)
            {
                res.TargetPath = Parse(propParts[0], out var targetProp);
                res.Target = targetProp;

                res.SourcePath = Parse(propParts[1], out var sourceProp);
                res.Source = sourceProp;
            }

            res._node = node;

            return res;
        }

        internal static JsonNodePropertyParser Create(JsonNode node, string target, string source)
        {
            var res = Create(node, source);

            var propParts = ParseTargetSource(target);
            if (propParts.Length == 1)
            {
                res.TargetPath = Parse(target, out var targetProp);
                res.Target = targetProp;
            }

            return res;
        }

        private static string[] Parse(string prop, out string propName)
        {
            var segs = ParseSegments(prop);

            propName = segs.LastOrDefault();
            var path = segs.Any()
                ? segs.Take(segs.Length - 1).ToArray()
                : segs;

            return path;
        }

        private static string[] ParseSegments(string segs)
        {
            return segs?
                .Split('.')
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray() ?? new string[0];
        }

        private static string[] ParseTargetSource(string propName)
        {
            return propName?
                .TrimStart('{')
                .TrimEnd('}')
                .Split(':')
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray() ?? new string[0];
        }
    }
}
