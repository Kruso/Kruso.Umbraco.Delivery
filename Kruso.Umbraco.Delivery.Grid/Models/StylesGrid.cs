using Newtonsoft.Json;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public class StylesGrid
    {
        [JsonProperty("display_name")]
        public string? Name { get; set; }

        private string _smallTemplateColumns = "12";
        [JsonProperty("small_template_columns")]
        public string SmallTemplateColumns
        {
            get { return _smallTemplateColumns; }
            set { _smallTemplateColumns = Styles.Validate(value, Styles.Columns); }
        }

        private string _smallTemplateRows = "6";
        [JsonProperty("small_template_rows")]
        public string SmallTemplateRows
        {
            get { return _smallTemplateRows; }
            set { _smallTemplateRows = Styles.Validate(value, Styles.Columns); }
        }

        private string _smallColumnGap = "medium";
        [JsonProperty("small_column_gap")]
        public string SmallColumnGap
        {
            get { return _smallColumnGap; }
            set { _smallColumnGap = Styles.Validate(value, Styles.Gaps); }
        }

        private string _smallRowGap = "medium";
        [JsonProperty("small_row_gap")]
        public string SmallRowGap
        {
            get { return _smallRowGap; }
            set { _smallRowGap = Styles.Validate(value, Styles.Gaps); }
        }

        private string _smallAlignItems = "start";
        [JsonProperty("small_align_items")]
        public string SmallAlignItems
        {
            get { return _smallAlignItems; }
            set { _smallAlignItems = Styles.Validate(value, Styles.Aligns); }
        }

        private string _smallJustifyItems = "start";
        [JsonProperty("small_justify_items")]
        public string SmallJustifyItems
        {
            get { return _smallJustifyItems; }
            set { _smallJustifyItems = Styles.Validate(value, Styles.Aligns); }
        }


        private string _mediumTemplateColumns = "12";
        [JsonProperty("medium_template_columns")]
        public string MediumTemplateColumns
        {
            get { return _mediumTemplateColumns; }
            set { _mediumTemplateColumns = Styles.Validate(value, Styles.Columns); }
        }

        private string _mediumTemplateRows = "6";
        [JsonProperty("medium_template_rows")]
        public string MediumTemplateRows
        {
            get { return _mediumTemplateRows; }
            set { _mediumTemplateRows = Styles.Validate(value, Styles.Columns); }
        }

        private string _mediumColumnGap = "medium";
        [JsonProperty("medium_column_gap")]
        public string MediumColumnGap
        {
            get { return _mediumColumnGap; }
            set { _mediumColumnGap = Styles.Validate(value, Styles.Gaps); }
        }

        private string _mediumRowGap = "medium";
        [JsonProperty("medium_row_gap")]
        public string MediumRowGap
        {
            get { return _mediumRowGap; }
            set { _mediumRowGap = Styles.Validate(value, Styles.Gaps); }
        }

        private string _mediumAlignItems = "start";
        [JsonProperty("medium_align_items")]
        public string MediumAlignItems
        {
            get { return _mediumAlignItems; }
            set { _mediumAlignItems = Styles.Validate(value, Styles.Aligns); }
        }

        private string _mediumJustifyItems = "start";
        [JsonProperty("medium_justify_items")]
        public string MediumJustifyItems
        {
            get { return _mediumJustifyItems; }
            set { _mediumJustifyItems = Styles.Validate(value, Styles.Aligns); }
        }


        private string _largeTemplateColumns = "12";
        [JsonProperty("large_template_columns")]
        public string LargeTemplateColumns
        {
            get { return _largeTemplateColumns; }
            set { _largeTemplateColumns = Styles.Validate(value, Styles.Columns); }
        }

        private string _largeTemplateRows = "6";
        [JsonProperty("large_template_rows")]
        public string LargeTemplateRows
        {
            get { return _largeTemplateRows; }
            set { _largeTemplateRows = Styles.Validate(value, Styles.Columns); }
        }

        private string _largeColumnGap = "medium";
        [JsonProperty("large_column_gap")]
        public string LargeColumnGap
        {
            get { return _largeColumnGap; }
            set { _largeColumnGap = Styles.Validate(value, Styles.Gaps); }
        }

        private string _largeRowGap = "medium";
        [JsonProperty("large_row_gap")]
        public string LargeRowGap
        {
            get { return _largeRowGap; }
            set { _largeRowGap = Styles.Validate(value, Styles.Gaps); }
        }

        private string _largeAlignItems = "start";
        [JsonProperty("large_align_items")]
        public string LargeAlignItems
        {
            get { return _largeAlignItems; }
            set { _largeAlignItems = Styles.Validate(value, Styles.Aligns); }
        }

        private string _largeJustifyItems = "start";
        [JsonProperty("large_justify_items")]
        public string LargeJustifyItems
        {
            get { return _largeJustifyItems; }
            set { _largeJustifyItems = Styles.Validate(value, Styles.Aligns); }
        }


        private string _extraLargeTemplateColumns = "12";
        [JsonProperty("extra_large_template_columns")]
        public string ExtraLargeTemplateColumns
        {
            get { return _extraLargeTemplateColumns; }
            set { _extraLargeTemplateColumns = Styles.Validate(value, Styles.Columns); }
        }

        private string _extraLargeTemplateRows = "6";
        [JsonProperty("extra_large_template_rows")]
        public string ExtraLargeTemplateRows
        {
            get { return _extraLargeTemplateRows; }
            set { _extraLargeTemplateRows = Styles.Validate(value, Styles.Columns); }
        }

        private string _extraLargeColumnGap = "medium";
        [JsonProperty("extra_large_column_gap")]
        public string ExtraLargeColumnGap
        {
            get { return _extraLargeColumnGap; }
            set { _extraLargeColumnGap = Styles.Validate(value, Styles.Gaps); }
        }

        private string _extraLargeRowGap = "medium";
        [JsonProperty("extra_large_row_gap")]
        public string ExtraLargeRowGap
        {
            get { return _extraLargeRowGap; }
            set { _extraLargeRowGap = Styles.Validate(value, Styles.Gaps); }
        }

        private string _extraLargeAlignItems = "start";
        [JsonProperty("extra_large_align_items")]
        public string ExtraLargeAlignItems
        {
            get { return _extraLargeAlignItems; }
            set { _extraLargeAlignItems = Styles.Validate(value, Styles.Aligns); }
        }

        private string _extraLargeJustifyItems = "start";
        [JsonProperty("extra_large_justify_items")]
        public string ExtraLargeJustifyItems
        {
            get { return _extraLargeJustifyItems; }
            set { _extraLargeJustifyItems = Styles.Validate(value, Styles.Aligns); }
        }

        public StylesGrid SetColumns(Styles.Breakpoint breakpoint, int colSpan) => SetColumns(breakpoint, colSpan.ToString());
        public StylesGrid SetColumns(Styles.Breakpoint breakpoint, string colSpan)
        {
            switch (breakpoint)
            {
                case Styles.Breakpoint.Small:
                    SmallTemplateColumns = colSpan;
                    MediumTemplateColumns = colSpan;
                    LargeTemplateColumns = colSpan;
                    ExtraLargeTemplateColumns = colSpan;
                    break;
                case Styles.Breakpoint.Medium:
                    MediumTemplateColumns = colSpan;
                    LargeTemplateColumns = colSpan;
                    ExtraLargeTemplateColumns = colSpan;
                    break;
                case Styles.Breakpoint.Large:
                    LargeTemplateColumns = colSpan;
                    ExtraLargeTemplateColumns = colSpan;
                    break;
                case Styles.Breakpoint.ExtraLarge:
                    ExtraLargeTemplateColumns = colSpan;
                    break;
                default: break;
            }

            return this;
        }

        public StylesGrid SetRows(Styles.Breakpoint breakpoint, int rowSpan) => SetRows(breakpoint, rowSpan.ToString());
        public StylesGrid SetRows(Styles.Breakpoint breakpoint, string rowSpan)
        {
            switch (breakpoint)
            {
                case Styles.Breakpoint.Small:
                    SmallTemplateRows = rowSpan;
                    MediumTemplateRows = rowSpan;
                    LargeTemplateRows = rowSpan;
                    ExtraLargeTemplateRows = rowSpan;
                    break;
                case Styles.Breakpoint.Medium:
                    MediumTemplateRows = rowSpan;
                    LargeTemplateRows = rowSpan;
                    ExtraLargeTemplateRows = rowSpan;
                    break;
                case Styles.Breakpoint.Large:
                    LargeTemplateRows = rowSpan;
                    ExtraLargeTemplateRows = rowSpan;
                    break;
                case Styles.Breakpoint.ExtraLarge:
                    ExtraLargeTemplateRows = rowSpan;
                    break;
                default: break;
            }

            return this;
        }
    }
}
