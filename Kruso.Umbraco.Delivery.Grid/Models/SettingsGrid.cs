using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPoco.fastJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public class SettingsGrid : Settings
    {
        private string? _templateColumns;
        public string? TemplateColumns
        {
            get { return _templateColumns; }
            set { _templateColumns = StylesConstants.Validate(value, StylesConstants.Columns); }
        }

        private string? _templateRows;
        public string? TemplateRows
        {
            get { return _templateRows; }
            set { _templateRows = StylesConstants.Validate(value, StylesConstants.Rows); }
        }

        private string? _columnGap;
        public string? ColumnGap
        {
            get { return _columnGap; }
            set { _columnGap = StylesConstants.Validate(value, StylesConstants.Gaps); }
        }

        private string? _rowGap;
        public string? RowGap
        {
            get { return _rowGap; }
            set { _rowGap = StylesConstants.Validate(value, StylesConstants.Gaps); }
        }

        private string? _alignItems;
        public string? AlignItems
        {
            get { return _alignItems; }
            set { _alignItems = StylesConstants.Validate(value, StylesConstants.Aligns); }
        }

        private string? _justifyItems;
        public string? JustifyItems
        {
            get { return _justifyItems; }
            set { _justifyItems = StylesConstants.Validate(value, StylesConstants.Aligns); }
        }

        public override void SetProp(string propName, string? propVal)
        {
            var parts = propName.Split('_');
            if (parts.Length > 1)
            {
                var name = string.Join('_', parts.Skip(1));
                switch (name)
                {
                    case "template_columns":
                        TemplateColumns = propVal;
                        break;
                    case "template_rows":
                        TemplateRows = propVal;
                        break;
                    case "column_gap":
                        ColumnGap = propVal;
                        break;
                    case "row_gap":
                        RowGap = propVal;
                        break;
                    case "align_items":
                        AlignItems = propVal;
                        break;
                    case "justify_items":
                        JustifyItems = propVal;
                        break;
                }
            }
        }

        public override void AddProps(JObject json, string prefix)
        {
            AddPropIfNotNull(json, $"{prefix}_template_columns", TemplateColumns);
            AddPropIfNotNull(json, $"{prefix}_template_rows", TemplateRows);
            AddPropIfNotNull(json, $"{prefix}_column_gap", ColumnGap);
            AddPropIfNotNull(json, $"{prefix}_row_gap", RowGap);
            AddPropIfNotNull(json, $"{prefix}_align_items", AlignItems);
            AddPropIfNotNull(json, $"{prefix}_justify_items", JustifyItems);
        }
    }
}
