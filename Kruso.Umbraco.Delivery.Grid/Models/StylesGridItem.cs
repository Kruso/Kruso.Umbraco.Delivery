using Newtonsoft.Json;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public class StylesGridItem
    {
        [JsonProperty("display_name")]
        public string? Name { get; set; }

        private string _smallColumnStart = "auto";
        [JsonProperty("small_column_start")]
        public string SmallColumnStart 
        {
            get { return _smallColumnStart; }
            set { _smallColumnStart = Styles.Validate(value, Styles.ColStarts); }
        }

        private string _smallColumnSpan = "full";
        [JsonProperty("small_column_span")]
        public string SmallColumnSpan
        {
            get { return _smallColumnSpan; }
            set { _smallColumnSpan = Styles.Validate(value, Styles.ColSpans); }
        }

        private string _smallRowStart = "auto";
        [JsonProperty("small_row_start")]
        public string SmallRowStart
        {
            get { return _smallRowStart; }
            set { _smallRowStart = Styles.Validate(value, Styles.RowStarts); }
        }

        private string _smallRowSpan = "auto";
        [JsonProperty("small_row_span")]
        public string SmallRowSpan
        {
            get { return _smallRowSpan; }
            set { _smallRowSpan = Styles.Validate(value, Styles.RowSpans); }
        }

        private string _smallAlignSelf = "auto";
        [JsonProperty("small_align_self")]
        public string SmallAlignSelf
        {
            get { return _smallAlignSelf; }
            set { _smallAlignSelf = Styles.Validate(value, Styles.Aligns); }
        }

        private string _smallJustifySelf = "auto";
        [JsonProperty("small_justify_self")]
        public string SmallJustifySelf
        {
            get { return _smallJustifySelf; }
            set { _smallJustifySelf = Styles.Validate(value, Styles.Aligns); }
        }


        private string _mediumColumnStart = "auto";
        [JsonProperty("medium_column_start")]
        public string MediumColumnStart
        {
            get { return _mediumColumnStart; }
            set { _mediumColumnStart = Styles.Validate(value, Styles.ColStarts); }
        }

        private string _mediumColumnSpan = "full";
        [JsonProperty("medium_column_span")]
        public string MediumColumnSpan
        {
            get { return _mediumColumnSpan; }
            set { _mediumColumnSpan = Styles.Validate(value, Styles.ColSpans); }
        }

        private string _mediumRowStart = "auto";
        [JsonProperty("medium_row_start")]
        public string MediumRowStart
        {
            get { return _mediumRowStart; }
            set { _mediumRowStart = Styles.Validate(value, Styles.RowStarts); }
        }

        private string _mediumRowSpan = "auto";
        [JsonProperty("medium_row_span")]
        public string MediumRowSpan
        {
            get { return _mediumRowSpan; }
            set { _mediumRowSpan = Styles.Validate(value, Styles.RowSpans); }
        }

        private string _mediumAlignSelf = "auto";
        [JsonProperty("medium_align_self")]
        public string MediumAlignSelf
        {
            get { return _mediumAlignSelf; }
            set { _mediumAlignSelf = Styles.Validate(value, Styles.Aligns); }
        }

        private string _mediumJustifySelf = "auto";
        [JsonProperty("medium_justify_self")]
        public string MediumJustifySelf
        {
            get { return _mediumJustifySelf; }
            set { _mediumJustifySelf = Styles.Validate(value, Styles.Aligns); }
        }


        private string _largeColumnStart = "auto";
        [JsonProperty("large_column_start")]
        public string LargeColumnStart
        {
            get { return _largeColumnStart; }
            set { _largeColumnStart = Styles.Validate(value, Styles.ColStarts); }
        }

        private string _largeColumnSpan = "full";
        [JsonProperty("large_column_span")]
        public string LargeColumnSpan
        {
            get { return _largeColumnSpan; }
            set { _largeColumnSpan = Styles.Validate(value, Styles.ColSpans); }
        }

        private string _largeRowStart = "auto";
        [JsonProperty("large_row_start")]
        public string LargeRowStart
        {
            get { return _largeRowStart; }
            set { _largeRowStart = Styles.Validate(value, Styles.RowStarts); }
        }

        private string _largeRowSpan = "auto";
        [JsonProperty("large_row_span")]
        public string LargeRowSpan
        {
            get { return _largeRowSpan; }
            set { _largeRowSpan = Styles.Validate(value, Styles.RowSpans); }
        }

        private string _largeAlignSelf = "auto";
        [JsonProperty("large_align_self")]
        public string LargeAlignSelf
        {
            get { return _largeAlignSelf; }
            set { _largeAlignSelf = Styles.Validate(value, Styles.Aligns); }
        }

        private string _largeJustifySelf = "auto";
        [JsonProperty("large_justify_self")]
        public string LargeJustifySelf
        {
            get { return _largeJustifySelf; }
            set { _largeJustifySelf = Styles.Validate(value, Styles.Aligns); }
        }


        private string _extraLargeColumnStart = "auto";
        [JsonProperty("extra_large_column_start")]
        public string ExtraLargeColumnStart
        {
            get { return _extraLargeColumnStart; }
            set { _extraLargeColumnStart = Styles.Validate(value, Styles.ColStarts); }
        }

        private string _extraLargeColumnSpan = "full";
        [JsonProperty("extra_large_column_span")]
        public string ExtraLargeColumnSpan
        {
            get { return _extraLargeColumnSpan; }
            set { _extraLargeColumnSpan = Styles.Validate(value, Styles.ColSpans); }
        }

        private string _extraLargeRowStart = "auto";
        [JsonProperty("extra_large_row_start")]
        public string ExtraLargeRowStart
        {
            get { return _extraLargeRowStart; }
            set { _extraLargeRowStart = Styles.Validate(value, Styles.RowStarts); }
        }

        private string _extraLargeRowSpan = "auto";
        [JsonProperty("extra_large_row_span")]
        public string ExtraLargeRowSpan
        {
            get { return _extraLargeRowSpan; }
            set { _extraLargeRowSpan = Styles.Validate(value, Styles.RowSpans); }
        }

        private string _extraLargeAlignSelf = "auto";
        [JsonProperty("extra_large_align_self")]
        public string ExtraLargeAlignSelf
        {
            get { return _extraLargeAlignSelf; }
            set { _extraLargeAlignSelf = Styles.Validate(value, Styles.Aligns); }
        }

        private string _extraLargeJustifySelf = "auto";
        [JsonProperty("extra_large_justify_self")]
        public string ExtraLargeJustifySelf
        {
            get { return _extraLargeJustifySelf; }
            set { _extraLargeJustifySelf = Styles.Validate(value, Styles.Aligns); }
        }

        public StylesGridItem SetColSpans(Styles.Breakpoint breakpoint, int colSpan) => SetColSpans(breakpoint, colSpan.ToString());
        public StylesGridItem SetColSpans(Styles.Breakpoint breakpoint, string colSpan)
        {
            switch (breakpoint)
            {
                case Styles.Breakpoint.Small: 
                    SmallColumnSpan = colSpan;
                    MediumColumnSpan = colSpan;
                    LargeColumnSpan = colSpan;
                    ExtraLargeColumnSpan = colSpan;
                    break;
                case Styles.Breakpoint.Medium:
                    MediumColumnSpan = colSpan;
                    LargeColumnSpan = colSpan;
                    ExtraLargeColumnSpan = colSpan;
                    break;
                case Styles.Breakpoint.Large:
                    LargeColumnSpan = colSpan;
                    ExtraLargeColumnSpan = colSpan;
                    break;
                case Styles.Breakpoint.ExtraLarge:
                    ExtraLargeColumnSpan = colSpan;
                    break;
                default: break;
            }

            return this;
        }

        public StylesGridItem SetRowSpans(Styles.Breakpoint breakpoint, int rowSpan) => SetRowSpans(breakpoint, rowSpan.ToString());
        public StylesGridItem SetRowSpans(Styles.Breakpoint breakpoint, string rowSpan)
        {
            switch (breakpoint)
            {
                case Styles.Breakpoint.Small:
                    SmallRowSpan = rowSpan;
                    MediumRowSpan = rowSpan;
                    LargeRowSpan = rowSpan;
                    ExtraLargeRowSpan = rowSpan;
                    break;
                case Styles.Breakpoint.Medium:
                    MediumRowSpan = rowSpan;
                    LargeRowSpan = rowSpan;
                    ExtraLargeRowSpan = rowSpan;
                    break;
                case Styles.Breakpoint.Large:
                    LargeRowSpan = rowSpan;
                    ExtraLargeRowSpan = rowSpan;
                    break;
                case Styles.Breakpoint.ExtraLarge:
                    ExtraLargeRowSpan = rowSpan;
                    break;
                default: break;
            }

            return this;
        }
    }
}
