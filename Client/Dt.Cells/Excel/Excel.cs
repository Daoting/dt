#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-17 创建
******************************************************************************/
#endregion

#region 名称空间
using Dt.CalcEngine.Expressions;
using Dt.CalcEngine.Functions;
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Represents a Excel control.
    /// </summary>
    public partial class Excel : Panel, IXmlSerializable
    {
        #region 构造方法
        public Excel()
        {
            Workbook = new Workbook();
            var sheet = new Worksheet();
            AttachSheet(sheet);
            Workbook.Sheets.Add(sheet);
            Workbook.Sheets.CollectionChanged += OnSheetsCollectionChanged;
            Workbook.PropertyChanged += OnWorkbookPropertyChanged;

            _invisibleRows = new HashSet<int>();
            _invisibleColumns = new HashSet<int>();

            InitPointer();
            InitKeyboard();
            InitLayout();
        }
        #endregion

        #region 属性
        /// <summary>
        /// Gets the workbook associated with the control. 
        /// </summary>
        public Workbook Workbook { get; }

        /// <summary>
        /// Gets the active sheet in the Excel control. 
        /// </summary>
        public Worksheet ActiveSheet
        {
            get { return Workbook.ActiveSheet; }
        }

        /// <summary>
        /// Gets or sets the index of the active sheet in the Excel control. 
        /// </summary>
        public int ActiveSheetIndex
        {
            get { return Workbook.ActiveSheetIndex; }
            set
            {
                if ((Workbook.ActiveSheetIndex != value) && !RaiseActiveSheetIndexChanging())
                {
                    Workbook.ActiveSheetIndex = value;
                    RaiseActiveSheetIndexChanged();
                    RefreshAll();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the formula is automatically calculated. 
        /// </summary>
        public bool AutoRecalculation
        {
            get { return Workbook.AutoRecalculation; }
            set { Workbook.AutoRecalculation = value; }
        }

        /// <summary>
        ///Gets or sets whether the Excel can auto-refresh itself.  
        /// </summary>
        public bool AutoRefresh
        {
            get { return Workbook.AutoRefresh; }
            set { Workbook.AutoRefresh = value; }
        }

        /// <summary>
        /// Gets or sets whether data can overflow into adjacent empty cells in the component. 
        /// </summary>
        public bool CanCellOverflow
        {
            get { return Workbook.CanCellOverflow; }
            set { Workbook.CanCellOverflow = value; }
        }

        /// <summary>
        /// Gets or sets the current theme information for the control.
        /// </summary>
        public SpreadTheme CurrentTheme
        {
            get { return Workbook.CurrentTheme; }
            set { Workbook.CurrentTheme = value; }
        }

        /// <summary>
        /// Gets or sets the current theme information for the control. 
        /// </summary>
        public string CurrentThemeName
        {
            get { return Workbook.CurrentThemeName; }
            set { Workbook.CurrentThemeName = value; }
        }

        /// <summary>
        /// Gets or sets the gridline color. 
        /// </summary>
        public Color GridLineColor
        {
            get { return Workbook.GridLineColor; }
            set
            {
                Workbook.GridLineColor = value;
                RefreshAll();
            }
        }

        /// <summary>
        /// Gets or sets the horizontal scroll bar visibility setting. 
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return Workbook.HorizontalScrollBarVisibility; }
            set
            {
                Workbook.HorizontalScrollBarVisibility = value;
                HorizontalScrollable = value != 0;
                InvalidateLayout();
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets a collection of StyleInfo objects for this sheet. 
        /// </summary>
        /// <value>
        /// The collection of StyleInfo objects for this sheet.
        /// </value>
        public StyleInfoCollection NamedStyles
        {
            get { return Workbook.NamedStyles; }
            set
            {
                Workbook.NamedStyles = value;
                RefreshAll();
            }
        }

        /// <summary>
        /// Gets or sets whether this workbook is protected.  
        /// </summary>
        public bool Protect
        {
            get
            {
                var sheet = Workbook.ActiveSheet;
                if (sheet != null)
                    return sheet.Protect;
                return false;
            }
            set
            {
                var sheet = Workbook.ActiveSheet;
                if (sheet != null)
                    sheet.Protect = value;
            }
        }

        /// <summary>
        /// 在报表预览中实现单元格不可编辑且图表可拖动
        /// </summary>
        public bool LockCell
        {
            get
            {
                var sheet = Workbook.ActiveSheet;
                if (sheet != null)
                    return sheet.LockCell;
                return false;
            }
            set
            {
                var sheet = Workbook.ActiveSheet;
                if (sheet != null)
                    sheet.LockCell = value;
            }
        }

        /// <summary>
        /// Gets or sets the style for cell and range references in cell formulas on this sheet. 
        /// </summary>
        public ReferenceStyle ReferenceStyle
        {
            get { return Workbook.ReferenceStyle; }
            set { Workbook.ReferenceStyle = value; }
        }

        /// <summary>
        /// Gets or sets the number of sheets for this control. 
        /// </summary>
        /// <value>
        /// The number of sheets for this control.
        /// </value>
        public int SheetCount
        {
            get { return Workbook.SheetCount; }
            set
            {
                Workbook.SheetCount = value;
                RefreshAll();
            }
        }

        /// <summary>
        /// Gets or sets how to display the scroll tip. 
        /// </summary>
        public ShowScrollTip ShowScrollTip
        {
            get { return Workbook.ShowScrollTip; }
            set { Workbook.ShowScrollTip = value; }
        }

        /// <summary>
        /// Gets or sets the index of the start sheet in the Excel control. 
        /// </summary>
        public int StartSheetIndex
        {
            get { return Workbook.StartSheetIndex; }
            set
            {
                if (Workbook.StartSheetIndex != value)
                {
                    Workbook.StartSheetIndex = value;
                    ProcessStartSheetIndexChanged();
                }
            }
        }

        /// <summary>
        ///Gets themes for the control.  
        /// </summary>
        public SpreadThemes Themes
        {
            get { return Workbook.Themes; }
        }
        /// <summary>
        /// Gets or sets the Worksheet collection for the Excel control. 
        /// </summary>
        public WorksheetCollection Sheets
        {
            get { return Workbook.Sheets; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to display the drag drop tip. 
        /// </summary>
        /// <value>
        /// true to display the drag drop tip; otherwise, false. 
        /// </value>
        public bool ShowDragDropTip
        {
            get { return Workbook.ShowDragDropTip; }
            set { Workbook.ShowDragDropTip = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show the drag fill tip. 
        /// </summary>
        /// <value>
        /// true to show the drag fill tip; otherwise, false. 
        /// </value>
        public bool ShowDragFillTip
        {
            get { return Workbook.ShowDragFillTip; }
            set { Workbook.ShowDragFillTip = value; }
        }

        /// <summary>
        /// Gets or sets whether gridlines are displayed. 
        /// </summary>
        public bool ShowGridLine
        {
            get { return Workbook.ShowGridLine; }
            set
            {
                Workbook.ShowGridLine = value;
                RefreshAll();
            }
        }

        /// <summary>
        /// Gets or sets how to display the resize tip. 
        /// </summary>
        public ShowResizeTip ShowResizeTip
        {
            get { return Workbook.ShowResizeTip; }
            set { Workbook.ShowResizeTip = value; }
        }

        /// <summary>
        /// Gets or sets the vertical scroll bar visibility setting. 
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return Workbook.VerticalScrollBarVisibility; }
            set
            {
                Workbook.VerticalScrollBarVisibility = value;
                VerticalScrollable = value != 0;
                InvalidateLayout();
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets whether the component handles the shortcut keys for Clipboard actions. 
        /// </summary>
        public bool AutoClipboard
        {
            get { return (bool)GetValue(AutoClipboardProperty); }
            set { SetValue(AutoClipboardProperty, value); }
        }

        /// <summary>
        /// Indicates whether the user can select multiple ranges by touch.
        /// </summary>
        public bool CanTouchMultiSelect
        {
            get { return (bool)GetValue(CanTouchMultiSelectProperty); }
            set { SetValue(CanTouchMultiSelectProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to allow users to drag and drop a range.
        /// </summary>
        public bool CanUserDragDrop
        {
            get { return (bool)GetValue(CanUserDragDropProperty); }
            set { SetValue(CanUserDragDropProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to allow users to drag and fill a range.
        /// </summary>
        public bool CanUserDragFill
        {
            get { return (bool)GetValue(CanUserDragFillProperty); }
            set { SetValue(CanUserDragFillProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to allow the user to enter formulas in a cell in the component.
        /// </summary>
        public bool CanUserEditFormula
        {
            get { return (bool)GetValue(CanUserEditFormulaProperty); }
            set { SetValue(CanUserEditFormulaProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to allow the user to undo edit operations.
        /// </summary>
        public bool CanUserUndo
        {
            get { return (bool)GetValue(CanUserUndoProperty); }
            set { SetValue(CanUserUndoProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the user can scale the display of the component using the Ctrl key and the mouse wheel. 
        /// </summary>
        public bool CanUserZoom
        {
            get { return (bool)GetValue(CanUserZoomProperty); }
            set { SetValue(CanUserZoomProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the component handles the shortcut keys for Clipboard actions. 
        /// </summary>
        public ClipboardPasteOptions ClipBoardOptions
        {
            get { return (ClipboardPasteOptions)GetValue(ClipBoardOptionsProperty); }
            set { SetValue(ClipBoardOptionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the column split box alignment. 
        /// </summary>
        /// <value>
        /// The column split box alignment. 
        /// </value>
        public SplitBoxAlignment ColumnSplitBoxAlignment
        {
            get { return (SplitBoxAlignment)GetValue(ColumnSplitBoxAlignmentProperty); }
            set { SetValue(ColumnSplitBoxAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets what conditions under which the GcSpreadSheet component permits column splits. 
        /// </summary>
        public SplitBoxPolicy ColumnSplitBoxPolicy
        {
            get { return (SplitBoxPolicy)GetValue(ColumnSplitBoxPolicyProperty); }
            set { SetValue(ColumnSplitBoxPolicyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the horizontal scroll bar. 
        /// </summary>
        /// <value>
        /// The height of the horizontal scroll bar. 
        /// </value>
        public double HorizontalScrollBarHeight
        {
            get { return (double)GetValue(HorizontalScrollBarHeightProperty); }
            set { SetValue(HorizontalScrollBarHeightProperty, value); }
        }

        /// <summary>
        /// Gets the information of the editor when the sheetview enters the formula selection mode.
        /// </summary>
        public EditorInfo EditorInfo
        {
            get
            {
                if (_editorInfo == null)
                {
                    _editorInfo = new EditorInfo(this);
                }
                return _editorInfo;
            }
        }

        /// <summary>
        /// Gets or sets the default type of the automatic fill.
        /// </summary>
        /// <value>
        /// The default type of the automatic fill.
        /// </value>
        public AutoFillType? DefaultAutoFillType
        {
            get { return (AutoFillType?)GetValue(DefaultAutoFillTypeProperty); }
            set { SetValue(DefaultAutoFillTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the document uri of the sheet. 
        /// </summary>
        /// <value>
        /// The document uri of the sheet.
        /// </value>
        public Uri DocumentUri
        {
            get { return (Uri)GetValue(DocumentUriProperty); }
            set { SetValue(DocumentUriProperty, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal scroll bar style. 
        /// </summary>
        public Style HorizontalScrollBarStyle
        {
            get { return (Style)GetValue(HorizontalScrollBarStyleProperty); }
            set { SetValue(HorizontalScrollBarStyleProperty, value); }
        }

        /// <summary>
        /// Returns the last input device type.
        /// </summary>
        public InputDeviceType InputDeviceType
        {
            get { return _inputDeviceType; }
            internal set { _inputDeviceType = value; }
        }

        /// <summary>
        /// Gets or sets the backgroud of the range group
        /// </summary>
        public Brush RangeGroupBackground
        {
            get { return (Brush)GetValue(RangeGroupBackgroundProperty); }
            set { SetValue(RangeGroupBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush of the border of the range group 
        /// </summary>
        public Brush RangeGroupBorderBrush
        {
            get { return (Brush)GetValue(RangeGroupBorderBrushProperty); }
            set { SetValue(RangeGroupBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke of the group line 
        /// </summary>
        public Brush RangeGroupLineStroke
        {
            get { return (Brush)GetValue(RangeGroupLineStrokeProperty); }
            set { SetValue(RangeGroupLineStrokeProperty, value); }
        }

        /// <summary>
        ///Specifies the drawing policy when the row or column is resized to zero.  
        /// </summary>
        public ResizeZeroIndicator ResizeZeroIndicator
        {
            get { return (ResizeZeroIndicator)GetValue(ResizeZeroIndicatorProperty); }
            set { SetValue(ResizeZeroIndicatorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the row split box alignment. 
        /// </summary>
        public SplitBoxAlignment RowSplitBoxAlignment
        {
            get { return (SplitBoxAlignment)GetValue(RowSplitBoxAlignmentProperty); }
            set { SetValue(RowSplitBoxAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets under which conditions the Excel component permits row splits. 
        /// </summary>
        public SplitBoxPolicy RowSplitBoxPolicy
        {
            get { return (SplitBoxPolicy)GetValue(RowSplitBoxPolicyProperty); }
            set { SetValue(RowSplitBoxPolicyProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the sheet in the control scrolls when the user moves the scroll box. 
        /// </summary>
        public ScrollBarTrackPolicy ScrollBarTrackPolicy
        {
            get { return (ScrollBarTrackPolicy)GetValue(ScrollBarTrackPolicyProperty); }
            set { SetValue(ScrollBarTrackPolicyProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the column range group is visible. 
        /// </summary>
        public bool ShowColumnRangeGroup
        {
            get { return (bool)GetValue(ShowColumnRangeGroupProperty); }
            set { SetValue(ShowColumnRangeGroupProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show freeze lines. 
        /// </summary>
        public bool ShowFreezeLine
        {
            get { return (bool)GetValue(ShowFreezeLineProperty); }
            set { SetValue(ShowFreezeLineProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the row range group is visible. 
        /// </summary>
        public bool ShowRowRangeGroup
        {
            get { return (bool)GetValue(ShowRowRangeGroupProperty); }
            set { SetValue(ShowRowRangeGroupProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the tab strip is editable. 
        /// </summary>
        public bool TabStripEditable
        {
            get { return (bool)GetValue(TabStripEditableProperty); }
            set { SetValue(TabStripEditableProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether a special tab is displayed that allows inserting new sheets. 
        /// </summary>
        public bool TabStripInsertTab
        {
            get { return (bool)GetValue(TabStripInsertTabProperty); }
            set { SetValue(TabStripInsertTabProperty, value); }
        }

        /// <summary>
        /// Gets or sets the display policy for the sheet tab strip for this component. 
        /// </summary>
        public Visibility TabStripVisibility
        {
            get { return (Visibility)GetValue(TabStripVisibilityProperty); }
            set { SetValue(TabStripVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets the undo manager for the control.
        /// </summary>
#if IOS
        new
#endif
        public UndoManager UndoManager
        {
            get
            {
                if (_undoManager == null)
                {
                    _undoManager = new UndoManager(this, -1, CanUserUndo);
                }
                return _undoManager;
            }
        }

        /// <summary>
        /// Gets or sets the vertical scroll bar style.
        /// </summary>
        public Style VerticalScrollBarStyle
        {
            get { return (Style)GetValue(VerticalScrollBarStyleProperty); }
            set { SetValue(VerticalScrollBarStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of vertical scroll bars in this control. 
        /// </summary>
        public double VerticalScrollBarWidth
        {
            get { return (double)GetValue(VerticalScrollBarWidthProperty); }
            set { SetValue(VerticalScrollBarWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the horizontal scroll bar is scrollable.
        /// </summary>
        /// <value>
        /// <c>true</c> if the horizontal scroll bar is scrollable; otherwise, <c>false</c>.
        /// </value>
        public bool HorizontalScrollable
        {
            get { return (bool)GetValue(HorizontalScrollableProperty); }
            set { SetValue(HorizontalScrollableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the vertical scroll bar is scrollable.
        /// </summary>
        /// <value>
        /// <c>true</c> if the vertical scroll bar is scrollable; otherwise, <c>false</c>.
        /// </value>
        public bool VerticalScrollable
        {
            get { return (bool)GetValue(VerticalScrollableProperty); }
            set { SetValue(VerticalScrollableProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether data can overflow into adjacent empty cells in the component while the cell is in edit mode. 
        /// </summary>
        public bool CanEditOverflow
        {
            get { return (bool)GetValue(CanEditOverflowProperty); }
            set { SetValue(CanEditOverflowProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scaling factor for displaying this sheet.
        /// </summary>
        /// <value>The scaling factor for displaying this sheet.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// Specified scaling amount is out of range; must be between 0.5 (50%) and 4.0 (400%).
        /// </exception>
        public float ZoomFactor
        {
            get
            {
                if (ActiveSheet != null)
                {
                    return ActiveSheet.ZoomFactor;
                }
                return 1f;
            }
            set
            {
                if (ActiveSheet != null)
                {
                    ActiveSheet.ZoomFactor = value;
                    RefreshRange(-1, -1, -1, -1, SheetArea.Cells);
                    RefreshRange(-1, -1, -1, -1, SheetArea.ColumnHeader);
                    RefreshRange(-1, -1, -1, -1, SheetArea.CornerHeader | SheetArea.RowHeader);
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 获取设置是否显示选择区域，默认true
        /// </summary>
        public bool ShowSelection
        {
            get { return (bool)GetValue(ShowSelectionProperty); }
            set { SetValue(ShowSelectionProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示修饰层
        /// </summary>
        public bool ShowDecoration
        {
            get { return (bool)GetValue(ShowDecorationProperty); }
            set { SetValue(ShowDecorationProperty, value); }
        }

        /// <summary>
        /// 获取设置页面大小，修饰层画线用
        /// </summary>
        public Size PaperSize
        {
            get { return _paperSize; }
            set
            {
                if (_paperSize != value)
                {
                    _paperSize = value;
                    InvalidateDecoration();
                }
            }
        }

        /// <summary>
        /// 获取设置修饰区域
        /// </summary>
        public CellRange DecorationRange
        {
            get { return _decorationRange; }
            set
            {
                if (_decorationRange != value)
                {
                    _decorationRange = value;
                    InvalidateDecoration();
                }
            }
        }

        /// <summary>
        /// 获取设置是否忙状态
        /// </summary>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        #endregion

        #region 打开保存
        /// <summary>
        /// Opens an Excel Compound Document File and loads it into Sheet. 
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="openFlags">The flag used to open the file.</param>
        /// <returns></returns>
        public Task OpenExcel(Stream stream, ExcelOpenFlags openFlags = ExcelOpenFlags.NoFlagsSet)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            return OpenStream(() => Workbook.OpenExcel(stream, openFlags));
        }

        /// <summary>
        /// Saves Sheet to an Excel Compound Document File. 
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <param name="format">The file format.</param>
        /// <param name="saveFlags">Options for saving to a file.</param>
        /// <returns></returns>
        public IAsyncAction SaveExcel(Stream stream, ExcelFileFormat format = ExcelFileFormat.XLSX, ExcelSaveFlags saveFlags = ExcelSaveFlags.NoFlagsSet)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Workbook.SaveExcel(stream, format, saveFlags));
        }

        /// <summary>
        /// Loads the data on the sheet from the specified XML stream. 
        /// </summary>
        /// <param name="xmlStream">The XML stream.</param>
        /// <returns></returns>
        public Task OpenXml(Stream xmlStream)
        {
            if (xmlStream == null)
                throw new ArgumentNullException("xmlStream");
            return OpenStream(() => OpenXmlOnBackground(xmlStream));
        }

        /// <summary>
        /// Saves the data on the sheet to the specified XML stream asynchronously. 
        /// </summary>
        /// <param name="xmlStream">The XML stream.</param>
        /// <param name="dataOnly">Whether to save data only.</param>
        /// <returns></returns>
        public IAsyncAction SaveXmlAsync(Stream xmlStream, bool dataOnly = false)
        {
            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => SaveXmlBackground(xmlStream, dataOnly));
        }

        /// <summary>
        /// Saves the content of the component to the specified stream asynchronously. 
        /// </summary>
        /// <param name="stream">Stream to which to save the data.</param>
        /// <param name="sheetIndexes">The sheet indexes.</param>
        /// <param name="settings">The export settings.</param>
        /// <returns></returns>
        public IAsyncAction SavePdf(Stream stream, int[] sheetIndexes = null, PdfExportSettings settings = null)
        {
            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Workbook.SavePdf(stream, sheetIndexes, settings));
        }

        /// <summary>
        /// Loads the CSV (comma-separated values) file asynchronously. 
        /// </summary>
        /// <param name="sheetIndex">The destination sheet index for loading.</param>
        /// <param name="stream">Stream from which to load.</param>
        /// <param name="flags">The import flags.</param>
        /// <returns></returns>
        public Task OpenCSV(int sheetIndex, Stream stream, TextFileOpenFlags flags = TextFileOpenFlags.None)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount) || stream == null)
                throw new ArgumentOutOfRangeException("sheetIndex");
            return OpenStream(() => Sheets[sheetIndex].OpenCsv(stream, flags));
        }

        /// <summary>
        /// Saves the CSV (comma-separated values) file asynchronously. 
        /// </summary>
        /// <param name="sheetIndex">The destination sheet index for saving.</param>
        /// <param name="stream">Stream to which to save the content.</param>
        /// <param name="flags">The export flags.</param>
        /// <returns></returns>
        public IAsyncAction SaveCSV(int sheetIndex, Stream stream, TextFileSaveFlags flags = TextFileSaveFlags.None)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
                throw new ArgumentOutOfRangeException("sheetIndex");
            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Sheets[sheetIndex].SaveCsv(stream, flags, Encoding.UTF8));
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
        public Task OpenTextFile(
            int sheetIndex,
            Stream stream,
            TextFileOpenFlags flags,
            string rowDelimiter,
            string columnDelimiter,
            string cellDelimiter)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SheetCount))
                throw new ArgumentOutOfRangeException("sheetIndex");
            return OpenStream(() => Sheets[sheetIndex].OpenTextFile(stream, flags, rowDelimiter, columnDelimiter, cellDelimiter));
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
                throw new ArgumentOutOfRangeException("sheetIndex");
            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Sheets[sheetIndex].SaveTextFileRange(row, column, rowCount, columnCount, stream, flags, rowDelimiter, columnDelimiter, cellDelimiter));
        }
        #endregion

        #region 重绘方法
        /// <summary>
        /// 延迟重绘
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// using (_excel.Defer())
        /// {
        ///     _excel.Sheets.AddTable("sampleTable1", 22, 5, 10, 5, TableStyles.Medium3);
        /// }
        /// </code>
        /// </example>
        public IDisposable Defer()
        {
            return new Deferral(this);
        }

        /// <summary>
        /// 刷新整个可视区域，全部重新测量布局
        /// </summary>
        public void RefreshAll()
        {
            if (IsSuspendInvalidate())
                return;

            if (IsEditing)
                StopCellEditing(true);
            InvalidateLayout();
            Children.Clear();
            _tabStrip?.UpdateTabs();

            _cornerPanel = null;
            _rowHeaders = null;
            _colHeaders = null;
            _cellsPanels = null;

            _groupCornerPresenter = null;
            _rowGroupHeaderPresenter = null;
            _columnGroupHeaderPresenter = null;
            _rowGroupPresenters = null;
            _columnGroupPresenters = null;
            _columnFreezeLine = null;
            _columnTrailingFreezeLine = null;
            _rowFreezeLine = null;
            _rowTrailingFreezeLine = null;

            _tooltipHelper = null;
            _currentActiveColumnIndex = (ActiveSheet == null) ? -1 : ActiveSheet.ActiveColumnIndex;
            _currentActiveRowIndex = (ActiveSheet == null) ? -1 : ActiveSheet.ActiveRowIndex;
            Navigation.UpdateStartPosition(_currentActiveRowIndex, _currentActiveColumnIndex);
        }

        /// <summary>
        /// Invalidates the charts.
        /// </summary>
        public void RefreshCharts()
        {
            if ((ActiveSheet != null) && (ActiveSheet.Charts.Count > 0))
            {
                RefreshCharts(ActiveSheet.Charts.ToArray());
            }
        }

        /// <summary>
        /// Invalidates the charts.
        /// </summary>
        /// <param name="charts">The charts.</param>
        public void RefreshCharts(params SpreadChart[] charts)
        {
            InvalidateFloatingObjectLayout();
            foreach (SpreadChart chart in charts)
            {
                RefreshViewportFloatingObjects(chart);
            }
        }

        /// <summary>
        /// Invalidates a range state in the control; the range layout and data is updated after the invalidation.
        /// </summary>
        /// <param name="row">The start row index.</param>
        /// <param name="column">The start column index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="sheetArea">The invalidated sheet area.</param>
        public void RefreshRange(int row, int column, int rowCount, int columnCount, SheetArea sheetArea = SheetArea.Cells)
        {
            if (!IsSuspendInvalidate())
            {
                if ((row < 0) || (column < 0))
                {
                    InvalidateLayout();
                }
                _cachedFilterButtonInfoModel = null;
                InvalidateMeasure();
                Worksheet worksheet = ActiveSheet;
                if (((byte)(sheetArea & SheetArea.Cells)) == 1)
                {
                    if (row < 0)
                    {
                        row = 0;
                        rowCount = (worksheet == null) ? 0 : worksheet.RowCount;
                    }
                    if (column < 0)
                    {
                        column = 0;
                        columnCount = (worksheet == null) ? 0 : worksheet.ColumnCount;
                    }
                    _cachedViewportCellLayoutModel = null;
                    RefreshViewportCells(_cellsPanels, row, column, rowCount, columnCount);
                }
                if (((byte)(sheetArea & SheetArea.ColumnHeader)) == 4)
                {
                    if (row < 0)
                    {
                        row = 0;
                        rowCount = (worksheet == null) ? 0 : worksheet.RowCount;
                    }
                    if (column < 0)
                    {
                        column = 0;
                        columnCount = (worksheet == null) ? 0 : worksheet.ColumnCount;
                    }
                    _cachedColumnHeaderCellLayoutModel = null;
                    RefreshHeaderCells(_colHeaders, row, column, rowCount, columnCount);
                }
                if (((byte)(sheetArea & (SheetArea.CornerHeader | SheetArea.RowHeader))) == 2)
                {
                    if (row < 0)
                    {
                        row = 0;
                        rowCount = (worksheet == null) ? 0 : worksheet.RowCount;
                    }
                    if (column < 0)
                    {
                        column = 0;
                        columnCount = (worksheet == null) ? 0 : worksheet.ColumnCount;
                    }
                    _cachedRowHeaderCellLayoutModel = null;
                    RefreshHeaderCells(_rowHeaders, row, column, rowCount, columnCount);
                }
            }
        }

        /// <summary>
        /// Invalidates the row state in the control. After the invalidation, the row layout and data are updated. 
        /// </summary>
        /// <param name="row">The start row index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="sheetArea">The invalid sheet area.</param>
        public void RefreshRows(int row, int rowCount, SheetArea sheetArea = SheetArea.Cells)
        {
            if (!IsSuspendInvalidate())
            {
                RefreshRange(row, -1, rowCount, -1, sheetArea);
            }
        }

        /// <summary>
        /// Invalidates the column state in the control. After the invalidation, the column layout and data are updated. 
        /// </summary>
        /// <param name="column">The start column index.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="sheetArea">The invalid sheet area</param>
        public void RefreshColumns(int column, int columnCount, SheetArea sheetArea = SheetArea.Cells)
        {
            if (!IsSuspendInvalidate())
            {
                RefreshRange(-1, column, -1, columnCount, sheetArea);
            }
        }

        /// <summary>
        /// Invalidates the custom floating objects.
        /// </summary>
        public void RefreshCustomFloatingObjects()
        {
            if ((ActiveSheet != null) && (ActiveSheet.FloatingObjects.Count > 0))
            {
                List<CustomFloatingObject> list = new List<CustomFloatingObject>();
                foreach (FloatingObject obj2 in ActiveSheet.FloatingObjects)
                {
                    if (obj2 is CustomFloatingObject)
                    {
                        list.Add(obj2 as CustomFloatingObject);
                    }
                }
                RefreshCustomFloatingObjects(list.ToArray());
            }
        }

        /// <summary>
        /// Invalidates the custom floating objects.
        /// </summary>
        /// <param name="floatingObjects">The floating objects.</param>
        public void RefreshCustomFloatingObjects(params CustomFloatingObject[] floatingObjects)
        {
            InvalidateFloatingObjectLayout();
            foreach (CustomFloatingObject obj2 in floatingObjects)
            {
                RefreshViewportFloatingObjects(obj2);
            }
        }

        /// <summary>
        /// Invalidates the charts.
        /// </summary>
        public void RefreshFloatingObjects()
        {
            InvalidateFloatingObjectLayout();
            RefreshViewportFloatingObjects();
        }

        /// <summary>
        /// Invalidates the floating object.
        /// </summary>
        /// <param name="floatingObjects">The floating objects.</param>
        public void RefreshFloatingObjects(params FloatingObject[] floatingObjects)
        {
            InvalidateFloatingObjectLayout();
            foreach (FloatingObject obj2 in floatingObjects)
            {
                RefreshViewportFloatingObjects(obj2);
            }
        }

        /// <summary>
        /// Invalidates the pictures.
        /// </summary>
        public void RefreshPictures()
        {
            if ((ActiveSheet != null) && (ActiveSheet.Pictures.Count > 0))
            {
                RefreshPictures(ActiveSheet.Pictures.ToArray());
            }
        }

        /// <summary>
        /// Invalidates the pictures.
        /// </summary>
        /// <param name="pictures">The pictures.</param>
        public void RefreshPictures(params Picture[] pictures)
        {
            InvalidateFloatingObjectLayout();
            foreach (Picture picture in pictures)
            {
                RefreshViewportFloatingObjects(picture);
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// Adds a new column viewport to the control. 
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index to add.</param>
        /// <param name="viewportWidth">The column viewport width.</param>
        public void AddColumnViewport(int columnViewportIndex, double viewportWidth)
        {
            ActiveSheet.AddColumnViewport(columnViewportIndex, viewportWidth / ((double)ZoomFactor));
            InvalidateLayout();
            InvalidateMeasure();
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
            ActiveSheet.AddRowViewport(rowViewportIndex, viewportHeight / ((double)ZoomFactor));
            InvalidateLayout();
            InvalidateMeasure();
        }

        /// <summary>
        /// Automatically fits the viewport column.
        /// </summary>
        /// <param name="column">The column index to automatically fit.</param>
        /// <param name="isRowHeader">The flag indicates whether sheetArea is a row header.</param>
        public void AutoFitColumn(int column, bool isRowHeader = false)
        {
            if ((column < 0) || (column >= ActiveSheet.ColumnCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            AutoFitColumnInternal(column, false, isRowHeader);
        }

        /// <summary>
        /// Automatically fits the viewport row.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="isColumnHeader">The flag indicates whether sheetArea is a column header.</param>
        public void AutoFitRow(int row, bool isColumnHeader = false)
        {
            if ((row < 0) || (row > ActiveSheet.RowCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            AutoFitRowInternal(row, false, isColumnHeader);
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
        /// Executes a command. 
        /// </summary>
        /// <param name="command">The command.</param>
        public bool DoCommand(ICommand command)
        {
            return UndoManager.Do(command);
        }

        /// <summary>
        /// Gets the current active column viewport index in the control. 
        /// </summary>
        /// <returns>The active column viewport index.</returns>
        public int GetActiveColumnViewportIndex()
        {
            return ActiveSheet.GetActiveColumnViewportIndex();
        }

        /// <summary>
        /// Gets the current active row viewport index in the control. 
        /// </summary>
        /// <returns>The active row viewport index.</returns>
        public int GetActiveRowViewportIndex()
        {
            return ActiveSheet.GetActiveRowViewportIndex();
        }

        /// <summary>
        ///Gets the column viewport count in the control.  
        /// </summary>
        /// <returns>The column viewport count.</returns>
        public int GetColumnViewportCount()
        {
            return GetViewportInfo().ColumnViewportCount;
        }

        /// <summary>
        /// Gets the row viewport count in the control. 
        /// </summary>
        /// <returns>The row viewport count.</returns>
        public int GetRowViewportCount()
        {
            return GetViewportInfo().RowViewportCount;
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
        /// Gets the row viewport's bottom row index. 
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <returns></returns>
        public int GetViewportBottomRow(int rowViewportIndex)
        {
            return GetViewportBottomRow(ActiveSheet, rowViewportIndex);
        }

        /// <summary>
        /// Gets the column viewport's left column index. 
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <returns></returns>
        public int GetViewportLeftColumn(int columnViewportIndex)
        {
            return ActiveSheet.GetViewportLeftColumn(columnViewportIndex);
        }

        /// <summary>
        /// Gets the column viewport's right column index. 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns>The column viewport index.</returns>
        public int GetViewportRightColumn(int columnViewportIndex)
        {
            return GetViewportRightColumn(ActiveSheet, columnViewportIndex);
        }

        /// <summary>
        /// Gets the row viewport's top row index. 
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <returns></returns>
        public int GetViewportTopRow(int rowViewportIndex)
        {
            return GetViewportTopRow(ActiveSheet, rowViewportIndex);
        }

        /// <summary>
        /// Performs a hit test. 
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns></returns>
        public HitTestInformation HitTest(double x, double y)
        {
            Point hitPoint = new Point(x, y);
            SheetLayout layout = GetSheetLayout();
            HitTestInformation hi = new HitTestInformation
            {
                HitTestType = HitTestType.Empty,
                ColumnViewportIndex = -2,
                RowViewportIndex = -2,
                HitPoint = hitPoint
            };
            if (GetTabStripRectangle().Contains(hitPoint))
            {
                hi.ColumnViewportIndex = 0;
                hi.HitTestType = HitTestType.TabStrip;
                return hi;
            }
            for (int i = 0; i < layout.ColumnPaneCount; i++)
            {
                if (GetHorizontalScrollBarRectangle(i).Contains(hitPoint))
                {
                    hi.ColumnViewportIndex = i;
                    hi.HitTestType = HitTestType.HorizontalScrollBar;
                    return hi;
                }
            }
            for (int j = 0; j < layout.RowPaneCount; j++)
            {
                if (GetVerticalScrollBarRectangle(j).Contains(hitPoint))
                {
                    hi.HitTestType = HitTestType.VerticalScrollBar;
                    hi.RowViewportIndex = j;
                    return hi;
                }
            }
            for (int k = 0; k < layout.ColumnPaneCount; k++)
            {
                if (GetHorizontalSplitBoxRectangle(k).Contains(hitPoint))
                {
                    hi.HitTestType = HitTestType.ColumnSplitBox;
                    hi.ColumnViewportIndex = k;
                }
            }
            for (int m = 0; m < layout.RowPaneCount; m++)
            {
                if (GetVerticalSplitBoxRectangle(m).Contains(hitPoint))
                {
                    hi.HitTestType = HitTestType.RowSplitBox;
                    hi.RowViewportIndex = m;
                }
            }
            for (int n = 0; n < (layout.ColumnPaneCount - 1); n++)
            {
                if (GetHorizontalSplitBarRectangle(n).Contains(hitPoint))
                {
                    hi.HitTestType = HitTestType.ColumnSplitBar;
                    hi.ColumnViewportIndex = n;
                }
            }
            for (int num6 = 0; num6 < (layout.RowPaneCount - 1); num6++)
            {
                if (GetVerticalSplitBarRectangle(num6).Contains(hitPoint))
                {
                    hi.HitTestType = HitTestType.RowSplitBar;
                    hi.RowViewportIndex = num6;
                }
            }

            if (hi.HitTestType != HitTestType.Empty)
                return hi;

            ViewportInfo viewportInfo = GetViewportInfo();
            int columnViewportCount = viewportInfo.ColumnViewportCount;
            int rowViewportCount = viewportInfo.RowViewportCount;
            if (IsCornerRangeGroupHitTest(hitPoint))
            {
                hi.HitTestType = HitTestType.CornerRangeGroup;
                return hi;
            }
            if (IsRowRangeGroupHitTest(hitPoint))
            {
                hi.HitTestType = HitTestType.RowRangeGroup;
                return hi;
            }
            if (IsColumnRangeGroupHitTest(hitPoint))
            {
                hi.HitTestType = HitTestType.ColumnRangeGroup;
                return hi;
            }
            if (GetCornerRectangle().Contains(hitPoint))
            {
                HeaderHitTestInformation information2 = new HeaderHitTestInformation
                {
                    Column = -1,
                    Row = -1,
                    ResizingColumn = -1,
                    ResizingRow = -1
                };
                hi.HitTestType = HitTestType.Corner;
                hi.HeaderInfo = information2;
                ColumnLayout rowHeaderColumnLayoutFromX = GetRowHeaderColumnLayoutFromX(hitPoint.X);
                RowLayout columnHeaderRowLayoutFromY = GetColumnHeaderRowLayoutFromY(hitPoint.Y);
                ColumnLayout rowHeaderResizingColumnLayoutFromX = GetRowHeaderResizingColumnLayoutFromX(hitPoint.X);
                RowLayout columnHeaderResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(hitPoint.Y);
                if (rowHeaderColumnLayoutFromX != null)
                {
                    information2.Column = rowHeaderColumnLayoutFromX.Column;
                }
                if (columnHeaderRowLayoutFromY != null)
                {
                    information2.Row = columnHeaderRowLayoutFromY.Row;
                }
                if (rowHeaderResizingColumnLayoutFromX != null)
                {
                    information2.InColumnResize = true;
                    information2.ResizingColumn = rowHeaderResizingColumnLayoutFromX.Column;
                }
                if (columnHeaderResizingRowLayoutFromY != null)
                {
                    information2.InRowResize = true;
                    information2.ResizingRow = columnHeaderResizingRowLayoutFromY.Row;
                }
                return hi;
            }
            for (int i = -1; i <= rowViewportCount; i++)
            {
                if (GetRowHeaderRectangle(i).Contains(hitPoint))
                {
                    HeaderHitTestInformation information4 = new HeaderHitTestInformation
                    {
                        Row = -1,
                        Column = -1,
                        ResizingColumn = -1,
                        ResizingRow = -1
                    };
                    hi.HitTestType = HitTestType.RowHeader;
                    hi.RowViewportIndex = i;
                    hi.HeaderInfo = information4;
                    if (((InputDeviceType == InputDeviceType.Touch) && ResizerGripperRect.HasValue) && ResizerGripperRect.Value.Contains(hitPoint))
                    {
                        hi.HeaderInfo.InRowResize = true;
                        hi.HeaderInfo.ResizingRow = GetActiveCell().Row;
                        return hi;
                    }
                    ColumnLayout layout5 = GetRowHeaderColumnLayoutFromX(hitPoint.X);
                    RowLayout viewportRowLayoutFromY = GetViewportRowLayoutFromY(i, hitPoint.Y);
                    RowLayout viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(i, hitPoint.Y);
                    if ((viewportResizingRowLayoutFromY == null) && (hi.RowViewportIndex == 0))
                    {
                        viewportResizingRowLayoutFromY = GetViewportResizingRowLayoutFromY(-1, hi.HitPoint.Y);
                    }
                    if ((viewportResizingRowLayoutFromY == null) && ((hi.RowViewportIndex == 0) || (hi.RowViewportIndex == -1)))
                    {
                        viewportResizingRowLayoutFromY = GetColumnHeaderResizingRowLayoutFromY(hi.HitPoint.Y);
                        if (viewportResizingRowLayoutFromY != null)
                        {
                            hi.HitTestType = HitTestType.Corner;
                        }
                    }
                    if (layout5 != null)
                    {
                        information4.Column = layout5.Column;
                    }
                    if (viewportRowLayoutFromY != null)
                    {
                        information4.Row = viewportRowLayoutFromY.Row;
                    }
                    if ((viewportResizingRowLayoutFromY != null) && (((viewportResizingRowLayoutFromY.Height > 0.0) || (viewportResizingRowLayoutFromY.Row >= ActiveSheet.RowCount)) || !ActiveSheet.RowRangeGroup.IsCollapsed(viewportResizingRowLayoutFromY.Row)))
                    {
                        information4.InRowResize = true;
                        information4.ResizingRow = viewportResizingRowLayoutFromY.Row;
                    }
                    return hi;
                }
            }
            for (int j = -1; j <= columnViewportCount; j++)
            {
                if (GetColumnHeaderRectangle(j).Contains(hitPoint))
                {
                    HeaderHitTestInformation information6 = new HeaderHitTestInformation
                    {
                        Row = -1,
                        Column = -1,
                        ResizingRow = -1,
                        ResizingColumn = -1
                    };
                    hi.HitTestType = HitTestType.ColumnHeader;
                    hi.HeaderInfo = information6;
                    hi.ColumnViewportIndex = j;
                    if (((InputDeviceType == InputDeviceType.Touch) && ResizerGripperRect.HasValue) && ResizerGripperRect.Value.Contains(hitPoint))
                    {
                        hi.HeaderInfo.InColumnResize = true;
                        hi.HeaderInfo.ResizingColumn = GetActiveCell().Column;
                        return hi;
                    }
                    ColumnLayout viewportColumnLayoutFromX = GetViewportColumnLayoutFromX(j, hitPoint.X);
                    RowLayout layout9 = GetColumnHeaderRowLayoutFromY(hitPoint.Y);
                    ColumnLayout viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(j, hitPoint.X);
                    if (viewportResizingColumnLayoutFromX == null)
                    {
                        if (hi.ColumnViewportIndex == 0)
                        {
                            viewportResizingColumnLayoutFromX = GetViewportResizingColumnLayoutFromX(-1, hitPoint.X);
                        }
                        if ((viewportResizingColumnLayoutFromX == null) && ((hi.ColumnViewportIndex == 0) || (hi.ColumnViewportIndex == -1)))
                        {
                            viewportResizingColumnLayoutFromX = GetRowHeaderResizingColumnLayoutFromX(hitPoint.X);
                            if (viewportResizingColumnLayoutFromX != null)
                            {
                                hi.HitTestType = HitTestType.Corner;
                            }
                        }
                    }
                    if (viewportColumnLayoutFromX != null)
                    {
                        hi.HeaderInfo.Column = viewportColumnLayoutFromX.Column;
                    }
                    if (layout9 != null)
                    {
                        hi.HeaderInfo.Row = layout9.Row;
                    }
                    if ((viewportResizingColumnLayoutFromX != null) && (((viewportResizingColumnLayoutFromX.Width > 0.0) || (viewportResizingColumnLayoutFromX.Column >= ActiveSheet.ColumnCount)) || !ActiveSheet.ColumnRangeGroup.IsCollapsed(viewportResizingColumnLayoutFromX.Column)))
                    {
                        hi.HeaderInfo.InColumnResize = true;
                        hi.HeaderInfo.ResizingColumn = viewportResizingColumnLayoutFromX.Column;
                    }
                    return hi;
                }
            }
            for (int k = -1; k <= rowViewportCount; k++)
            {
                for (int m = -1; m <= columnViewportCount; m++)
                {
                    if (GetViewportRectangle(k, m).Contains(hitPoint))
                    {
                        hi.ColumnViewportIndex = m;
                        hi.RowViewportIndex = k;
                        ViewportHitTestInformation information8 = new ViewportHitTestInformation
                        {
                            Column = -1,
                            Row = -1
                        };
                        hi.HitTestType = HitTestType.Viewport;
                        hi.ViewportInfo = information8;
                        ColumnLayout layout11 = GetViewportColumnLayoutFromX(m, hitPoint.X);
                        RowLayout layout12 = GetViewportRowLayoutFromY(k, hitPoint.Y);
                        if (layout11 != null)
                        {
                            hi.ViewportInfo.Column = layout11.Column;
                        }
                        if (layout12 != null)
                        {
                            hi.ViewportInfo.Row = layout12.Row;
                        }

                        if (IsInSelectionGripper(new Point(x, y)) || !HitTestFloatingObject(k, m, hitPoint.X, hitPoint.Y, hi))
                        {
                            CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(k, m);
                            if ((layout11 != null) && (layout12 != null))
                            {
                                if (IsMouseInDragFillIndicator(hitPoint.X, hitPoint.Y, k, m, false))
                                {
                                    hi.ViewportInfo.InDragFillIndicator = true;
                                }
                                else if (IsMouseInDragDropLocation(hitPoint.X, hitPoint.Y, k, m, false))
                                {
                                    hi.ViewportInfo.InSelectionDrag = true;
                                }
                            }
                            if (((IsEditing && !hi.ViewportInfo.InSelectionDrag) && (!hi.ViewportInfo.InDragFillIndicator && (viewportRowsPresenter != null))) && viewportRowsPresenter.EditorBounds.Contains(new Point(x - viewportRowsPresenter.Location.X, y - viewportRowsPresenter.Location.Y)))
                            {
                                hi.ViewportInfo.InEditor = true;
                            }
                            return hi;
                        }
                    }
                }
            }
            return hi;
        }

        /// <summary>
        /// Removes a column viewport from the control. 
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index to remove.</param>
        public void RemoveColumnViewport(int columnViewportIndex)
        {
            ActiveSheet.RemoveColumnViewport(columnViewportIndex);
            InvalidateLayout();
            InvalidateMeasure();
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
            ActiveSheet.RemoveRowViewport(rowViewportIndex);
            InvalidateLayout();
            InvalidateMeasure();
        }

        /// <summary>
        /// Resets the changed theme color and font name. 
        /// </summary>
        public void ResetThemes()
        {
            Workbook.ResetThemes();
            RefreshAll();
        }

        /// <summary>
        /// Resumes the calculation service. 
        /// </summary>
        public void ResumeCalcService()
        {
            Workbook.ResumeCalcService();
            RefreshAll();
        }

        /// <summary>
        /// Resumes the event. 
        /// </summary>
        public void ResumeEvent()
        {
            Workbook.ResumeEvent();
            _eventSuspended--;
            if (_eventSuspended < 0)
            {
                _eventSuspended = 0;
            }
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
            SetActiveColumnViewportIndex(columnViewportIndex);
            SetActiveRowViewportIndex(rowViewportIndex);
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
            if ((ActiveSheet != null) && (HorizontalScrollable || _isTouchScrolling))
            {
                value = Math.Max(ActiveSheet.FrozenColumnCount, value);
                value = Math.Min((ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1, value);
                value = TryGetNextScrollableColumn(value);

                ViewportInfo viewportInfo = GetViewportInfo();
                value = Math.Max(ActiveSheet.FrozenColumnCount, value);
                value = Math.Min((ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - 1, value);
                value = TryGetNextScrollableColumn(value);
                if (((columnViewportIndex >= 0) && (columnViewportIndex < viewportInfo.ColumnViewportCount)) && (viewportInfo.LeftColumns[columnViewportIndex] != value))
                {
                    int oldIndex = viewportInfo.LeftColumns[columnViewportIndex];
                    viewportInfo.LeftColumns[columnViewportIndex] = value;
                    InvalidateViewportColumnsLayout();
                    InvalidateViewportHorizontalArrangement(columnViewportIndex);
                    if (_columnGroupPresenters != null)
                    {
                        GcRangeGroup group = _columnGroupPresenters[columnViewportIndex + 1];
                        if (group != null)
                        {
                            group.InvalidateMeasure();
                        }
                    }
                    RaiseLeftChanged(oldIndex, value, columnViewportIndex);
                }
                if (!IsWorking)
                {
                    SaveHitInfo(null);
                }

                if (_horizontalScrollBar != null)
                {
                    GetSheetLayout();
                    if (((columnViewportIndex > -1) && (columnViewportIndex < _horizontalScrollBar.Length)) && (_horizontalScrollBar[columnViewportIndex].Value != value))
                    {
                        int invisibleColumnsBeforeColumn = GetInvisibleColumnsBeforeColumn(ActiveSheet, value);
                        int num2 = value - invisibleColumnsBeforeColumn;
                        _horizontalScrollBar[columnViewportIndex].Value = (double)num2;
                        _horizontalScrollBar[columnViewportIndex].InvalidateArrange();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the row viewport's top row.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <param name="value">The row index.</param>
        public void SetViewportTopRow(int rowViewportIndex, int value)
        {
            if ((ActiveSheet != null) && (VerticalScrollable || _isTouchScrolling))
            {
                value = Math.Max(ActiveSheet.FrozenRowCount, value);
                value = Math.Min((ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1, value);
                value = TryGetNextScrollableRow(value);
                if (_verticalScrollBar != null)
                {
                    GetSheetLayout();
                    if (((rowViewportIndex > -1) && (rowViewportIndex < _verticalScrollBar.Length)) && (value != _verticalScrollBar[rowViewportIndex].Value))
                    {
                        int invisibleRowsBeforeRow = GetInvisibleRowsBeforeRow(ActiveSheet, value);
                        int num2 = value - invisibleRowsBeforeRow;
                        _verticalScrollBar[rowViewportIndex].Value = (double)num2;
                        _verticalScrollBar[rowViewportIndex].InvalidateArrange();
                    }
                }

                ViewportInfo viewportInfo = GetViewportInfo();
                value = Math.Max(ActiveSheet.FrozenRowCount, value);
                value = Math.Min((ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - 1, value);
                value = TryGetNextScrollableRow(value);
                if (((rowViewportIndex >= 0) && (rowViewportIndex < viewportInfo.RowViewportCount)) && (viewportInfo.TopRows[rowViewportIndex] != value))
                {
                    int oldIndex = viewportInfo.TopRows[rowViewportIndex];
                    viewportInfo.TopRows[rowViewportIndex] = value;
                    InvalidateViewportRowsLayout();
                    InvalidateViewportRowsPresenterMeasure(rowViewportIndex, false);
                    for (int i = -1; i < viewportInfo.ColumnViewportCount; i++)
                    {
                        CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(rowViewportIndex, i);
                        if (viewportRowsPresenter != null)
                        {
                            viewportRowsPresenter.InvalidateMeasure();
                            viewportRowsPresenter.InvalidateBordersMeasureState();
                            viewportRowsPresenter.InvalidateSelectionMeasureState();
                            viewportRowsPresenter.InvalidateFloatingObjectsMeasureState();
                        }
                    }
                    var rowHeaderRowsPresenter = GetRowHeaderRowsPresenter(rowViewportIndex);
                    if (rowHeaderRowsPresenter != null)
                    {
                        rowHeaderRowsPresenter.InvalidateMeasure();
                    }
                    if (_rowGroupPresenters != null)
                    {
                        GcRangeGroup group = _rowGroupPresenters[rowViewportIndex + 1];
                        if (group != null)
                        {
                            group.InvalidateMeasure();
                        }
                    }
                    RaiseTopChanged(oldIndex, value, rowViewportIndex);
                }
                if (!IsWorking)
                {
                    SaveHitInfo(null);
                }
            }
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
        /// Moves a cell to the specified position.  
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index.</param>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <param name="row">The cell row index to show.</param>
        /// <param name="column">The cell column index to show.</param>
        /// <param name="verticalPosition">The VerticalPosition to show.</param>
        /// <param name="horizontalPosition">The HorizontalPosition to show.</param>
        public void ShowCell(int rowViewportIndex, int columnViewportIndex, int row, int column, VerticalPosition verticalPosition, HorizontalPosition horizontalPosition)
        {
            Worksheet worksheet = ActiveSheet;
            if (((worksheet != null) && (row <= worksheet.RowCount)) && (column <= worksheet.ColumnCount))
            {
                int viewportTopRow = GetViewportTopRow(rowViewportIndex);
                int viewportLeftColumn = GetViewportLeftColumn(columnViewportIndex);
                switch (horizontalPosition)
                {
                    case HorizontalPosition.Center:
                        {
                            double num3 = RoundToPoint((GetViewportWidth(columnViewportIndex) - RoundToPoint(worksheet.Columns[column].ActualWidth * ZoomFactor)) / 2.0);
                            while (0 < column)
                            {
                                num3 -= RoundToPoint(worksheet.Columns[column - 1].ActualWidth * ZoomFactor);
                                if (num3 < 0.0)
                                {
                                    break;
                                }
                                column--;
                            }
                            break;
                        }
                    case HorizontalPosition.Right:
                        {
                            double num4 = GetViewportWidth(columnViewportIndex) - RoundToPoint(worksheet.Columns[column].ActualWidth * ZoomFactor);
                            while (0 < column)
                            {
                                num4 -= RoundToPoint(worksheet.Columns[column - 1].ActualWidth * ZoomFactor);
                                if (num4 < 0.0)
                                {
                                    break;
                                }
                                column--;
                            }
                            break;
                        }
                    case HorizontalPosition.Nearest:
                        if (column >= viewportLeftColumn)
                        {
                            double num5 = GetViewportWidth(columnViewportIndex) - RoundToPoint(worksheet.Columns[column].Width * ZoomFactor);
                            while (viewportLeftColumn < column)
                            {
                                num5 -= RoundToPoint(worksheet.Columns[column - 1].ActualWidth * ZoomFactor);
                                if (num5 < 0.0)
                                {
                                    break;
                                }
                                column--;
                            }
                        }
                        break;
                }
                switch (verticalPosition)
                {
                    case VerticalPosition.Center:
                        {
                            double num6 = RoundToPoint((GetViewportHeight(rowViewportIndex) - RoundToPoint(worksheet.Rows[row].ActualHeight * ZoomFactor)) / 2.0);
                            while (0 < row)
                            {
                                num6 -= RoundToPoint(worksheet.Rows[row - 1].ActualHeight * ZoomFactor);
                                if (num6 < 0.0)
                                {
                                    break;
                                }
                                row--;
                            }
                            break;
                        }
                    case VerticalPosition.Bottom:
                        {
                            double num7 = GetViewportHeight(rowViewportIndex) - RoundToPoint(worksheet.Rows[row].ActualHeight * ZoomFactor);
                            while (0 < row)
                            {
                                num7 -= RoundToPoint(worksheet.Rows[row - 1].ActualHeight * ZoomFactor);
                                if (num7 < 0.0)
                                {
                                    break;
                                }
                                row--;
                            }
                            break;
                        }
                    case VerticalPosition.Nearest:
                        if ((row >= viewportTopRow) && (viewportTopRow != -1))
                        {
                            double num8 = GetViewportHeight(rowViewportIndex) - RoundToPoint(worksheet.Rows[row].ActualHeight * ZoomFactor);
                            while (viewportTopRow < row)
                            {
                                num8 -= RoundToPoint(worksheet.Rows[row - 1].ActualHeight * ZoomFactor);
                                if (num8 < 0.0)
                                {
                                    break;
                                }
                                row--;
                            }
                        }
                        break;
                }
                if (row != viewportTopRow)
                {
                    SetViewportTopRow(rowViewportIndex, row);
                }
                if (column != viewportLeftColumn)
                {
                    SetViewportLeftColumn(columnViewportIndex, column);
                }
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
                SheetLayout layout = GetSheetLayout();
                if ((columnViewportIndex < -1) || (columnViewportIndex > layout.ColumnPaneCount))
                {
                    throw new ArgumentOutOfRangeException("columnViewportIndex");
                }
                if ((columnViewportIndex != -1) && (columnViewportIndex != layout.ColumnPaneCount))
                {
                    if ((column < 0) || (column >= activeSheet.ColumnCount))
                    {
                        throw new ArgumentOutOfRangeException("column");
                    }
                    if (!Enum.IsDefined((Type)typeof(HorizontalPosition), horizontalPosition))
                    {
                        throw new ArgumentException(ResourceStrings.gcSpreadInvalidHorizontalPosition);
                    }

                    int viewportTopRow = GetViewportTopRow(0);
                    ShowCell(0, columnViewportIndex, viewportTopRow, column, VerticalPosition.Top, horizontalPosition);
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
                SheetLayout layout = GetSheetLayout();
                if ((rowViewportIndex < -1) || (rowViewportIndex > layout.RowPaneCount))
                {
                    throw new ArgumentOutOfRangeException("rowViewportIndex");
                }
                if ((rowViewportIndex != -1) && (rowViewportIndex != layout.RowPaneCount))
                {
                    if ((row < 0) || (row >= activeSheet.RowCount))
                    {
                        throw new ArgumentOutOfRangeException("row");
                    }
                    if (!Enum.IsDefined((Type)typeof(VerticalPosition), verticalPosition))
                    {
                        throw new ArgumentException(ResourceStrings.gcSpreadInvalidVerticalPosition);
                    }

                    int viewportLeftColumn = GetViewportLeftColumn(0);
                    ShowCell(rowViewportIndex, 0, row, viewportLeftColumn, verticalPosition, HorizontalPosition.Left);
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
            _eventSuspended++;
        }

        /// <summary>
        /// Adjusts the adjacent row viewport's height.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index to adjust, it adjusts the row viewport and its next row viewport.</param>
        /// <param name="deltaViewportHeight">The row height adjusted offset.</param>
        public void AdjustRowViewport(int rowViewportIndex, double deltaViewportHeight)
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            if ((rowViewportIndex < 0) || (rowViewportIndex > (viewportInfo.RowViewportCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            if ((viewportInfo.RowViewportCount > 1) && (rowViewportIndex != (viewportInfo.RowViewportCount - 1)))
            {
                int index = rowViewportIndex + 1;
                viewportInfo.ViewportHeight[rowViewportIndex] = DoubleUtil.Formalize(GetViewportHeight(rowViewportIndex) + deltaViewportHeight) / ((double)ZoomFactor);
                viewportInfo.ViewportHeight[index] = DoubleUtil.Formalize(GetViewportHeight(index) - deltaViewportHeight) / ((double)ZoomFactor);
                if (viewportInfo.ViewportHeight[index] == 0.0)
                {
                    ActiveSheet.RemoveRowViewport(rowViewportIndex + 1);
                }
                if (viewportInfo.ViewportHeight[rowViewportIndex] == 0.0)
                {
                    ActiveSheet.RemoveRowViewport(rowViewportIndex);
                }
                viewportInfo = GetViewportInfo();
                viewportInfo.ViewportHeight[viewportInfo.RowViewportCount - 1] = -1.0;
                ActiveSheet.SetViewportInfo(viewportInfo);
                InvalidateLayout();
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Adjusts the adjacent column viewport's width.
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index to adjust, it adjusts the column viewport and its next column viewport.</param>
        /// <param name="deltaViewportWidth">The column width adjusted offset.</param>
        public void AdjustColumnViewport(int columnViewportIndex, double deltaViewportWidth)
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            if ((columnViewportIndex < 0) || (columnViewportIndex > (viewportInfo.ColumnViewportCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            if ((viewportInfo.ColumnViewportCount > 1) && (columnViewportIndex != (viewportInfo.ColumnViewportCount - 1)))
            {
                int index = columnViewportIndex + 1;
                viewportInfo.ViewportWidth[columnViewportIndex] = DoubleUtil.Formalize(GetViewportWidth(columnViewportIndex) + deltaViewportWidth) / ((double)ZoomFactor);
                viewportInfo.ViewportWidth[index] = DoubleUtil.Formalize(GetViewportWidth(index) - deltaViewportWidth) / ((double)ZoomFactor);
                if (viewportInfo.ViewportWidth[index] == 0.0)
                {
                    ActiveSheet.RemoveColumnViewport(index);
                }
                if (viewportInfo.ViewportWidth[columnViewportIndex] == 0.0)
                {
                    ActiveSheet.RemoveColumnViewport(columnViewportIndex);
                }
                viewportInfo = GetViewportInfo();
                viewportInfo.ViewportWidth[viewportInfo.ColumnViewportCount - 1] = -1.0;
                ActiveSheet.SetViewportInfo(viewportInfo);
                InvalidateLayout();
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// Clears all undo and redo actions in the current UndoManager. 
        /// </summary>
        public void ClearUndoManager()
        {
            if (_undoManager != null)
            {
                _undoManager.UndoList.Clear();
                _undoManager.RedoList.Clear();
            }
        }

        /// <summary>
        /// Copies the text of a cell range to the Clipboard.
        /// </summary>
        /// <param name="range">The copied cell range.</param>
        public void ClipboardCopy(CellRange range)
        {
            if (ActiveSheet != null)
            {
                if (range == null)
                {
                    throw new ArgumentNullException("range");
                }
                if (!IsValidRange(range.Row, range.Column, range.RowCount, range.ColumnCount, ActiveSheet.RowCount, ActiveSheet.ColumnCount))
                {
                    throw new ArgumentException(ResourceStrings.SheetViewClipboardArgumentException);
                }
                CopyToClipboard(range, false);
            }
        }

        /// <summary>
        /// Cuts the text of a cell range to the Clipboard.
        /// </summary>
        /// <param name="range">The cut cell range.</param>
        public void ClipboardCut(CellRange range)
        {
            if (ActiveSheet != null)
            {
                if (range == null)
                {
                    throw new ArgumentNullException("range");
                }
                if (!IsValidRange(range.Row, range.Column, range.RowCount, range.ColumnCount, ActiveSheet.RowCount, ActiveSheet.ColumnCount))
                {
                    throw new ArgumentException(ResourceStrings.SheetViewClipboardArgumentException);
                }
                CopyToClipboard(range, true);
            }
        }

        /// <summary>
        /// Pastes content from the Clipboard to a cell range on the sheet.
        /// </summary>
        /// <param name="range">The pasted cell range on the sheet.</param>
        public void ClipboardPaste(CellRange range)
        {
            ClipboardPaste(range, ClipboardPasteOptions.All);
        }

        /// <summary>
        /// Pastes content from the Clipboard to a cell range on the sheet.
        /// </summary>
        /// <param name="range">The pasted cell range.</param>
        /// <param name="option">The Clipboard paste option that indicates which content type to paste.</param>
        public void ClipboardPaste(CellRange range, ClipboardPasteOptions option)
        {
            if (ActiveSheet != null)
            {
                CellRange range1;
                bool flag2;
                if (range == null)
                {
                    throw new ArgumentNullException("range");
                }
                if (!IsValidRange(range.Row, range.Column, range.RowCount, range.ColumnCount, ActiveSheet.RowCount, ActiveSheet.ColumnCount))
                {
                    throw new ArgumentException(ResourceStrings.SheetViewClipboardArgumentException);
                }
                var fromSheet = SpreadXClipboard.Worksheet;
                CellRange fromRange = SpreadXClipboard.Range;
                string clipboardText = ClipboardHelper.GetClipboardData();
                bool isCutting = SpreadXClipboard.IsCutting;
                if (((isCutting && (fromSheet != null)) && ((fromRange != null) && fromSheet.Protect)) && IsAnyCellInRangeLocked(fromSheet, fromRange.Row, fromRange.Column, fromRange.RowCount, fromRange.ColumnCount))
                {
                    isCutting = false;
                }
                if (CheckPastedRange(fromSheet, fromRange, range, isCutting, clipboardText, out range1, out flag2))
                {
                    if (isCutting)
                    {
                        option = ClipboardPasteOptions.All;
                    }
                    if (flag2)
                    {
                        ClipboardPaste(fromSheet, fromRange, ActiveSheet, range1, isCutting, clipboardText, option);
                    }
                    else
                    {
                        ClipboardPaste(null, null, ActiveSheet, range1, isCutting, clipboardText, option);
                    }
                    SetSelection(range1.Row, range1.Column, range1.RowCount, range1.ColumnCount);
                    SetActiveCell((range.Row < 0) ? 0 : range.Row, (range.Column < 0) ? 0 : range.Column, false);
                    RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                }
            }
        }

        /// <summary>
        /// Gets the row count when scrolling down one page.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index one page down.</param>
        /// <returns>The row count when scrolling down one page.</returns>
        public int GetNextPageRowCount(int rowViewportIndex)
        {
            return GetNextPageRowCount(ActiveSheet, rowViewportIndex);
        }

        /// <summary>
        /// Gets the column count when scrolling right one page.
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index one page to the right.</param>
        /// <returns>The column count when scrolling right one page.</returns>
        public int GetNextPageColumnCount(int columnViewportIndex)
        {
            return GetNextPageColumnCount(ActiveSheet, columnViewportIndex);
        }

        /// <summary>
        /// Gets the row count when scrolling up one page.
        /// </summary>
        /// <param name="rowViewportIndex">The row viewport index one page up.</param>
        /// <returns>The row count when scrolling up one page.</returns>
        public int GetPrePageRowCount(int rowViewportIndex)
        {
            return GetPrePageRowCount(ActiveSheet, rowViewportIndex);
        }

        /// <summary>
        /// Gets the column count when scrolling left one page.
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index one page to the left.</param>
        /// <returns>The column count when scrolling left one page.</returns>
        public int GetPrePageColumnCount(int columnViewportIndex)
        {
            return GetPrePageColumnCount(ActiveSheet, columnViewportIndex);
        }

        /// <summary>
        /// Ges the spread chart view.
        /// </summary>
        /// <param name="chartName">Name of the chart.</param>
        /// <returns></returns>
        public SpreadChartView GetSpreadChartView(string chartName)
        {
            int activeRowViewportIndex = GetActiveRowViewportIndex();
            int activeColumnViewportIndex = GetActiveColumnViewportIndex();
            CellsPanel viewport = _cellsPanels[activeRowViewportIndex + 1, activeColumnViewportIndex + 1];
            if (viewport != null)
            {
                return viewport.GetSpreadChartView(chartName);
            }
            return null;
        }

        /// <summary>
        /// Sets the active cell of the sheet.
        /// </summary>
        /// <param name="row">The active row index.</param>
        /// <param name="column">The active column index.</param>
        /// <param name="clearSelection"> if set to <c>true</c> clears the old selection.</param>
        public void SetActiveCell(int row, int column, bool clearSelection)
        {
            if (ActiveSheet.GetActualStyleInfo(row, column, SheetArea.Cells).Focusable)
            {
                SetActiveCellInternal(row, column, clearSelection);
            }
        }

        /// <summary>
        /// Sets the index of the floating object Z.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="zIndex">Index of the z.</param>
        public void SetFloatingObjectZIndex(string name, int zIndex)
        {
            if (_cellsPanels != null)
            {
                CellsPanel[,] viewportArray = _cellsPanels;
                int upperBound = viewportArray.GetUpperBound(0);
                int num2 = viewportArray.GetUpperBound(1);
                for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
                {
                    for (int j = viewportArray.GetLowerBound(1); j <= num2; j++)
                    {
                        CellsPanel viewport = viewportArray[i, j];
                        if (viewport != null)
                        {
                            viewport.SetFlotingObjectZIndex(name, zIndex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Displays the automatic fill indicator.
        /// </summary>
        public void ShowAutoFillIndicator()
        {
            if (CanUserDragFill)
            {
                CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex());
                if (viewportRowsPresenter != null)
                {
                    CellRange activeSelection = GetActiveSelection();
                    if ((activeSelection == null) && (ActiveSheet.Selections.Count > 0))
                    {
                        activeSelection = ActiveSheet.Selections[0];
                    }
                    if (activeSelection != null)
                    {
                        _autoFillIndicator.Width = 16.0;
                        _autoFillIndicator.Height = 16.0;
                        _autoFillIndicatorRect = new Rect?(GetAutoFillIndicatorRect(viewportRowsPresenter, activeSelection));
                        base.InvalidateArrange();
                        CachedGripperLocation = null;
                    }
                }
            }
        }

        /// <summary>
        /// Starts to edit the active cell.
        /// </summary>
        /// <param name="selectAll">if set to <c>true</c> selects all the text when the text is changed during editing.</param>
        /// <param name="defaultText">if set to <c>true</c> [default text].</param>
        public void StartCellEditing(bool selectAll = false, string defaultText = null)
        {
            StartCellEditing(selectAll, defaultText, EditorStatus.Edit);
        }

        /// <summary>
        /// Stops editing the active cell.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> does not apply the edited text to the cell.</param>
        /// <returns><c>true</c> when able to stop cell editing successfully; otherwise, <c>false</c>.</returns>
        public bool StopCellEditing(bool cancel = false)
        {
            if (IsEditing && ActiveSheet != null)
            {
                CellsPanel editingViewport = EditingViewport;
                if (editingViewport != null)
                {
                    if (!cancel && (ApplyEditingValue(cancel) == DataValidationResult.Retry))
                    {
                        editingViewport.RetryEditing();
                    }
                    else
                    {
                        bool editorDirty = editingViewport.EditorDirty;
                        editingViewport.StopCellEditing();
                        IsEditing = editingViewport.IsEditing();
                        if (editorDirty && !cancel)
                        {
                            RefreshViewportCells(_cellsPanels, 0, 0, ActiveSheet.RowCount, ActiveSheet.ColumnCount);
                        }
                    }
                    if (editingViewport.IsEditing())
                    {
                        return false;
                    }
                    EditingViewport = null;
                }
            }
            IsEditing = false;
            return true;
        }

        /// <summary>
        /// Calculates the start index to bring the tab into view. 
        /// </summary>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <returns></returns>
        public int GetStartIndexToBringTabIntoView(int tabIndex)
        {
            if (_tabStrip != null)
            {
                return _tabStrip.GetStartIndexToBringTabIntoView(tabIndex);
            }
            return StartSheetIndex;
        }

        /// <summary>
        /// 打印Sheet内容
        /// </summary>
        /// <param name="p_printInfo">打印设置</param>
        /// <param name="p_sheetIndex">要打印的Sheet索引，-1表示当前活动Sheet</param>
        /// <param name="p_title">标题</param>
        public void Print(PrintInfo p_printInfo = null, int p_sheetIndex = -1, string p_title = null)
        {
            // 超出打印范围
            if (p_sheetIndex >= SheetCount)
                return;

            // Sheet索引
            int index = ActiveSheetIndex;
            if (p_sheetIndex > -1)
                index = p_sheetIndex;
            ExcelPrinter printer = new ExcelPrinter(this, p_printInfo == null ? new PrintInfo() : p_printInfo, index);
            string jobName = string.IsNullOrEmpty(p_title) ? Sheets[index].Name : p_title;
            printer.Print(jobName);
        }
        #endregion
    }
}
