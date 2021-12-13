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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents a worksheet 
    /// </summary>
    public class ExcelWorksheet : IExcelWorksheet, INameSupport
    {
        private List<IExcelChart> _charts;
        private int _columnCount;
        private Vector<IExcelColumn> _columns1;
        private List<IExcelConditionalFormat> _conditionalFormatList;
        private Vector<Vector<IExcelCell>> _dataTable1;
        private List<IExcelDataValidation> _dataValidations;
        private double _defaultColumnWidth;
        private double _defaultRowHeight;
        private List<IExcelImage> _images;
        private int _left;
        private List<IRange> _mergedCells;
        private Dictionary<string, IName> _namedCellRanges;
        private IExcelPrintSettings _printSettings;
        private int _rowCount;
        private Vector<IExcelRow> _rows1;
        private List<ISelectionRange> _selections;
        private bool _showGridLines;
        private bool _showHeaders;
        private bool _showZeros;
        private bool _summaryColumnsToRightOfDetail;
        private bool _summaryRowsBelowDetail;
        private List<IExcelTable> _tables;
        private int _top;
        private bool _visible;
        private float _zoom;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelWorksheet" /> class.
        /// </summary>
        /// <param name="rowCount">The row count of the item.
        /// The default maximum row count is <b>65536</b>.</param>
        /// <param name="columnCount">The column count of the item.
        /// The default maximum column count is <b>265</b>.</param>
        public ExcelWorksheet(int rowCount = 0x10000, int columnCount = 0x100)
        {
            this._columns1 = new Vector<IExcelColumn>();
            this._rows1 = new Vector<IExcelRow>();
            this._dataTable1 = new Vector<Vector<IExcelCell>>();
            this._showHeaders = true;
            this._showGridLines = true;
            this._visible = true;
            this._zoom = 1f;
            this._defaultColumnWidth = 8.0;
            this._defaultRowHeight = 15.0;
            this._showZeros = true;
            this._summaryRowsBelowDetail = true;
            this._summaryColumnsToRightOfDetail = true;
            this.RowCount = rowCount;
            this.ColumnCount = columnCount;
            this.DefaultViewport = new ExcelViewportInfo(this);
        }

        /// <summary>
        /// Initialize a new <see cref="T:Dt.Xls.ExcelWorksheet" />
        /// which <see cref="P:Dt.Xls.ExcelWorksheet.RowCount" /> is specified by <paramref name="rowCount" />
        /// and <see cref="P:Dt.Xls.ExcelWorksheet.ColumnCount" /> is specified by <paramref name="columnCount" />.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="rowCount">The row count.
        /// The default maximum row count is <b>65536</b>.</param>
        /// <param name="columnCount">The column count.
        /// The default maximum column count is <b>256</b>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="rowCount" /> is less than Zero or The <paramref name="columnCount" /> is less that Zero.
        /// </exception>
        public ExcelWorksheet(string name, int rowCount = 0x10000, int columnCount = 0x100)
        {
            this._columns1 = new Vector<IExcelColumn>();
            this._rows1 = new Vector<IExcelRow>();
            this._dataTable1 = new Vector<Vector<IExcelCell>>();
            this._showHeaders = true;
            this._showGridLines = true;
            this._visible = true;
            this._zoom = 1f;
            this._defaultColumnWidth = 8.0;
            this._defaultRowHeight = 15.0;
            this._showZeros = true;
            this._summaryRowsBelowDetail = true;
            this._summaryColumnsToRightOfDetail = true;
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(ResourceHelper.GetResourceString("worksheetNameError"));
            }
            this.RowCount = rowCount;
            this.ColumnCount = columnCount;
            this.Name = name;
            this.DefaultViewport = new ExcelViewportInfo(this);
        }

        /// <summary>
        /// Adds the chart.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <param name="chartName">Name of the chart.</param>
        /// <param name="fromRow">From row.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toRow">To row.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        /// <exception cref="T:System.NotSupportedException"></exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">fromRow</exception>
        public IExcelChart AddChart(ExcelChartType chartType, string chartName, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            if (string.IsNullOrWhiteSpace(chartName))
            {
                throw new NotSupportedException(ResourceHelper.GetResourceString("EmptyChartNameError"));
            }
            using (List<IExcelChart>.Enumerator enumerator = this.ExcelCharts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Name == chartName)
                    {
                        throw new NotSupportedException(ResourceHelper.GetResourceString("CurrentWorksheetHasSameChartError"));
                    }
                }
            }
            if (fromRow < 0)
            {
                throw new ArgumentOutOfRangeException("fromRow");
            }
            if (toRow >= this.RowCount)
            {
                throw new ArgumentOutOfRangeException("toRow");
            }
            if (fromColumn < 0)
            {
                throw new ArgumentOutOfRangeException("fromColumn");
            }
            if (toColumn >= this.ColumnCount)
            {
                throw new ArgumentOutOfRangeException("toColumn");
            }
            IExcelChart chart = ChartFactory.GetChart(chartType);
            chart.Anchor = new TwoCellAnchor(fromRow, fromColumn, toRow, toColumn);
            chart.Name = chartName;
            this.ExcelCharts.Add(chart);
            return chart;
        }

        /// <summary>
        /// Gets the cell at the specified row and column.
        /// </summary>
        /// <param name="row">The zero-based row index of the cell</param>
        /// <param name="column">The zero-based column index of the cell.</param>
        /// <param name="create">If set to <see langword="true" />. It will create the cell if
        /// the cell is null at the specified row and column when the row and column is in the valid range</param>
        /// <returns>
        /// An <see cref="T:Dt.Xls.IExcelCell" /> instance represents the cell at he specified location
        /// </returns>
        public IExcelCell GetCell(int row, int column, bool create = true)
        {
            Vector<IExcelCell> vector = this._dataTable1[row];
            if (vector != null)
            {
                IExcelCell cell = vector[column];
                if (cell == null)
                {
                    if ((!create || (column < 0)) || (column >= this._columnCount))
                    {
                        return null;
                    }
                    cell = new ExcelCell(this, row, column);
                    vector[column] = cell;
                    if (this.ActualColumnCount <= column)
                    {
                        this.ActualColumnCount = column + 1;
                    }
                    if (this.ActualRowCount <= row)
                    {
                        this.ActualRowCount = row + 1;
                    }
                }
                return cell;
            }
            if (((!create || (column < 0)) || ((column >= this._columnCount) || (row < 0))) || (row >= this._rowCount))
            {
                return null;
            }
            Vector<IExcelCell> vector2 = new Vector<IExcelCell>();
            ExcelCell cell2 = new ExcelCell(this, row, column);
            vector2[column] = cell2;
            this._dataTable1[row] = vector2;
            if (this._rows1[row] == null)
            {
                ExcelRow row2 = new ExcelRow(this) {
                    Index = row
                };
                this._rows1[row] = row2;
            }
            if (this.ActualColumnCount <= column)
            {
                this.ActualColumnCount = column + 1;
            }
            if (this.ActualRowCount <= row)
            {
                this.ActualRowCount = row + 1;
            }
            return cell2;
        }

        /// <summary>
        /// Gets the column at the specified column index
        /// </summary>
        /// <param name="index">The zero-based index of the column.</param>
        /// <param name="create">If set to <see langword="true" />. It will create the column if
        /// the column is null at the specified index when the index is in the valid range</param>
        /// <returns>
        /// An <see cref="T:Dt.Xls.IExcelColumn" /> instance represent the column at the specified index
        /// </returns>
        public IExcelColumn GetColumn(int index, bool create = true)
        {
            IExcelColumn column = this._columns1[index];
            if (column != null)
            {
                return column;
            }
            if (((index >= 0) && (index <= this._columnCount)) && create)
            {
                ExcelColumn column2 = new ExcelColumn(this) {
                    Index = index,
                    Width = this._defaultColumnWidth
                };
                column2.SetFormatId(-1);
                column2.Visible = true;
                this._columns1[index] = column2;
                return column2;
            }
            return null;
        }

        /// <summary>
        /// Gets the all data validation setting in the worksheet.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExcelDataValidation" /> represents all data validation settings in the worksheet
        /// </returns>
        public List<IExcelDataValidation> GetDataValidations()
        {
            return this._dataValidations;
        }

        /// <summary>
        /// Gets the non empty cells.
        /// </summary>
        /// <param name="row">The zero-based row index</param>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExcelCell" /> instances represents the non-empty cells in the specified row
        /// </returns>
        public List<IExcelCell> GetNonEmptyCells(int row)
        {
            List<IExcelCell> list = new List<IExcelCell>();
            Vector<IExcelCell> vector = this._dataTable1[row];
            if (vector != null)
            {
                int count = vector.Count;
                for (int i = 0; i < count; i++)
                {
                    IExcelCell item = vector.GetItem(i);
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the non-empty columns.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExcelColumn" /> instances represents the non-empty columns
        /// </returns>
        public List<IExcelColumn> GetNonEmptyColumns()
        {
            List<IExcelColumn> list = new List<IExcelColumn>();
            for (int i = 0; i < this._columns1.Count; i++)
            {
                if (this._columns1[i] != null)
                {
                    list.Add(this._columns1[i]);
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the non-empty rows.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:Dt.Xls.IExcelRow" /> instances represents the non-empty rows
        /// </returns>
        public List<IExcelRow> GetNonEmptyRows()
        {
            List<IExcelRow> list = new List<IExcelRow>();
            int num = Math.Max(this._rows1.Capacity, this._dataTable1.Capacity);
            for (int i = 0; i < num; i++)
            {
                if (this._rows1[i] != null)
                {
                    list.Add(this._rows1[i]);
                }
                else if (this._dataTable1[i] != null)
                {
                    ExcelRow row = new ExcelRow(this) {
                        Index = i
                    };
                    list.Add(row);
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the row at the specified column index
        /// </summary>
        /// <param name="index">The zero-based index of the row.</param>
        /// <param name="create">If set to <see langword="true" />. It will create the column if
        /// the row is null at the specified index when the index is in the valid range</param>
        /// <returns>
        /// An <see cref="T:Dt.Xls.IExcelRow" /> instance represent the column at the specified index
        /// </returns>
        public IExcelRow GetRow(int index, bool create = true)
        {
            IExcelRow row = this._rows1[index];
            if (row != null)
            {
                return row;
            }
            if (((index >= 0) && (index < this._rowCount)) && create)
            {
                ExcelRow row2 = new ExcelRow(this) {
                    Index = index,
                    Height = this._defaultRowHeight
                };
                row = row2;
                this._rows1[index] = row;
                return row;
            }
            return null;
        }

        /// <summary>
        /// Returns all charts defined in the current worksheet.
        /// </summary>
        /// <returns></returns>
        public List<IExcelChart> GetSheetCharts()
        {
            return this.ExcelCharts;
        }

        /// <summary>
        /// Returns all images defined in the current worksheet.
        /// </summary>
        /// <returns></returns>
        public List<IExcelImage> GetSheetImages()
        {
            return this.ExcelImages;
        }

        /// <summary>
        /// Returns all tables defined in the current worksheet.
        /// </summary>
        /// <returns>A collection a sheet tables.</returns>
        public List<IExcelTable> GetSheetTables()
        {
            return this._tables;
        }

        /// <summary>
        /// Gets the range of cells that are spanned at a specified cell in this sheet.
        /// </summary>
        /// <param name="row">The row index </param>
        /// <param name="column">The column index</param>
        /// <returns>
        /// Returns a <see cref="T:Dt.Xls.IRange" /> object containing the span information, or null if no span exists.
        /// </returns>
        public IRange GetSpanCell(int row, int column)
        {
            if (this._mergedCells != null)
            {
                foreach (IRange range in this._mergedCells)
                {
                    if (this.InRange(row, range.Row, range.Row + range.RowSpan) && this.InRange(column, range.Column, range.Column + range.ColumnSpan))
                    {
                        return range;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the location of the top left visible cell in the current sheet
        /// </summary>
        /// <param name="top">The zero-based row index of the top left cell</param>
        /// <param name="left">The zero-based column index of the top left cell</param>
        public void GetTopLeft(ref int top, ref int left)
        {
            top = this._top;
            left = this._left;
        }

        /// <summary>
        /// Sets the value of the cell.
        /// </summary>
        /// <param name="row">The zero-based row index used to locate the cell</param>
        /// <param name="column">The zero-base column index used to locate the cell</param>
        /// <returns>The value of the cell</returns>
        public object GetValue(int row, int column)
        {
            IExcelCell cell = this.GetCell(row, column, true);
            if (cell != null)
            {
                return cell.Value;
            }
            if ((row < 0) || (row >= this.RowCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            if ((column < 0) || (column >= this.ColumnCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            return null;
        }

        private bool InRange(int value, int start, int endExclusive)
        {
            return ((value >= start) && (value < endExclusive));
        }

        /// <summary>
        /// Initialize the column at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the column</param>
        /// <param name="column">An <see cref="T:Dt.Xls.IExcelColumn" /> used to initialize the column at the specified index</param>
        public void SetColumn(int index, IExcelColumn column)
        {
            this._columns1[index] = column;
        }

        /// <summary>
        /// Sets the data validation used in the worksheet
        /// </summary>
        /// <param name="dv">An <see cref="T:Dt.Xls.IExcelDataValidation" /> instance represent a data validation settings</param>
        public void SetDataValidations(IExcelDataValidation dv)
        {
            if (this._dataValidations == null)
            {
                this._dataValidations = new List<IExcelDataValidation>();
            }
            this._dataValidations.Add(dv);
        }

        /// <summary>
        /// Add a chart to the current worksheet.
        /// </summary>
        /// <param name="chart">An <see cref="T:Dt.Xls.Chart.IExcelChart" /> instance describe a chart settings.</param>
        /// <exception cref="T:System.NotSupportedException"></exception>
        public void SetSheetChart(IExcelChart chart)
        {
            if (chart == null)
            {
                throw new ArgumentNullException("chart");
            }
            if (string.IsNullOrWhiteSpace(chart.Name))
            {
                throw new NotSupportedException(ResourceHelper.GetResourceString("EmptyChartNameError"));
            }
            using (List<IExcelChart>.Enumerator enumerator = this.ExcelCharts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Name == chart.Name)
                    {
                        throw new NotSupportedException(ResourceHelper.GetResourceString("CurrentWorksheetHasSameChartError"));
                    }
                }
            }
            if (chart != null)
            {
                this.ExcelCharts.Add(chart);
            }
        }

        /// <summary>
        /// Add a image to the current worksheet.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <exception cref="T:System.ArgumentNullException">image</exception>
        /// <exception cref="T:System.NotSupportedException"></exception>
        public void SetSheetImage(IExcelImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            if (string.IsNullOrWhiteSpace(image.Name))
            {
                throw new NotSupportedException(ResourceHelper.GetResourceString("EmptyImageNameError"));
            }
            using (List<IExcelChart>.Enumerator enumerator = this.ExcelCharts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Name == image.Name)
                    {
                        throw new NotSupportedException(ResourceHelper.GetResourceString("CurrentWorksheetHasSameImageError"));
                    }
                }
            }
            if (image != null)
            {
                this.ExcelImages.Add(image);
            }
        }

        /// <summary>
        /// Add a sheet table to the current worksheet.
        /// </summary>
        /// <param name="table">An <see cref="T:Dt.Xls.IExcelTable" /> instance describe a table settings.</param>
        public void SetSheetTable(IExcelTable table)
        {
            if (this._tables == null)
            {
                this._tables = new List<IExcelTable>();
            }
            this._tables.Add(table);
        }

        /// <summary>
        /// Set the location of the top left visible cell in the current sheet
        /// </summary>
        /// <param name="top">The zero-based row index of the top left cell</param>
        /// <param name="left">The zero-based column index of the top left cell</param>
        public void SetTopLeft(int top, int left)
        {
            if ((top >= 0) && (left >= 0))
            {
                this._top = top;
                this._left = left;
            }
        }

        /// <summary>
        /// Sets the value of the cell.
        /// </summary>
        /// <param name="row">The zero-based row index used to locate the cell</param>
        /// <param name="column">The zero-base column index used to locate the cell</param>
        /// <param name="value">The value of the cell</param>
        public void SetValue(int row, int column, object value)
        {
            IExcelCell cell = this.GetCell(row, column, true);
            if (cell != null)
            {
                cell.Value = value;
            }
            else
            {
                if ((row < 0) || (row >= this.RowCount))
                {
                    throw new ArgumentOutOfRangeException("row");
                }
                if ((column < 0) || (column >= this.ColumnCount))
                {
                    throw new ArgumentOutOfRangeException("column");
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the active column.
        /// </summary>
        /// <value>The index of the active column.</value>
        public int ActiveColumnIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the active row.
        /// </summary>
        /// <value>The index of the active row.</value>
        public int ActiveRowIndex { get; set; }

        /// <summary>
        /// Actual total columns count used by the current worksheet
        /// </summary>
        /// <remarks>
        /// </remarks>
        public int ActualColumnCount { get; internal set; }

        /// <summary>
        /// Actual total rows count used by the current worksheet
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public int ActualRowCount { get; internal set; }

        /// <summary>
        /// Gets or sets the auto filter used in the worksheet.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExcelAutoFilter" /> instance used to repents auto filter settings used in the worksheet.
        /// </value>
        public IExcelAutoFilter AutoFilter { get; set; }

        /// <summary>
        /// Get or set the column count.
        /// </summary>
        /// <value>
        /// The <see langword="int" /> indicate the column count.
        /// The default value is <b>0</b>.
        /// </value>
        /// <remarks>
        /// If the new value is less than current,
        /// all existed <see cref="T:Dt.Xls.ExcelCell" /> which column index is great than new value would be removed;
        /// And all existed <see cref="T:Dt.Xls.ExcelColumn" /> which index is great than new value would be removed.
        /// </remarks>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The value is less than <b>0</b>.
        /// </exception>
        public int ColumnCount
        {
            get { return  this._columnCount; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this._columnCount)
                {
                    this._columnCount = value;
                    if (this._dataTable1.Count > 0)
                    {
                        foreach (Vector<IExcelCell> vector in this._dataTable1)
                        {
                            if ((vector != null) && (vector.Count > 0))
                            {
                                while (vector.Count >= this._columnCount)
                                {
                                    vector.RemoveAt(this._columnCount);
                                    if (vector.Count == this._columnCount)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (this._columns1.Count > 0)
                    {
                        while (this._columns1.Count >= this._columnCount)
                        {
                            this._columns1.RemoveAt(this._columnCount);
                            if (this._columns1.Count == this._columnCount)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the column gutter.
        /// </summary>
        /// <value>The column gutter.</value>
        public short ColumnGutter { get; set; }

        /// <summary>
        /// Gets or sets the column max outline level.
        /// </summary>
        /// <value>The column max outline level.</value>
        public short ColumnMaxOutlineLevel { get; set; }

        /// <summary>
        /// Gets or sets the conditional formats used in the worksheet.
        /// </summary>
        /// <value>
        /// A collection of <see cref="T:Dt.Xls.IExcelConditionalFormat" /> represents conditional formats settings used in the work sheet.
        /// </value>
        public List<IExcelConditionalFormat> ConditionalFormat
        {
            get
            {
                if (this._conditionalFormatList == null)
                {
                    this._conditionalFormatList = new List<IExcelConditionalFormat>();
                }
                return this._conditionalFormatList;
            }
            set { this._conditionalFormatList = value; }
        }

        /// <summary>
        /// Gets or sets the default width of the column.
        /// </summary>
        /// <value>The default width of the column.</value>
        public double DefaultColumnWidth
        {
            get { return  this._defaultColumnWidth; }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._defaultColumnWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the default height of the row.
        /// </summary>
        /// <value>The default height of the row.</value>
        public double DefaultRowHeight
        {
            get { return  this._defaultRowHeight; }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._defaultRowHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the default viewport information of the worksheet.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.Xls.ExcelWorksheet.ExcelViewportInfo" /> instance represents the worksheet default viewpoint information
        /// </value>
        public ExcelViewportInfo DefaultViewport { get; set; }

        /// <summary>
        /// Gets the excel charts.
        /// </summary>
        /// <value>
        /// The excel charts.
        /// </value>
        public List<IExcelChart> ExcelCharts
        {
            get
            {
                if (this._charts == null)
                {
                    this._charts = new List<IExcelChart>();
                }
                return this._charts;
            }
        }

        /// <summary>
        /// Gets the excel images.
        /// </summary>
        /// <value>
        /// The excel images.
        /// </value>
        public List<IExcelImage> ExcelImages
        {
            get
            {
                if (this._images == null)
                {
                    this._images = new List<IExcelImage>();
                }
                return this._images;
            }
        }

        /// <summary>
        /// Gets or sets the extended formats for the worksheet.
        /// </summary>
        /// <value>The extended formats</value>
        public Dictionary<int, IExtendedFormat> ExtendedFormats { get; set; }

        /// <summary>
        /// Gets or sets the frozen column count.
        /// </summary>
        /// <value>The frozen column count.</value>
        public int FrozenColumnCount { get; set; }

        /// <summary>
        /// Gets or sets the frozen row count.
        /// </summary>
        /// <value>The frozen row count.</value>
        public int FrozenRowCount { get; set; }

        /// <summary>
        /// Gets or sets the color of the grid line.
        /// </summary>
        /// <value>The color of the grid line.</value>
        public IExcelColor GridLineColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is locked; otherwise, <see langword="false" />.
        /// </value>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is visible; otherwise, <see langword="false" />.
        /// </value>
        public bool IsVisible
        {
            get { return  this._visible; }
            set { this._visible = value; }
        }

        /// <summary>
        /// Get the <see cref="T:Dt.Xls.ExcelCell" /> at the specified by <paramref name="row" /> and <paramref name="column" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="T:Dt.Xls.ExcelCell" /> indicates the excel cell which row
        /// index is <paramref name="row" /> and column index is <paramref name="column" />.
        /// </returns>
        /// <remarks>
        /// If the cell indicated by <paramref name="row" /> and <paramref name="column" /> is not existed,
        /// create a new one and return it.
        /// </remarks>
        public IExcelCell this[int row, int column]
        {
            get { return  this.GetCell(row, column, true); }
        }

        /// <summary>
        /// Gets or sets the merged cells information.
        /// </summary>
        /// <value>
        /// A collection of <see cref="T:Dt.Xls.IRange" /> represents the merged cell information
        /// </value>
        public List<IRange> MergedCells
        {
            get
            {
                if (this._mergedCells == null)
                {
                    this._mergedCells = new List<IRange>();
                }
                return this._mergedCells;
            }
            set { this._mergedCells = value; }
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="T:Dt.Xls.ExcelWorksheet" />.
        /// </summary>
        /// <value>
        /// The name of the <see cref="T:Dt.Xls.ExcelWorksheet" /> instance
        /// The default is <see langword="null" />.
        /// </value>
        /// <remarks>
        /// The component will not check whether the name is valid or invalid, make sure pass
        /// a valid name to it.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the named cell ranges information
        /// </summary>
        /// <value>A dictionary represents the named cell ranges information.</value>
        public Dictionary<string, IName> NamedCellRanges
        {
            get
            {
                if (this._namedCellRanges == null)
                {
                    this._namedCellRanges = new Dictionary<string, IName>();
                }
                return this._namedCellRanges;
            }
            set { this._namedCellRanges = value; }
        }

        /// <summary>
        /// Gets or sets the print area information of the worksheet.
        /// </summary>
        /// <value>The print area information</value>
        public string PrintArea { get; set; }

        /// <summary>
        /// Gets or sets the print settings of the sheet.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExcelPrintSettings" /> represents the print settings
        /// </value>
        public IExcelPrintSettings PrintSettings
        {
            get
            {
                if (this._printSettings == null)
                {
                    this._printSettings = new ExcelPrintSettings();
                }
                return this._printSettings;
            }
            set { this._printSettings = value; }
        }

        /// <summary>
        /// Gets or sets the print title information of the worksheet.
        /// </summary>
        /// <value>The print title information</value>
        public string PrintTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sheet is in 'right to left' display mode
        /// </summary>
        public bool RightToLeftColumns { get; set; }

        /// <summary>
        /// Get or set the row count.
        /// </summary>
        /// <value>
        /// The <see langword="int" /> indicate the row count.
        /// The default value is <b>0</b>.
        /// </value>
        /// <remarks>
        /// If the new value is less than current,
        /// all existed <see cref="T:Dt.Xls.ExcelCell" /> which row index is great than new value would be removed;
        /// And all existed <see cref="T:Dt.Xls.ExcelRow" /> which index is great than new value would be removed.
        /// </remarks>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The value is less than <b>0</b>.
        /// </exception>
        public int RowCount
        {
            get { return  this._rowCount; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this._rowCount)
                {
                    this._rowCount = value;
                    if (this._dataTable1.Count > 0)
                    {
                        while (this._dataTable1.Count >= this._rowCount)
                        {
                            this._dataTable1.RemoveAt(this._rowCount);
                            if (this._dataTable1.Count == this._rowCount)
                            {
                                break;
                            }
                        }
                    }
                    if (this._rows1.Count > 0)
                    {
                        while (this._rows1.Count >= this._rowCount)
                        {
                            this._rows1.RemoveAt(this._rowCount);
                            if (this._rowCount == this._rows1.Count)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the row gutter.
        /// </summary>
        /// <value>The row gutter.</value>
        public short RowGutter { get; set; }

        /// <summary>
        /// Gets or sets the row max outline level.
        /// </summary>
        /// <value>The row max outline level.</value>
        public short RowMaxOutlineLevel { get; set; }

        /// <summary>
        /// Gets the selection information.
        /// </summary>
        /// <value>
        /// A collection of <see cref="T:Dt.Xls.ISelectionRange" /> represents the selection information
        /// </value>
        public List<ISelectionRange> Selections
        {
            get
            {
                if (this._selections == null)
                {
                    this._selections = new List<ISelectionRange>();
                }
                return this._selections;
            }
        }

        /// <summary>
        /// Gets or sets the color of the sheet tab.
        /// </summary>
        /// <value>
        /// The color of the sheet tab.
        /// </value>
        public IExcelColor SheetTabColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this sheet should display formulas.
        /// </summary>
        public bool ShowFormulas { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show gridlines or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show gridline; otherwise, <see langword="false" />.
        /// </value>
        public bool ShowGridLines
        {
            get { return  this._showGridLines; }
            set { this._showGridLines = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show headers.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show headers.; otherwise, <see langword="false" />.
        /// </value>
        public bool ShowHeaders
        {
            get { return  this._showHeaders; }
            set { this._showHeaders = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window should show 0 (zero) in cells containing zero value.
        /// </summary>
        public bool ShowZeros
        {
            get { return  this._showZeros; }
            set { this._showZeros = value; }
        }

        /// <summary>
        /// Gets or sets the groups of sparklines on the sheet. 
        /// </summary>
        /// <remarks>Sparkline is a new feature supported since Excel 2010 </remarks>
        public List<IExcelSparklineGroup> SparklineGroups { get; set; }

        /// <summary>
        /// Get or set the direction of the summary columns.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// True if the summary columns to right of detail, otherwise; false.
        /// </returns>
        public bool SummaryColumnsToRightOfDetail
        {
            get { return  this._summaryColumnsToRightOfDetail; }
            set { this._summaryColumnsToRightOfDetail = value; }
        }

        /// <summary>
        /// Get or set the direction of the summary rows.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// True if the summary rows below detail, otherwise; false.
        /// </returns>
        public bool SummaryRowsBelowDetail
        {
            get { return  this._summaryRowsBelowDetail; }
            set { this._summaryRowsBelowDetail = value; }
        }

        /// <summary>
        /// Gets or set the zoom factor of the entire ExcelWorksheet.
        /// </summary>
        /// <value>
        /// The <see langword="float" /> indicate the zoom factory, it's a percentage;
        /// The default value is <b>100</b>.
        /// </value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The value is less than <b>0.1f</b> or great than <b>4.0f</b>.
        /// </exception>
        public float Zoom
        {
            get { return  this._zoom; }
            set
            {
                if ((value < 0.1f) || (4f < value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._zoom = value;
            }
        }

        /// <summary>
        /// Specifies the position of frozen panes or unfrozen panes in the window used to display the sheet.
        /// </summary>
        public class ExcelViewportInfo
        {
            /// <summary>
            /// The index of column pane that is active.
            /// </summary>
            public int ActiveColumnPane;
            /// <summary>
            /// The index of row pane that is active.
            /// </summary>
            public int ActiveRowPane;
            /// <summary>
            /// Column pane count
            /// </summary>
            public int ColumnPaneCount;
            /// <summary>
            /// Specifies the first visible logical left column in the logical right pane.
            /// </summary>
            public int[] LeftColumn;
            /// <summary>
            /// Vertical position of the split
            /// </summary>
            public int[] PreferredHeight;
            /// <summary>
            /// Horizontal position of the split
            /// </summary>
            public int[] PreferredWidth;
            /// <summary>
            /// Row pane count.
            /// </summary>
            public int RowPaneCount;
            /// <summary>
            /// Specifies the topmost visible row in the bottom pane.
            /// </summary>
            public int[] TopRow;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelWorksheet.ExcelViewportInfo" /> class.
            /// </summary>
            public ExcelViewportInfo()
            {
                this.RowPaneCount = 1;
                this.ColumnPaneCount = 1;
                this.TopRow = new int[1];
                this.LeftColumn = new int[1];
                this.PreferredWidth = new int[] { -1 };
                this.PreferredHeight = new int[] { -1 };
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelWorksheet.ExcelViewportInfo" /> class.
            /// </summary>
            /// <param name="sheet">The sheet.</param>
            public ExcelViewportInfo(ExcelWorksheet sheet)
            {
                this.RowPaneCount = 1;
                this.ColumnPaneCount = 1;
                this.TopRow = new int[1];
                this.LeftColumn = new int[1];
                this.PreferredWidth = new int[] { -1 };
                this.PreferredHeight = new int[] { -1 };
                this.TopRow[0] = sheet.FrozenRowCount;
                this.LeftColumn[0] = sheet.FrozenColumnCount;
            }
        }
    }
}

