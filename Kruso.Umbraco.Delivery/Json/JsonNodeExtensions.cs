using Microsoft.CodeAnalysis.CSharp.Syntax;
using NPoco.ArrayExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kruso.Umbraco.Delivery.Json
{
    public static class ContentNodeExtensions
    {
        public static IEnumerable<JsonNode> Blocks(this JsonNode node)
        {
            if (node != null)
            {
                foreach (var propName in node.AllPropNames())
                {
                    var dataNode = node.Node(propName);
                    if (dataNode.IsBlock())
                    {
                        foreach (var child in dataNode.Blocks())
                            yield return child;

                        yield return dataNode;
                    }
                    else
                    {
                        if (node.PropIs<IEnumerable<JsonNode>>(propName))
                        {
                            foreach (var block in node.Nodes(propName).Where(x => x.IsBlock()))
                            {
                                foreach (var child in block.Blocks())
                                    yield return child;

                                yield return block;
                            }
                        }
                    }
                }
            }
        }

        public static bool IsNodeProp(this JsonNode node, string prop)
        {
            return Node(node, prop) != null;
        }

        public static bool IsBlock(this JsonNode node)
        {
            return node != null
                && node.Id != Guid.Empty
                && !string.IsNullOrEmpty(node.Type);
        }

        public static T ValOrDefault<T>(this JsonNode node, string prop, T def)
        {
            object res = node.Val(prop);
            if (res != null && res is T)
                return (T)res;

            return def;
        }

        public static T ValOrAltVal<T>(this JsonNode node, string prop, string altProp)
        {
            object res = node.Val(prop);
            if (res == null || !(res is T))
                return node.Val<T>(altProp);

            if (res is string && string.IsNullOrEmpty(res as string))
                return node.Val<T>(altProp);

            return node.Val<T>(altProp);
        }

        public static object Val(this JsonNode node, string prop)
        {
            var parser = JsonNodePropertyParser.Create(node, prop);
            if (!parser.IsValidForGet)
                return null;

            node = NodeFromPath(node, parser.SourcePath);
            return node?[parser.Source];
        }

        public static T Val<T>(this JsonNode node, string prop)
        {
            var val = Val(node, prop);

            if (typeof(T) == typeof(string))
                return (T)(val?.ToString() as object);

            if (val == null)
                return default;

            if (val is T)
                return (T)val;

            if (IsStringList<T>())
            {
                List<string> res = null;
                if (IsStringList(val))
                {
                    res = ((IEnumerable<string>)val).ToList();
                }
                else
                {
                    var strVal = val?.ToString() ?? string.Empty;
                    res = new List<string> { strVal };
                }
                if (typeof(T) == typeof(string[]))
                    return (T)(IEnumerable<string>)res.ToArray();

                return (T)(IEnumerable<string>)res;
            }

            return default;
        }

        public static bool PropIs<T>(this JsonNode node, string prop)
        {
            var val = node.Val(prop);

            return val == null
                ? false
                : val is T;
        }

        public static bool PropExists(this JsonNode node, string prop)
        {
            var parser = JsonNodePropertyParser.Create(node, prop);
            if (!parser.IsValidForGet) 
                return false;

            node = NodeFromPath(node, parser.SourcePath);
            return node?.HasProp(parser.Source) ?? false;
        }

        public static JsonNode AddNode(this JsonNode node, string prop)
        {
            var parser = JsonNodePropertyParser.Create(node, prop, null);
            if (!parser.IsValidForUpdate)
                return null;

            var currNode = AddNodeFromPath(node, parser.TargetPath);
            var newNode = new JsonNode();
            currNode[parser.Target] = newNode;

            return newNode;
        }

        private static JsonNode AddNodeFromPath(this JsonNode node, string[] path)
        {
            if (node == null) 
                return null;

            var currNode = node;
            foreach (var seg in path)
            {
                var nextNode = currNode.Node(seg);
                currNode = nextNode == null
                    ? (JsonNode)(currNode[seg] = new JsonNode())
                    : nextNode;
            }

            return currNode;
        }

        public static JsonNode Node(this JsonNode node, string prop)
        {
            var parser = JsonNodePropertyParser.Create(node, prop);
            if (!parser.IsValidForGet)
                return null;

            node = NodeFromPath(node, parser.SourcePath);

            return node.Val<JsonNode>(parser.Source);
        }

        private static JsonNode NodeFromPath(this JsonNode node, string[] path)
        {
            foreach (var seg in path)
                node = node.Node(seg);

            return node;
        }

        public static IEnumerable<JsonNode> Nodes(this JsonNode node, string prop)
        {
            return ValOrDefault(node, prop, Enumerable.Empty<JsonNode>());
        }

        public static JsonNode AddProp(this JsonNode node, string prop, object value)
        {
            var parser = JsonNodePropertyParser.Create(node, prop, null);
            if (parser.IsValidForUpdate)
            {
                var currNode = AddNodeFromPath(node, parser.TargetPath);
                currNode[parser.Target] = value;
            }

            return node;
        }

        public static JsonNode AddProp<T>(this JsonNode node, string prop, Func<JsonNode, T> propValFunc)
        {
            if (node != null && propValFunc != null)
            {
                var val = propValFunc(node);
                AddProp(node, prop, val);
            }

            return node;
        }

        public static JsonNode AddPropIfNotExists(this JsonNode node, string prop, object value)
        {
            if (node == null || PropExists(node, prop))
                return node;

            return AddProp(node, prop, value);
        }

        public static JsonNode AddPropIfNotNull(this JsonNode node, string prop, object value)
        {
            return value == null
                ? node
                : AddProp(node, prop, value);
        }

        public static JsonNode RemoveProp(this JsonNode node, string prop)
        {
            var parser = JsonNodePropertyParser.Create(node, prop, null);
            if (parser.IsValidForUpdate)
                NodeFromPath(node, parser.TargetPath)?.Remove(parser.Target);

            return node;
        }

        public static JsonNode RemoveProps(this JsonNode node, params string[] parms)
        {
            if (parms != null)
                Array.ForEach(parms, prop => RemoveProp(node, prop));

            return node;
        }

        public static JsonNode KeepProps(this JsonNode node, params string[] parms)
        {
            if (node != null)
            {
                var removeProps = node.AllPropNames()
                    .Except(parms)
                    .ToArray();

                return node.RemoveProps(removeProps);
            }

            return node;
        }

        public static JsonNode CopyProp(this JsonNode node, string targetProp, JsonNode source, string sourceProp)
        {
            var sourceParser = JsonNodePropertyParser.Create(source, targetProp, sourceProp);
            if (sourceParser.IsValidForCopy)
            {
                source = NodeFromPath(source, sourceParser.SourcePath);
                if (source?.HasProp(sourceParser.Source) ?? false)
                {
                    var val = source[sourceParser.Source];
                    var currNode = AddNodeFromPath(node, sourceParser.TargetPath);
                    currNode[sourceParser.Target] = val;
                }
            }

            return node;
        }

        public static JsonNode CopyProp(this JsonNode node, JsonNode source, string sourceProp)
        {
            return CopyProp(node, null, source, sourceProp);
        }

        public static JsonNode CopyProps(this JsonNode node, JsonNode source, params string[] parms)
        {
            if (node == null || source == null)
                return node;

            parms = parms?.Any() == true
                ? parms
                : source.AllPropNames();

            foreach (var parm in parms)
                node.CopyProp(source, parm);

            return node;
        }

        public static JsonNode CopyAllProps(this JsonNode node, JsonNode source)
        {
            return CopyProps(node, source);
        }

        public static JsonNode CopyAllExceptProps(this JsonNode node, JsonNode source, params string[] parms)
        {
            if (node == null || source == null)
                return node;

            if (parms == null || !parms.Any())
                return CopyAllProps(node, source);

            var props = source
                .AllPropNames()
                .Where(x => !parms.Contains(x))
                .ToArray();

            return CopyProps(node, source, props);
        }

        public static JsonNode Clone(this JsonNode source, string[] include = null, string[] exclude = null)
        {
            if (source == null)
                return null;

            var res = new JsonNode()
                .CopyProps(source, include);

            res = new JsonNode()
                .CopyAllExceptProps(res, exclude);

            return res;
        }

        public static JsonNode RenameProp(this JsonNode node, string prop, string newProp)
        {
            var parser = JsonNodePropertyParser.Create(node, newProp, prop);
            if (parser.IsValidForCopy)
            {
                var currNode = NodeFromPath(node, parser.SourcePath);
                if (currNode?.HasProp(parser.Source) ?? false)
                {
                    var val = currNode[parser.Source];
                    currNode.Remove(parser.Source);

                    currNode = AddNodeFromPath(node, parser.TargetPath);
                    currNode[parser.Target] = val;
                }
            }

            return node;
        }

        private static bool IsStringList<T>()
        {
            return typeof(T) == typeof(IEnumerable<string>)
                || typeof(T) == typeof(List<string>)
                || typeof(T) == typeof(string[]);
        }

        private static bool IsStringList(object obj)
        {
            return obj.GetType() == typeof(IEnumerable<string>)
                || obj.GetType() == typeof(List<string>)
                || obj.GetType() == typeof(string[]);
        }
    }
}
