using Newtonsoft.Json;

namespace Kruso.Umbraco.Delivery.Grid.Models
{
    public class StylesGrid : Styles<SettingsGrid>
    {
        public StylesGrid SetColumns(StylesConstants.Breakpoint breakpoint, int colSpan) => SetColumns(breakpoint, colSpan.ToString());
        public StylesGrid SetColumns(StylesConstants.Breakpoint breakpoint, string colSpan)
        {
            switch (breakpoint)
            {
                case StylesConstants.Breakpoint.Small:
                    Small.TemplateColumns = colSpan;
                    Medium.TemplateColumns = colSpan;
                    Large.TemplateColumns = colSpan;
                    ExtraLarge.TemplateColumns = colSpan;
                    break;
                case StylesConstants.Breakpoint.Medium:
                    Medium.TemplateColumns = colSpan;
                    Large.TemplateColumns = colSpan;
                    ExtraLarge.TemplateColumns = colSpan;
                    break;
                case StylesConstants.Breakpoint.Large:
                    Large.TemplateColumns = colSpan;
                    ExtraLarge.TemplateColumns = colSpan;
                    break;
                case StylesConstants.Breakpoint.ExtraLarge:
                    ExtraLarge.TemplateColumns = colSpan;
                    break;
                default: break;
            }

            return this;
        }

        public StylesGrid SetColumnGap(StylesConstants.Breakpoint breakpoint, string columnGap)
        {
            switch (breakpoint)
            {
                case StylesConstants.Breakpoint.Small:
                    Small.ColumnGap = columnGap;
                    Medium.ColumnGap = columnGap;
                    Large.ColumnGap = columnGap;
                    ExtraLarge.ColumnGap = columnGap;
                    break;
                case StylesConstants.Breakpoint.Medium:
                    Medium.ColumnGap = columnGap;
                    Large.ColumnGap = columnGap;
                    ExtraLarge.ColumnGap = columnGap;
                    break;
                case StylesConstants.Breakpoint.Large:
                    Large.ColumnGap = columnGap;
                    ExtraLarge.ColumnGap = columnGap;
                    break;
                case StylesConstants.Breakpoint.ExtraLarge:
                    ExtraLarge.ColumnGap = columnGap;
                    break;
                default: break;
            }

            return this;
        }

        public StylesGrid SetRowGap(StylesConstants.Breakpoint breakpoint, string rowGap)
        {
            switch (breakpoint)
            {
                case StylesConstants.Breakpoint.Small:
                    Small.RowGap = rowGap;
                    Medium.RowGap = rowGap;
                    Large.RowGap = rowGap;
                    ExtraLarge.RowGap = rowGap;
                    break;
                case StylesConstants.Breakpoint.Medium:
                    Medium.RowGap = rowGap;
                    Large.RowGap = rowGap;
                    ExtraLarge.RowGap = rowGap;
                    break;
                case StylesConstants.Breakpoint.Large:
                    Large.RowGap = rowGap;
                    ExtraLarge.RowGap = rowGap;
                    break;
                case StylesConstants.Breakpoint.ExtraLarge:
                    ExtraLarge.RowGap = rowGap;
                    break;
                default: break;
            }

            return this;
        }


        public StylesGrid SetRows(StylesConstants.Breakpoint breakpoint, int rowSpan) => SetRows(breakpoint, rowSpan.ToString());
        public StylesGrid SetRows(StylesConstants.Breakpoint breakpoint, string rowSpan)
        {
            switch (breakpoint)
            {
                case StylesConstants.Breakpoint.Small:
                    Small.TemplateRows = rowSpan;
                    Medium.TemplateRows = rowSpan;
                    Large.TemplateRows = rowSpan;
                    ExtraLarge.TemplateRows = rowSpan;
                    break;
                case StylesConstants.Breakpoint.Medium:
                    Medium.TemplateRows = rowSpan;
                    Large.TemplateRows = rowSpan;
                    ExtraLarge.TemplateRows = rowSpan;
                    break;
                case StylesConstants.Breakpoint.Large:
                    Large.TemplateRows = rowSpan;
                    ExtraLarge.TemplateRows = rowSpan;
                    break;
                case StylesConstants.Breakpoint.ExtraLarge:
                    ExtraLarge.TemplateRows = rowSpan;
                    break;
                default: break;
            }

            return this;
        }
    }
}
