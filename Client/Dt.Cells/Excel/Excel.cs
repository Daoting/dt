#region 名称空间
using Dt.CalcEngine.Expressions;
using Dt.CalcEngine.Functions;
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Represents a Excel control.
    /// </summary>
    public partial class Excel : Control
    {
        #region 成员变量
        Uri _documentUri;
        Workbook _workbook;
        #endregion

        /// <summary>
        /// Initializes a new instance of the Excel class. 
        /// </summary>
        public Excel()
        {
            DefaultStyleKey = typeof(Excel);
            View = new SpreadView(this);

            AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            AddHandler(KeyUpEvent, new KeyEventHandler(OnKeyUp), true);
        }

        /// <summary>
        /// Adds a new column viewport to the control. 
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index to add.</param>
        /// <param name="viewportWidth">The column viewport width.</param>
        public void AddColumnViewport(int columnViewportIndex, double viewportWidth)
        {
            View.AddColumnViewport(columnViewportIndex, viewportWidth);
        }

        /// <summary>
        /// Adds a custom function to the workbook. 
        /// </summary>
        /// <param name="function">The CalcFunction to add.</param>
        public void AddCustomFunction(CalcFunction function)
        {
            Workbook.AddCustomFunction(function);
        }

        /// <summary>
        /// Adds a new row viewport to the control. 
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index to add.</param>
        /// <param name="viewportHeight">The row viewport height.</param>
        public void AddRowViewport(int rowViewportIndex, double viewportHeight)
        {
            View.AddRowViewport(rowViewportIndex, viewportHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="rowViewportIndex"></param>
        /// <param name="viewportHeight"></param>
        internal void AddRowViewport(int sheetIndex, int rowViewportIndex, double viewportHeight)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            if (sheetIndex == ActiveSheetIndex)
            {
                AddRowViewport(rowViewportIndex, viewportHeight);
            }
            else
            {
                Sheets[sheetIndex].AddRowViewport(rowViewportIndex, viewportHeight);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        void AttachSheet(Worksheet sheet)
        {
            DetachSheet(sheet);
            sheet.PropertyChanged += new PropertyChangedEventHandler(OnSheetPropertyChanged);
            sheet.RowHeader.PropertyChanged += new PropertyChangedEventHandler(OnSheetRowHeaderPropertyChanged);
            sheet.ColumnHeader.PropertyChanged += new PropertyChangedEventHandler(OnSheetColumnHeaderPropertyChanged);
            sheet.CellChanged += new EventHandler<CellChangedEventArgs>(OnSheetCellChanged);
            sheet.RowChanged += new EventHandler<SheetChangedEventArgs>(OnSheetRowChanged);
            sheet.ColumnChanged += new EventHandler<SheetChangedEventArgs>(OnSheetColumnChanged);
            sheet.SelectionChanged += new EventHandler<SheetSelectionChangedEventArgs>(OnSelectionChanged);
            sheet.SpanModel.Changed += new EventHandler<SheetSpanModelChangedEventArgs>(OnSpanModelChanged);
            sheet.RowHeaderSpanModel.Changed += new EventHandler<SheetSpanModelChangedEventArgs>(OnRowHeaderSpanModelChanged);
            sheet.ColumnHeaderSpanModel.Changed += new EventHandler<SheetSpanModelChangedEventArgs>(OnColumnHeaderSpanModelChanged);
            sheet.ChartChanged += new EventHandler<ChartChangedEventArgs>(OnSheetChartChanged);
            sheet.FloatingObjectChanged += new EventHandler<FloatingObjectChangedEventArgs>(OnSheetFloatingObjectChanged);
            sheet.PictureChanged += new EventHandler<PictureChangedEventArgs>(OnPictureChanged);
        }

        /// <summary>
        /// Automatically fits the viewport column. 
        /// </summary>
        /// <param name="column">The automatic fit column index.</param>
        public void AutoFitColumn(int column)
        {
            AutoFitColumn(column, false);
        }

        /// <summary>
        /// Automatically fits the viewport column. 
        /// </summary>
        /// <param name="column">The automatic fit column index.</param>
        /// <param name="isRowHeader">The flag indicates wheather sheetArea is row header or not.</param>
        public void AutoFitColumn(int column, bool isRowHeader)
        {
            View.AutoFitColumn(column, isRowHeader);
        }

        /// <summary>
        /// Automatically fits the viewport row. 
        /// </summary>
        /// <param name="row">The row index.</param>
        public void AutoFitRow(int row)
        {
            AutoFitRow(row, false);
        }

        /// <summary>
        /// Automatically fits the viewport row. 
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="isColumnHeader">The flag indicates wheather sheetArea is row header or not.</param>
        public void AutoFitRow(int row, bool isColumnHeader)
        {
            View.AutoFitRow(row, isColumnHeader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AutoRefreshRowColumn(object sender, SheetChangedEventArgs e)
        {
            if ((sender == View.Worksheet) && AutoRefresh)
            {
                if (((e.PropertyName == "Height") || (e.PropertyName == "Width")) || ((e.PropertyName == "IsVisible") || (e.PropertyName == "Axis")))
                {
                    View.InvalidateSheetLayout();
                }
                else
                {
                    View.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                }
            }
        }

        /// <summary>
        /// Removes all user-defined custom functions (FunctionInfo object) on this sheet. 
        /// </summary>
        public void ClearCustomFunctions()
        {
            Workbook.ClearCustomFunctions();
        }

        /// <summary>
        /// Clears the custom name in the workbook. 
        /// </summary>
        /// <param name="name">The custom name.</param>
        public void ClearCustomName(string name)
        {
            Workbook.ClearCustomName(name);
        }

        /// <summary>
        /// Removes all custom names on this workbook. 
        /// </summary>
        public void ClearCustomNames()
        {
            Workbook.ClearCustomNames();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        void DetachSheet(Worksheet sheet)
        {
            sheet.PropertyChanged -= new PropertyChangedEventHandler(OnSheetPropertyChanged);
            sheet.RowHeader.PropertyChanged -= new PropertyChangedEventHandler(OnSheetRowHeaderPropertyChanged);
            sheet.ColumnHeader.PropertyChanged -= new PropertyChangedEventHandler(OnSheetColumnHeaderPropertyChanged);
            sheet.CellChanged -= new EventHandler<CellChangedEventArgs>(OnSheetCellChanged);
            sheet.RowChanged -= new EventHandler<SheetChangedEventArgs>(OnSheetRowChanged);
            sheet.ColumnChanged -= new EventHandler<SheetChangedEventArgs>(OnSheetColumnChanged);
            sheet.SelectionChanged -= new EventHandler<SheetSelectionChangedEventArgs>(OnSelectionChanged);
            sheet.SpanModel.Changed += new EventHandler<SheetSpanModelChangedEventArgs>(OnSpanModelChanged);
            sheet.RowHeaderSpanModel.Changed -= new EventHandler<SheetSpanModelChangedEventArgs>(OnRowHeaderSpanModelChanged);
            sheet.ColumnHeaderSpanModel.Changed -= new EventHandler<SheetSpanModelChangedEventArgs>(OnColumnHeaderSpanModelChanged);
            sheet.ChartChanged -= new EventHandler<ChartChangedEventArgs>(OnSheetChartChanged);
            sheet.FloatingObjectChanged -= new EventHandler<FloatingObjectChangedEventArgs>(OnSheetFloatingObjectChanged);
            sheet.PictureChanged -= new EventHandler<PictureChangedEventArgs>(OnPictureChanged);
        }

        /// <summary>
        /// Executes a command. 
        /// </summary>
        /// <param name="command">The command.</param>
        public void DoCommand(ICommand command)
        {
            View.DoCommand(command);
        }

        /// <summary>
        /// Gets the current active column viewport index in the control. 
        /// </summary>
        /// <returns>The active column viewport index.</returns>
        public int GetActiveColumnViewportIndex()
        {
            return GetActiveColumnViewportIndex(ActiveSheetIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <returns></returns>
        internal int GetActiveColumnViewportIndex(int sheetIndex)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            return View.GetActiveColumnViewportIndex(sheetIndex);
        }

        /// <summary>
        /// Gets the current active row viewport index in the control. 
        /// </summary>
        /// <returns>The active row viewport index.</returns>
        public int GetActiveRowViewportIndex()
        {
            return GetActiveRowViewportIndex(ActiveSheetIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <returns></returns>
        internal int GetActiveRowViewportIndex(int sheetIndex)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            return View.GetActiveRowViewportIndex(sheetIndex);
        }

        /// <summary>
        ///Gets the column viewport count in the control.  
        /// </summary>
        /// <returns>The column viewport count.</returns>
        public int GetColumnViewportCount()
        {
            return View.GetColumnPaneCount();
        }

        /// <summary>
        /// Gets a custom function from the workbook.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <returns></returns>
        public CalcFunction GetCustomFunction(string name)
        {
            return Workbook.GetCustomFunction(name);
        }

        /// <summary>
        /// Gets custom name information from the workbook. 
        /// </summary>
        /// <param name="name">The custom name.</param>
        /// <returns></returns>
        public NameInfo GetCustomName(string name)
        {
            return Workbook.GetCustomName(name);
        }

        /// <summary>
        /// Gets the row viewport count in the control. 
        /// </summary>
        /// <returns>The row viewport count.</returns>
        public int GetRowViewportCount()
        {
            return View.GetRowPaneCount();
        }

        /// <summary>
        /// Gets the row viewport's bottom row index. 
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <returns></returns>
        public int GetViewportBottomRow(int rowViewportIndex)
        {
            return View.GetViewportBottomRow(rowViewportIndex);
        }

        /// <summary>
        /// Gets the column viewport's left column index. 
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <returns></returns>
        public int GetViewportLeftColumn(int columnViewportIndex)
        {
            return View.GetViewportLeftColumn(columnViewportIndex);
        }

        /// <summary>
        /// Gets the column viewport's right column index. 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns>The column viewport index.</returns>
        public int GetViewportRightColumn(int columnViewportIndex)
        {
            return View.GetViewportRightColumn(columnViewportIndex);
        }

        /// <summary>
        /// Gets the row viewport's top row index. 
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <returns></returns>
        public int GetViewportTopRow(int rowViewportIndex)
        {
            return View.GetViewportTopRow(rowViewportIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HideOpeningStatusOnOpenExcelCompleted()
        {
            View.HideOpeningStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HideProgressRingOnOpenCSVCompleted()
        {
            View.HideOpeningProgressRing();
        }

        /// <summary>
        /// Performs a hit test. 
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns></returns>
        public HitTestInformation HitTest(double x, double y)
        {
            Point point = new Point(x, y);
            Point point2 = base.TransformToVisual(View).TransformPoint(point);
            return View.HitTest(point2.X, point2.Y);
        }

        /// <summary>
        /// Invalidates the measurement state (layout) and the arrangement state (layout) for the control. After the invalidation, the control layout and data are updated. 
        /// </summary>
#if ANDROID
    new
#endif
        public void Invalidate()
        {
            View.InvalidateLayout();
            View.InvalidateMeasure();
            View.InvalidateArrange();
            View.Invalidate();
        }

        /// <summary>
        /// Invalidates the charts. 
        /// </summary>
        public void InvalidateCharts()
        {
            View.InvalidateCharts();
        }

        /// <summary>
        /// View.InvalidateCharts(charts);
        /// </summary>
        /// <param name="charts">The charts.</param>
        public void InvalidateCharts(params SpreadChart[] charts)
        {
            View.InvalidateCharts(charts);
        }

        /// <summary>
        /// Invalidates the column state in the control. After the invalidation, the column layout and data are updated. 
        /// </summary>
        /// <param name="column">The start column index.</param>
        /// <param name="columnCount">The column count.</param>
        public void InvalidateColumns(int column, int columnCount)
        {
            InvalidateRange(-1, column, -1, columnCount, SheetArea.Cells);
        }

        /// <summary>
        /// Invalidates the column state in the control. After the invalidation, the column layout and data are updated. 
        /// </summary>
        /// <param name="column">The start column index.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="sheetArea">The invalid sheet area</param>
        public void InvalidateColumns(int column, int columnCount, SheetArea sheetArea)
        {
            InvalidateRange(-1, column, -1, columnCount, sheetArea);
        }

        /// <summary>
        ///Invalidates the custom floating objects. 
        /// </summary>
        public void InvalidateCustomFloatingObjects()
        {
            View.InvalidateCustomFloatingObjects();
        }

        /// <summary>
        /// Invalidates the custom floating objects. 
        /// </summary>
        /// <param name="floatingObjects">The floating objects.</param>
        public void InvalidateCustomFloatingObjects(params CustomFloatingObject[] floatingObjects)
        {
            InvalidateCustomFloatingObjects(floatingObjects);
        }

        /// <summary>
        /// Invalidates the charts. 
        /// </summary>
        public void InvalidateFloatingObjects()
        {
            View.InvalidateFloatingObjects();
        }

        /// <summary>
        /// Invalidates the floating object. 
        /// </summary>
        /// <param name="floatingObjects">The floating objects.</param>
        public void InvalidateFloatingObjects(params FloatingObject[] floatingObjects)
        {
            View.InvalidateFloatingObjects(floatingObjects);
        }

        /// <summary>
        /// Invalidates the pictures. 
        /// </summary>
        public void InvalidatePictures()
        {
            View.InvalidatePictures();
        }

        /// <summary>
        /// Invalidates the pictures. 
        /// </summary>
        /// <param name="pictures">The pictures.</param>
        public void InvalidatePictures(params Picture[] pictures)
        {
            View.InvalidatePictures(pictures);
        }

        /// <summary>
        /// Invalidates a range state in the control. After the invalidation, the range layout and data are updated. 
        /// </summary>
        /// <param name="row">The start row index.</param>
        /// <param name="column">The start column index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        public void InvalidateRange(int row, int column, int rowCount, int columnCount)
        {
            InvalidateRange(row, column, rowCount, columnCount, SheetArea.Cells);
        }

        /// <summary>
        /// Invalidates a range state in the control. After the invalidation, the range layout and data are updated. 
        /// </summary>
        /// <param name="row">The start row index.</param>
        /// <param name="column">The start column index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="sheetArea">The invalid sheet area.</param>
        public void InvalidateRange(int row, int column, int rowCount, int columnCount, SheetArea sheetArea)
        {
            View.InvalidateRange(row, column, rowCount, columnCount, sheetArea);
        }

        /// <summary>
        /// Invalidates the row state in the control. After the invalidation, the row layout and data are updated. 
        /// </summary>
        /// <param name="row">The start row index.</param>
        /// <param name="rowCount">The row count.</param>
        public void InvalidateRows(int row, int rowCount)
        {
            InvalidateRange(row, -1, rowCount, -1, SheetArea.Cells);
        }

        /// <summary>
        /// Invalidates the row state in the control. After the invalidation, the row layout and data are updated. 
        /// </summary>
        /// <param name="row">The start row index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="sheetArea">The invalid sheet area.</param>
        public void InvalidateRows(int row, int rowCount, SheetArea sheetArea)
        {
            InvalidateRange(row, -1, rowCount, -1, sheetArea);
        }

        /// <summary>
        /// Invalidates the measurement state (layout) and the arrangement state (layout) for the view. After the invalidation, the view layout and data are updated. 
        /// </summary>
        public void InvalidateSheet()
        {
            View.Invalidate();
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call ApplyTemplate, when overridden in a derived class. 
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Grid root = (Grid)GetTemplateChild("Root");
            root.Children.Add(View);

            View.SetBinding(Panel.BackgroundProperty, new Binding { Path = new PropertyPath("Background"), Source = this });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnColumnHeaderSpanModelChanged(object sender, SheetSpanModelChangedEventArgs e)
        {
            OnSpanModelChanged(e.Row, e.Column, e.RowCount, e.ColumnCount, SheetArea.ColumnHeader);
        }

        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            View.ProcessKeyDown(e);
        }

        void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            View.ProcessKeyUp(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPictureChanged(object sender, PictureChangedEventArgs e)
        {
            View.HandlePictureChanged(sender, e, AutoRefresh);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRowHeaderSpanModelChanged(object sender, SheetSpanModelChangedEventArgs e)
        {
            OnSpanModelChanged(e.Row, e.Column, e.RowCount, e.ColumnCount, SheetArea.CornerHeader | SheetArea.RowHeader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnSelectionChanged(object sender, SheetSelectionChangedEventArgs e)
        {
            //hdt
            if (!this.AreHandlersSuspended())
            {
                if (base.Dispatcher.HasThreadAccess)
                {
                    View.HandleSheetSelectionChanged(sender, e);
                }
                else
                {
                    await base.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        View.HandleSheetSelectionChanged(sender, e);
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSheetCellChanged(object sender, CellChangedEventArgs e)
        {
            if (AutoRefresh)
            {
                View.HandleCellChanged(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSheetChartChanged(object sender, ChartChangedEventArgs e)
        {
            View.HandleChartChanged(sender, e, AutoRefresh);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSheetColumnChanged(object sender, SheetChangedEventArgs e)
        {
            AutoRefreshRowColumn(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSheetColumnHeaderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AutoRefresh)
            {
                View.HandleSheetColumnHeaderPropertyChanged(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSheetFloatingObjectChanged(object sender, FloatingObjectChangedEventArgs e)
        {
            View.HandleFloatingObjectChanged(sender, e, AutoRefresh);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSheetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (AutoRefresh)
                {
                    View.InvalidTabStrip();
                }
            }
            else
            {
                View.HandleSheetPropertyChanged(sender, e, AutoRefresh);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSheetRowChanged(object sender, SheetChangedEventArgs e)
        {
            AutoRefreshRowColumn(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSheetRowHeaderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AutoRefresh)
            {
                View.HandleSheetRowHeaderPropertyChanged(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSheetsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable enumerable = (e.NewItems == null) ? ((IEnumerable)new Worksheet[0]) : ((IEnumerable)e.NewItems);
            IEnumerable enumerable2 = (e.OldItems == null) ? ((IEnumerable)new Worksheet[0]) : ((IEnumerable)e.OldItems);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    foreach (Worksheet worksheet in enumerable2)
                    {
                        DetachSheet(worksheet);
                    }
                    foreach (Worksheet worksheet2 in enumerable)
                    {
                        AttachSheet(worksheet2);
                    }
                    if ((SpreadXClipboard.Worksheet != null) && ((Workbook.Sheets == null) || !Workbook.Sheets.Contains(SpreadXClipboard.Worksheet)))
                    {
                        ClipboardHelper.ClearClipboard();
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSpanModelChanged(object sender, SheetSpanModelChangedEventArgs e)
        {
            OnSpanModelChanged(e.Row, e.Column, e.RowCount, e.ColumnCount, SheetArea.Cells);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="rowCount"></param>
        /// <param name="columnCount"></param>
        /// <param name="area"></param>
        void OnSpanModelChanged(int row, int column, int rowCount, int columnCount, SheetArea area)
        {
            if (AutoRefresh)
            {
                InvalidateRange(row, column, rowCount, columnCount, area);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnWorkbookPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AutoRefresh)
            {
                View.HandleWorkbookPropertyChanged(sender, e);
            }
            if (e.PropertyName == "ActiveSheetIndex")
            {
                View.OnActiveSheetChanged();
            }
        }

        /// <summary>
        /// Loads the CSV (comma-separated values) file asynchronously. 
        /// </summary>
        /// <param name="sheetIndex">The destination sheet index for loading.</param>
        /// <param name="stream">Stream from which to load.</param>
        /// <param name="flags">The import flags.</param>
        /// <returns></returns>
        public IAsyncAction OpenCSVAsync(int sheetIndex, Stream stream, TextFileOpenFlags flags)
        {
            return OpenCSVAsync(sheetIndex, stream, flags, Encoding.UTF8);
        }

        /// <summary>
        /// Loads the CSV (comma-separated values) file asynchronously. 
        /// </summary>
        /// <param name="sheetIndex">The destination sheet index for loading.</param>
        /// <param name="stream">Stream from which to load.</param>
        /// <param name="flags">The import flags.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public IAsyncAction OpenCSVAsync(int sheetIndex, Stream stream, TextFileOpenFlags flags, Encoding encoding)
        {
            IAsyncAction action;
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Workbook.SuspendEvent();
            try
            {
                View.ShowOpeningProgressRing();
                action = Sheets[sheetIndex].OpenCsvAsync(stream, flags, encoding);
            }
            finally
            {
                Workbook.ResumeEvent();
            }
            return action;
        }

        /// <summary>
        /// Opens an Excel Compound Document File and loads it into GcSpreadSheet. 
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="password">The file password.</param>
        /// <returns></returns>
        internal IAsyncAction OpenExcel(Stream stream, string password)
        {
            return OpenExcel(stream, ExcelOpenFlags.NoFlagsSet, password);
        }

        /// <summary>
        /// Opens an Excel Compound Document File and loads it into GcSpreadSheet. 
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="openFlags">The flag used to open the file.</param>
        /// <param name="password">The file password.</param>
        /// <returns></returns>
        internal IAsyncAction OpenExcel(Stream stream, ExcelOpenFlags openFlags, string password)
        {
            IAsyncAction action2;
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            try
            {
                View.ShowOpeningStatus();
                action2 = Workbook.OpenExcelAsync(stream, openFlags);
            }
            catch (Exception exception)
            {
                while ((exception is TargetInvocationException) && (exception.InnerException != null))
                {
                    exception = exception.InnerException;
                }
                throw exception;
            }
            return action2;
        }

        /// <summary>
        /// Opens an Excel Compound Document File and loads it into GcSpreadSheet.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <returns></returns>
        public IAsyncAction OpenExcelAsync(Stream stream)
        {
            return OpenExcel(stream, ExcelOpenFlags.NoFlagsSet, null);
        }

        /// <summary>
        /// Opens an Excel Compound Document File and loads it into GcSpreadSheet. 
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="openFlags">The flag used to open the file.</param>
        /// <returns></returns>
        public IAsyncAction OpenExcelAsync(Stream stream, ExcelOpenFlags openFlags)
        {
            return OpenExcel(stream, openFlags, null);
        }

        /// <summary>
        /// Loads the CSV file with the specified separator asynchronously. 
        /// </summary>
        /// <param name="sheetIndex">The destination sheet index for loading.</param>
        /// <param name="stream">Stream from which to load.</param>
        /// <param name="flags">The import flags.</param>
        /// <param name="rowDelimiter">Row delimiter string.</param>
        /// <param name="columnDelimiter">Column delimiter string.</param>
        /// <param name="cellDelimiter">Cell delimiter string.</param>
        /// <returns></returns>
        public IAsyncAction OpenTextFileAsync(int sheetIndex, Stream stream, TextFileOpenFlags flags, string rowDelimiter, string columnDelimiter, string cellDelimiter)
        {
            IAsyncAction action;
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Workbook.SuspendEvent();
            try
            {
                View.ShowOpeningProgressRing();
                action = Sheets[sheetIndex].OpenTextFileAsync(stream, flags, rowDelimiter, columnDelimiter, cellDelimiter);
            }
            finally
            {
                Workbook.ResumeEvent();
            }
            return action;
        }

        /// <summary>
        /// Loads the data on the sheet from the specified XML stream. 
        /// </summary>
        /// <param name="xmlStream">The XML stream.</param>
        /// <returns></returns>
        public IAsyncAction OpenXmlAsync(Stream xmlStream)
        {
            return AsyncInfo.Run(delegate (CancellationToken token)
            {
                return Task.Factory.StartNew(delegate
                {
                    OpenXmlOnBackground(xmlStream);
                });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlStream"></param>
        internal void OpenXmlInternal(Stream xmlStream)
        {
            XmlReader reader = null;
            Workbook.SuspendEvent();
            try
            {
                if (_workbook != null)
                {
                    _workbook.Sheets.Clear();
                    _workbook.Sheets.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnSheetsCollectionChanged);
                    _workbook.PropertyChanged -= new PropertyChangedEventHandler(OnWorkbookPropertyChanged);
                    _workbook = null;
                }
                using (reader = XmlReader.Create(xmlStream))
                {
                    Serializer.InitReader(reader);
                    while (reader.Read())
                    {
                        string str;
                        ReadXmlInternal(reader);
                        if ((reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element))) && ((str = reader.Name) != null))
                        {
                            if (str == "Data")
                            {
                                XmlReader reader2 = Serializer.ExtractNode(reader);
                                Serializer.InitReader(reader2);
                                reader2.Read();
                                _workbook = new Workbook();
                                _workbook.OpenXml(reader);
                            }
                            else if (str == "View")
                            {
                                goto Label_00D7;
                            }
                        }
                        continue;
                    Label_00D7:
                        Serializer.DeserializeSerializableObject(View, reader);
                    }
                }
                if (_workbook != null)
                {
                    foreach (Worksheet worksheet in _workbook.Sheets)
                    {
                        AttachSheet(worksheet);
                    }
                    _workbook.Sheets.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSheetsCollectionChanged);
                    _workbook.PropertyChanged += new PropertyChangedEventHandler(OnWorkbookPropertyChanged);
                }
            }
            catch (Exception exception)
            {
                while ((exception is TargetInvocationException) && (exception.InnerException != null))
                {
                    exception = exception.InnerException;
                }
                throw exception;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                Workbook.ResumeEvent();
            }
            View.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlStream"></param>
        void OpenXmlOnBackground(Stream xmlStream)
        {
            XmlReader reader = null;
            Workbook.SuspendEvent();
            try
            {
                if (_workbook != null)
                {
                    _workbook.Reset();
                    _workbook.Sheets.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnSheetsCollectionChanged);
                    _workbook.PropertyChanged -= new PropertyChangedEventHandler(OnWorkbookPropertyChanged);
                }
                View.ShowOpeningStatus();
                using (reader = XmlReader.Create(xmlStream))
                {
                    Serializer.InitReader(reader);
                    while (reader.Read())
                    {
                        string str;
                        ReadXmlInternal(reader);
                        if ((reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element))) && ((str = reader.Name) != null))
                        {
                            if (str == "Data")
                            {
                                XmlReader reader2 = Serializer.ExtractNode(reader);
                                Serializer.InitReader(reader2);
                                reader2.Read();
                                _workbook = new Workbook();
                                _workbook.SuspendEvent();
                                _workbook.OpenXml(reader);
                            }
                            else if (str == "View")
                            {
                                goto Label_0112;
                            }
                        }
                        continue;
                    Label_0112:
                        Serializer.DeserializeSerializableObject(View, reader);
                    }
                }
                if (_workbook != null)
                {
                    foreach (Worksheet worksheet in _workbook.Sheets)
                    {
                        AttachSheet(worksheet);
                    }
                    _workbook.Sheets.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSheetsCollectionChanged);
                    _workbook.PropertyChanged += new PropertyChangedEventHandler(OnWorkbookPropertyChanged);
                }
            }
            catch (Exception exception)
            {
                while ((exception is TargetInvocationException) && (exception.InnerException != null))
                {
                    exception = exception.InnerException;
                }
                throw exception;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                Workbook.ResumeEvent();
            }
            Invalidate();
            View.HideOpeningStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        void ReadXmlInternal(XmlReader reader)
        {
        }

        /// <summary>
        /// Removes a column viewport from the control. 
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index to remove.</param>
        public void RemoveColumnViewport(int columnViewportIndex)
        {
            View.RemoveColumnViewport(columnViewportIndex);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="columnViewportIndex"></param>
        internal void RemoveColumnViewport(int sheetIndex, int columnViewportIndex)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            if (sheetIndex == ActiveSheetIndex)
            {
                RemoveColumnViewport(columnViewportIndex);
            }
            else
            {
                Sheets[sheetIndex].RemoveColumnViewport(columnViewportIndex);
            }
        }

        /// <summary>
        /// Removes a custom function from the workbook. 
        /// </summary>
        /// <param name="name">The function name.</param>
        public void RemoveCustomFunctions(string name)
        {
            Workbook.RemoveCustomFunctions(name);
        }

        /// <summary>
        /// Removes a row viewport from the control. 
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index to remove.</param>
        public void RemoveRowViewport(int rowViewportIndex)
        {
            View.RemoveRowViewport(rowViewportIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="rowViewportIndex"></param>
        internal void RemoveRowViewport(int sheetIndex, int rowViewportIndex)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            if (sheetIndex == ActiveSheetIndex)
            {
                RemoveRowViewport(rowViewportIndex);
            }
            else
            {
                Sheets[sheetIndex].RemoveRowViewport(rowViewportIndex);
            }
        }

        /// <summary>
        /// Resets the changed theme color and font name. 
        /// </summary>
        public void ResetThemes()
        {
            Workbook.ResetThemes();
            InvalidateSheet();
        }

        /// <summary>
        /// Resumes the calculation service. 
        /// </summary>
        public void ResumeCalcService()
        {
            Workbook.ResumeCalcService();
            InvalidateSheet();
        }

        /// <summary>
        /// Resumes the event. 
        /// </summary>
        public void ResumeEvent()
        {
            Workbook.ResumeEvent();
            View.ResumeEvent();
        }

        /// <summary>
        /// Saves the CSV (comma-separated values) file asynchronously. 
        /// </summary>
        /// <param name="sheetIndex">The destination sheet index for saving.</param>
        /// <param name="stream">Stream to which to save the content.</param>
        /// <param name="flags">The export flags.</param>
        /// <returns></returns>
        public IAsyncAction SaveCSVAsync(int sheetIndex, Stream stream, TextFileSaveFlags flags)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            return Sheets[sheetIndex].SaveCsvAsync(stream, flags);
        }

        /// <summary>
        /// Saves the CSV (comma-separated values) file asynchronously. 
        /// </summary>
        /// <param name="sheetIndex">The destination sheet index for saving.</param>
        /// <param name="stream">Stream to which to save the content.</param>
        /// <param name="flags">The export flags.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public IAsyncAction SaveCSVAsync(int sheetIndex, Stream stream, TextFileSaveFlags flags, Encoding encoding)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            return Sheets[sheetIndex].SaveCsvAsync(stream, flags, encoding);
        }

        /// <summary>
        /// Saves GcSpreadSheet to an Excel Compound Document File. 
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <param name="format">The file format.</param>
        /// <param name="saveFlags">Options for saving to a file.</param>
        /// <param name="password">The file password.</param>
        /// <returns></returns>
        internal IAsyncAction SaveExcel(Stream stream, ExcelFileFormat format, ExcelSaveFlags saveFlags, string password)
        {
            IAsyncAction action;
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            try
            {
                action = Workbook.SaveExcelAsync(stream, format, saveFlags);
            }
            catch (Exception exception)
            {
                while ((exception is TargetInvocationException) && (exception.InnerException != null))
                {
                    exception = exception.InnerException;
                }
                throw exception;
            }
            return action;
        }

        /// <summary>
        /// Saves GcSpreadSheet to an Excel Compound Document File. 
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <param name="format">The file format to save to.</param>
        /// <returns></returns>
        public IAsyncAction SaveExcelAsync(Stream stream, ExcelFileFormat format)
        {
            return SaveExcel(stream, format, ExcelSaveFlags.NoFlagsSet, null);
        }

        /// <summary>
        /// Saves GcSpreadSheet to an Excel Compound Document File. 
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <param name="format">The file format to save to.</param>
        /// <param name="saveFlags">Options for saving to a file.</param>
        /// <returns></returns>
        public IAsyncAction SaveExcelAsync(Stream stream, ExcelFileFormat format, ExcelSaveFlags saveFlags)
        {
            return SaveExcel(stream, format, saveFlags, null);
        }

        /// <summary>
        /// Saves GcSpreadSheet to an Excel Compound Document File. 
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <param name="format">The file format to save to.</param>
        /// <param name="password">The password for the file.</param>
        /// <returns></returns>
        internal IAsyncAction SaveExcelAsync(Stream stream, ExcelFileFormat format, string password)
        {
            return SaveExcel(stream, format, ExcelSaveFlags.NoFlagsSet, password);
        }

        /// <summary>
        /// Saves the content of the component to the specified stream asynchronously. 
        /// </summary>
        /// <param name="stream">Stream to which to save the data.</param>
        /// <param name="sheetIndexes">The sheet indexes collection.</param>
        /// <returns></returns>
        public IAsyncAction SavePdfAsync(Stream stream, params int[] sheetIndexes)
        {
            return SavePdfAsync(stream, null, sheetIndexes);
        }

        /// <summary>
        /// Saves the content of the component to the specified stream asynchronously. 
        /// </summary>
        /// <param name="stream">Stream to which to save the data.</param>
        /// <param name="settings">The export settings.</param>
        /// <param name="sheetIndexes">The sheet indexes.</param>
        /// <returns></returns>
        public IAsyncAction SavePdfAsync(Stream stream, PdfExportSettings settings, params int[] sheetIndexes)
        {
            return Workbook.SavePdfAsync(stream, settings, sheetIndexes);
        }

        /// <summary>
        /// Saves the range of cells in the specified sheet as delimited text with the specified delimiters, to a stream asynchronously. 
        /// </summary>
        /// <param name="sheetIndex">The destination sheet index to save to.</param>
        /// <param name="row">Starting row index.</param>
        /// <param name="column">Starting column index.</param>
        /// <param name="rowCount">The number of rows.</param>
        /// <param name="columnCount">The number of columns.</param>
        /// <param name="stream">Stream to which to save the text range.</param>
        /// <param name="flags">The export flags.</param>
        /// <param name="rowDelimiter">Row delimiter string.</param>
        /// <param name="columnDelimiter">Column delimiter string.</param>
        /// <param name="cellDelimiter">Cell delimiter string.</param>
        /// <returns></returns>
        public IAsyncAction SaveTextFileRangeAsync(int sheetIndex, int row, int column, int rowCount, int columnCount, Stream stream, TextFileSaveFlags flags, string rowDelimiter, string columnDelimiter, string cellDelimiter)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            return Sheets[sheetIndex].SaveTextFileRangeAsync(row, column, rowCount, columnCount, stream, flags, rowDelimiter, columnDelimiter, cellDelimiter);
        }

        /// <summary>
        /// Saves the data on the sheet to the specified XML stream asynchronously. 
        /// </summary>
        /// <param name="xmlStream">The XML stream.</param>
        /// <returns></returns>
        public IAsyncAction SaveXmlAsync(Stream xmlStream)
        {
            return SaveXmlAsync(xmlStream, false);
        }

        /// <summary>
        /// Saves the data on the sheet to the specified XML stream asynchronously. 
        /// </summary>
        /// <param name="xmlStream">The XML stream.</param>
        /// <param name="dataOnly">Whether to save data only.</param>
        /// <returns></returns>
        public IAsyncAction SaveXmlAsync(Stream xmlStream, bool dataOnly)
        {
            return AsyncInfo.Run(delegate (CancellationToken token)
            {
                return Task.Factory.StartNew(delegate
                {
                    SaveXmlBackGround(xmlStream, dataOnly);
                });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <param name="dataOnly"></param>
        void SaveXmlBackGround(Stream xmlStream, bool dataOnly)
        {
            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(xmlStream);
                if (writer != null)
                {
                    Serializer.WriteStartObj("Spread", writer);
                    WriteXmlInternal(writer);
                    Serializer.WriteStartObj("View", writer);
                    Serializer.SerializeObj(View, null, writer);
                    Serializer.WriteEndObj(writer);
                    Serializer.WriteStartObj("Data", writer);
                    Workbook.SaveXml(writer, dataOnly, false);
                    Serializer.WriteEndObj(writer);
                    Serializer.WriteEndObj(writer);
                }
            }
            catch (Exception exception)
            {
                while ((exception is TargetInvocationException) && (exception.InnerException != null))
                {
                    exception = exception.InnerException;
                }
                throw exception;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlStream"></param>
        internal void SaveXmlInternal(Stream xmlStream)
        {
            SaveXmlBackGround(xmlStream, false);
        }

        /// <summary>
        /// Searches the text in the cells in the specified sheet. 
        /// </summary>
        /// <param name="sheetIndex">The index of the sheet on which to search.</param>
        /// <param name="searchString">The string for which to search.</param>
        /// <param name="foundRowIndex">The index of the row at which a match is found.</param>
        /// <param name="foundColumnIndex">The index of the column at which a match is found.</param>
        public void Search(int sheetIndex, string searchString, out int foundRowIndex, out int foundColumnIndex)
        {
            Workbook.Search(sheetIndex, searchString, out foundRowIndex, out foundColumnIndex);
        }

        /// <summary>
        /// Searches the text in the cells in the specified sheet for the specified string with the specified criteria. 
        /// </summary>
        /// <param name="sheetIndex">The index of the sheet on which to search.</param>
        /// <param name="searchString">The string for which to search.</param>
        /// <param name="searchFlags">The search options.</param>
        /// <param name="foundRowIndex">The index of the row at which a match is found.</param>
        /// <param name="foundColumnIndex">The index of the column at which a match is found.</param>
        public void Search(int sheetIndex, string searchString, SearchFlags searchFlags, out int foundRowIndex, out int foundColumnIndex)
        {
            Workbook.Search(sheetIndex, searchString, searchFlags, out foundRowIndex, out foundColumnIndex);
        }

        /// <summary>
        /// Searches the text in the cells in the specified sheet for the specified string with the specified criteria. 
        /// </summary>
        /// <param name="sheetIndex">The index of the sheet on which to search.</param>
        /// <param name="searchString">The string for which to search.</param>
        /// <param name="searchFlags">The search options.</param>
        /// <param name="searchOrder">Whether to conduct the search by column, row coordinates or row, column coordinates.</param>
        /// <param name="foundRowIndex">The index of the row at which a match is found.</param>
        /// <param name="foundColumnIndex">The index of the column at which a match is found.</param>
        public void Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, out int foundRowIndex, out int foundColumnIndex)
        {
            Workbook.Search(sheetIndex, searchString, searchFlags, searchOrder, out foundRowIndex, out foundColumnIndex);
        }

        /// <summary>
        /// Searches the text in the cells in the specified sheet for the specified string with the specified criteria. 
        /// </summary>
        /// <param name="sheetIndex">The index of the sheet on which to search.</param>
        /// <param name="searchString">The string for which to search.</param>
        /// <param name="searchFlags">The SearchFlags enumeration that specifies the options of the search.</param>
        /// <param name="searchOrder">The SearchFlags enumeration that specifies whether the search goes by column, row coordinates or row, column coordinates.</param>
        /// <param name="searchTarget">The SearchFoundFlags enumeration that indicates whether the search includes the content in the cell notes, tags, or text.</param>
        /// <param name="foundRowIndex">The index of the row at which a match is found.</param>
        /// <param name="foundColumnIndex">The index of the column at which a match is found.</param>
        /// <returns>The found flags.</returns>
        public SearchFoundFlags Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, out int foundRowIndex, out int foundColumnIndex)
        {
            return Workbook.Search(sheetIndex, searchString, searchFlags, searchOrder, searchTarget, out foundRowIndex, out foundColumnIndex);
        }

        /// <summary>
        /// Searches the text in the cells in the specified sheet for the specified string with the specified criteria and whether to search notes and tags as well. 
        /// </summary>
        /// <param name="sheetIndex">The index of the sheet on which to search.</param>
        /// <param name="searchString">The string for which to search.</param>
        /// <param name="searchFlags">The SearchFlags enumeration that specifies the search options.</param>
        /// <param name="searchOrder">The SearchFlags enumeration that specifies whether the search goes by column, row coordinates or row, column coordinates.</param>
        /// <param name="searchTarget">The SearchFoundFlags enumeration that indicates whether the search includes the content in the cell notes, tags, or text.</param>
        /// <param name="sheetArea">The area of the sheet to search.</param>
        /// <param name="foundRowIndex">The index of the row at which a match is found.</param>
        /// <param name="foundColumnIndex">The index of the column at which a match is found.</param>
        /// <returns>The found flags.</returns>
        public SearchFoundFlags Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, out int foundRowIndex, out int foundColumnIndex)
        {
            return Workbook.Search(sheetIndex, searchString, searchFlags, searchOrder, searchTarget, sheetArea, out foundRowIndex, out foundColumnIndex);
        }

        /// <summary>
        /// Searches the text in the cells in the specified sheet for the specified string with the specified criteria and start location, and whether to search notes and tags as well. 
        /// </summary>
        /// <param name="sheetIndex">The index of the sheet on which to search.</param>
        /// <param name="searchString">The string for which to search.</param>
        /// <param name="searchFlags">The SearchFlags enumeration that specifies the search options.</param>
        /// <param name="searchOrder">The SearchFlags enumeration that specifies whether the search goes by column, row coordinates or row, column coordinates.</param>
        /// <param name="searchTarget">The SearchFoundFlags enumeration that indicates whether the search includes the content in the cell notes, tags, or text.</param>
        /// <param name="sheetArea">The area of the sheet to search.</param>
        /// <param name="rowStart">The index of the row at which to start.</param>
        /// <param name="columnStart">The index of the column at which to start.</param>
        /// <param name="foundRowIndex">The index of the row at which a match is found.</param>
        /// <param name="foundColumnIndex">The index of the column at which a match is found.</param>
        /// <returns>The found flags.</returns>
        public SearchFoundFlags Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, int rowStart, int columnStart, out int foundRowIndex, out int foundColumnIndex)
        {
            return Workbook.Search(sheetIndex, searchString, searchFlags, searchOrder, searchTarget, sheetArea, rowStart, columnStart, out foundRowIndex, out foundColumnIndex);
        }

        /// <summary>
        /// Searches the text in the cells in the specified sheet for the specified string with the specified criteria and start and end location, and whether to search notes and tags as well. 
        /// </summary>
        /// <param name="sheetIndex">The index of the sheet on which to search.</param>
        /// <param name="searchString">The string for which to search.</param>
        /// <param name="searchFlags">The SearchFlags enumeration that specifies the search options.</param>
        /// <param name="searchOrder">The SearchFlags enumeration that specifies whether the search goes by column, row coordinates or row, column coordinates.</param>
        /// <param name="searchTarget">The SearchFoundFlags enumeration that indicates whether the search includes the content in the cell notes, tags, or text.</param>
        /// <param name="sheetArea">The area of the sheet to search.</param>
        /// <param name="rowStart">The index of the row at which to start.</param>
        /// <param name="columnStart">The index of the column at which to start.</param>
        /// <param name="rowEnd">The index of the row at which to stop searching.</param>
        /// <param name="columnEnd">The index of the column at which to stop searching.</param>
        /// <param name="foundRowIndex">The index of the row at which a match is found.</param>
        /// <param name="foundColumnIndex">The index of the column at which a match is found.</param>
        /// <returns>Returns the found flags.</returns>
        public SearchFoundFlags Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, int rowStart, int columnStart, int rowEnd, int columnEnd, out int foundRowIndex, out int foundColumnIndex)
        {
            return Workbook.Search(sheetIndex, searchString, searchFlags, searchOrder, searchTarget, sheetArea, rowStart, columnStart, rowEnd, columnEnd, out foundRowIndex, out foundColumnIndex);
        }

        /// <summary>
        /// Searches the text in the cells in the specified sheet for the specified string with the specified criteria and start and end location, and whether to search notes and tags as well. 
        /// </summary>
        /// <param name="startSheetIndex">Index of the sheet on which to start the search.</param>
        /// <param name="endSheetIndex">Index of the sheet on which to end the search.</param>
        /// <param name="searchString">The string for which to search.</param>
        /// <param name="searchFlags">The SearchFlags enumeration that specifies the search options.</param>
        /// <param name="searchOrder">The SearchFlags enumeration that specifies whether the search goes by column, row coordinates or row, column coordinates.</param>
        /// <param name="searchTarget">The SearchFoundFlags enumeration that indicates whether the search includes the content in the cell notes, tags, or text.</param>
        /// <param name="sheetArea">The area of the sheet to search.</param>
        /// <param name="rowStart">The index of the row at which to start.</param>
        /// <param name="columnStart">The index of the column at which to start.</param>
        /// <param name="rowEnd">The index of the row at which to end.</param>
        /// <param name="columnEnd">The index of the column at which to end.</param>
        /// <param name="foundSheetIndex">The index of the sheet at which a match is found.</param>
        /// <param name="foundRowIndex">The row index at which a match is found.</param>
        /// <param name="foundColumnIndex">The column index at which a match is found.</param>
        /// <returns>A SearchFoundFlags enumeration that specifies what is matched. </returns>
        public SearchFoundFlags Search(int startSheetIndex, int endSheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, int rowStart, int columnStart, int rowEnd, int columnEnd, out int foundSheetIndex, out int foundRowIndex, out int foundColumnIndex)
        {
            return Workbook.Search(startSheetIndex, endSheetIndex, searchString, searchFlags, searchOrder, searchTarget, sheetArea, rowStart, columnStart, rowEnd, columnEnd, out foundSheetIndex, out foundRowIndex, out foundColumnIndex);
        }

        /// <summary>
        /// Searches the text in the cells in the specified sheet for the specified string with the specified criteria and start and end location, and whether to search notes and tags as well. 
        /// </summary>
        /// <param name="startSheetIndex">Index of the sheet on which to start the search.</param>
        /// <param name="endSheetIndex">Index of the sheet on which to end the search.</param>
        /// <param name="searchString">The string for which to search.</param>
        /// <param name="searchFlags">The SearchFlags enumeration that specifies the search options.</param>
        /// <param name="searchOrder">The SearchFlags enumeration that specifies whether the search goes by column, row coordinates or row, column coordinates.</param>
        /// <param name="searchTarget">The SearchFoundFlags enumeration that indicates whether the search includes the content in the cell notes, tags, or text.</param>
        /// <param name="sheetArea">The area of the sheet to search.</param>
        /// <param name="rowStart">The row index at which to start.</param>
        /// <param name="columnStart">The column index at which to start.</param>
        /// <param name="rowEnd">The row index at which to stop searching.</param>
        /// <param name="columnEnd">The column index at which to stop searching.</param>
        /// <param name="foundSheetIndex">The index of the sheet at which a match is found.</param>
        /// <param name="foundRowIndex">The index of the row at which a match is found.</param>
        /// <param name="foundColumnIndex">The index of the column at which a match is found.</param>
        /// <param name="foundString">The found string.</param>
        /// <returns>A SearchFoundFlags enumeration that specifies what is matched.</returns>
        public SearchFoundFlags Search(int startSheetIndex, int endSheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, int rowStart, int columnStart, int rowEnd, int columnEnd, out int foundSheetIndex, out int foundRowIndex, out int foundColumnIndex, out string foundString)
        {
            return Workbook.Search(startSheetIndex, endSheetIndex, searchString, searchFlags, searchOrder, searchTarget, sheetArea, rowStart, columnStart, rowEnd, columnEnd, out foundSheetIndex, out foundRowIndex, out foundColumnIndex, out foundString);
        }

        /// <summary>
        /// Activates a viewport in the control. 
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        public void SetActiveViewport(int rowViewportIndex, int columnViewportIndex)
        {
            View.SetActiveColumnViewportIndex(columnViewportIndex);
            View.SetActiveRowViewportIndex(rowViewportIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="rowViewportIndex"></param>
        /// <param name="columnViewportIndex"></param>
        internal void SetActiveViewport(int sheetIndex, int rowViewportIndex, int columnViewportIndex)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            if (sheetIndex == ActiveSheetIndex)
            {
                SetActiveViewport(rowViewportIndex, columnViewportIndex);
            }
            else
            {
                Sheets[sheetIndex].SetActiveViewport(rowViewportIndex, columnViewportIndex);
            }
        }

        /// <summary>
        /// Sets a custom name expression to the workbook. 
        /// </summary>
        /// <param name="name">The custom name.</param>
        /// <param name="baseRow">The row index.</param>
        /// <param name="baseColumn">The column index.</param>
        /// <param name="expression">The CalcExpression.</param>
        public void SetCustomName(string name, int baseRow, int baseColumn, CalcExpression expression)
        {
            Workbook.SetCustomName(name, baseRow, baseColumn, expression);
        }

        /// <summary>
        /// Sets a custom name formula to the workbook. 
        /// </summary>
        /// <param name="name">The custom name.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="baseRow">The row index.</param>
        /// <param name="baseColumn">The column index.</param>
        /// <param name="formula">The formula.</param>
        public void SetCustomName(string name, Worksheet worksheet, int baseRow, int baseColumn, string formula)
        {
            Workbook.SetCustomName(name, worksheet, baseRow, baseColumn, formula);
        }

        /// <summary>
        /// Sets a custom name external range expression to the workbook. 
        /// </summary>
        /// <param name="name">The custom name.</param>
        /// <param name="worksheet">The worksheet to create the expression.</param>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        public void SetCustomName(string name, Worksheet worksheet, int row, int column, int rowCount, int columnCount)
        {
            Workbook.SetCustomName(name, worksheet, row, column, rowCount, columnCount);
        }

        /// <summary>
        /// Sets the column viewport's left column. 
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <param name="value">The column index.</param>
        public void SetViewportLeftColumn(int columnViewportIndex, int value)
        {
            View.SetViewportLeftColumn(columnViewportIndex, value);
        }

        /// <summary>
        /// Sets the row viewport's top row. 
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <param name="value">The row index.</param>
        public void SetViewportTopRow(int rowViewportIndex, int value)
        {
            View.SetViewportTopRow(rowViewportIndex, value);
        }

        /// <summary>
        /// Moves the current active cell to the specified position. 
        /// </summary>
        /// <param name="verticalPosition">The VerticalPosition to show.</param>
        /// <param name="horizontalPosition">The HorizontalPosition to show.</param>
        public void ShowActiveCell(VerticalPosition verticalPosition, HorizontalPosition horizontalPosition)
        {
            if (ActiveSheet != null)
            {
                int activeRowIndex = ActiveSheet.ActiveRowIndex;
                int activeColumnIndex = ActiveSheet.ActiveColumnIndex;
                int activeRowViewportIndex = GetActiveRowViewportIndex();
                int activeColumnViewportIndex = GetActiveColumnViewportIndex();
                ShowCell(activeRowViewportIndex, activeColumnViewportIndex, activeRowIndex, activeColumnIndex, verticalPosition, horizontalPosition);
            }
        }

        /// <summary>
        ///Moves a cell to the specified position.  
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <param name="row">The cell row index to show.</param>
        /// <param name="column">The cell column index to show.</param>
        /// <param name="verticalPosition">The VerticalPosition to show.</param>
        /// <param name="horizontalPosition">The HorizontalPosition to show.</param>
        public void ShowCell(int rowViewportIndex, int columnViewportIndex, int row, int column, VerticalPosition verticalPosition, HorizontalPosition horizontalPosition)
        {
            if (ActiveSheet != null)
            {
                View.ShowCell(rowViewportIndex, columnViewportIndex, row, column, verticalPosition, horizontalPosition);
            }
        }

        /// <summary>
        /// Moves a column to the specified position. 
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <param name="column">The column index to show.</param>
        /// <param name="horizontalPosition">The HorizontalPosition to show.</param>
        public void ShowColumn(int columnViewportIndex, int column, HorizontalPosition horizontalPosition)
        {
            Worksheet activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                SpreadLayout spreadLayout = View.GetSpreadLayout();
                if ((columnViewportIndex < -1) || (columnViewportIndex > spreadLayout.ColumnPaneCount))
                {
                    throw new ArgumentOutOfRangeException("columnViewportIndex");
                }
                if ((columnViewportIndex != -1) && (columnViewportIndex != spreadLayout.ColumnPaneCount))
                {
                    if ((column < 0) || (column >= activeSheet.ColumnCount))
                    {
                        throw new ArgumentOutOfRangeException("column");
                    }
                    if (!Enum.IsDefined((Type)typeof(HorizontalPosition), horizontalPosition))
                    {
                        throw new ArgumentException(ResourceStrings.gcSpreadInvalidHorizontalPosition);
                    }
                    View.ShowColumn(columnViewportIndex, column, horizontalPosition);
                }
            }
        }

        /// <summary>
        /// Moves a row to the specified position. 
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <param name="row">The row index to show.</param>
        /// <param name="verticalPosition">The VerticalPosition to show.</param>
        public void ShowRow(int rowViewportIndex, int row, VerticalPosition verticalPosition)
        {
            Worksheet activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                SpreadLayout spreadLayout = View.GetSpreadLayout();
                if ((rowViewportIndex < -1) || (rowViewportIndex > spreadLayout.RowPaneCount))
                {
                    throw new ArgumentOutOfRangeException("rowViewportIndex");
                }
                if ((rowViewportIndex != -1) && (rowViewportIndex != spreadLayout.RowPaneCount))
                {
                    if ((row < 0) || (row >= activeSheet.RowCount))
                    {
                        throw new ArgumentOutOfRangeException("row");
                    }
                    if (!Enum.IsDefined((Type)typeof(VerticalPosition), verticalPosition))
                    {
                        throw new ArgumentException(ResourceStrings.gcSpreadInvalidVerticalPosition);
                    }
                    View.ShowRow(rowViewportIndex, row, verticalPosition);
                }
            }
        }

        /// <summary>
        /// Suspends the calculation service. 
        /// </summary>
        public void SuspendCalcService()
        {
            Workbook.SuspendCalcService();
        }

        /// <summary>
        /// Suspends the event. 
        /// </summary>
        public void SuspendEvent()
        {
            Workbook.SuspendEvent();
            View.SuspendEvent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void WriteXmlInternal(XmlWriter writer)
        {
        }
    }
}
