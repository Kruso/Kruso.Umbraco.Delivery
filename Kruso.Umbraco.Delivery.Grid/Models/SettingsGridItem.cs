using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public class SettingsGridItem : Settings
    {
        private string? _columnStart;
        public string? ColumnStart
        {
            get { return _columnStart; }
            set { _columnStart = StylesConstants.Validate(value, StylesConstants.ColStarts); }
        }

        private string? _columnSpan;
        public string? ColumnSpan
        {
            get { return _columnSpan; }
            set { _columnSpan = StylesConstants.Validate(value, StylesConstants.ColSpans); }
        }

        private string? _rowStart;
        public string? RowStart
        {
            get { return _rowStart; }
            set { _rowStart = StylesConstants.Validate(value, StylesConstants.RowStarts); }
        }

        private string? _rowSpan;
        public string? RowSpan
        {
            get { return _rowSpan; }
            set { _rowSpan = StylesConstants.Validate(value, StylesConstants.RowSpans); }
        }

        private string? _alignSelf;
        public string? AlignSelf
        {
            get { return _alignSelf; }
            set { _alignSelf = StylesConstants.Validate(value, StylesConstants.Aligns); }
        }

        private string? _justifySelf;
        public string? JustifySelf
        {
            get { return _justifySelf; }
            set { _justifySelf = StylesConstants.Validate(value, StylesConstants.Aligns); }
        }

        public override void SetProp(string propName, string? propVal)
        {
            var parts = propName.Split('_');
            if (parts.Length > 1)
            {
                var name = string.Join('_', parts.Skip(1));
                switch (name)
                {
                    case "column_span":
                        ColumnSpan = propVal;
                        break;
                    case "row_span":
                        RowSpan = propVal;
                        break;
                    case "column_start":
                        ColumnStart = propVal;
                        break;
                    case "row_start":
                        RowStart = propVal;
                        break;
                    case "align_self":
                        AlignSelf = propVal;
                        break;
                    case "justify_self":
                        JustifySelf = propVal;
                        break;
                }
            }
        }

        public override void AddProps(JObject json, string prefix)
        {
            AddPropIfNotNull(json, $"{prefix}_column_span", ColumnSpan);
            AddPropIfNotNull(json, $"{prefix}_row_span", RowSpan);
            AddPropIfNotNull(json, $"{prefix}_column_start", ColumnStart);
            AddPropIfNotNull(json, $"{prefix}_row_start", RowStart);
            AddPropIfNotNull(json, $"{prefix}_align_self", AlignSelf);
            AddPropIfNotNull(json, $"{prefix}_justify_self", JustifySelf);
        }
    }
}
