#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    public partial class Excel : Control
    {
        /// <summary>
        /// Gets the active sheet in the Excel control. 
        /// </summary>
        [Browsable(false)]
        public Worksheet ActiveSheet
        {
            get { return Workbook.ActiveSheet; }
            internal set
            {
                Workbook.ActiveSheet = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the index of the active sheet in the Excel control. 
        /// </summary>
        [DefaultValue(0), Category("Spread Workbook")]
        public int ActiveSheetIndex
        {
            get { return Workbook.ActiveSheetIndex; }
            set
            {
                if ((Workbook.ActiveSheetIndex != value) && !View.RaiseActiveSheetIndexChanging())
                {
                    Workbook.ActiveSheetIndex = value;
                    View.RaiseActiveSheetIndexChanged();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the component handles the shortcut keys for Clipboard actions. 
        /// </summary>
        [DefaultValue(true), Category("Spread Workbook")]
        public bool AutoClipboard
        {
            get { return View.AutoClipboard; }
            set { View.AutoClipboard = value; }
        }

        /// <summary>
        /// Gets or sets whether the formula is automatically calculated. 
        /// </summary>
        [DefaultValue(true), Category("Spread Formulas")]
        public bool AutoRecalculation
        {
            get { return Workbook.AutoRecalculation; }
            set { Workbook.AutoRecalculation = value; }
        }

        /// <summary>
        ///Gets or sets whether the Excel can auto-refresh itself.  
        /// </summary>
        [Browsable(false), DefaultValue(true)]
        public bool AutoRefresh
        {
            get { return Workbook.AutoRefresh; }
            set { Workbook.AutoRefresh = value; }
        }

        /// <summary>
        /// Gets or sets whether data can overflow into adjacent empty cells in the component. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(false)]
        public bool CanCellOverflow
        {
            get { return Workbook.CanCellOverflow; }
            set { Workbook.CanCellOverflow = value; }
        }

        /// <summary>
        /// Gets or sets whether data can overflow into adjacent empty cells in the component while that cell is in edit mode. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(true)]
        public bool CanEditOverflow
        {
            get { return View.CanEditOverflow; }
            set { View.CanEditOverflow = value; }
        }

        /// <summary>
        /// Indicates whether the user can select multiple ranges by touch. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(false)]
        public bool CanTouchMultiSelect
        {
            get { return View.CanTouchMultiSelect; }
            set { View.CanTouchMultiSelect = value; }
        }

        /// <summary>
        /// Gets or sets whether to allow the user to drag and drop cell range data to another range. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(true)]
        public bool CanUserDragDrop
        {
            get { return View.CanUserDragDrop; }
            set { View.CanUserDragDrop = value; }
        }

        /// <summary>
        /// Gets or sets whether to allow the user to drag fill a range of cells. 
        /// </summary>
        [DefaultValue(true), Category("Spread Workbook")]
        public bool CanUserDragFill
        {
            get { return View.CanUserDragFill; }
            set { View.CanUserDragFill = value; }
        }

        /// <summary>
        /// Gets or sets whether to allow the user to enter formulas in a cell in the component. 
        /// </summary>
        [Category("Spread Formulas"), DefaultValue(true)]
        public bool CanUserEditFormula
        {
            get { return View.CanUserEditFormula; }
            set { View.CanUserEditFormula = value; }
        }

        /// <summary>
        /// Gets or sets whether to allow the user to undo edit operations. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(true)]
        public bool CanUserUndo
        {
            get { return View.CanUserUndo; }
            set { View.CanUserUndo = value; }
        }

        /// <summary>
        ///Gets or sets whether the user can scale the display of the component using the Ctrl key and the mouse wheel.  
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(true)]
        public bool CanUserZoom
        {
            get { return View.CanUserZoom; }
            set { View.CanUserZoom = value; }
        }

        /// <summary>
        ///Gets or sets whether the component handles the shortcut keys for Clipboard actions.  
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(0xff)]
        public ClipboardPasteOptions ClipBoardOptions
        {
            get { return View.ClipBoardOptions; }
            set { View.ClipBoardOptions = value; }
        }

        /// <summary>
        /// Gets or sets the column split box alignment. 
        /// </summary>
        [DefaultValue(0), Category("Spread Workbook")]
        public SplitBoxAlignment ColumnSplitBoxAlignment
        {
            get { return View.ColumnSplitBoxAlignment; }
            set { View.ColumnSplitBoxAlignment = value; }
        }

        /// <summary>
        /// Gets or sets under which conditions the GcSpreadSheet component permits column splits. 
        /// </summary>
        [DefaultValue(0)]
        public SplitBoxPolicy ColumnSplitBoxPolicy
        {
            get { return View.ColumnSplitBoxPolicy; }
            set { View.ColumnSplitBoxPolicy = value; }
        }

        /// <summary>
        /// Gets or sets the current theme information for the control.
        /// </summary>
        [Browsable(false)]
        public SpreadTheme CurrentTheme
        {
            get { return Workbook.CurrentTheme; }
            set { Workbook.CurrentTheme = value; }
        }

        /// <summary>
        /// Gets or sets the current theme information for the control. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue("Office")]
        public string CurrentThemeName
        {
            get { return Workbook.CurrentThemeName; }
            set { Workbook.CurrentThemeName = value; }
        }

        /// <summary>
        /// Gets or sets the default type of the automatic fill. 
        /// </summary>
        /// <value>
        /// The default type of the automatic fill. 
        /// </value>
        [DefaultValue((string)null)]
        public AutoFillType? DefaultAutoFillType
        {
            get { return View.DefaultAutoFillType; }
            set { View.DefaultAutoFillType = value; }
        }

        /// <summary>
        /// Gets or sets the document uri of the sheet. 
        /// </summary>
        /// <value>
        /// The document uri of the sheet.
        /// </value>
        [Browsable(false)]
        public Uri DocumentUri
        {
            get { return _documentUri; }
            set
            {
                _documentUri = value;
                if (value != null)
                {
                    Action<Task<StorageFile>> action = null;
                    Action<Task<StorageFile>> action2 = null;
                    bool xmlFile = false;
                    bool excelFile = false;
                    string str = _documentUri.IsAbsoluteUri ? _documentUri.LocalPath : _documentUri.OriginalString;
                    if (!string.IsNullOrEmpty(str))
                    {
                        string extension = Path.GetExtension(str);
                        switch (extension)
                        {
                            case ".xml":
                            case ".ssxml":
                                xmlFile = true;
                                break;
                        }
                        if ((extension == ".xls") || (extension == ".xlsx"))
                        {
                            excelFile = true;
                        }
                    }
                    if (!DesignMode.DesignModeEnabled)
                    {
                        if (_documentUri.IsAbsoluteUri)
                        {
                            Uri uri = _documentUri;
                            SpreadView view = View;
                            Workbook workbook = Workbook;
                            new StyleInfo();
                            if (action == null)
                            {
                                action = delegate (Task<StorageFile> fr)
                                {
                                    Func<Task<IRandomAccessStreamWithContentType>, Task> func = null;
                                    if ((fr.Result != null) && !fr.IsFaulted)
                                    {
                                        if (func == null)
                                        {
                                            func = async delegate (Task<IRandomAccessStreamWithContentType> r)
                                            {
                                                if ((r.Result != null) && !r.IsFaulted)
                                                {
                                                    using (Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(r.Result))
                                                    {
                                                        if (xmlFile)
                                                        {
                                                            await OpenXmlAsync(stream);
                                                        }
                                                        if (excelFile)
                                                        {
                                                            await OpenExcelAsync(stream);
                                                        }
                                                    }
                                                }
                                            };
                                        }
                                        WindowsRuntimeSystemExtensions.AsTask<IRandomAccessStreamWithContentType>(fr.Result.OpenReadAsync()).ContinueWith<Task>(func);
                                    }
                                };
                            }
                            WindowsRuntimeSystemExtensions.AsTask<StorageFile>(StorageFile.GetFileFromApplicationUriAsync(uri)).ContinueWith(action);
                        }
                        else
                        {
                            if (action2 == null)
                            {
                                action2 = delegate (Task<StorageFile> fr)
                                {
                                    Func<Task<IRandomAccessStreamWithContentType>, Task> func = null;
                                    if (fr.Result != null)
                                    {
                                        if (func == null)
                                        {
                                            func = async delegate (Task<IRandomAccessStreamWithContentType> r)
                                            {
                                                if (r.Result != null)
                                                {
                                                    using (Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(r.Result))
                                                    {
                                                        if (xmlFile)
                                                        {
                                                            await OpenXmlAsync(stream);
                                                        }
                                                        if (excelFile)
                                                        {
                                                            await OpenExcelAsync(stream);
                                                        }
                                                    }
                                                }
                                            };
                                        }
                                        WindowsRuntimeSystemExtensions.AsTask<IRandomAccessStreamWithContentType>(fr.Result.OpenReadAsync()).ContinueWith<Task>(func);
                                    }
                                };
                            }
                            WindowsRuntimeSystemExtensions.AsTask<StorageFile>(StorageFile.GetFileFromApplicationUriAsync(_documentUri)).ContinueWith(action2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the gridline color. 
        /// </summary>
        [Category("Spread Workbook")]
        public Color GridLineColor
        {
            get { return Workbook.GridLineColor; }
            set
            {
                Workbook.GridLineColor = value;
                InvalidateSheet();
            }
        }

        /// <summary>
        /// Gets or sets the color of the gridline in the header. 
        /// </summary>
        /// <remarks>
        /// The default is null. This property is supposed to be used if customizing the control theme in XAML.
        /// </remarks>
        [EditorBrowsable((EditorBrowsableState)EditorBrowsableState.Never), Browsable(false)]
        public Color? HeaderGridLineColor
        {
            get { return (Color?)base.GetValue(HeaderGridLineColorProperty); }
            set { base.SetValue(HeaderGridLineColorProperty, value); }
        }

        /// <summary>
        /// Specifies whether to highlight invalid data. 
        /// </summary>
        [DefaultValue(false)]
        public bool HighlightInvalidData
        {
            get { return View.HighlightInvalidData; }
            set { View.HighlightInvalidData = value; }
        }

        /// <summary>
        /// Gets or sets the height of horizontal scroll bars in this control. 
        /// </summary>
        [DefaultValue((double)25.0), Category("Spread Workbook")]
        public double HorizontalScrollBarHeight
        {
            get { return View.HorizontalScrollBarHeight; }
            set { View.HorizontalScrollBarHeight = value; }
        }

        /// <summary>
        /// Gets or sets the horizontal scroll bar style. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue((string)null)]
        public Style HorizontalScrollBarStyle
        {
            get { return View.HorizontalScrollBarStyle; }
            set { View.HorizontalScrollBarStyle = value; }
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
                View.HorizontalScrollable = value != 0;
                View.InvalidateLayout();
                View.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Returns the last input device type. 
        /// </summary>
        [DefaultValue(0), Category("Spread Workbook")]
        public InputDeviceType InputDeviceType
        {
            get { return View.InputDeviceType; }
        }

        /// <summary>
        /// Gets or sets a collection of StyleInfo objects for this sheet. 
        /// </summary>
        /// <value>
        /// The collection of StyleInfo objects for this sheet.
        /// </value>
        [Browsable(false), DefaultValue((string)null)]
        public StyleInfoCollection NamedStyles
        {
            get { return Workbook.NamedStyles; }
            set
            {
                Workbook.NamedStyles = value;
                InvalidateSheet();
            }
        }

        /// <summary>
        ///Gets or sets whether this workbook is protected.  
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(false)]
        public bool Protect
        {
            get { return Workbook.Protect; }
            set { Workbook.Protect = value; }
        }

        /// <summary>
        /// Gets or sets the backgroud of the range group 
        /// </summary>
        [DefaultValue((string)null)]
        public Brush RangeGroupBackground
        {
            get { return View.RangeGroupBackground; }
            set { View.RangeGroupBackground = value; }
        }

        /// <summary>
        /// Gets or sets the brush of the border of the range group 
        /// </summary>
        [DefaultValue((string)null)]
        public Brush RangeGroupBorderBrush
        {
            get { return View.RangeGroupBorderBrush; }
            set { View.RangeGroupBorderBrush = value; }
        }

        /// <summary>
        /// Gets or sets the stroke of the group line 
        /// </summary>
        [DefaultValue((string)null)]
        public Brush RangeGroupLineStroke
        {
            get { return View.RangeGroupLineStroke; }
            set { View.RangeGroupLineStroke = value; }
        }

        /// <summary>
        /// Gets or sets the style for cell and range references in cell formulas on this sheet. 
        /// </summary>
        [Category("Spread Formulas"), DefaultValue(0)]
        public ReferenceStyle ReferenceStyle
        {
            get { return Workbook.ReferenceStyle; }
            set { Workbook.ReferenceStyle = value; }
        }

        /// <summary>
        ///Specifies the drawing policy when the row or column is resized to zero.  
        /// </summary>
        [DefaultValue(0)]
        public ResizeZeroIndicator ResizeZeroIndicator
        {
            get { return View.ResizeZeroIndicator; }
            set { View.ResizeZeroIndicator = value; }
        }

        /// <summary>
        /// Gets or sets the row split box alignment. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(0)]
        public SplitBoxAlignment RowSplitBoxAlignment
        {
            get { return View.RowSplitBoxAlignment; }
            set { View.RowSplitBoxAlignment = value; }
        }

        /// <summary>
        /// Gets or sets under which conditions the Excel component permits row splits. 
        /// </summary>
        [DefaultValue(0)]
        public SplitBoxPolicy RowSplitBoxPolicy
        {
            get { return View.RowSplitBoxPolicy; }
            set { View.RowSplitBoxPolicy = value; }
        }

        /// <summary>
        /// Gets or sets whether the sheet in the control scrolls when the user moves the scroll box. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(3)]
        public ScrollBarTrackPolicy ScrollBarTrackPolicy
        {
            get { return View.ScrollBarTrackPolicy; }
            set { View.ScrollBarTrackPolicy = value; }
        }

        /// <summary>
        /// Gets or sets the number of sheets for this control. 
        /// </summary>
        /// <value>
        /// The number of sheets for this control.
        /// </value>
        [Category("Spread Workbook"), Browsable(false), DefaultValue(1)]
        public int SheetCount
        {
            get { return Workbook.SheetCount; }
            set
            {
                Workbook.SheetCount = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the Worksheet collection for the Excel control. 
        /// </summary>
        [Browsable(false)]
        public WorksheetCollection Sheets
        {
            get { return Workbook.Sheets; }
        }

        /// <summary>
        /// Gets or sets whether the column range group is visible. 
        /// </summary>
        [DefaultValue(true), Category("Spread Workbook")]
        public bool ShowColumnRangeGroup
        {
            get { return View.ShowColumnRangeGroup; }
            set { View.ShowColumnRangeGroup = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to display the drag drop tip. 
        /// </summary>
        /// <value>
        /// true to display the drag drop tip; otherwise, false. 
        /// </value>
        [DefaultValue(true)]
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
        [DefaultValue(true)]
        public bool ShowDragFillTip
        {
            get { return Workbook.ShowDragFillTip; }
            set { Workbook.ShowDragFillTip = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show freeze lines. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(true)]
        public bool ShowFreezeLine
        {
            get { return View.ShowFreezeLine; }
            set { View.ShowFreezeLine = value; }
        }

        /// <summary>
        /// Gets or sets whether gridlines are displayed. 
        /// </summary>
        [DefaultValue(true), Category("Spread Workbook")]
        public bool ShowGridLine
        {
            get { return Workbook.ShowGridLine; }
            set
            {
                Workbook.ShowGridLine = value;
                InvalidateSheet();
            }
        }

        /// <summary>
        /// Gets or sets how to display the resize tip. 
        /// </summary>
        [DefaultValue(0)]
        public ShowResizeTip ShowResizeTip
        {
            get { return Workbook.ShowResizeTip; }
            set { Workbook.ShowResizeTip = value; }
        }

        /// <summary>
        /// Gets or sets whether the row range group is visible. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue(true)]
        public bool ShowRowRangeGroup
        {
            get { return View.ShowRowRangeGroup; }
            set { View.ShowRowRangeGroup = value; }
        }

        /// <summary>
        /// Gets or sets how to display the scroll tip. 
        /// </summary>
        [DefaultValue(0)]
        public ShowScrollTip ShowScrollTip
        {
            get { return Workbook.ShowScrollTip; }
            set { Workbook.ShowScrollTip = value; }
        }

        /// <summary>
        /// Gets or sets the index of the start sheet in the Excel control. 
        /// </summary>
        [DefaultValue(0), Category("Spread Workbook")]
        public int StartSheetIndex
        {
            get { return Workbook.StartSheetIndex; }
            set
            {
                if (Workbook.StartSheetIndex != value)
                {
                    Workbook.StartSheetIndex = value;
                    View.ProcessStartSheetIndexChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the tab strip is editable. 
        /// </summary>
        [DefaultValue(true), Category("Spread Workbook")]
        public bool TabStripEditable
        {
            get { return View.TabStripEditable; }
            set { View.TabStripEditable = value; }
        }

        /// <summary>
        /// Gets or sets whether a special tab is displayed that allows inserting new sheets. 
        /// </summary>
        [DefaultValue(true), Category("Spread Workbook")]
        public bool TabStripInsertTab
        {
            get { return View.TabStripInsertTab; }
            set { View.TabStripInsertTab = value; }
        }

        /// <summary>
        /// Gets or sets the width of the tab strip for this component expressed as a percentage of the overall horizontal scroll bar width. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue((double)0.5)]
        public double TabStripRatio
        {
            get { return View.TabStripRatio; }
            set { View.TabStripRatio = value; }
        }

        /// <summary>
        /// Gets or sets the display policy for the sheet tab strip for this component. 
        /// </summary>
        public Visibility TabStripVisibility
        {
            get { return View.TabStripVisibility; }
            set { View.TabStripVisibility = value; }
        }

        /// <summary>
        ///Gets themes for the control.  
        /// </summary>
        [Browsable(false)]
        public SpreadThemes Themes
        {
            get { return Workbook.Themes; }
        }

        /// <summary>
        /// Gets the undo manager for the control. 
        /// </summary>
#if IOS
        new
#endif
        public UndoManager UndoManager
        {
            get { return View.UndoManager; }
        }

        /// <summary>
        /// Gets or sets the vertical scroll bar style.
        /// </summary>
        [Category("Spread Workbook"), DefaultValue((string)null)]
        public Style VerticalScrollBarStyle
        {
            get { return View.VerticalScrollBarStyle; }
            set { View.VerticalScrollBarStyle = value; }
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
                View.VerticalScrollable = value != 0;
                View.InvalidateLayout();
                View.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the width of vertical scroll bars in this control. 
        /// </summary>
        [Category("Spread Workbook"), DefaultValue((double)25.0)]
        public double VerticalScrollBarWidth
        {
            get { return View.VerticalScrollBarWidth; }
            set { View.VerticalScrollBarWidth = value; }
        }

        /// <summary>
        /// Gets the spread view associated with the control. 
        /// </summary>
        [Browsable(false)]
        public SpreadView View { get; }

        /// <summary>
        /// Gets the workbook associated with the control. 
        /// </summary>
        [Browsable(false)]
        public Workbook Workbook
        {
            get
            {
                if (_workbook == null)
                {
                    _workbook = new Workbook();
                    _workbook.Sheets.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSheetsCollectionChanged);
                    _workbook.PropertyChanged += new PropertyChangedEventHandler(OnWorkbookPropertyChanged);
                    _workbook.Sheets.Add(new Worksheet());
                }
                return _workbook;
            }
        }
    }
}
