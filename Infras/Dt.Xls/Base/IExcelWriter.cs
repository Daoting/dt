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
    /// Represents a collection of methods which be used to write customized data module to Excel file or stream.
    /// </summary>
    public interface IExcelWriter
    {
        /// <summary>
        /// Finish - called after the Excel file has completed saving.
        /// </summary>
        void Finish();
        /// <summary>
        /// Get the AutoFilter information for the specified sheet
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// An <see cref="T:Dt.Xls.IExcelAutoFilter" /> instance represent the AutoFilter settings for the specified sheet.
        /// </returns>
        IExcelAutoFilter GetAutoFilter(short sheet);
        /// <summary>
        /// Get the built-in name information used in the workbook
        /// </summary>
        /// <returns>
        /// A collection contains all built-in names 
        /// </returns>
        /// <remarks>
        /// It's a BIFF only method
        /// </remarks>
        List<IBuiltInName> GetBuiltInNameList();
        /// <summary>
        /// Gets the current workbook calculation property.
        /// </summary>
        /// <returns>
        /// An <see cref="T:Dt.Xls.ICalculationProperty" /> instances represent current calculation property
        /// </returns>
        ICalculationProperty GetCalculationProperty();
        /// <summary>
        /// Get all non empty cells used in the specified sheet.
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="cells">A collection of <see cref="T:Dt.Xls.IExcelCell" /> instances used to represents cells used in the sheet</param>
        bool GetCells(short sheet, List<IExcelCell> cells);
        /// <summary>
        /// Get the color palette used in the workbook
        /// </summary>
        /// <returns> An dictionary represents the color palette</returns>
        Dictionary<int, GcColor> GetColorPalette();
        /// <summary>
        /// Get all conditional formatting settings defined in the specified sheet.
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExcelConditionalFormat" /> formatting setting defined in the sheet.
        /// </returns>
        List<IExcelConditionalFormat> GetConditionalFormat(int sheet);
        /// <summary>
        /// Get the custom and function name information used in the workbook
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IFunction" /> contains all custom and function names
        /// </returns>
        List<IFunction> GetCustomOrFunctionNameList();
        /// <summary>
        /// Gets the default column width of the specified sheet
        /// </summary>
        /// <param name="sheet"> The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// The default column width of the specified sheet
        /// </returns>
        /// <remarks>
        /// Default column width measure as the number of characters of the maximum digit width of the normal style's font.
        /// </remarks>
        double GetDefaultColumnWidth(short sheet);
        /// <summary>
        /// Gets the default row height of the specified sheet
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="customHeight">Indicate whether the default row height has been manually set or set.</param>
        /// <returns>
        /// The default row height of the specified sheet
        /// </returns>
        /// <remarks>
        /// Default row height measure in point size. It's measure as the height of the characters of the normal style's font.
        /// </remarks>
        double GetDefaultRowHeight(short sheet, ref bool customHeight);
        /// <summary>
        /// Gets the differential formatting settings used in the workbook
        /// </summary>
        /// <returns>
        /// A collection represent all differential formatting settings 
        /// </returns>
        List<IDifferentialFormatting> GetDifferentialFormattingRecords();
        /// <summary>
        /// Get the minimum and maximum bounds of the sheet.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="row">Number of rows used in the sheet</param>
        /// <param name="column">Number of columns used in the sheet</param>
        void GetDimensions(short sheet, ref int row, ref int column);
        /// <summary>
        /// Gets whether the specified sheet should show formula, show zeros, show gridlines, show row column headers, and whether its column is right to left.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="showFormula">A flag indicate whether the specified sheet should show formula</param>
        /// <param name="showZeros">A flag indicate whether the specified sheet should show zero</param>
        /// <param name="showGridLine">A flag indicate whether the specified sheet should show gridline</param>
        /// <param name="showRowColumnHeader">A flag indicate whether the specified sheet should show row column header</param>
        /// <param name="rightToLeftColumns">A flag indicate whether the column of the specified sheet is right to left</param>
        void GetDisplayElements(short sheet, ref bool showFormula, ref bool showZeros, ref bool showGridLine, ref bool showRowColumnHeader, ref bool rightToLeftColumns);
        /// <summary>
        /// Gets the format settings used in the current workbook
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExtendedFormat" /> instances used in the current workbook
        /// </returns>
        List<IExtendedFormat> GetExcelCellFormats();
        /// <summary>
        /// Get the default cell format setting
        /// </summary>
        /// <returns>The default cell format setting</returns>
        IExtendedFormat GetExcelDefaultCellFormat();
        /// <summary>
        /// Gets the excel styles used in the current workbook
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExcelStyle" /> instances used in the current workbook
        /// </returns>
        List<IExcelStyle> GetExcelStyles();
        /// <summary>
        /// Get the external referenced workbook information used in the workbook
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExternalWorkbookInfo" /> represents all external referenced workbook information
        /// </returns>
        List<IExternalWorkbookInfo> GetExternWorkbookInfo();
        /// <summary>
        /// Gets the frozen row and column count of the specified sheet
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="frozenRowCount">Number of fixed or frozen leading rows</param>
        /// <param name="frozenColumnCount">Number of fixed or frozen leading columns</param>
        /// <param name="frozenTrailingRowCount">Number of fixed or frozen trailing rows</param>
        /// <param name="frozenTrailingColumnCount">Number of fixed or frozen trailing columns</param>
        void GetFrozen(short sheet, ref int frozenRowCount, ref int frozenColumnCount, ref int frozenTrailingRowCount, ref int frozenTrailingColumnCount);
        /// <summary>
        /// Get the gridline color of the specified sheet.
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// An <see cref="T:Dt.Xls.IExcelColor" /> instance used to represents the color of the grid line of the specified sheet
        /// </returns>
        IExcelColor GetGridlineColor(short sheet);
        /// <summary>
        /// Gets the row and column gutter settings of the specified sheet.
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="rowMaxOutLineLevel">Maximum level of row gutter</param>
        /// <param name="columnMaxOutLineLevel">Maximum level of column gutter</param>
        void GetGutters(short sheet, ref int rowMaxOutLineLevel, ref int columnMaxOutLineLevel);
        /// <summary>
        /// Get all named cell ranged defined in the current workbook
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IName" /> contains all named cell ranged defined in the current workbook
        /// </returns>
        List<IName> GetInternalDefinedNames();
        /// <summary>
        /// Get merged cells information of the specified sheet
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>A collection of <see cref="T:Dt.Xls.IRange" /> instances used to represent merged cell information</returns>
        List<IRange> GetMergedCells(short sheet);
        /// <summary>
        /// Get all non empty columns in the specified sheet.
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExcelColumn" /> contains all non empty columns
        /// </returns>
        List<IExcelColumn> GetNonEmptyColumns(short sheet);
        /// <summary>
        /// Get all non empty row index in the specified sheet.
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// A collections of <see cref="T:Dt.Xls.IExcelRow" />contains all non-empty rows.
        /// </returns>
        List<IExcelRow> GetNonEmptyRows(short sheet);
        /// <summary>
        /// Get the summary rows/columns directions
        /// </summary>
        /// <param name="sheet">The zero based sheet index.</param>
        /// <param name="summaryColumnsRightToDetail">A flag indicate whether the summary columns to right of details, true if it's right to detail, otherwise, false</param>
        /// <param name="summaryRowsBelowDetail">A flag indicate whether the summary rows below details, true if it below detail, otherwise, false</param>
        void GetOutlineDirection(int sheet, ref bool summaryColumnsRightToDetail, ref bool summaryRowsBelowDetail);
        /// <summary>
        /// Get the index of the closest color of the workbook color palette
        /// </summary>
        /// <param name="color"> An <see cref="T:Dt.Xls.IExcelColor" /> instance used to compare with the palette color</param>
        /// <returns> The closest palette color index</returns>
        ExcelPaletteColor GetPaletteColor(IExcelColor color);
        /// <summary>
        /// Gets the number and position of unfrozen panes in a window
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="horizontalPosition">set the value for the horizontal position of the split, 0 if none</param>
        /// <param name="verticalPosition">set the value for the vertical position of the split, 0 if none</param>
        /// <param name="topVisibleRow">set the count of top visible rows  in the bottom pane</param>
        /// <param name="leftmostVisibleColumn">set the count of leftmost visible columns in the right pane</param>
        /// <param name="paneIndex">the active pane number</param>
        void GetPane(short sheet, ref int horizontalPosition, ref int verticalPosition, ref int topVisibleRow, ref int leftmostVisibleColumn, ref int paneIndex);
        /// <summary>
        /// Get the print area setting of the specified sheet.
        /// </summary>
        /// <param name="sheet">The sheet index</param>
        /// <returns>The print area setting. null if the print area has not been set</returns>
        string GetPrintArea(int sheet);
        /// <summary>
        /// Gets the print options of the sheet
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// An <see cref="T:Dt.Xls.IExcelPrintOptions" /> instance represent the sheet print option
        /// </returns>
        IExcelPrintOptions GetPrintOptions(short sheet);
        /// <summary>
        /// Gets the print page margin information
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// An <see cref="T:Dt.Xls.IExcelPrintPageMargin" /> instance represent page margin setting
        /// </returns>
        IExcelPrintPageMargin GetPrintPageMargin(short sheet);
        /// <summary>
        /// Gets the print page setting of the sheet.
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// A <see cref="T:Dt.Xls.IExcelPrintPageSetting" /> instance represent the sheet print page setting
        /// </returns>
        IExcelPrintPageSetting GetPrintPageSetting(short sheet);
        /// <summary>
        /// Get the print title setting of the specified sheet.
        /// </summary>
        /// <param name="sheet">The sheet index</param>
        /// <returns>The print title setting. null if the print area has not been set</returns>
        string GetPrintTitle(int sheet);
        /// <summary>
        /// Gets the protection setting  of the specified sheet
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="isProtect">Whether sheet is protected</param>
        void GetProtect(short sheet, ref bool isProtect);
        /// <summary>
        /// Gets the scroll bars setting for the horizontal and vertical scroll bars.
        /// </summary>
        /// <param name="horizontalScroll">Indicate whether the horizontal scroll bar policy is AsNeeded</param>
        /// <param name="verticalScroll">Indicate whether the vertical scroll bar policy is AsNeeded</param>
        void GetScroll(ref bool horizontalScroll, ref bool verticalScroll);
        /// <summary>
        /// Gets the selection list of the specified sheet
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="selectionList">A collection of the all selections defined in the current sheet</param>
        /// <param name="activeCell">Active cell location</param>
        /// <param name="paneType">Type of the pane</param>
        /// <returns>
        /// <see langword="true" /> if the selection is not null, otherwise, <see langword="false" /> 
        /// </returns>
        bool GetSelectionList(short sheet, List<GcRect> selectionList, ref GcPoint activeCell, ref PaneType paneType);
        /// <summary>
        /// Gets the sheets count of the current workbook
        /// </summary>
        /// <returns>
        /// The sheets count of the current workbook
        /// </returns>
        int GetSheetCount();
        /// <summary>
        /// Gets the name of the sheet.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// The sheet name
        /// </returns>
        string GetSheetName(int sheet);
        /// <summary>
        /// Get the tab settings of the current workbook
        /// </summary>
        /// <param name="showTabs">A flag indicate whether show workbook tabs or not</param>
        /// <param name="selectedTabIndex">Zero based index of the selected workbook tab</param>
        /// <param name="firstDisplayedTabIndex">Zero based index of the first displayed workbook tab</param>
        /// <param name="selectedTabCount">Number of workbook tabs that are selected</param>
        /// <param name="tabRatio">Ratio of the width of the workbook tabs to the width of the horizontal scroll bar.</param>
        bool GetTabs(ref bool showTabs, ref int selectedTabIndex, ref int firstDisplayedTabIndex, ref int selectedTabCount, ref int tabRatio);
        /// <summary>
        /// Get the theme used in the workbook
        /// </summary>
        /// <returns> An <seealso cref="T:Dt.Xls.IExcelTheme" /> instance used to represents an theme instance</returns>
        IExcelTheme GetTheme();
        /// <summary>
        /// Gets the first visible row and column information in the window
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="topRow">Top row visible in the window</param>
        /// <param name="leftColumn">Leftmost column visible in the window</param>
        bool GetTopLeft(short sheet, ref int topRow, ref int leftColumn);
        /// <summary>
        /// Gets the data validation information for the specified sheet
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExcelDataValidation" /> represents all data validation settings for the specified sheet.
        /// </returns>
        List<IExcelDataValidation> GetValidationData(short sheet);
        /// <summary>
        /// Get the location of the window, whether it's hidden or currently displayed as an icon
        /// </summary>
        /// <param name="hidden">A flag indicate whether the window is hidden</param>
        /// <param name="iconic">A flag indicate whether the window is currently display as an icon.</param>
        /// <returns>
        /// An <see cref="T:Dt.Xls.IExcelRect" /> instance used to represent the window properties.
        /// </returns>
        IExcelRect GetWindow(ref bool hidden, ref bool iconic);
        /// <summary>
        /// Gets the current workbook property setting
        /// </summary>
        /// <returns>
        /// An <see cref="T:Dt.Xls.IExcelWorkbookPropery" /> instance represents current workbook setting
        /// </returns>
        IExcelWorkbookPropery GetWorkbookProperty();
        /// <summary>
        /// Gets the zoom (scaling factor) for exporting to Excel file.
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.ExcelWorksheet" /> instance</param>
        /// <param name="zoom">the zoom (scaling factor) of the specified sheet</param>
        void GetZoom(short sheet, ref float zoom);
        /// <summary>
        /// Determine if the value is a calculation error; if it is, get the error id
        /// </summary>
        /// <param name="value">the value which will be determined whether it's a calculate error</param>
        /// <param name="errorVal">the error id if the value is a calculation error, otherwise, 0XFF indicate it's an unknown error. </param>
        /// <returns>
        /// <see langword="true" /> if the value is a calculate error, otherwise, <see longword="false" />
        /// </returns>
        bool IsCalculationError(object value, ref short errorVal);
        /// <summary>
        /// Get the value which indicate whether the sheet is visible or hidden.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <returns>
        /// <see langword="true" /> if the sheet is hidden, otherwise, <see langword="false" />.
        /// </returns>
        bool IsSheetHidden(int sheet);
        /// <summary>
        /// This method will be invoked when exception or abnormal condition happens during save file.
        /// </summary>
        /// <param name="excelWarning">An <see cref="T:Dt.Xls.ExcelWarning" /> instance used to represents errors during load the excel file.</param>
        void OnExcelSaveError(ExcelWarning excelWarning);
    }
}

