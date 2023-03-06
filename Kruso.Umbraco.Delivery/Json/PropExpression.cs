using System.Diagnostics;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Json
{
    internal struct Prop
    {
        public string Name { get; set; }
        public string[] Path { get; set; } = new string[0];
        public object DefaultValue { get; set; }

        internal Prop(string propExpr) 
        {
            var segments = ParseSegments(propExpr);
            if (!segments.Any())
                throw new JsonNodeException($"Invalid property expression {propExpr}");

            var nameParts = segments.Last().Split('=');
            if (nameParts.Length is < 1 or > 2)
                throw new JsonNodeException($"Invalid property expression {propExpr}");

            Name = nameParts.First();

            DefaultValue = GetDefaultValue(nameParts.Length == 2
                ? nameParts[1]
                : null);

            if (segments.Length > 1)
                Path = segments.Take(segments.Length - 1).ToArray();
        }

        private string[] ParseSegments(string propExpr) 
            => propExpr?
                .Split('.')
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray() ?? new string[0];

        private object GetDefaultValue(string val)
        {
            if (val == null) 
                return null;

            if (int.TryParse(val, out var i))
                return i;

            return val;
        }
    }

    internal class PropExpression
    {
        private string _expr;

        private Prop Source { get; set; }
        private Prop Target { get; set; }

        private bool SamePropExpressions = false;

        public PropExpression(string propExpr)
        {
            _expr = propExpr;

            var propstrs = Parse(propExpr);

            if (propstrs.Length is < 1 or > 2)
                throw new JsonNodeException($"Invalid property expression {propExpr}");

            if (propstrs.Length == 1)
            {
                Source = new Prop(propstrs[0]);
                Target = new Prop(propstrs[0]);

                SamePropExpressions = true;
            }
            else if (propstrs.Length == 2)
            {
                Source = new Prop(propstrs[1]);
                Target = new Prop(propstrs[0]);
            }
        }

        internal bool CanRead(JsonNode source) => source != null && !string.IsNullOrEmpty(Source.Name);

        internal bool CanWrite(JsonNode target) => target != null && !string.IsNullOrEmpty(Target.Name);

        internal bool CanCopy(JsonNode target, JsonNode source) => CanRead(source) && CanWrite(target);

        internal bool CanRename(JsonNode target) 
            => CanWrite(target) 
            && !string.IsNullOrEmpty(Source.Name)
            && !Source.Name.Equals(Target.Name)
            && !Source.Path.Any();

        internal bool Exists(JsonNode node) => CanRead(node) && (GetNodeFromPath(Source.Path, node)?.PropExists(Source.Name) ?? false);

        internal object Read(JsonNode source)
        {
            return CanRead(source)
                ? (GetNodeFromPath(Source.Path, source)?[Source.Name] ?? Source.DefaultValue)
                : null;
        }

        internal void Write(JsonNode target, object val) 
        {
            if (CanWrite(target))
                AddNodeFromPath(Target.Path, target)[Target.Name] = val ?? Source.DefaultValue;       
        }

        internal void Copy(JsonNode target, JsonNode source)
        {
            if (CanCopy(target, source))
            {
                var val = Read(source);
                if (SamePropExpressions)
                    target[Target.Name] = val;
                else
                    Write(target, val);
            }
        }

        internal void Remove(JsonNode node)
        {
            if (CanWrite(node))
                GetNodeFromPath(Target.Path, node)?.Remove(Target.Name);
        }

        internal void Rename(JsonNode node)
        {
            if (CanRename(node))
            {

                node = GetNodeFromPath(Target.Path, node);
                if (node != null)
                {
                    var val = node[Target.Name];
                    node.Remove(Target.Name);
                    node[Source.Name] = val;
                }
            }
        }

        private string[] Parse(string propExpr)
            => propExpr?
                .TrimStart('{')
                .TrimEnd('}')
                .Split(':')
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray() ?? new string[0];


        private JsonNode GetNodeFromPath(string[] path, JsonNode source)
        {
            foreach (var seg in path)
            {
                source = source?[seg] as JsonNode;
                if (source == null)
                    break;
            }

            return source;
        }

        private JsonNode AddNodeFromPath(string[] path, JsonNode target)
        {
            if (target == null)
                return null;

            var currNode = target;
            foreach (var seg in path)
            {
                var nextNode = currNode?[seg] as JsonNode;
                if (nextNode == null)
                {
                    nextNode = new JsonNode();
                    currNode[seg] = nextNode;
                }

                currNode = nextNode;
            }

            return currNode;
        }
    }
}
