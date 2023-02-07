using System;
using System.Collections.Generic;
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
            return node?.Node(prop) != null || node?.Nodes(prop)?.Any() == true;
        }

        public static bool IsBlock(this JsonNode node)
        {
            return node != null
                && node.Id != Guid.Empty
                && !string.IsNullOrEmpty(node.Type);
        }

        public static bool IsNestedContentProp(this JsonNode node)
        {
            var val = node != null
                ? node.Val<string>("fieldType")
                : null;

            return !string.IsNullOrEmpty(val) && (val == "Umbraco.NestedContent" || val == "Umbraco.MultiNodeTreePicker" || val == "Umbraco.ContentPicker");
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
            return node?.Val<object>(prop);
        }

        public static T Val<T>(this JsonNode node, string prop)
        {
            var propVal = node != null
                ? node[prop]
                : null;

            if (propVal == null)
                return default;

            if (propVal is T)
            {
                if (propVal is JsonNode && ((JsonNode)propVal).Val<string>("name") == prop)
                {
                    return ((JsonNode)propVal).Val<T>("value");
                }

                return (T)propVal;
            }

            if (IsStringList<T>())
            {
                List<string> res = null;
                if (IsStringList(propVal))
                {
                    res = ((IEnumerable<string>)propVal).ToList();
                }
                else
                {
                    var strVal = propVal?.ToString() ?? string.Empty;
                    res = new List<string> { strVal };
                }
                if (typeof(T) == typeof(string[]))
                    return (T)(IEnumerable<string>)res.ToArray();

                return (T)(IEnumerable<string>)res;
            }

            if (typeof(T) == typeof(string))
                return (T)((propVal?.ToString() ?? string.Empty) as object);

            return default;
        }

        public static bool PropIs<T>(this JsonNode node, string prop)
        {
            var propVal = node != null
                ? node[prop]
                : null;

            return propVal == null
                ? false
                : propVal is T;
        }

        public static JsonNode AddNode(this JsonNode node, string prop)
        {
            node.AddProp(prop, new JsonNode());
            return node.Node(prop);
        }

        public static JsonNode Node(this JsonNode node, string prop)
        {
            return node?.Val<JsonNode>(prop);
        }

        public static IEnumerable<JsonNode> Nodes(this JsonNode node, string prop)
        {
            return node?.Val<IEnumerable<JsonNode>>(prop) ?? Enumerable.Empty<JsonNode>();
        }

        public static JsonNode AddProp(this JsonNode node, string prop, object value)
        {
            if (node == null)
                return null;

            node[prop] = value;

            return node;
        }

        public static JsonNode AddProp<T>(this JsonNode node, string prop, Func<JsonNode, T> propValFunc)
        {
            if (node == null)
                return null;

            if (propValFunc != null)
            {
                node[prop] = propValFunc(node);
            }


            return node;
        }
        public static JsonNode AddPropIfNotExists(this JsonNode node, string prop, object value)
        {
            if (node == null)
                return null;

            if (!node.HasProp(prop))
            {
                node[prop] = value;
            }

            return node;
        }

        public static JsonNode AddPropIfNotNull(this JsonNode node, string prop, object value)
        {
            return value == null
                ? node
                : node.AddProp(prop, value);
        }

        public static JsonNode RemoveProp(this JsonNode node, string prop)
        {
            node?.Remove(prop);
            return node;
        }

        public static JsonNode RemoveProps(this JsonNode node, params string[] parms)
        {
            if (node != null)
            {
                foreach (var parm in parms)
                {
                    node.Remove(parm);
                }
            }

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

        public static JsonNode CopyProp(this JsonNode node, JsonNode source, string sourceProp)
        {
            if (node != null && source != null && source.HasProp(sourceProp))
            {
                var value = source.Val(sourceProp);
                node.AddProp(sourceProp, value);
            }

            return node;
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

        public static JsonNode CopyProp(this JsonNode node, string targetProp, JsonNode source, string sourceProp)
        {
            if (node != null && source != null && !string.IsNullOrEmpty(targetProp) && source.HasProp(sourceProp))
            {
                var value = source.Val(sourceProp);
                node.AddProp(targetProp, value);
            }

            return node;
        }

        public static JsonNode CopyProps(this JsonNode node, JsonNode source, params string[] parms)
        {
            if (node != null)
            {
                if (parms == null || !parms.Any())
                    return node.CopyAllProps(source);

                foreach (var parm in parms)
                {
                    node.CopyProp(parm, source, parm);
                }
            }

            return node;
        }

        public static JsonNode CopyAllProps(this JsonNode node, JsonNode source)
        {
            if (node != null && source != null)
            {
                foreach (var prop in source.AllPropNames())
                {
                    node.CopyProp(prop, source, prop);
                }
            }

            return node;
        }

        public static JsonNode CopyAllExceptProps(this JsonNode node, JsonNode source, params string[] parms)
        {
            if (node != null && source != null)
            {
                if (parms == null || parms.Length == 0)
                    return node.CopyAllProps(source);

                foreach (var prop in source.AllPropNames().Where(x => parms == null || !parms.Contains(x)))
                {
                    node.CopyProp(prop, source, prop);
                }
            }

            return node;
        }

        public static JsonNode RenameProp(this JsonNode node, string prop, string newProp)
        {
            if (node != null && node.HasProp(prop))
            {
                var val = node[prop];
                node.Remove(prop);
                node.AddProp(newProp, val);
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
