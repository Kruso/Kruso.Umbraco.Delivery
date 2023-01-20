using Kruso.Umbraco.Delivery.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using Umbraco.Cms.Core.PropertyEditors;

namespace Kruso.Umbraco.Delivery.ModelConversion
{
    public abstract class Enricher
    {
        private readonly string _documentType;
        private readonly TemplateType _templateType;

        internal class Op
        {
            internal string OpType { get; set; }
            internal string Prop { get; set; }
            internal Func<JsonNode, object> ValueFunc { get; set; }
            internal object GetValue(JsonNode node)
            {
                return ValueFunc?.Invoke(node);
            }
            internal Type DotNetType { get; set; }
        }

        internal class BlockOp : Op
        {
            internal Func<JsonNode, JsonNode, object> BlockFunc { get; set; }
            internal BlockOp(Guid id, string type)
            {
                ValueFunc = (node) =>
                {
                    if (BlockFunc != null)
                    {
                        var block = new JsonNode
                        {
                            Id = id,
                            Type = type
                        };
                        return BlockFunc(node, block);
                    }

                    return null;
                };
            }
        }

        private List<Op> _ops = new List<Op>();

        public Enricher Add(string prop, string type, Guid id)
        {
            _ops.Add(new BlockOp(id, type)
            {
                OpType = "add",
                Prop = prop,
                DotNetType = typeof(JsonNode)
            });

            return this;
        }

        public Enricher Add(string prop, string type, Func<IEnumerable<Guid>> idGenerationFunc)
        {
            //var id = idGenerationFunc?.Invoke();
            //while (id != null)
            //{
            //    Add(prop, type, id);
            //    id = idGenerationFunc();
            //}

            return this;
        }

        public Enricher Add(string prop, Type valueType, Func<JsonNode, object> valueFunc)
        {
            //var parts = propName.Split('.');

            //if (parts.Length > 2)
            //    return this;

            //var documentType = parts.Length == 2
            //    ? parts[0]
            //    : "*";

            //var propertyName = parts.Length == 2
            //    ? parts[1]
            //    : parts[0];

            _ops.Add(new Op
            {
                OpType = "add",
                Prop = prop,
                DotNetType = valueType,
                ValueFunc = valueFunc
            });

            return this;
        }

        public Enricher Remove(params string[] props)
        {
            foreach (var prop in props)
            {
                _ops.Add(new Op
                {
                    OpType = "remove",
                    Prop = prop
                });
            }

            return this;
        }

        public abstract void Enrich();

        internal List<Op> GetOps()
        {
            Enrich();
            return _ops;
        }
    }
}
