using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public class StylesGrid
    {
        private readonly string[] TemplateCols = new[]
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"
        };

        private readonly string[] TemplateRows = new[]
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"
        };

        private readonly string[] Gaps = new[]
        { 
            "x-small",
            "small",
            "medium",
            "large",
            "x-large"
        };

        private readonly string[] Aligns = new[]
{
            "start",
            "center",
            "end",
            "stretch"
        };

        [JsonProperty("display_name")]
        public string? Name { get; set; }

        private string _smallTemplateColumns = "12";
        [JsonProperty("small_template_columns")]
        public string SmallTemplateColumns
        {
            get { return _smallTemplateColumns; }
            set { _smallTemplateColumns = Validate(value, TemplateCols); }
        }

        private string _smallTemplateRows = "6";
        [JsonProperty("small_template_rows")]
        public string SmallTemplateRows
        {
            get { return _smallTemplateRows; }
            set { _smallTemplateRows = Validate(value, TemplateRows); }
        }

        private string _smallColumnGap = "medium";
        [JsonProperty("small_column_gap")]
        public string SmallColumnGap
        {
            get { return _smallColumnGap; }
            set { _smallColumnGap = Validate(value, Gaps); }
        }

        private string _smallRowGap = "medium";
        [JsonProperty("small_row_gap")]
        public string SmallRowGap
        {
            get { return _smallRowGap; }
            set { _smallRowGap = Validate(value, Gaps); }
        }

        private string _smallAlignItems = "start";
        [JsonProperty("small_align_items")]
        public string SmallAlignItems
        {
            get { return _smallAlignItems; }
            set { _smallAlignItems = Validate(value, Aligns); }
        }

        private string _smallJustifyItems = "start";
        [JsonProperty("small_justify_items")]
        public string SmallJustifyItems
        {
            get { return _smallJustifyItems; }
            set { _smallJustifyItems = Validate(value, Aligns); }
        }


        private string _mediumTemplateColumns = "12";
        [JsonProperty("medium_template_columns")]
        public string MediumTemplateColumns
        {
            get { return _mediumTemplateColumns; }
            set { _mediumTemplateColumns = Validate(value, TemplateCols); }
        }

        private string _mediumTemplateRows = "6";
        [JsonProperty("medium_template_rows")]
        public string MediumTemplateRows
        {
            get { return _mediumTemplateRows; }
            set { _mediumTemplateRows = Validate(value, TemplateRows); }
        }

        private string _mediumColumnGap = "medium";
        [JsonProperty("medium_column_gap")]
        public string MediumColumnGap
        {
            get { return _mediumColumnGap; }
            set { _mediumColumnGap = Validate(value, Gaps); }
        }

        private string _mediumRowGap = "medium";
        [JsonProperty("medium_row_gap")]
        public string MediumRowGap
        {
            get { return _mediumRowGap; }
            set { _mediumRowGap = Validate(value, Gaps); }
        }

        private string _mediumAlignItems = "start";
        [JsonProperty("medium_align_items")]
        public string MediumAlignItems
        {
            get { return _mediumAlignItems; }
            set { _mediumAlignItems = Validate(value, Aligns); }
        }

        private string _mediumJustifyItems = "start";
        [JsonProperty("medium_justify_items")]
        public string MediumJustifyItems
        {
            get { return _mediumJustifyItems; }
            set { _mediumJustifyItems = Validate(value, Aligns); }
        }


        private string _largeTemplateColumns = "12";
        [JsonProperty("large_template_columns")]
        public string LargeTemplateColumns
        {
            get { return _largeTemplateColumns; }
            set { _largeTemplateColumns = Validate(value, TemplateCols); }
        }

        private string _largeTemplateRows = "6";
        [JsonProperty("large_template_rows")]
        public string LargeTemplateRows
        {
            get { return _largeTemplateRows; }
            set { _largeTemplateRows = Validate(value, TemplateRows); }
        }

        private string _largeColumnGap = "medium";
        [JsonProperty("large_column_gap")]
        public string LargeColumnGap
        {
            get { return _largeColumnGap; }
            set { _largeColumnGap = Validate(value, Gaps); }
        }

        private string _largeRowGap = "medium";
        [JsonProperty("large_row_gap")]
        public string LargeRowGap
        {
            get { return _largeRowGap; }
            set { _largeRowGap = Validate(value, Gaps); }
        }

        private string _largeAlignItems = "start";
        [JsonProperty("large_align_items")]
        public string LargeAlignItems
        {
            get { return _largeAlignItems; }
            set { _largeAlignItems = Validate(value, Aligns); }
        }

        private string _largeJustifyItems = "start";
        [JsonProperty("large_justify_items")]
        public string LargeJustifyItems
        {
            get { return _largeJustifyItems; }
            set { _largeJustifyItems = Validate(value, Aligns); }
        }


        private string _extraLargeTemplateColumns = "12";
        [JsonProperty("extra_large_template_columns")]
        public string ExtraLargeTemplateColumns
        {
            get { return _extraLargeTemplateColumns; }
            set { _extraLargeTemplateColumns = Validate(value, TemplateCols); }
        }

        private string _extraLargeTemplateRows = "6";
        [JsonProperty("extra_large_template_rows")]
        public string ExtraLargeTemplateRows
        {
            get { return _extraLargeTemplateRows; }
            set { _extraLargeTemplateRows = Validate(value, TemplateRows); }
        }

        private string _extraLargeColumnGap = "medium";
        [JsonProperty("extra_large_column_gap")]
        public string ExtraLargeColumnGap
        {
            get { return _extraLargeColumnGap; }
            set { _extraLargeColumnGap = Validate(value, Gaps); }
        }

        private string _extraLargeRowGap = "medium";
        [JsonProperty("extra_large_row_gap")]
        public string ExtraLargeRowGap
        {
            get { return _extraLargeRowGap; }
            set { _extraLargeRowGap = Validate(value, Gaps); }
        }

        private string _extraLargeAlignItems = "start";
        [JsonProperty("extra_large_align_items")]
        public string ExtraLargeAlignItems
        {
            get { return _extraLargeAlignItems; }
            set { _extraLargeAlignItems = Validate(value, Aligns); }
        }

        private string _extraLargeJustifyItems = "start";
        [JsonProperty("extra_large_justify_items")]
        public string ExtraLargeJustifyItems
        {
            get { return _extraLargeJustifyItems; }
            set { _extraLargeJustifyItems = Validate(value, Aligns); }
        }


        private string Validate(string val, string[] allowedValues)
        {
            if (allowedValues.Contains(val))
                return val;

            throw new JsonException("Invalid value " + val);
        }
    }
}
