#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Defines generalized properties and methods that value or class implements to represents excel workbook.
    /// </summary>
    public interface IExcelWorkbook
    {
        /// <summary>
        /// Gets the closest palette color of specified color.
        /// </summary>
        /// <param name="color">An <see cref="T:Dt.Xls.IExcelColor" /> instance used to locate the palette color.</param>
        /// <returns>The closest palette color</returns>
        ExcelPaletteColor GetPaletteColor(IExcelColor color);
        /// <summary>
        /// Gets the color of the specified color scheme index.
        /// </summary>
        /// <param name="themeIndex">Index of color scheme</param>
        /// <returns>The theme color at the specified color scheme index</returns>
        GcColor GetThemeColor(ColorSchemeIndex themeIndex);

        /// <summary>
        /// Gets or sets the index of the active pane.
        /// </summary>
        /// <value>The index of the active pane.</value>
        int ActivePaneIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the active sheet.
        /// </summary>
        /// <value>The index of the active sheet.</value>
        int ActiveSheetIndex { get; set; }

        /// <summary>
        /// Gets or sets the built-in names used in the workbook
        /// </summary>
        /// <value>A collection of <see cref="T:Dt.Xls.IBuiltInName" /> represents the built-in names information of the workbook</value>
        List<IBuiltInName> BuiltInNameList { get; set; }

        /// <summary>
        /// Gets or sets the calculation mode of the workbook
        /// </summary>
        /// <value>The calculation mode of the workbook</value>
        ExcelCalculationMode CalculationMode { get; set; }

        /// <summary>
        /// Gets or sets the color palette used in the workbook.
        /// </summary>
        /// <value>The color palette used in the workbook</value>
        /// <remarks>It the value is null. it means the workbook use the default color palette.</remarks>
        Dictionary<int, GcColor> ColorPalette { get; set; }

        /// <summary>
        /// Gets or sets the custom or function names used in the workbook.
        /// </summary>
        /// <value>A collection of <see cref="T:Dt.Xls.IFunction" /> represents the custom or function names</value>
        List<IFunction> CustomOrFunctionNameList { get; set; }

        /// <summary>
        /// Gets or sets the default cell format of the workbook..
        /// </summary>
        /// <value>The default cell format.</value>
        IExtendedFormat DefaultCellFormat { get; set; }

        /// <summary>
        /// Get or set the name of the default table style to apply to new PivotTables.
        /// </summary>
        string DefaultPivotTableStyleName { get; set; }

        /// <summary>
        /// Get or set the name of the default table style to apply to new tables.
        /// </summary>
        string DefaultTableStyleName { get; set; }

        /// <summary>
        /// Gets or sets the differential formatting settings of the workbook.
        /// </summary>
        /// <value>A collection of <see cref="T:Dt.Xls.IDifferentialFormatting" /> represents the differential formatting settings.</value>
        List<IDifferentialFormatting> DifferentialFormattings { get; set; }

        /// <summary>
        /// Gets or sets the excel cell formats used in the workbook.
        /// </summary>
        /// <value>The excel cell formats.</value>
        Dictionary<int, IExtendedFormat> ExcelCellFormats { get; set; }

        /// <summary>
        /// Gets or sets the excel rect used to represents the workbook window.
        /// </summary>
        /// <value>The excel rect represents the workbook window</value>
        IExcelRect ExcelRect { get; set; }

        /// <summary>
        /// Gets the excel styles used in the workbook.
        /// </summary>
        /// <value>The excel styles.</value>
        List<IExcelStyle> ExcelStyles { get; }

        /// <summary>
        /// Gets or sets the extern workbook information used in the workbook
        /// </summary>
        /// <value>A collection of <see cref="T:Dt.Xls.IExternalWorkbookInfo" /> represents the extern workbook information.</value>
        List<IExternalWorkbookInfo> ExternWorkbooks { get; set; }

        /// <summary>
        /// Gets or sets the first index of the displayed tab.
        /// </summary>
        /// <value>The first index of the displayed tab.</value>
        int FirstDisplayedTabIndex { get; set; }

        /// <summary>
        /// Gets or sets the horizontal scroll bar policy.
        /// </summary>
        /// <value>The horizontal scroll bar policy.</value>
        ExcelScrollBarPolicy HorizontalScrollBarPolicy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workbook use 1904 date system.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it use 1904 date system; otherwise, <see langword="false" />.
        /// </value>
        bool Is1904Date { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is iterate calculate.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is iterate calculate; otherwise, <see langword="false" />.
        /// </value>
        bool IsIterataCalculate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is window display as icon.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is window display as icon; otherwise, <see langword="false" />.
        /// </value>
        bool IsWindowDisplayAsIcon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is window hidden.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is window hidden; otherwise, <see langword="false" />.
        /// </value>
        bool IsWindowHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="T:Dt.Xls.IExcelWorkbook" /> instance is locked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if locked; otherwise, <see langword="false" />.
        /// </value>
        bool Locked { get; set; }

        /// <summary>
        /// Gets or sets the maximum change of the workbook.
        /// </summary>
        /// <value>The value of maximum change.</value>
        double MaximumChange { get; set; }

        /// <summary>
        /// Gets or sets the maximum iterations of the workbook
        /// </summary>
        /// <value>The maximum iterations of the workbook</value>
        int MaximumIterations { get; set; }

        /// <summary>
        /// Gets or sets the global named cell ranges.
        /// </summary>
        /// <value>A dictionary represents the global named cell ranges of the workbook</value>
        Dictionary<string, IName> NamedCellRanges { get; set; }

        /// <summary>
        /// Gets the excel operator which used to read or write information from (to) excel.
        /// </summary>
        /// <value>The excel operator.</value>
        ExcelOperator Operator { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is full precision.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if is full precision; otherwise, <see langword="false" />.
        /// </value>
        bool PrecisionAsDisplay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether recalculate formulas before save.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if recalculate all formulas before save; otherwise, <see langword="false" />.
        /// </value>
        bool RecalculateBeforeSave { get; set; }

        /// <summary>
        /// Gets or sets the workbook reference style.
        /// </summary>
        /// <value>The reference style of the workbook.</value>
        ExcelReferenceStyle ReferenceStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether save external links during save.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if save external links; otherwise, <see langword="false" />.
        /// </value>
        bool SaveExternalLinks { get; set; }

        /// <summary>
        /// Gets or sets the selected tab count.
        /// </summary>
        /// <value>The selected tab count.</value>
        int SelectedTabCount { get; set; }

        /// <summary>
        /// Get the table styles used in the current workbook 
        /// </summary>
        List<IExcelTableStyle> TableStyles { get; }

        /// <summary>
        /// Gets or sets the tab strip policy.
        /// </summary>
        /// <value>The tab strip policy.</value>
        ExcelTabStripPolicy TabStripPolicy { get; set; }

        /// <summary>
        /// Gets or sets the tab strip ratio.
        /// </summary>
        /// <value>The tab strip ratio.</value>
        double TabStripRatio { get; set; }

        /// <summary>
        /// Gets or sets the theme used in the <see cref="T:Dt.Xls.IExcelWorkbook" /> instance.
        /// </summary>
        /// <value>The <see cref="T:Dt.Xls.IExcelTheme" /> instance used in the current workbook</value>
        IExcelTheme Theme { get; set; }

        /// <summary>
        /// Gets or sets the vertical scroll bar policy.
        /// </summary>
        /// <value>The vertical scroll bar policy.</value>
        ExcelScrollBarPolicy VerticalScrollBarPolicy { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="T:Dt.Xls.IExcelWorksheet" /> of the workbook.
        /// </summary>
        /// <value>An <see cref="T:Dt.Xls.ExcelWorksheetCollection" /> represents all <see cref="T:Dt.Xls.IExcelWorksheet" /> instances used in the workbook</value>
        ExcelWorksheetCollection Worksheets { get; }
    }
}

