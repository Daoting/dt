#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Defines generalized properties and methods that value or class implements to represents excel worksheet.
    /// </summary>
    public interface IExcelWorksheet
    {
        /// <summary>
        /// Gets the cell at the specified row and column.
        /// </summary>
        /// <param name="row">The zero-based row index of the cell</param>
        /// <param name="column">The zero-based column index of the cell.</param>
        /// <param name="create"> If set to <see langword="true" />. It will create the cell if
        /// the cell is null at the specified row and column when the row and column is in the valid range</param>
        /// <returns>An <see cref="T:Dt.Xls.IExcelCell" /> instance represents the cell at he specified location</returns>
        IExcelCell GetCell(int row, int column, bool create = true);
        /// <summary>
        /// Gets the column at the specified column index
        /// </summary>
        /// <param name="index">The zero-based index of the column.</param>
        /// <param name="create"> If set to <see langword="true" />. It will create the column if
        /// the column is null at the specified index when the index is in the valid range</param>
        /// <returns>An <see cref="T:Dt.Xls.IExcelColumn" /> instance represent the column at the specified index</returns>
        IExcelColumn GetColumn(int index, bool create = true);
        /// <summary>
        /// Gets the all data validation setting in the worksheet.
        /// </summary>
        /// <returns>A collection of <see cref="T:Dt.Xls.IExcelDataValidation" /> represents all data validation settings in the worksheet</returns>
        List<IExcelDataValidation> GetDataValidations();
        /// <summary>
        /// Gets the non empty cells.
        /// </summary>
        /// <param name="row">The zero-based row index</param>
        /// <returns>A collection of <see cref="T:Dt.Xls.IExcelCell" /> instances represents the non-empty cells in the specified row</returns>
        List<IExcelCell> GetNonEmptyCells(int row);
        /// <summary>
        /// Gets the non-empty columns.
        /// </summary>
        /// <returns> A collection of <see cref="T:Dt.Xls.IExcelColumn" /> instances represents the non-empty columns</returns>
        List<IExcelColumn> GetNonEmptyColumns();
        /// <summary>
        /// Gets the non-empty rows.
        /// </summary>
        /// <returns> A collection of <see cref="T:Dt.Xls.IExcelRow" /> instances represents the non-empty rows</returns>
        List<IExcelRow> GetNonEmptyRows();
        /// <summary>
        /// Gets the row at the specified column index
        /// </summary>
        /// <param name="index">The zero-based index of the row.</param>
        /// <param name="create"> If set to <see langword="true" />. It will create the column if
        /// the row is null at the specified index when the index is in the valid range</param>
        /// <returns>An <see cref="T:Dt.Xls.IExcelRow" /> instance represent the column at the specified index</returns>
        IExcelRow GetRow(int index, bool create = true);
        /// <summary>
        /// Returns all charts defined in the current worksheet.
        /// </summary>
        List<IExcelChart> GetSheetCharts();
        /// <summary>
        /// Returns all images defined in the current worksheet.
        /// </summary>
        /// <returns></returns>
        List<IExcelImage> GetSheetImages();
        /// <summary>
        /// Returns all tables defined in the current worksheet.
        /// </summary>
        /// <returns>A collection a sheet tables.</returns>
        List<IExcelTable> GetSheetTables();
        /// <summary>
        /// Gets the range of cells that are spanned at a specified cell in this sheet.
        /// </summary>
        /// <param name="row">The row index </param>
        /// <param name="column">The column index</param>
        /// <returns>
        /// Returns a <see cref="T:Dt.Xls.IRange" /> object containing the span information, or null if no span exists.
        /// </returns>
        IRange GetSpanCell(int row, int column);
        /// <summary>
        /// Get the location of the top left visible cell in the current sheet
        /// </summary>
        /// <param name="top">The zero-based row index of the top left cell</param>
        /// <param name="left">The zero-based column index of the top left cell</param>
        void GetTopLeft(ref int top, ref int left);
        /// <summary>
        /// Sets the value of the cell.
        /// </summary>
        /// <param name="row">The zero-based row index used to locate the cell</param>
        /// <param name="column">The zero-base column index used to locate the cell</param>
        /// <returns>The value of the cell</returns>
        object GetValue(int row, int column);
        /// <summary>
        /// Initialize the column at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the column</param>
        /// <param name="column">An <see cref="T:Dt.Xls.IExcelColumn" /> used to initialize the column at the specified index</param>
        void SetColumn(int index, IExcelColumn column);
        /// <summary>
        /// Sets the data validation used in the worksheet
        /// </summary>
        /// <param name="dv">An <see cref="T:Dt.Xls.IExcelDataValidation" /> instance represent a data validation settings</param>
        void SetDataValidations(IExcelDataValidation dv);
        /// <summary>
        /// Add a chart to the current worksheet.
        /// </summary>
        /// <param name="chart">An <see cref="T:Dt.Xls.Chart.IExcelChart" /> instance describe a chart settings.</param>
        void SetSheetChart(IExcelChart chart);
        /// <summary>
        /// Add a image to the current worksheet.
        /// </summary>
        /// <param name="image">The image.</param>
        void SetSheetImage(IExcelImage image);
        /// <summary>
        /// Add a sheet table to the current worksheet.
        /// </summary>
        /// <param name="table">An <see cref="T:Dt.Xls.IExcelTable" /> instance describe a table settings.</param>
        void SetSheetTable(IExcelTable table);
        /// <summary>
        /// Set the location of the top left visible cell.
        /// </summary>
        /// <param name="top">The zero-based row index of the top left cell</param>
        /// <param name="left">The zero-based column index of the top left cell</param>
        void SetTopLeft(int top, int left);
        /// <summary>
        /// Sets the value of the cell.
        /// </summary>
        /// <param name="row">The zero-based row index used to locate the cell</param>
        /// <param name="column">The zero-base column index used to locate the cell</param>
        /// <param name="value">The value of the cell</param>
        void SetValue(int row, int column, object value);

        /// <summary>
        /// Gets or sets the index of the active column.
        /// </summary>
        /// <value>The index of the active column.</value>
        int ActiveColumnIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the active row.
        /// </summary>
        /// <value>The index of the active row.</value>
        int ActiveRowIndex { get; set; }

        /// <summary>
        /// Gets or sets the auto filter used in the worksheet.
        /// </summary>
        /// <value>An <see cref="T:Dt.Xls.IExcelAutoFilter" /> instance used to repents auto filter settings used in the worksheet.</value>
        IExcelAutoFilter AutoFilter { get; set; }

        /// <summary>
        /// Gets or sets the column count of the sheet.
        /// </summary>
        /// <value>The column count of the sheet.</value>
        int ColumnCount { get; set; }

        /// <summary>
        /// Gets or sets the column gutter.
        /// </summary>
        /// <value>The column gutter.</value>
        short ColumnGutter { get; set; }

        /// <summary>
        /// Gets or sets the column max outline level.
        /// </summary>
        /// <value>The column max outline level.</value>
        short ColumnMaxOutlineLevel { get; set; }

        /// <summary>
        /// Gets or sets the conditional formats used in the worksheet.
        /// </summary>
        /// <value> A collection of <see cref="T:Dt.Xls.IExcelConditionalFormat" /> represents conditional formats settings used in the work sheet.</value>
        List<IExcelConditionalFormat> ConditionalFormat { get; set; }

        /// <summary>
        /// Gets or sets the default width of the column.
        /// </summary>
        /// <value>The default width of the column.</value>
        double DefaultColumnWidth { get; set; }

        /// <summary>
        /// Gets or sets the default height of the row.
        /// </summary>
        /// <value>The default height of the row.</value>
        double DefaultRowHeight { get; set; }

        /// <summary>
        /// Gets or sets the default viewport information of the worksheet.
        /// </summary>
        /// <value> A <see cref="T:Dt.Xls.ExcelWorksheet.ExcelViewportInfo" /> instance represents the worksheet default viewpoint information</value>
        ExcelWorksheet.ExcelViewportInfo DefaultViewport { get; set; }

        /// <summary>
        /// Gets or sets the extended formats for the worksheet.
        /// </summary>
        /// <value>The extended formats</value>
        Dictionary<int, IExtendedFormat> ExtendedFormats { get; set; }

        /// <summary>
        /// Gets or sets the frozen column count.
        /// </summary>
        /// <value>The frozen column count.</value>
        int FrozenColumnCount { get; set; }

        /// <summary>
        /// Gets or sets the frozen row count.
        /// </summary>
        /// <value>The frozen row count.</value>
        int FrozenRowCount { get; set; }

        /// <summary>
        /// Gets or sets the color of the grid line.
        /// </summary>
        /// <value>The color of the grid line.</value>
        IExcelColor GridLineColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is locked; otherwise, <see langword="false" />.
        /// </value>
        bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is visible; otherwise, <see langword="false" />.
        /// </value>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets the <see cref="T:Dt.Xls.IExcelCell" /> with the specified location
        /// </summary>
        /// <value>An <see cref="T:Dt.Xls.IExcelCell" /> instance represents the cell at he specified location</value>
        IExcelCell this[int row, int column] { get; }

        /// <summary>
        /// Gets or sets the merged cells information.
        /// </summary>
        /// <value>A collection of <see cref="T:Dt.Xls.IRange" /> represents the merged cell information</value>
        List<IRange> MergedCells { get; set; }

        /// <summary>
        /// Gets or sets the sheet name.
        /// </summary>
        /// <value>The name of the sheet</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the named cell ranges information
        /// </summary>
        /// <value>A dictionary represents the named cell ranges information.</value>
        Dictionary<string, IName> NamedCellRanges { get; set; }

        /// <summary>
        /// Gets or sets the print area information of the worksheet.
        /// </summary>
        /// <value>The print area information</value>
        string PrintArea { get; set; }

        /// <summary>
        /// Gets or sets the print settings of the sheet.
        /// </summary>
        /// <value> An <see cref="T:Dt.Xls.IExcelPrintSettings" /> represents the print settings</value>
        IExcelPrintSettings PrintSettings { get; set; }

        /// <summary>
        /// Gets or sets the print title information of the worksheet.
        /// </summary>
        /// <value>The print title information</value>
        string PrintTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sheet is in 'right to left' display mode
        /// </summary>
        bool RightToLeftColumns { get; set; }

        /// <summary>
        /// Gets or sets the row count of the sheet.
        /// </summary>
        /// <value>The row count of the sheet.</value>
        int RowCount { get; set; }

        /// <summary>
        /// Gets or sets the row gutter.
        /// </summary>
        /// <value>The row gutter.</value>
        short RowGutter { get; set; }

        /// <summary>
        /// Gets or sets the row max outline level.
        /// </summary>
        /// <value>The row max outline level.</value>
        short RowMaxOutlineLevel { get; set; }

        /// <summary>
        /// Gets the selection information.
        /// </summary>
        /// <value>A collection of <see cref="T:Dt.Xls.ISelectionRange" /> represents the selection information</value>
        List<ISelectionRange> Selections { get; }

        /// <summary>
        /// Gets or sets the color of the sheet tab.
        /// </summary>
        /// <value>
        /// The color of the sheet tab.
        /// </value>
        IExcelColor SheetTabColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this sheet should display formulas.
        /// </summary>
        bool ShowFormulas { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show gridlines or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show gridline; otherwise, <see langword="false" />.
        /// </value>
        bool ShowGridLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show headers.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show headers.; otherwise, <see langword="false" />.
        /// </value>
        bool ShowHeaders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window should show 0 (zero) in cells containing zero value.
        /// </summary>
        bool ShowZeros { get; set; }

        /// <summary>
        /// Gets or sets the groups of sparklines on the sheet. 
        /// </summary>
        /// <remarks>Sparkline is a new feature supported since Excel 2010 </remarks>
        List<IExcelSparklineGroup> SparklineGroups { get; set; }

        /// <summary>
        /// Get or set the direction of the summary columns. 
        /// </summary>
        /// <returns>
        /// True if the summary columns to right of detail, otherwise; false.
        /// </returns>
        bool SummaryColumnsToRightOfDetail { get; set; }

        /// <summary>
        /// Get or set the direction of the summary rows. 
        /// </summary>
        /// <returns>
        /// True if the summary rows below detail, otherwise; false.
        /// </returns>
        bool SummaryRowsBelowDetail { get; set; }

        /// <summary>
        /// Gets or sets the zoom factor of the sheet.
        /// </summary>
        /// <value>The zoom factor of the sheet</value>
        float Zoom { get; set; }
    }
}

