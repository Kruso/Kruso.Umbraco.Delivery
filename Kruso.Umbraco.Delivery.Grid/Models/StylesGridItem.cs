using Newtonsoft.Json;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public class StylesGridItem : Styles<SettingsGridItem>
    {
        public StylesGridItem SetColSpans(StylesConstants.Breakpoint breakpoint, int colSpan) => SetColSpans(breakpoint, colSpan.ToString());
        public StylesGridItem SetColSpans(StylesConstants.Breakpoint breakpoint, string colSpan)
        {
            switch (breakpoint)
            {
                case StylesConstants.Breakpoint.Small: 
                    Small.ColumnSpan = colSpan;
                    Medium.ColumnSpan = colSpan;
                    Large.ColumnSpan = colSpan;
                    ExtraLarge.ColumnSpan = colSpan;
                    break;
                case StylesConstants.Breakpoint.Medium:
                    Medium.ColumnSpan = colSpan;
                    Large.ColumnSpan = colSpan;
                    ExtraLarge.ColumnSpan = colSpan;
                    break;
                case StylesConstants.Breakpoint.Large:
                    Large.ColumnSpan = colSpan;
                    ExtraLarge.ColumnSpan = colSpan;
                    break;
                case StylesConstants.Breakpoint.ExtraLarge:
                    ExtraLarge.ColumnSpan = colSpan;
                    break;
                default: break;
            }

            return this;
        }

        public StylesGridItem SetRowSpans(StylesConstants.Breakpoint breakpoint, int rowSpan) => SetRowSpans(breakpoint, rowSpan.ToString());
        public StylesGridItem SetRowSpans(StylesConstants.Breakpoint breakpoint, string rowSpan)
        {
            switch (breakpoint)
            {
                case StylesConstants.Breakpoint.Small:
                    Small.RowSpan = rowSpan;
                    Medium.RowSpan = rowSpan;
                    Large.RowSpan = rowSpan;
                    ExtraLarge.RowSpan = rowSpan;
                    break;
                case StylesConstants.Breakpoint.Medium:
                    Medium.RowSpan = rowSpan;
                    Large.RowSpan = rowSpan;
                    ExtraLarge.RowSpan = rowSpan;
                    break;
                case StylesConstants.Breakpoint.Large:
                    Large.RowSpan = rowSpan;
                    ExtraLarge.RowSpan = rowSpan;
                    break;
                case StylesConstants.Breakpoint.ExtraLarge:
                    ExtraLarge.RowSpan = rowSpan;
                    break;
                default: break;
            }

            return this;
        }
    }
}
