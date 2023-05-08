using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public class StylesGridItem
    {
        private readonly string[] ColStarts = new[] 
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
            "12",
            "13",
            "auto"
        };

        private readonly string[] ColSpans = new[]
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
            "12",
            "full"
        };

        private readonly string[] RowStarts = new[]
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "auto"
        };

        private readonly string[] RowSpans = new[]
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "full",
            "auto"
        };

        private readonly string[] Aligns = new[]
        {
            "start",
            "center",
            "end",
            "stretch",
            "auto"
        };

        [JsonProperty("display_name")]
        public string? Name { get; set; }

        private string _smallColumnStart = "auto";
        [JsonProperty("small_column_start")]
        public string SmallColumnStart 
        {
            get { return _smallColumnStart; }
            set { _smallColumnStart = Validate(value, ColStarts); }
        }

        private string _smallColumnSpan = "full";
        [JsonProperty("small_column_span")]
        public string SmallColumnSpan
        {
            get { return _smallColumnSpan; }
            set { _smallColumnSpan = Validate(value, ColSpans); }
        }

        private string _smallRowStart = "auto";
        [JsonProperty("small_row_start")]
        public string SmallRowStart
        {
            get { return _smallRowStart; }
            set { _smallRowStart = Validate(value, RowStarts); }
        }

        private string _smallRowSpan = "auto";
        [JsonProperty("small_row_span")]
        public string SmallRowSpan
        {
            get { return _smallRowSpan; }
            set { _smallRowSpan = Validate(value, RowSpans); }
        }

        private string _smallAlignSelf = "auto";
        [JsonProperty("small_align_self")]
        public string SmallAlignSelf
        {
            get { return _smallAlignSelf; }
            set { _smallAlignSelf = Validate(value, Aligns); }
        }

        private string _smallJustifySelf = "auto";
        [JsonProperty("small_justify_self")]
        public string SmallJustifySelf
        {
            get { return _smallJustifySelf; }
            set { _smallJustifySelf = Validate(value, Aligns); }
        }


        private string _mediumColumnStart = "auto";
        [JsonProperty("medium_column_start")]
        public string MediumColumnStart
        {
            get { return _mediumColumnStart; }
            set { _mediumColumnStart = Validate(value, ColStarts); }
        }

        private string _mediumColumnSpan = "full";
        [JsonProperty("medium_column_span")]
        public string MediumColumnSpan
        {
            get { return _mediumColumnSpan; }
            set { _mediumColumnSpan = Validate(value, ColSpans); }
        }

        private string _mediumRowStart = "auto";
        [JsonProperty("medium_row_start")]
        public string MediumRowStart
        {
            get { return _mediumRowStart; }
            set { _mediumRowStart = Validate(value, RowStarts); }
        }

        private string _mediumRowSpan = "auto";
        [JsonProperty("medium_row_span")]
        public string MediumRowSpan
        {
            get { return _mediumRowSpan; }
            set { _mediumRowSpan = Validate(value, RowSpans); }
        }

        private string _mediumAlignSelf = "auto";
        [JsonProperty("medium_align_self")]
        public string MediumAlignSelf
        {
            get { return _mediumAlignSelf; }
            set { _mediumAlignSelf = Validate(value, Aligns); }
        }

        private string _mediumJustifySelf = "auto";
        [JsonProperty("medium_justify_self")]
        public string MediumJustifySelf
        {
            get { return _mediumJustifySelf; }
            set { _mediumJustifySelf = Validate(value, Aligns); }
        }


        private string _largeColumnStart = "auto";
        [JsonProperty("large_column_start")]
        public string LargeColumnStart
        {
            get { return _largeColumnStart; }
            set { _largeColumnStart = Validate(value, ColStarts); }
        }

        private string _largeColumnSpan = "full";
        [JsonProperty("large_column_span")]
        public string LargeColumnSpan
        {
            get { return _largeColumnSpan; }
            set { _largeColumnSpan = Validate(value, ColSpans); }
        }

        private string _largeRowStart = "auto";
        [JsonProperty("large_row_start")]
        public string LargeRowStart
        {
            get { return _largeRowStart; }
            set { _largeRowStart = Validate(value, RowStarts); }
        }

        private string _largeRowSpan = "auto";
        [JsonProperty("large_row_span")]
        public string LargeRowSpan
        {
            get { return _largeRowSpan; }
            set { _largeRowSpan = Validate(value, RowSpans); }
        }

        private string _largeAlignSelf = "auto";
        [JsonProperty("large_align_self")]
        public string LargeAlignSelf
        {
            get { return _largeAlignSelf; }
            set { _largeAlignSelf = Validate(value, Aligns); }
        }

        private string _largeJustifySelf = "auto";
        [JsonProperty("large_justify_self")]
        public string LargeJustifySelf
        {
            get { return _largeJustifySelf; }
            set { _largeJustifySelf = Validate(value, Aligns); }
        }


        private string _extraLargeColumnStart = "auto";
        [JsonProperty("extra_large_column_start")]
        public string ExtraLargeColumnStart
        {
            get { return _extraLargeColumnStart; }
            set { _extraLargeColumnStart = Validate(value, ColStarts); }
        }

        private string _extraLargeColumnSpan = "full";
        [JsonProperty("extra_large_column_span")]
        public string ExtraLargeColumnSpan
        {
            get { return _extraLargeColumnSpan; }
            set { _extraLargeColumnSpan = Validate(value, ColSpans); }
        }

        private string _extraLargeRowStart = "auto";
        [JsonProperty("extra_large_row_start")]
        public string ExtraLargeRowStart
        {
            get { return _extraLargeRowStart; }
            set { _extraLargeRowStart = Validate(value, RowStarts); }
        }

        private string _extraLargeRowSpan = "auto";
        [JsonProperty("extra_large_row_span")]
        public string ExtraLargeRowSpan
        {
            get { return _extraLargeRowSpan; }
            set { _extraLargeRowSpan = Validate(value, RowSpans); }
        }

        private string _extraLargeAlignSelf = "auto";
        [JsonProperty("extra_large_align_self")]
        public string ExtraLargeAlignSelf
        {
            get { return _extraLargeAlignSelf; }
            set { _extraLargeAlignSelf = Validate(value, Aligns); }
        }

        private string _extraLargeJustifySelf = "auto";
        [JsonProperty("extra_large_justify_self")]
        public string ExtraLargeJustifySelf
        {
            get { return _extraLargeJustifySelf; }
            set { _extraLargeJustifySelf = Validate(value, Aligns); }
        }


        private string Validate(string val, string[] allowedValues)
        {
            if (allowedValues.Contains(val)) 
                return val;

            throw new JsonException("Invalid value " + val);
        }
    }
}
