using Newtonsoft.Json;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public static class StylesConstants
    {
        public enum Breakpoint
        {
            Small,
            Medium,
            Large,
            ExtraLarge
        };

        public static string[] Gaps = new[]
{
            "x-small",
            "small",
            "medium",
            "large",
            "x-large"
        };

        public static string[] Aligns = new[]
{
            "start",
            "center",
            "end",
            "stretch",
            "auto"
        };

        public static string[] Columns = new[]
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

        public static string[] Rows = new[]
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"
        };

        public static string[] ColStarts = new[]
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

        public static string[] ColSpans = new[]
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

        public static string[] RowStarts = new[]
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

        public static string[] RowSpans = new[]
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

        public static string? Validate(string? val, string[] allowedValues = null)
        {
            return val;
            //if (val == null || allowedValues == null || allowedValues.Contains(val))
                //return val;

            //throw new JsonException("Invalid value " + val);
        }
    }
}
