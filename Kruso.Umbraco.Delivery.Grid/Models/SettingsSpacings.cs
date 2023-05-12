using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public class SettingsSpacings : Settings
    {
        private string? _margins;
        public string? Margins
        {
            get { return _margins; }
            set { _margins = StylesConstants.Validate(value); }
        }

        private string? _marginTop;
        public string? MarginTop
        {
            get { return _marginTop; }
            set { _marginTop = StylesConstants.Validate(value); }
        }

        private string? _marginBottom;
        public string? MarginBottom
        {
            get { return _marginBottom; }
            set { _marginBottom = StylesConstants.Validate(value); }
        }

        private string? _marginLeft;
        public string? MarginLeft
        {
            get { return _marginLeft; }
            set { _marginLeft = StylesConstants.Validate(value); }
        }

        private string? _marginRight;
        public string? MarginRight
        {
            get { return _marginRight; }
            set { _marginRight = StylesConstants.Validate(value); }
        }


        private string? _paddings;
        public string? Paddings
        {
            get { return _paddings; }
            set { _paddings = StylesConstants.Validate(value); }
        }

        private string? _paddingTop;
        public string? PaddingTop
        {
            get { return _paddingTop; }
            set { _paddingTop = StylesConstants.Validate(value); }
        }

        private string? _paddingBottom;
        public string? PaddingBottom
        {
            get { return _paddingBottom; }
            set { _paddingBottom = StylesConstants.Validate(value); }
        }

        private string? _paddingLeft;
        public string? PaddingLeft
        {
            get { return _paddingLeft; }
            set { _paddingLeft = StylesConstants.Validate(value); }
        }

        private string? _paddingRight;
        public string? PaddingRight
        {
            get { return _paddingRight; }
            set { _paddingRight = StylesConstants.Validate(value); }
        }

        public override void SetProp(string propName, string? propVal)
        {
            var parts = propName.Split('_');
            if (parts.Length > 1)
            {
                var name = string.Join('_', parts.Skip(1));
                switch (name)
                {
                    case "margins":
                        Margins = propVal;
                        break;
                    case "margin_top":
                        MarginTop = propVal;
                        break;
                    case "margin_bottom":
                        MarginBottom = propVal;
                        break;
                    case "margin_left":
                        MarginLeft = propVal;
                        break;
                    case "margin_right":
                        MarginRight = propVal;
                        break;
                    case "paddings":
                        Paddings = propVal;
                        break;
                    case "padding_top":
                        PaddingTop = propVal;
                        break;
                    case "padding_bottom":
                        PaddingBottom = propVal;
                        break;
                    case "padding_left":
                        PaddingLeft = propVal;
                        break;
                    case "padding_right":
                        PaddingRight = propVal;
                        break;
                }
            }
        }

        public override void AddProps(JObject json, string prefix)
        {
            AddPropIfNotNull(json, $"{prefix}_margins", Margins);
            AddPropIfNotNull(json, $"{prefix}_margin_top", MarginTop);
            AddPropIfNotNull(json, $"{prefix}_margin_bottom", MarginBottom);
            AddPropIfNotNull(json, $"{prefix}_margin_left", MarginLeft);
            AddPropIfNotNull(json, $"{prefix}_margin_right", MarginRight);

            AddPropIfNotNull(json, $"{prefix}_paddings", Paddings);
            AddPropIfNotNull(json, $"{prefix}_padding_top", PaddingTop);
            AddPropIfNotNull(json, $"{prefix}_padding_bottom", PaddingBottom);
            AddPropIfNotNull(json, $"{prefix}_padding_left", PaddingLeft);
            AddPropIfNotNull(json, $"{prefix}_padding_right", PaddingRight);
        }
    }
}
