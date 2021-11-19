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
    /// Represents a collection of generalized methods which be used to read Excel file or stream.
    /// </summary>
    public interface IExcelReader
    {
        /// <summary>
        /// Add a new <see cref="T:Dt.Xls.IExcelWorksheet" /> instance to the underlying <see cref="T:Dt.Xls.IExcelWorkbook" /> model.
        /// </summary>
        /// <param name="name">the name of the <see cref="T:Dt.Xls.IExcelWorksheet" /></param>
        /// <param name="hiddenState">
        /// A value used to represent the sheet status, Worksheet in Excel has three status, visible, hidden and very hidden
        /// the corresponding value is 0,1,2. Since user can't set the Excelsheet status to very hidden by UI, it will be treated as hidden by default.
        /// </param>
        /// <param name="sheetType">The sheet type.</param>
        void AddSheet(string name, byte hiddenState, ExcelSheetType sheetType);
        /// <summary>
        /// Beginning of file; notifies the implementer that the global workbook stream is loading or we are loading a new Worksheet. 
        /// </summary>
        /// <param name="sheet">The zero based index of Worksheet. If the value is -1, it means it loading the global workbook properties.</param>
        void Bof(short sheet);
        /// <summary>
        /// It will be called after the Excel file has completed loading.
        /// </summary>
        void Finish();
        /// <summary>
        /// This method will be invoked when exception or abnormal condition happens
        /// </summary>
        /// <param name="excelWarning">An <see cref="T:Dt.Xls.ExcelWarning" /> instance used to represents errors during load the excel file.</param>
        void OnExcelLoadError(ExcelWarning excelWarning);
        /// <summary>
        /// Set array formula to a range of cells
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="rowFirst">The zero based inclusive top left cell row index</param>
        /// <param name="rowLast">The zero based inclusive bottom right cell row index</param>
        /// <param name="columnFirst">The zero based inclusive top left cell column index</param>
        /// <param name="columnLast">The zero based inclusive top left cell column index</param>
        /// <param name="arrayFormula">The array formula for these cell</param>
        /// <exception cref="T:System.ArgumentNullException">throws when the formula is null or empty.</exception>
        void SetArrayFormula(int sheet, int rowFirst, int rowLast, short columnFirst, short columnLast, IExcelFormula arrayFormula);
        /// <summary>
        /// Add an <see cref="T:Dt.Xls.IExcelAutoFilter" /> instance to the specified sheet
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="autoFilter">An <see cref="T:Dt.Xls.IExcelAutoFilter" /> instance used to represents an active AutoFilter</param>
        void SetAutoFilter(short sheet, IExcelAutoFilter autoFilter);
        /// <summary>
        /// Add all used builtIn names to workbook
        /// </summary>
        /// <param name="builtInNames">A collection contains all used builtIn names.</param>
        /// <remarks>
        /// This method used in BIFF only
        /// </remarks>
        void SetBuiltInNameList(List<IBuiltInName> builtInNames);
        /// <summary>
        /// Set the <see cref="T:Dt.Xls.CalculationProperty" /> property for the entire workbook.
        /// </summary>
        /// <param name="calcProperty"> A <see cref="T:Dt.Xls.ICalculationProperty" /> instance used to set the workbook CalculationProperty.</param>
        /// <exception cref="T:System.ArgumentNullException">throws when the calPropoperty is null</exception>
        void SetCalculationProperty(ICalculationProperty calcProperty);
        /// <summary>
        /// A all-in-one method which will set value, cell type, format index and formula for the specified cell.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.ExcelWorksheet" /> instance</param>
        /// <param name="row">The zero based cell row index</param>
        /// <param name="column">The zero based cell column index</param>
        /// <param name="value">The value for the cell</param>
        /// <param name="type">The cell type for the cell</param>
        /// <param name="formatIndex">The format index for the cell</param>
        /// <param name="cellFormula">The formula for the cell</param>
        /// <returns>
        /// <see langword="true" /> If the cell is a valid cell. otherwise, <see langword="false" />
        /// </returns>
        /// <remarks>
        /// This method is used for performance consideration.  It will set all cell related properties at once.
        /// It will only set the value to the cell if the passed value is not null, only set the formula to the cell
        /// if the passed formula is not null or empty.
        /// </remarks>
        bool SetCell(short sheet, int row, int column, object value, CellType type, int formatIndex, IExcelFormula cellFormula);
        /// <summary>
        /// Set formula for a specified cell.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorkbook" /> instance</param>
        /// <param name="row">The zero based cell row index</param>
        /// <param name="column">The zero based cell column index</param>
        /// <param name="cellFormula">The formula used to set the value</param>
        /// <returns> 
        /// <see langword="true" /> If the cell is in a valid cell in the specified worksheet, otherwise, <see langword="false" />
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">throws if the formula is null or empty.</exception>
        bool SetCellFormula(short sheet, int row, int column, IExcelFormula cellFormula);
        /// <summary>
        /// Set hyperlink to a specified cell.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorkbook" /> instance</param>
        /// <param name="row">The zero based cell row index</param>
        /// <param name="column">The zero based cell column index</param>
        /// <param name="hyperLink">An <see cref="T:Dt.Xls.IExcelHyperLink" /> instance used to set the value</param>
        /// <returns> 
        /// <see langword="true" /> If the cell is in a valid cell in the specified worksheet, otherwise, <see langword="false" />
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">throws if the hyperLink is null</exception>
        bool SetCellHyperLink(short sheet, int row, int column, IExcelHyperLink hyperLink);
        /// <summary>
        /// Sets cell note for the specified cell.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorkbook" /> instance</param>
        /// <param name="row">The zero based cell row index</param>
        /// <param name="column">The zero based cell column index</param>
        /// <param name="stickyNote"> A flag used to indicate whether the note style is sticky note.</param>
        /// <param name="note">The note string used to set the cell note.</param>
        void SetCellNote(short sheet, int row, int column, bool stickyNote, string note);
        /// <summary>
        /// Set the format properties for a specified cell.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorkbook" /> instance</param>
        /// <param name="row">The zero based cell row index</param>
        /// <param name="column">The zero based cell column index</param>
        /// <param name="formatIndex">The zero based format index used to locate the format settings which stores in a global section.</param>
        /// <param name="type">A <see cref="T:Dt.Xls.CellType" /> instance used to set the cell type.</param>
        /// <returns> <see langword="true" /> if the cell is in a valid cell in the specified worksheet, otherwise, <see langword="false" /></returns>
        bool SetCellStyle(short sheet, int row, int column, int formatIndex, CellType type);
        /// <summary>
        /// Set value to a specified cell
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="row">The zero based cell row index</param>
        /// <param name="column">The zero based cell column index</param>
        /// <param name="value">The object used to set the value</param>
        /// <returns> 
        /// <see langword="true" /> If the cell is in a valid cell in the specified worksheet, otherwise, <see langword="false" />
        /// </returns>
        bool SetCellValue(short sheet, int row, int column, object value);
        /// <summary>
        /// Set the custom palette used in the workbook
        /// </summary>
        /// <param name="palette">The custom palette used to replace the default built-in palette</param>
        void SetColorPalette(Dictionary<int, GcColor> palette);
        /// <summary>
        /// Set serials properties to specified columns.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="columnFirst">The zero based first column index</param>
        /// <param name="columnLast">The zero based last column index</param>
        /// <param name="formatIndex">The zero based index used to locate format settings stored in a global section</param>
        /// <param name="width">The column width</param>
        /// <param name="hidden">A flag used to indicate whether the column is hidden</param>
        /// <param name="outlineLevel">A byte value used to set the column outline level.</param>
        /// <param name="collapsed">A flag used to indicate whether the column is collapsed.</param>
        void SetColumnInfo(short sheet, short columnFirst, short columnLast, short formatIndex, double width, bool hidden, byte outlineLevel, bool collapsed);
        /// <summary>
        /// Add a new <see cref="T:Dt.Xls.ExcelConditionalFormat" /> instance to the specified worksheet
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="format">A <see cref="T:Dt.Xls.IExcelConditionalFormat" /> instance will be added to the collection</param>
        /// <exception cref="T:System.ArgumentNullException"> throws if the format is null</exception>
        void SetConditionalFormatting(short sheet, IExcelConditionalFormat format);
        /// <summary>
        /// Add all custom or function names to workbook
        /// </summary>
        /// <param name="customOrFunctionNames">A collection contains all custom or function names.</param>
        /// <remarks>
        /// This method used in BIFF only
        /// </remarks>
        void SetCustomOrFunctionNameList(List<IFunction> customOrFunctionNames);
        /// <summary>
        /// Set the default column width
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="baseColumnWidth">Specifies the number of characters of the maximum digit width of the normal style's font
        /// This value does not include margin padding or extra padding for gridlines. It is only the number of characters.
        /// </param>
        /// <param name="defaultColumnWidth">Default column width measured as the number of characters of the maximum digit width of the normal style's font.</param>
        /// <remarks>
        /// If the defaultColumnWidth has not value set. it can be calculated: defaultColumnWidth = baseColumnWidth + {margin padding (2 pixels on each side)} + {gridline(1 pixel)}
        /// </remarks>
        void SetDefaultColumnWidth(short sheet, double baseColumnWidth, double? defaultColumnWidth);
        /// <summary>
        /// Set the default row height.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="defaultHeight">The value used to set the default row height.</param>
        void SetDefaultRowHeight(short sheet, double defaultHeight);
        /// <summary>
        /// Add <see cref="T:Dt.Xls.IDifferentialFormatting" /> collection to the workbook, which used to  represent formatting properties that are used in various features to modify cell formatting
        /// </summary>
        /// <param name="dxf">The <see cref="T:Dt.Xls.IDifferentialFormatting" /> records will be added to the workbook</param>
        void SetDifferentialFormattingRecord(List<IDifferentialFormatting> dxf);
        /// <summary>
        /// Set the minimum and maximum bounds of the sheet.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="rowFirst">The zero based inclusive first row index</param>
        /// <param name="rowLast">The zero based exclusive last row index</param>
        /// <param name="columnFirst">The zero based inclusive first column index</param>
        /// <param name="columnLast">The zero based exclusive last column index</param>
        void SetDimensions(short sheet, int rowFirst, int rowLast, short columnFirst, short columnLast);
        /// <summary>
        /// Set whether the specified sheet should show formula, show zeros, show gridline, show row column header, and whether its column is right to left.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="showFormula">A flag indicate whether the specified sheet should show formula</param>
        /// <param name="showZeros">A flag indicate whether the specified sheet should show zero</param>
        /// <param name="showGridLine">A flag indicate whether the specified sheet should show gridline</param>
        /// <param name="showRowColumnHeader">A flag indicate whether the specified sheet should show row column header</param>
        /// <param name="rightToLeftColumns">A flag indicate whether the column of the specified sheet is right to left</param>
        void SetDisplayElements(short sheet, bool showFormula, bool showZeros, bool showGridLine, bool showRowColumnHeader, bool rightToLeftColumns);
        /// <summary>
        /// Add an <see cref="T:Dt.Xls.IExtendedFormat" /> instance to workbook
        /// </summary>
        /// <param name="format">An <see cref="T:Dt.Xls.IExtendedFormat" /> instance which will be added to the workbook extended format collection</param>
        void SetExcelCellFormat(IExtendedFormat format);
        /// <summary>
        /// Set the default cell format of the workbook
        /// </summary>
        /// <param name="format">An <see cref="T:Dt.Xls.IExtendedFormat" /> instance defines the default cell format of the workbook</param>
        void SetExcelDefaultCellFormat(IExtendedFormat format);
        /// <summary>
        /// Add built-in or user defined style to the workbook
        /// </summary>
        /// <param name="style">A <see cref="T:Dt.Xls.IExcelStyle" /> instance which will be added to the workbook ExcelStyle collection</param>
        void SetExcelStyle(IExcelStyle style);
        /// <summary>
        /// Set the <see cref="T:Dt.Xls.IExcelWorkbookPropery" /> for the entire Workbook. 
        /// </summary>
        /// <param name="workbookPropety">The item used to set the <see cref="T:Dt.Xls.IExcelWorkbookPropery" />  property for Workbook</param>
        /// <exception cref="T:System.ArgumentNullException">throws when the passed argument is null.</exception>
        void SetExcelWorkbookProperty(IExcelWorkbookPropery workbookPropety);
        /// <summary>
        /// Add the external referenced workbook info to the workbook
        /// </summary>
        /// <param name="externWorkbookInfo">An <see cref="T:Dt.Xls.IExternalWorkbookInfo" /> used to represents an external workbook reference</param>
        void SetExternalReferencedWorkbookInfo(IExternalWorkbookInfo externWorkbookInfo);
        /// <summary>
        /// Set the gridline color for the specified worksheet.
        /// </summary>
        /// <param name="sheet">the zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="color">the value used to set the gridline color for the specified sheet.</param>
        void SetGridlineColor(short sheet, IExcelColor color);
        /// <summary>
        /// Set the merged cell information
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="rowStart">The zero based inclusive row index indicate the top left cell row index</param>
        /// <param name="rowEnd">The zero based inclusive row index indicate the bottom right cell row index</param>
        /// <param name="columnStart">The zero based inclusive column index indicate the top left cell row index</param>
        /// <param name="columnEnd">The zero based inclusive column index indicate the bottom right cell row index</param>
        /// <returns>
        /// <see langword="true" /> If passed cell range is a valid cell range, otherwise, <see langword="false" />
        /// </returns>
        bool SetMergeCells(short sheet, int rowStart, int rowEnd, int columnStart, int columnEnd);
        /// <summary>
        /// Sets the named cell range
        /// </summary>
        /// <param name="namedCellRange">
        /// An instance of <see cref="T:Dt.Xls.IName" /> used to represents a cell range in Excel
        /// </param>
        void SetNamedCellRange(IName namedCellRange);
        /// <summary>
        /// Set the summary row/column directions
        /// </summary>
        /// <param name="sheet">The zero based sheet index.</param>
        /// <param name="summaryColumnsRightToDetail">A flag indicate whether the summary columns to right of details, true if it's right to detail, otherwise, false</param>
        /// <param name="summaryRowsBelowDetail">A flag indicate whether the summary rows below details, true if it below detail, otherwise, false</param>
        void SetOutlineDirection(int sheet, bool summaryColumnsRightToDetail, bool summaryRowsBelowDetail);
        /// <summary>
        /// Set the number and position of unfrozen panes in a window
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="horizontalPosition">Set the value for the horizontal position of the split, 0 if none</param>
        /// <param name="verticalPosition">Set the value for the vertical position of the split, 0 if none</param>
        /// <param name="topVisibleRow">Set the count of top visible rows  in the bottom pane</param>
        /// <param name="leftmostVisibleColumn">Set the count of leftmost visible columns in the right pane</param>
        /// <param name="paneIndex">The active pane number</param>
        /// <param name="isPanesFrozen">A flag indicate whether the pane is frozen</param>
        void SetPane(short sheet, int horizontalPosition, int verticalPosition, int topVisibleRow, int leftmostVisibleColumn, int paneIndex, bool isPanesFrozen);
        /// <summary>
        /// Set the print area of the specified zero-based sheet
        /// </summary>
        /// <param name="sheet">The sheet index</param>
        /// <param name="printArea">The print area</param>
        void SetPrintArea(int sheet, string printArea);
        /// <summary>
        /// Set the print page margins for specified <see cref="T:Dt.Xls.IExcelWorkbook" /> instance.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorkbook" /> instance</param>
        /// <param name="printOption">A <see cref="T:Dt.Xls.IExcelPrintOptions" /> instance used to set the property <see cref="T:Dt.Xls.ExcelPrintOptions" /> for the specified <see cref="T:Dt.Xls.ExcelWorksheet" /> instance.</param>
        /// <exception cref="T:System.ArgumentNullException">throws when the printOption is null</exception>
        void SetPrintOption(short sheet, IExcelPrintOptions printOption);
        /// <summary>
        /// Set the print page margins for specified <see cref="T:Dt.Xls.IExcelWorkbook" /> instance.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.ExcelWorksheet" /> instance</param>
        /// <param name="printMargin">A <see cref="T:Dt.Xls.IExcelPrintPageMargin" /> instance used to set the property <see cref="T:Dt.Xls.ExcelPrintPageMargin" /> for the specified <see cref="T:Dt.Xls.ExcelWorksheet" /> instance.</param>
        /// <exception cref="T:System.ArgumentNullException">throws when the printMargin is null</exception>
        void SetPrintPageMargin(short sheet, IExcelPrintPageMargin printMargin);
        /// <summary>
        /// Set the print page margins for specified <see cref="T:Dt.Xls.IExcelWorkbook" /> instance.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorkbook" /> instance</param>
        /// <param name="pageSetting">A <see cref="T:Dt.Xls.IExcelPrintPageSetting" /> instance used to set the property <see cref="T:Dt.Xls.ExcelPrintPageSetting" /> for the specified <see cref="T:Dt.Xls.ExcelWorksheet" /> instance.</param>
        /// <exception cref="T:System.ArgumentNullException">throws when the pageSetting is null</exception>
        void SetPrintPageSetting(short sheet, IExcelPrintPageSetting pageSetting);
        /// <summary>
        /// Set the print title of the specified zero-based sheet
        /// </summary>
        /// <param name="sheet">The sheet index</param>
        /// <param name="title">The print title</param>
        void SetPrintTitles(int sheet, string title);
        /// <summary>
        /// Set the protection state for a sheet or workbook
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance, if the passed value is -1, 
        /// it will set the workbook protection state</param>
        /// <param name="isProtect">A flag used to indicate the protection state</param>
        void SetProtect(short sheet, bool isProtect);
        /// <summary>
        /// Set the size of row and column gutters which is measured in screen units.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="rowGutter">The value for the row gutter</param>
        /// <param name="columnGutter">The value for the column gutter</param>
        /// <param name="rowMaxOutlineLevel">The value for the max outline level of row</param>
        /// <param name="columnMaxOutlineLevel">The value for the max outline level of column</param>
        void SetRowColumnGutters(short sheet, short rowGutter, short columnGutter, short rowMaxOutlineLevel, short columnMaxOutlineLevel);
        /// <summary>
        /// Set the row information at the specified index.
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="row">The zero base row index used to get the row</param>
        /// <param name="firstDefinedColumn">The inclusive first defined column index in the row</param>
        /// <param name="lastDefinedColumn">The exclusive last defined column index in the row</param>
        /// <param name="formatIndex">The zero based index used to locate the format information which stores in a global section</param>
        /// <param name="height">The height of the row </param>
        /// <param name="outlineLevel">The outline level of the row </param>
        /// <param name="collapsed">A flag indicate whether the row is collapsed</param>
        /// <param name="zeroHeight">A flag indicate whether the height of the row is 0</param>
        /// <param name="unSynced">A flag indicate whether the row is unsynced</param>
        /// <param name="ghostDirty">A flag indicate whether the row should use the specified format index</param>
        /// <returns>
        /// <see langword="true" /> If the row is a valid row, otherwise, <see langword="false" />
        /// </returns>
        bool SetRowInfo(short sheet, int row, int firstDefinedColumn, int lastDefinedColumn, short formatIndex, double height, byte outlineLevel, bool collapsed, bool zeroHeight, bool unSynced, bool ghostDirty);
        /// <summary>
        /// Set the horizontal and vertical scroll bar policies for workbook
        /// </summary>
        /// <param name="showHorizontalScrollbarAsNeeded">A flag show whether show horizontal scrollbar as needed</param>
        /// <param name="showVerticalScrollBarAsNeeded">A flag show whether show vertical scrollbar as needed</param>
        void SetScroll(bool showHorizontalScrollbarAsNeeded, bool showVerticalScrollBarAsNeeded);
        /// <summary>
        /// Set the cell selections for the specified worksheet
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="paneType">The type of the pane</param>
        /// <param name="rowActive">The zero base active row index</param>
        /// <param name="columnActive">The zero base active column index</param>
        /// <param name="selectionCount">The count of the selections in the specified sheet</param>
        /// <param name="rowFirst">The list contains all selections' top left cell row index </param>
        /// <param name="rowLast">The list contains all selections' bottom right cell row index</param>
        /// <param name="colFirst">The list contains all selections' top left cell column index</param>
        /// <param name="colLast">The list contains all selections' bottom right cell column index</param>
        void SetSelection(short sheet, PaneType paneType, int rowActive, int columnActive, int selectionCount, List<int> rowFirst, List<int> rowLast, List<int> colFirst, List<int> colLast);
        /// <summary>
        /// Set the tab information of the workbook
        /// </summary>
        /// <param name="showTabs">A flag indicate whether show workbook tabs or not</param>
        /// <param name="selectedTabIndex">Zero based index of the selected workbook tab</param>
        /// <param name="firstDisplayedTabIndex">Zero based index of the first displayed workbook tab</param>
        /// <param name="selectedTabCount">Number of workbook tabs that are selected</param>
        /// <param name="tabRatio">Ratio of the width of the workbook tabs to the width of the horizontal scroll bar.</param>
        void SetTabs(bool showTabs, int selectedTabIndex, int firstDisplayedTabIndex, int selectedTabCount, int tabRatio);
        /// <summary>
        /// Set the theme used in the workbook
        /// </summary>
        /// <param name="theme">the theme used in the workbook</param>
        void SetTheme(IExcelTheme theme);
        /// <summary>
        /// Set the first visible row and column information in the window
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="topRow">Top row visible in the window</param>
        /// <param name="leftColumn">Leftmost column visible in the window</param>
        void SetTopLeft(short sheet, int topRow, int leftColumn);
        /// <summary>
        /// Add an <see cref="T:Dt.Xls.IExcelDataValidation" /> instance for a range of cells
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="dataValidation"> A data validation criteria for a range of cells</param>
        /// <exception cref="T:System.ArgumentNullException">throw if the passed data validation value is null</exception>
        void SetValidationData(short sheet, IExcelDataValidation dataValidation);
        /// <summary>
        /// Set the location of the window, whether it's hidden or currently displayed as an icon
        /// </summary>
        /// <param name="rect">An <see cref="T:Dt.Xls.IExcelRect" /> instance used to defined the window properties.</param>
        /// <param name="hidden">A flag indicate whether the window is hidden</param>
        /// <param name="iconic">A flag indicate whether the window is currently display as an icon.</param>
        void SetWindow(IExcelRect rect, bool hidden, bool iconic);
        /// <summary>
        /// Set the zoom magnification for a specified sheet
        /// </summary>
        /// <param name="sheet">The zero based sheet index used to locate the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        /// <param name="zoom">The zoom magnification</param>
        void SetZoom(short sheet, float zoom);
    }
}

