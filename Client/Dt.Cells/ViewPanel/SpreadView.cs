using Dt.Base;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a view of the spreadsheet display. 
    /// </summary>
    public partial class SpreadView : SheetView
    {
        Size _cachedLastAvailableSize;
        SpreadLayout _cachedSpreadLayout;
        SplitBoxAlignment _columnSplitBoxAlignment;
        SplitBoxPolicy _columnSplitBoxPolicy;
        Line _columnSplittingTracker;
        CrossSplitBar[,] _crossSplitBar;
        ScrollBar[] _horizontalScrollBar;
        double _horizontalScrollBarHeight;
        Style _horizontalScrollBarStyle;
        HorizontalSplitBar[] _horizontalSplitBar;
        HorizontalSplitBox[] _horizontalSplitBox;
        HashSet<int> _invisibleColumns;
        HashSet<int> _invisibleRows;
        bool _pendinging;
        Grid _progressGrid;
        ProgressRing _progressRing;
        SplitBoxAlignment _rowSplitBoxAlignment;
        SplitBoxPolicy _rowSplitBoxPolicy;
        Line _rowSplittingTracker;
        ScrollBarTrackPolicy _scrollBarTrackPolicy;
        int _scrollTo;
        bool _showScrollTip;
        Canvas _splittingTrackerContainer;
        TabStrip _tabStrip;
        bool _tabStripEditable;
        bool _tabStripInsertTab;
        double _tabStripRatio;
        Visibility _tabStripVisibility;
        ScrollBar[] _verticalScrollBar;
        Style _verticalScrollBarStyle;
        double _verticalScrollBarWidth;
        VerticalSplitBar[] _verticalSplitBar;
        VerticalSplitBox[] _verticalSplitBox;
        internal const double GCSPREAD_HorizontalScrollBarDefaultHeight = 25.0;
        internal const ScrollBarTrackPolicy GCSPREAD_ScrollBarTrackPolicy = ScrollBarTrackPolicy.Both;
        internal const double GCSPREAD_TabStripRatio = 0.5;
        internal const double GCSPREAD_VerticalScrollBarDefaultWidth = 25.0;
        internal static bool IsSwitchingSheet;
        bool IsTouchColumnSplitting;
        bool IsTouchRowSplitting;
        bool IsTouchTabStripScrolling;
        HorizontalSplitBox tabStripSplitBox;
        const double TABSTRIPSPLITBOX_WIDTH = 16.0;

        internal SpreadView(Excel owningSpread)
            : base(owningSpread)
        {
            _cachedLastAvailableSize = new Size(0.0, 0.0);
            _invisibleRows = new HashSet<int>();
            _invisibleColumns = new HashSet<int>();
            SpreadSheet = owningSpread;
            Init();
        }

        /// <summary>
        /// Occurs when the user has changed the active sheet. 
        /// </summary>
        public event EventHandler ActiveSheetChanged;

        /// <summary>
        /// Occurs when the user changes the active sheet. 
        /// </summary>
        public event EventHandler<CancelEventArgs> ActiveSheetChanging;

        /// <summary>
        /// Occurs when the user has changed a viewport column width. 
        /// </summary>
        public event EventHandler<ColumnViewportWidthChangedEventArgs> ColumnViewportWidthChanged;

        /// <summary>
        /// Occurs when the user changes a viewport column width. 
        /// </summary>
        public event EventHandler<ColumnViewportWidthChangingEventArgs> ColumnViewportWidthChanging;

        /// <summary>
        /// Occurs when the user has changed a viewport row height. 
        /// </summary>
        public event EventHandler<RowViewportHeightChangedEventArgs> RowViewportHeightChanged;

        /// <summary>
        /// Occurs when the user changes a viewport row height. 
        /// </summary>
        public event EventHandler<RowViewportHeightChangingEventArgs> RowViewportHeightChanging;

        /// <summary>
        /// Gets the active sheet in the current view. 
        /// </summary>
        public Worksheet ActiveSheet
        {
            get { return SpreadSheet.ActiveSheet; }
        }

        /// <summary>
        /// 
        /// </summary>
        double ActualHorizontalScrollBarHeight
        {
            get
            {
                if (SpreadSheet.HorizontalScrollBarHeight >= 0.0)
                {
                    return SpreadSheet.HorizontalScrollBarHeight;
                }
                return 25.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        double ActualVerticalScrollBarWidth
        {
            get
            {
                if (SpreadSheet.VerticalScrollBarWidth >= 0.0)
                {
                    return SpreadSheet.VerticalScrollBarWidth;
                }
                return 25.0;
            }
        }

        /// <summary>
        /// Gets or sets the column split box alignment. 
        /// </summary>
        /// <value>
        /// The column split box alignment. 
        /// </value>
        [DefaultValue(0)]
        public SplitBoxAlignment ColumnSplitBoxAlignment
        {
            get { return _columnSplitBoxAlignment; }
            set
            {
                _columnSplitBoxAlignment = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets what conditions under which the GcSpreadSheet component permits column splits. 
        /// </summary>
        [DefaultValue(0)]
        public SplitBoxPolicy ColumnSplitBoxPolicy
        {
            get { return _columnSplitBoxPolicy; }
            set
            {
                if (value != _columnSplitBoxPolicy)
                {
                    _columnSplitBoxPolicy = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the horizontal scroll bar. 
        /// </summary>
        /// <value>
        /// The height of the horizontal scroll bar. 
        /// </value>
        [DefaultValue((double)25.0)]
        public double HorizontalScrollBarHeight
        {
            get { return _horizontalScrollBarHeight; }
            set
            {
                _horizontalScrollBarHeight = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ScrollBarVisibility HorizontalScrollBarPolicy
        {
            get { return SpreadSheet.HorizontalScrollBarVisibility; }
        }

        /// <summary>
        /// Gets or sets the horizontal scroll bar style. 
        /// </summary>
        /// <value>
        /// The horizontal scroll bar style. 
        /// </value>
        [DefaultValue((string)null)]
        public Style HorizontalScrollBarStyle
        {
            get { return _horizontalScrollBarStyle; }
            set
            {
                _horizontalScrollBarStyle = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsColumnSplitting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal override bool IsEditing
        {
            get
            {
                if ((_tabStrip != null) && _tabStrip.IsEditing)
                {
                    return false;
                }
                return base.IsEditing;
            }
            set { base.IsEditing = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsRowSplitting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IsTabStripResizing { get; set; }

        /// <summary>
        /// Gets or sets the row split box alignment. 
        /// </summary>
        /// <value>
        /// The row split box alignment. 
        /// </value>
        [DefaultValue(0)]
        public SplitBoxAlignment RowSplitBoxAlignment
        {
            get { return _rowSplitBoxAlignment; }
            set
            {
                _rowSplitBoxAlignment = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets what conditions under which the GcSpreadSheet component permits row splits. 
        /// </summary>
        [DefaultValue(0)]
        public SplitBoxPolicy RowSplitBoxPolicy
        {
            get { return _rowSplitBoxPolicy; }
            set
            {
                if (value != _rowSplitBoxPolicy)
                {
                    _rowSplitBoxPolicy = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the sheet in the control scrolls when the user moves the scroll box. 
        /// </summary>
        ///<value>
        ///The scroll bar track policy. 
        ///</value>
        [DefaultValue(3)]
        public ScrollBarTrackPolicy ScrollBarTrackPolicy
        {
            get { return _scrollBarTrackPolicy; }
            set
            {
                _scrollBarTrackPolicy = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        Canvas SplittingTrackerContainer
        {
            get
            {
                if (_splittingTrackerContainer == null)
                {
                    _splittingTrackerContainer = new Canvas();
                    Canvas.SetZIndex(_splittingTrackerContainer, 0x63);
                }
                return _splittingTrackerContainer;
            }
        }

        /// <summary>
        /// Gets the Excel control that is associated with the view. 
        /// </summary>
        public Excel SpreadSheet
        {
            get { return (base._host as Excel); }
            set { base._host = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal TabStrip TabStrip
        {
            get { return _tabStrip; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the tab strip is editable.
        /// </summary>
        /// <value>
        /// true if the tab strip can be edited; otherwise, false. 
        /// </value>
        [DefaultValue(true)]
        public bool TabStripEditable
        {
            get { return _tabStripEditable; }
            set
            {
                _tabStripEditable = value;
                if ((!value && (_tabStrip != null)) && _tabStrip.IsEditing)
                {
                    _tabStrip.StopTabEditing(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether a special tab is displayed to allow the user to insert new sheets. 
        /// </summary>
        [DefaultValue(true)]
        public bool TabStripInsertTab
        {
            get { return _tabStripInsertTab; }
            set
            {
                if (value != _tabStripInsertTab)
                {
                    _tabStripInsertTab = value;
                    if ((_tabStrip != null) && (value != _tabStrip.HasInsertTab))
                    {
                        _tabStrip.HasInsertTab = value;
                        InvalidateLayout();
                        base.InvalidateMeasure();
                    }
                }
            }
        }

        /// <summary>
        ///Gets or sets the width of the tab strip for this component expressed as a percentage of the overall horizontal scroll bar width.  
        /// </summary>
        [DefaultValue((double)0.5)]
        public double TabStripRatio
        {
            get { return _tabStripRatio; }
            set
            {
                if (((value >= 0.0) && (value <= 1.0)) && (value != _tabStripRatio))
                {
                    _tabStripRatio = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        ///Gets or sets the display policy for the sheet tab strip for this component.  
        /// </summary>
        public Visibility TabStripVisibility
        {
            get { return _tabStripVisibility; }
            set
            {
                if (value != _tabStripVisibility)
                {
                    _tabStripVisibility = value;
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ScrollBarVisibility VerticalScrollBarPolicy
        {
            get { return SpreadSheet.VerticalScrollBarVisibility; }
        }

        /// <summary>
        ///Gets or sets the vertical scroll bar style. 
        /// </summary>
        /// <value>
        /// The vertical scroll bar style. 
        /// </value>
        [DefaultValue((string)null)]
        public Style VerticalScrollBarStyle
        {
            get { return _verticalScrollBarStyle; }
            set
            {
                _verticalScrollBarStyle = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        ///Gets or sets the width of the vertical scroll bar.  
        /// </summary>
        /// <value>
        /// The width of the vertical scroll bar. 
        /// </value>
        [DefaultValue((double)25.0)]
        public double VerticalScrollBarWidth
        {
            get { return _verticalScrollBarWidth; }
            set
            {
                _verticalScrollBarWidth = value;
                InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets the worksheet associated with the view.
        /// </summary>
        public override Worksheet Worksheet
        {
            get { return ActiveSheet; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void ClearMouseLeftButtonDownStates()
        {
            if (IsColumnSplitting)
            {
                EndColumnSplitting();
            }
            if (IsRowSplitting)
            {
                EndRowSplitting();
            }
            if (IsTabStripResizing)
            {
                EndTabStripResizing();
            }
            base.ClearMouseLeftButtonDownStates();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override RowLayoutModel CreateColumnHeaderRowLayoutModel()
        {
            RowLayoutModel model = new RowLayoutModel();
            SpreadLayout spreadLayout = GetSpreadLayout();
            Worksheet activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = base.ZoomFactor;
                double headerY = spreadLayout.HeaderY;
                for (int i = 0; i < activeSheet.ColumnHeader.RowCount; i++)
                {
                    double height = Math.Ceiling((double)(activeSheet.GetActualRowHeight(i, SheetArea.ColumnHeader) * zoomFactor));
                    model.Add(new RowLayout(i, headerY, height));
                    headerY += height;
                }
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        internal override ColumnLayoutModel CreateEnhancedResizeToZeroColumnHeaderViewportColumnLayoutModel(int columnViewportIndex)
        {
            if (base.ResizeZeroIndicator == ResizeZeroIndicator.Default)
            {
                return CreateViewportColumnLayoutModel(columnViewportIndex);
            }
            SpreadLayout spreadLayout = GetSpreadLayout();
            ColumnLayoutModel model = new ColumnLayoutModel();
            Worksheet activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = base.ZoomFactor;
                if (columnViewportIndex == -1)
                {
                    int frozenColumnCount = activeSheet.FrozenColumnCount;
                    if (frozenColumnCount > activeSheet.ColumnCount)
                    {
                        frozenColumnCount = activeSheet.ColumnCount;
                    }
                    double x = spreadLayout.HeaderX + spreadLayout.HeaderWidth;
                    for (int i = 0; i < frozenColumnCount; i++)
                    {
                        double width = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(i, SheetArea.Cells) * zoomFactor));
                        model.Add(new ColumnLayout(i, x, width));
                        x += width;
                    }
                    return model;
                }
                if ((columnViewportIndex >= 0) && (columnViewportIndex < spreadLayout.ColumnPaneCount))
                {
                    double viewportWidth = spreadLayout.GetViewportWidth(columnViewportIndex);
                    int viewportLeftColumn = base.GetViewportLeftColumn(columnViewportIndex);
                    if ((viewportLeftColumn > 0) && activeSheet.GetActualColumnWidth(viewportLeftColumn - 1, SheetArea.Cells).IsZero())
                    {
                        viewportLeftColumn--;
                    }
                    int num8 = (activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount) - 1;
                    int num9 = (num8 - viewportLeftColumn) + 1;
                    HashSet<int> set = new HashSet<int>();
                    HashSet<int> set2 = new HashSet<int>();
                    Dictionary<int, double> dictionary = new Dictionary<int, double>();
                    for (int j = viewportLeftColumn; ((viewportWidth > 0.0) && (j != -1)) && (j <= num8); j++)
                    {
                        dictionary.Add(j, Math.Ceiling((double)(activeSheet.GetActualColumnWidth(j, SheetArea.Cells) * zoomFactor)));
                    }
                    for (int k = viewportLeftColumn; ((viewportWidth > 0.0) && (k != -1)) && (k <= num8); k++)
                    {
                        int num12 = -1;
                        double minValue = dictionary[k];
                        while (minValue.IsZero())
                        {
                            set2.Add(k);
                            num12 = k;
                            k++;
                            if (k <= num8)
                            {
                                minValue = dictionary[k];
                            }
                            else
                            {
                                minValue = double.MinValue;
                            }
                        }
                        if (num12 != -1)
                        {
                            if (num9 != set2.Count)
                            {
                                set.Add(num12);
                            }
                            k--;
                        }
                    }
                    for (int m = viewportLeftColumn; ((viewportWidth > 0.0) && (m != -1)) && (m <= num8); m++)
                    {
                        double num15 = dictionary[m];
                        if (set.Contains(m))
                        {
                            int num16 = m - 1;
                            int num17 = m + 1;
                            while (true)
                            {
                                if (!set2.Contains(num16))
                                {
                                    break;
                                }
                                num16--;
                            }
                            while (set2.Contains(num17))
                            {
                                num17++;
                            }
                            if ((num16 >= viewportLeftColumn) && (num17 <= num8))
                            {
                                double num18 = dictionary[num16];
                                double num19 = dictionary[num17];
                                num15 = Math.Min(num18, 3.0) + Math.Min(num19, 3.0);
                                dictionary[num16] = Math.Max((double)0.0, (double)(num18 - 3.0));
                                dictionary[num17] = Math.Max((double)0.0, (double)(num19 - 3.0));
                            }
                            else if ((num16 < viewportLeftColumn) && (num17 <= num8))
                            {
                                double num20 = dictionary[num17];
                                num15 = Math.Min(num20, 3.0);
                                dictionary[num17] = Math.Max((double)0.0, (double)(num20 - 3.0));
                            }
                            else if ((num16 >= viewportLeftColumn) && (num17 > num8))
                            {
                                double num21 = dictionary[num16];
                                num15 = Math.Min(num21, 3.0);
                                dictionary[num16] = Math.Max((double)0.0, (double)(num21 - 3.0));
                            }
                            dictionary[m] = num15;
                        }
                        viewportWidth -= num15;
                    }
                    viewportWidth = spreadLayout.GetViewportWidth(columnViewportIndex);
                    double viewportX = spreadLayout.GetViewportX(columnViewportIndex);
                    for (int n = viewportLeftColumn; ((viewportWidth > 0.0) && (n != -1)) && (n <= num8); n++)
                    {
                        double num24 = dictionary[n];
                        model.Add(new ColumnLayout(n, viewportX, num24));
                        viewportX += num24;
                        viewportWidth -= num24;
                    }
                    return model;
                }
                if (columnViewportIndex == spreadLayout.ColumnPaneCount)
                {
                    double num25 = spreadLayout.GetViewportX(spreadLayout.ColumnPaneCount - 1) + spreadLayout.GetViewportWidth(spreadLayout.ColumnPaneCount - 1);
                    if ((base.IsTouching && (Worksheet.FrozenTrailingColumnCount > 0)) && ((base._touchStartHitTestInfo.ColumnViewportIndex == (spreadLayout.ColumnPaneCount - 1)) && (base._translateOffsetX < 0.0)))
                    {
                        num25 += base._translateOffsetX;
                    }
                    for (int num26 = Math.Max(activeSheet.FrozenColumnCount, activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount); num26 < activeSheet.ColumnCount; num26++)
                    {
                        double num27 = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(num26, SheetArea.Cells) * zoomFactor));
                        if ((num27 == 0.0) && (base.ResizeZeroIndicator == ResizeZeroIndicator.Enhanced))
                        {
                            num27 = 4.0;
                        }
                        model.Add(new ColumnLayout(num26, num25, num27));
                        num25 += num27;
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        internal override RowLayoutModel CreateEnhancedResizeToZeroRowHeaderViewportRowLayoutModel(int rowViewportIndex)
        {
            if (base.ResizeZeroIndicator == ResizeZeroIndicator.Default)
            {
                return CreateViewportRowLayoutModel(rowViewportIndex);
            }
            RowLayoutModel model = new RowLayoutModel();
            SpreadLayout spreadLayout = GetSpreadLayout();
            Worksheet activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = base.ZoomFactor;
                if (rowViewportIndex == -1)
                {
                    double y = spreadLayout.HeaderY + spreadLayout.HeaderHeight;
                    int frozenRowCount = Worksheet.FrozenRowCount;
                    if (Worksheet.RowCount < frozenRowCount)
                    {
                        frozenRowCount = Worksheet.RowCount;
                    }
                    for (int i = 0; i < frozenRowCount; i++)
                    {
                        double height = Math.Ceiling((double)(activeSheet.GetActualRowHeight(i, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(i, y, height));
                        y += height;
                    }
                    return model;
                }
                if ((rowViewportIndex >= 0) && (rowViewportIndex < spreadLayout.RowPaneCount))
                {
                    double viewportHeight = spreadLayout.GetViewportHeight(rowViewportIndex);
                    int viewportTopRow = base.GetViewportTopRow(rowViewportIndex);
                    if ((viewportTopRow > 0) && activeSheet.GetActualRowHeight(viewportTopRow - 1, SheetArea.Cells).IsZero())
                    {
                        viewportTopRow--;
                    }
                    int num8 = (activeSheet.RowCount - activeSheet.FrozenTrailingRowCount) - 1;
                    int num9 = (num8 - viewportTopRow) + 1;
                    HashSet<int> set = new HashSet<int>();
                    HashSet<int> set2 = new HashSet<int>();
                    Dictionary<int, double> dictionary = new Dictionary<int, double>();
                    for (int j = viewportTopRow; ((viewportHeight > 0.0) && (j != -1)) && (j <= num8); j++)
                    {
                        dictionary.Add(j, Math.Ceiling((double)(activeSheet.GetActualRowHeight(j, SheetArea.Cells) * zoomFactor)));
                    }
                    for (int k = viewportTopRow; ((viewportHeight > 0.0) && (k != -1)) && (k <= num8); k++)
                    {
                        int num12 = -1;
                        double minValue = dictionary[k];
                        while (minValue.IsZero())
                        {
                            set2.Add(k);
                            num12 = k;
                            k++;
                            if (k <= num8)
                            {
                                minValue = dictionary[k];
                            }
                            else
                            {
                                minValue = double.MinValue;
                            }
                        }
                        if (num12 != -1)
                        {
                            if (num9 != set2.Count)
                            {
                                set.Add(num12);
                            }
                            k--;
                        }
                    }
                    for (int m = viewportTopRow; ((viewportHeight > 0.0) && (m != -1)) && (m <= num8); m++)
                    {
                        double num15 = dictionary[m];
                        if (set.Contains(m))
                        {
                            int num16 = m - 1;
                            int num17 = m + 1;
                            while (true)
                            {
                                if (!set2.Contains(num16))
                                {
                                    break;
                                }
                                num16--;
                            }
                            while (set2.Contains(num17))
                            {
                                num17++;
                            }
                            if ((num16 >= viewportTopRow) && (num17 <= num8))
                            {
                                double num18 = dictionary[num16];
                                double num19 = dictionary[num17];
                                num15 = Math.Min(num18, 3.0) + Math.Min(num19, 3.0);
                                dictionary[num16] = Math.Max((double)0.0, (double)(num18 - 3.0));
                                dictionary[num17] = Math.Max((double)0.0, (double)(num19 - 3.0));
                            }
                            else if ((num16 < viewportTopRow) && (num17 <= num8))
                            {
                                double num20 = dictionary[num17];
                                num15 = Math.Min(num20, 3.0);
                                dictionary[num17] = Math.Max((double)0.0, (double)(num20 - 3.0));
                            }
                            else if ((num16 >= viewportTopRow) && (num17 > num8))
                            {
                                double num21 = dictionary[num16];
                                num15 = Math.Min(num21, 3.0);
                                dictionary[num16] = Math.Max((double)0.0, (double)(num21 - 3.0));
                            }
                            dictionary[m] = num15;
                        }
                        viewportHeight -= num15;
                    }
                    double viewportY = spreadLayout.GetViewportY(rowViewportIndex);
                    viewportHeight = spreadLayout.GetViewportHeight(rowViewportIndex);
                    for (int n = viewportTopRow; ((viewportHeight > 0.0) && (n != -1)) && (n <= num8); n++)
                    {
                        double num24 = dictionary[n];
                        model.Add(new RowLayout(n, viewportY, num24));
                        viewportY += num24;
                        viewportHeight -= num24;
                    }
                    return model;
                }
                if (rowViewportIndex == spreadLayout.RowPaneCount)
                {
                    double num25 = spreadLayout.GetViewportY(spreadLayout.RowPaneCount - 1) + spreadLayout.GetViewportHeight(spreadLayout.RowPaneCount - 1);
                    if ((base.IsTouching && (Worksheet.FrozenTrailingColumnCount > 0)) && ((base._touchStartHitTestInfo.RowViewportIndex == (spreadLayout.RowPaneCount - 1)) && (base._translateOffsetY < 0.0)))
                    {
                        num25 += base._translateOffsetY;
                    }
                    for (int num26 = Math.Max(activeSheet.FrozenRowCount, activeSheet.RowCount - activeSheet.FrozenTrailingRowCount); num26 < activeSheet.RowCount; num26++)
                    {
                        double num27 = Math.Ceiling((double)(activeSheet.GetActualRowHeight(num26, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(num26, num25, num27));
                        num25 += num27;
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override ColumnLayoutModel CreateRowHeaderColumnLayoutModel()
        {
            ColumnLayoutModel model = new ColumnLayoutModel();
            SpreadLayout spreadLayout = GetSpreadLayout();
            Worksheet activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = base.ZoomFactor;
                double headerX = spreadLayout.HeaderX;
                for (int i = 0; i < activeSheet.RowHeader.ColumnCount; i++)
                {
                    double width = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(i, SheetArea.CornerHeader | SheetArea.RowHeader) * zoomFactor));
                    model.Add(new ColumnLayout(i, headerX, width));
                    headerX += width;
                }
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        internal override ColumnLayoutModel CreateViewportColumnLayoutModel(int columnViewportIndex)
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            ColumnLayoutModel model = new ColumnLayoutModel();
            Worksheet activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = base.ZoomFactor;
                if (columnViewportIndex == -1)
                {
                    int frozenColumnCount = activeSheet.FrozenColumnCount;
                    if (frozenColumnCount > activeSheet.ColumnCount)
                    {
                        frozenColumnCount = activeSheet.ColumnCount;
                    }
                    double x = spreadLayout.HeaderX + spreadLayout.HeaderWidth;
                    for (int i = 0; i < frozenColumnCount; i++)
                    {
                        double width = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(i, SheetArea.Cells) * zoomFactor));
                        model.Add(new ColumnLayout(i, x, width));
                        x += width;
                    }
                    return model;
                }
                if ((columnViewportIndex >= 0) && (columnViewportIndex < spreadLayout.ColumnPaneCount))
                {
                    double viewportX = spreadLayout.GetViewportX(columnViewportIndex);
                    double viewportWidth = spreadLayout.GetViewportWidth(columnViewportIndex);
                    for (int j = base.GetViewportLeftColumn(columnViewportIndex); ((viewportWidth > 0.0) && (j != -1)) && (j < (activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount)); j++)
                    {
                        double num9 = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(j, SheetArea.Cells) * zoomFactor));
                        model.Add(new ColumnLayout(j, viewportX, num9));
                        viewportX += num9;
                        viewportWidth -= num9;
                    }
                    return model;
                }
                if (columnViewportIndex == spreadLayout.ColumnPaneCount)
                {
                    double num10 = spreadLayout.GetViewportX(spreadLayout.ColumnPaneCount - 1) + spreadLayout.GetViewportWidth(spreadLayout.ColumnPaneCount - 1);
                    if ((base.IsTouching && (Worksheet.FrozenTrailingColumnCount > 0)) && ((base._touchStartHitTestInfo.ColumnViewportIndex == (spreadLayout.ColumnPaneCount - 1)) && (base._translateOffsetX < 0.0)))
                    {
                        num10 += base._translateOffsetX;
                    }
                    for (int k = Math.Max(activeSheet.FrozenColumnCount, activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount); k < activeSheet.ColumnCount; k++)
                    {
                        double num12 = Math.Ceiling((double)(activeSheet.GetActualColumnWidth(k, SheetArea.Cells) * zoomFactor));
                        model.Add(new ColumnLayout(k, num10, num12));
                        num10 += num12;
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        internal override RowLayoutModel CreateViewportRowLayoutModel(int rowViewportIndex)
        {
            RowLayoutModel model = new RowLayoutModel();
            SpreadLayout spreadLayout = GetSpreadLayout();
            Worksheet activeSheet = ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = base.ZoomFactor;
                if (rowViewportIndex == -1)
                {
                    double y = spreadLayout.HeaderY + spreadLayout.HeaderHeight;
                    int frozenRowCount = Worksheet.FrozenRowCount;
                    if (Worksheet.RowCount < frozenRowCount)
                    {
                        frozenRowCount = Worksheet.RowCount;
                    }
                    for (int i = 0; i < frozenRowCount; i++)
                    {
                        double height = Math.Ceiling((double)(activeSheet.GetActualRowHeight(i, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(i, y, height));
                        y += height;
                    }
                    return model;
                }
                if ((rowViewportIndex >= 0) && (rowViewportIndex < spreadLayout.RowPaneCount))
                {
                    double viewportY = spreadLayout.GetViewportY(rowViewportIndex);
                    double viewportHeight = spreadLayout.GetViewportHeight(rowViewportIndex);
                    int rowCount = activeSheet.RowCount;
                    for (int j = base.GetViewportTopRow(rowViewportIndex); ((viewportHeight > 0.0) && (j != -1)) && (j < (rowCount - activeSheet.FrozenTrailingRowCount)); j++)
                    {
                        double num10 = Math.Ceiling((double)(activeSheet.GetActualRowHeight(j, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(j, viewportY, num10));
                        viewportY += num10;
                        viewportHeight -= num10;
                    }
                    return model;
                }
                if (rowViewportIndex == spreadLayout.RowPaneCount)
                {
                    double num11 = spreadLayout.GetViewportY(spreadLayout.RowPaneCount - 1) + spreadLayout.GetViewportHeight(spreadLayout.RowPaneCount - 1);
                    if ((base.IsTouching && (Worksheet.FrozenTrailingColumnCount > 0)) && ((base._touchStartHitTestInfo.RowViewportIndex == (spreadLayout.RowPaneCount - 1)) && (base._translateOffsetY < 0.0)))
                    {
                        num11 += base._translateOffsetY;
                    }
                    for (int k = Math.Max(activeSheet.FrozenRowCount, activeSheet.RowCount - activeSheet.FrozenTrailingRowCount); k < activeSheet.RowCount; k++)
                    {
                        double num13 = Math.Ceiling((double)(activeSheet.GetActualRowHeight(k, SheetArea.Cells) * zoomFactor));
                        model.Add(new RowLayout(k, num11, num13));
                        num11 += num13;
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// Sets the column viewport's left column. 
        /// </summary>
        /// <param name="columnViewportIndex">The column viewport index.</param>
        /// <param name="value">The column index.</param>
        public override void SetViewportLeftColumn(int columnViewportIndex, int value)
        {
            if (base.HorizontalScrollable)
            {
                value = Math.Max(Worksheet.FrozenColumnCount, value);
                value = Math.Min((Worksheet.ColumnCount - Worksheet.FrozenTrailingColumnCount) - 1, value);
                value = base.TryGetNextScrollableColumn(value);
                base.SetViewportLeftColumn(columnViewportIndex, value);
                if (_horizontalScrollBar != null)
                {
                    GetSpreadLayout();
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
        public override void SetViewportTopRow(int rowViewportIndex, int value)
        {
            Action action = null;
            if (base.VerticalScrollable)
            {
                value = Math.Max(Worksheet.FrozenRowCount, value);
                value = Math.Min((Worksheet.RowCount - Worksheet.FrozenTrailingRowCount) - 1, value);
                value = base.TryGetNextScrollableRow(value);
                if (_verticalScrollBar != null)
                {
                    GetSpreadLayout();
                    if (action == null)
                    {
                        action = delegate
                        {
                            if (((rowViewportIndex > -1) && (rowViewportIndex < _verticalScrollBar.Length)) && (value != _verticalScrollBar[rowViewportIndex].Value))
                            {
                                int invisibleRowsBeforeRow = GetInvisibleRowsBeforeRow(ActiveSheet, value);
                                int num2 = value - invisibleRowsBeforeRow;
                                _verticalScrollBar[rowViewportIndex].Value = (double)num2;
                                _verticalScrollBar[rowViewportIndex].InvalidateArrange();
                            }
                        };
                    }
                    UIAdaptor.InvokeSync(action);
                }
                base.SetViewportTopRow(rowViewportIndex, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <param name="viewportWidth"></param>
        internal void AddColumnViewport(int columnViewportIndex, double viewportWidth)
        {
            Worksheet.AddColumnViewport(columnViewportIndex, viewportWidth / ((double)base.ZoomFactor));
            InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <param name="viewportHeight"></param>
        internal void AddRowViewport(int rowViewportIndex, double viewportHeight)
        {
            Worksheet.AddRowViewport(rowViewportIndex, viewportHeight / ((double)base.ZoomFactor));
            InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        void AdjustViewportLeftColumn()
        {
            if (base._translateOffsetX != 0.0)
            {
                int viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
                if (viewportLeftColumn >= Worksheet.FrozenColumnCount)
                {
                    ColumnLayout layout = base.GetColumnLayoutModel(base._touchStartHitTestInfo.ColumnViewportIndex, SheetArea.Cells).FindColumn(viewportLeftColumn);
                    if (layout != null)
                    {
                        double width = layout.Width;
                        int maxLeftScrollableColumn = base.GetMaxLeftScrollableColumn();
                        if (viewportLeftColumn <= maxLeftScrollableColumn)
                        {
                            if ((base._translateOffsetX < 0.0) && (Math.Abs(base._translateOffsetX) >= (width / 2.0)))
                            {
                                int nextScrollableColumn = base.GetNextScrollableColumn(viewportLeftColumn);
                                if (nextScrollableColumn != -1)
                                {
                                    SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, nextScrollableColumn);
                                }
                            }
                        }
                        else if (Math.Abs(base._translateOffsetX) >= (width / 2.0))
                        {
                            int num5 = base.GetNextScrollableColumn(viewportLeftColumn);
                            if (num5 != -1)
                            {
                                SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num5);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void AdjustViewportSize()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if (spreadLayout != null)
            {
                if (((base._cachedViewportWidths != null) && (base._touchStartHitTestInfo.ColumnViewportIndex != -1)) && (base._touchStartHitTestInfo.ColumnViewportIndex < spreadLayout.ColumnPaneCount))
                {
                    spreadLayout.SetViewportWidth(base._touchStartHitTestInfo.ColumnViewportIndex, base._cachedViewportWidths[base._touchStartHitTestInfo.ColumnViewportIndex + 1]);
                }
                if (((base._cachedViewportHeights != null) && (base._touchStartHitTestInfo.RowViewportIndex != -1)) && (base._touchStartHitTestInfo.RowViewportIndex < spreadLayout.RowPaneCount))
                {
                    spreadLayout.SetViewportHeight(base._touchStartHitTestInfo.RowViewportIndex, base._cachedViewportHeights[base._touchStartHitTestInfo.RowViewportIndex + 1]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void AdjustViewportTopRow()
        {
            if (base._translateOffsetY != 0.0)
            {
                int viewportTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
                if (viewportTopRow >= Worksheet.FrozenRowCount)
                {
                    RowLayout layout = base.GetRowLayoutModel(base._touchStartHitTestInfo.RowViewportIndex, SheetArea.Cells).FindRow(viewportTopRow);
                    if (layout != null)
                    {
                        double height = layout.Height;
                        int maxTopScrollableRow = base.GetMaxTopScrollableRow();
                        if (viewportTopRow <= maxTopScrollableRow)
                        {
                            if ((base._translateOffsetY < 0.0) && (Math.Abs(base._translateOffsetY) >= (height / 2.0)))
                            {
                                int nextScrollableRow = base.GetNextScrollableRow(viewportTopRow);
                                if (nextScrollableRow != -1)
                                {
                                    SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, nextScrollableRow);
                                }
                            }
                        }
                        else if (Math.Abs(base._translateOffsetY) >= (height / 2.0))
                        {
                            int num5 = base.GetNextScrollableRow(viewportTopRow);
                            if (num5 != -1)
                            {
                                SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, num5);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the top row index async, for performance optimization 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        async void AsynSetViewportTopRow(int rowViewportIndex)
        {
            if (!_pendinging)
            {
                _pendinging = true;
                await base.Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate
                {
                    _pendinging = false;
                    if (GetViewportTopRow(rowViewportIndex) != _scrollTo)
                    {
                        base.SetViewportTopRow(rowViewportIndex, _scrollTo);
                    }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ClearViewportsClip()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if (spreadLayout != null)
            {
                if (base._viewportPresenters != null)
                {
                    for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
                    {
                        for (int j = -1; j <= spreadLayout.RowPaneCount; j++)
                        {
                            GcViewport viewport = base._viewportPresenters[j + 1, i + 1];
                            if ((viewport != null) && (viewport.Clip != null))
                            {
                                viewport.Clip = null;
                            }
                        }
                    }
                }
                if (base._columnHeaderPresenters != null)
                {
                    for (int k = -1; k <= spreadLayout.ColumnPaneCount; k++)
                    {
                        GcViewport viewport2 = base._columnHeaderPresenters[k + 1];
                        if ((viewport2 != null) && (viewport2.Clip != null))
                        {
                            viewport2.Clip = null;
                        }
                    }
                }
                if (base._rowHeaderPresenters != null)
                {
                    for (int m = -1; m <= spreadLayout.RowPaneCount; m++)
                    {
                        GcViewport viewport3 = base._rowHeaderPresenters[m + 1];
                        if ((viewport3 != null) && (viewport3.Clip != null))
                        {
                            viewport3.Clip = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ContinueColumnSplitting()
        {
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            SpreadLayout spreadLayout = GetSpreadLayout();
            int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (base.MousePosition.X <= _columnSplittingTracker.X1)
                    {
                        _columnSplittingTracker.X1 = Math.Max(base.MousePosition.X, spreadLayout.GetViewportX(columnViewportIndex) + (spreadLayout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0));
                        break;
                    }
                    _columnSplittingTracker.X1 = Math.Min(base.MousePosition.X, (spreadLayout.GetViewportX(columnViewportIndex + 1) + spreadLayout.GetViewportWidth(columnViewportIndex + 1)) - (spreadLayout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0));
                    break;

                case HitTestType.ColumnSplitBox:
                    if (base.MousePosition.X <= _columnSplittingTracker.X1)
                    {
                        _columnSplittingTracker.X1 = Math.Max(base.MousePosition.X, spreadLayout.GetViewportX(columnViewportIndex) + (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0));
                        break;
                    }
                    _columnSplittingTracker.X1 = Math.Min(base.MousePosition.X, (spreadLayout.GetViewportX(columnViewportIndex) + spreadLayout.GetViewportWidth(columnViewportIndex)) - (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0));
                    break;
            }
            _columnSplittingTracker.X2 = _columnSplittingTracker.X1;
        }

        /// <summary>
        /// 
        /// </summary>
        void ContinueRowSplitting()
        {
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            SpreadLayout spreadLayout = GetSpreadLayout();
            int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (base.MousePosition.Y <= _rowSplittingTracker.Y1)
                    {
                        _rowSplittingTracker.Y1 = Math.Max(base.MousePosition.Y, spreadLayout.GetViewportY(rowViewportIndex) + (spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0));
                        break;
                    }
                    _rowSplittingTracker.Y1 = Math.Min(base.MousePosition.Y, (spreadLayout.GetViewportY(rowViewportIndex + 1) + spreadLayout.GetViewportHeight(rowViewportIndex + 1)) - (spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0));
                    break;

                case HitTestType.RowSplitBox:
                    if (base.MousePosition.Y <= _rowSplittingTracker.Y1)
                    {
                        _rowSplittingTracker.Y1 = Math.Max(base.MousePosition.Y, spreadLayout.GetViewportY(rowViewportIndex) + (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0));
                        break;
                    }
                    _rowSplittingTracker.Y1 = Math.Min(base.MousePosition.Y, (spreadLayout.GetViewportY(rowViewportIndex) + spreadLayout.GetViewportHeight(rowViewportIndex)) - (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0));
                    break;
            }
            _rowSplittingTracker.Y2 = _rowSplittingTracker.Y1;
        }

        /// <summary>
        /// 
        /// </summary>
        void ContinueTabStripResizing()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double tabStripX = spreadLayout.TabStripX;
            double tabStripHeight = spreadLayout.TabStripHeight;
            double num2 = spreadLayout.GetHorizontalScrollBarWidth(0) + spreadLayout.TabStripWidth;
            double num3 = Math.Min(Math.Max((double)0.0, (double)(base.MousePosition.X - tabStripX)), num2);
            _tabStripRatio = num3 / num2;
            InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="renderSize"></param>
        /// <returns></returns>
        BitmapSource CreateCachedIamge(UIElement child, Size renderSize)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        Image CreateCachedVisual(ImageSource image, Rect bounds)
        {
#if UWP
            if (!IsZero(bounds.Width) && !IsZero(bounds.Height))
            {
                WriteableBitmap bitmap = image as WriteableBitmap;
                if (bitmap != null)
                {
                    Image image2 = new Image();
                    image2.Width = (double)bitmap.PixelWidth;
                    image2.Height = (double)bitmap.PixelHeight;
                    image2.HorizontalAlignment = (HorizontalAlignment)3;
                    image2.VerticalAlignment = (VerticalAlignment)3;
                    image2.IsHitTestVisible = false;
                    image2.Source = image;
                    image2.Stretch = 0;
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = bounds;
                    image2.Clip = geometry;
                    image2.Margin = new Thickness(-bounds.X, -bounds.Y, bounds.Right - bitmap.PixelWidth, bounds.Bottom - bitmap.PixelHeight);
                    return image2;
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnX"></param>
        /// <param name="rowY"></param>
        /// <param name="viewportWidth"></param>
        /// <param name="viewportHeight"></param>
        /// <param name="viewportColumnIndex"></param>
        /// <param name="viewportRowIndex"></param>
        /// <returns></returns>
        Rect CreateClipRect(double columnX, double rowY, double viewportWidth, double viewportHeight, int viewportColumnIndex, int viewportRowIndex)
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double num = columnX;
            double num2 = rowY;
            double num3 = viewportWidth;
            double num4 = viewportHeight;
            if ((Worksheet.FrozenColumnCount > 0) && base.ShowFreezeLine)
            {
                if (viewportColumnIndex == 0)
                {
                    num++;
                    num3 = Math.Max((double)0.0, (double)(num3 - 1.0));
                }
                else if (viewportColumnIndex == -1)
                {
                    num3 = Math.Max((double)0.0, (double)(num3 - 1.0));
                }
            }
            if ((Worksheet.FrozenRowCount > 0) && base.ShowFreezeLine)
            {
                if (viewportRowIndex == 0)
                {
                    num2++;
                    num4 = Math.Max((double)0.0, (double)(num4 - 1.0));
                }
                else if (viewportRowIndex == -1)
                {
                    num4 = Math.Max((double)0.0, (double)(num4 - 1.0));
                }
            }
            if ((Worksheet.FrozenTrailingColumnCount > 0) && base.ShowFreezeLine)
            {
                if (viewportColumnIndex == (spreadLayout.ColumnPaneCount - 1))
                {
                    num3 = Math.Max((double)0.0, (double)(num3 - 1.0));
                }
                else if (viewportColumnIndex == spreadLayout.ColumnPaneCount)
                {
                    num++;
                    num3 = Math.Max((double)0.0, (double)(num3 - 1.0));
                }
            }
            if ((Worksheet.FrozenTrailingRowCount > 0) && base.ShowFreezeLine)
            {
                if (viewportRowIndex == (spreadLayout.RowPaneCount - 1))
                {
                    num4 = Math.Max((double)0.0, (double)(num4 - 1.0));
                }
                else if (viewportRowIndex == spreadLayout.RowPaneCount)
                {
                    num2++;
                    num4 = Math.Max((double)0.0, (double)(num4 - 1.0));
                }
            }
            double num5 = 0.0;
            int viewportTopRow = base.GetViewportTopRow(viewportRowIndex);
            int viewportBottomRow = base.GetViewportBottomRow(viewportRowIndex);
            if (((viewportTopRow >= 0) && (viewportBottomRow < Worksheet.RowCount)) && (viewportTopRow <= viewportBottomRow))
            {
                RowLayoutModel rowLayoutModel = base.GetRowLayoutModel(viewportRowIndex, SheetArea.Cells);
                if (rowLayoutModel != null)
                {
                    for (int i = viewportTopRow; i <= viewportBottomRow; i++)
                    {
                        RowLayout layout2 = rowLayoutModel.FindRow(i);
                        if (layout2 != null)
                        {
                            num5 += layout2.Height;
                        }
                        if (num5 >= viewportHeight)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    num5 = num4;
                }
            }
            else
            {
                num5 = num4;
            }
            double num9 = 0.0;
            int viewportLeftColumn = base.GetViewportLeftColumn(viewportColumnIndex);
            int viewportRightColumn = base.GetViewportRightColumn(viewportColumnIndex);
            if (((viewportLeftColumn >= 0) && (viewportRightColumn < Worksheet.ColumnCount)) && (viewportLeftColumn <= viewportRightColumn))
            {
                ColumnLayoutModel columnLayoutModel = base.GetColumnLayoutModel(viewportColumnIndex, SheetArea.Cells);
                if (columnLayoutModel != null)
                {
                    for (int j = viewportLeftColumn; j <= viewportRightColumn; j++)
                    {
                        ColumnLayout layout3 = columnLayoutModel.FindColumn(j);
                        if (layout3 != null)
                        {
                            num9 += layout3.Width;
                        }
                        if (num9 >= viewportWidth)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    num9 = num3;
                }
            }
            else
            {
                num9 = num3;
            }
            return new Rect { X = num, Y = num2, Width = Math.Min(num9, num3), Height = Math.Min(num5, num4) };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        SpreadLayout CreateSpreadLayout()
        {
            Worksheet activeSheet = ActiveSheet;
            ViewportInfo viewportInfo = base.GetViewportInfo();
            double width = base.AvailableSize.Width;
            double height = base.AvailableSize.Height;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            SpreadLayout layout = new SpreadLayout(viewportInfo.RowViewportCount, viewportInfo.ColumnViewportCount)
            {
                X = 0.0,
                Y = 0.0
            };
            if ((activeSheet == null) || !activeSheet.Visible)
            {
                layout.TabStripX = 0.0;
                layout.TabStripHeight = 25.0;
                layout.TabStripY = Math.Max((double)0.0, (double)(height - layout.TabStripHeight));
                layout.TabStripWidth = Math.Max(0.0, width);
                return layout;
            }
            GroupLayout groupLayout = base.GetGroupLayout();
            layout.HeaderX = layout.X + groupLayout.Width;
            layout.HeaderY = layout.Y + groupLayout.Height;
            float zoomFactor = base.ZoomFactor;
            if (activeSheet.RowHeader.IsVisible)
            {
                for (int num8 = 0; num8 < activeSheet.RowHeader.Columns.Count; num8++)
                {
                    layout.HeaderWidth += Math.Ceiling((double)(activeSheet.GetActualColumnWidth(num8, SheetArea.CornerHeader | SheetArea.RowHeader) * zoomFactor));
                }
                num5 += layout.HeaderWidth;
            }
            if (activeSheet.ColumnHeader.IsVisible)
            {
                for (int num9 = 0; num9 < activeSheet.ColumnHeader.Rows.Count; num9++)
                {
                    layout.HeaderHeight += Math.Ceiling((double)(activeSheet.GetActualRowHeight(num9, SheetArea.ColumnHeader) * zoomFactor));
                }
                num6 += layout.HeaderHeight;
            }
            layout.FrozenX = layout.HeaderX + layout.HeaderWidth;
            layout.FrozenY = layout.HeaderY + layout.HeaderHeight;
            for (int i = 0; i < activeSheet.FrozenColumnCount; i++)
            {
                layout.FrozenWidth += Math.Ceiling((double)(activeSheet.GetActualColumnWidth(i, SheetArea.Cells) * zoomFactor));
            }
            for (int j = 0; j < activeSheet.FrozenRowCount; j++)
            {
                layout.FrozenHeight += Math.Ceiling((double)(activeSheet.GetActualRowHeight(j, SheetArea.Cells) * zoomFactor));
            }
            for (int k = Math.Max(activeSheet.FrozenColumnCount, activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount); k < activeSheet.ColumnCount; k++)
            {
                layout.FrozenTrailingWidth += Math.Ceiling((double)(activeSheet.GetActualColumnWidth(k, SheetArea.Cells) * zoomFactor));
            }
            for (int m = Math.Max(activeSheet.FrozenRowCount, activeSheet.RowCount - activeSheet.FrozenTrailingRowCount); m < activeSheet.RowCount; m++)
            {
                layout.FrozenTrailingHeight += Math.Ceiling((double)(activeSheet.GetActualRowHeight(m, SheetArea.Cells) * zoomFactor));
            }
            num5 += layout.FrozenWidth + layout.FrozenTrailingWidth;
            num6 += layout.FrozenHeight + layout.FrozenTrailingHeight;
            for (int n = activeSheet.FrozenColumnCount; (num3 <= width) && (n < (activeSheet.ColumnCount - activeSheet.FrozenTrailingColumnCount)); n++)
            {
                num3 += Math.Ceiling((double)(activeSheet.GetActualColumnWidth(n, SheetArea.Cells) * zoomFactor));
            }
            for (int num15 = activeSheet.FrozenRowCount; (num4 <= height) && (num15 < (activeSheet.RowCount - activeSheet.FrozenTrailingRowCount)); num15++)
            {
                num4 += Math.Ceiling((double)(activeSheet.GetActualRowHeight(num15, SheetArea.Cells) * zoomFactor));
            }
            num5 += num3;
            num6 += num4;
            bool flag = (HorizontalScrollBarPolicy == (ScrollBarVisibility)3) || (HorizontalScrollBarPolicy == 0);
            if (HorizontalScrollBarPolicy == (ScrollBarVisibility)1)
            {
                if (layout.ColumnPaneCount > 1)
                {
                    flag = true;
                }
                else if ((VerticalScrollBarPolicy == (ScrollBarVisibility)3) || (VerticalScrollBarPolicy == 0))
                {
                    flag |= num5 > ((width - ActualVerticalScrollBarWidth) - groupLayout.Width);
                }
                else if (VerticalScrollBarPolicy == (ScrollBarVisibility)1)
                {
                    if (num4 > height)
                    {
                        flag |= num5 > ((width - ActualVerticalScrollBarWidth) - groupLayout.Width);
                    }
                    else
                    {
                        flag |= num5 > (width - groupLayout.Width);
                    }
                }
                else
                {
                    flag |= num5 > (width - groupLayout.Width);
                }
            }
            if (flag)
            {
                layout.OrnamentHeight = ActualHorizontalScrollBarHeight;
                height -= layout.OrnamentHeight;
                height = Math.Max(0.0, height);
            }
            if (TabStripVisibility == 0)
            {
                if (layout.OrnamentHeight > 0.0)
                {
                    layout.TabStripHeight = layout.OrnamentHeight;
                }
                else
                {
                    layout.TabStripHeight = 25.0;
                    height -= layout.TabStripHeight;
                    height = Math.Max(0.0, height);
                }
            }
            bool flag3 = ((VerticalScrollBarPolicy == (ScrollBarVisibility)3) || (VerticalScrollBarPolicy == 0)) || ((VerticalScrollBarPolicy == (ScrollBarVisibility)1) && ((layout.RowPaneCount > 1) || (num6 > (height - groupLayout.Height))));
            if (flag3)
            {
                layout.OrnamentWidth = ActualVerticalScrollBarWidth;
                width -= layout.OrnamentWidth;
                width = Math.Max(0.0, width);
            }
            width -= layout.HeaderX;
            width -= layout.HeaderWidth;
            width = Math.Max(0.0, width);
            if (width < layout.FrozenWidth)
            {
                layout.FrozenWidth = width;
                width = 0.0;
            }
            else
            {
                width -= layout.FrozenWidth;
            }
            width -= layout.FrozenTrailingWidth;
            width = Math.Max(0.0, width);
            height -= layout.HeaderY;
            height -= layout.HeaderHeight;
            height = Math.Max(0.0, height);
            if (height < layout.FrozenHeight)
            {
                layout.FrozenHeight = height;
                height = 0.0;
            }
            else
            {
                height -= layout.FrozenHeight;
            }
            height -= layout.FrozenTrailingHeight;
            height = Math.Max(0.0, height);
            for (int num16 = 0; num16 < (layout.ColumnPaneCount - 1); num16++)
            {
                double num17 = 6.0;
                layout.SetHorizontalSplitBarWidth(num16, num17);
                width -= layout.GetHorizontalSplitBarWidth(num16);
                width = Math.Max(0.0, width);
            }
            for (int num18 = 0; num18 < (layout.RowPaneCount - 1); num18++)
            {
                double num19 = 6.0;
                layout.SetVerticalSplitBarHeight(num18, num19);
                height -= layout.GetVerticalSplitBarHeight(num18);
                height = Math.Max(0.0, height);
            }
            int num20 = 0;
            int num21 = 0;
            for (int num22 = 0; num22 < layout.ColumnPaneCount; num22++)
            {
                if (viewportInfo.ViewportWidth[num22] < 0.0)
                {
                    num20++;
                }
                else
                {
                    layout.SetViewportWidth(num22, Math.Max(0.0, Math.Min(width, viewportInfo.ViewportWidth[num22] * zoomFactor)));
                    width -= layout.GetViewportWidth(num22);
                }
            }
            for (int num23 = 0; num23 < layout.RowPaneCount; num23++)
            {
                if (viewportInfo.ViewportHeight[num23] < 0.0)
                {
                    num21++;
                }
                else
                {
                    layout.SetViewportHeight(num23, Math.Max(0.0, Math.Min(height, viewportInfo.ViewportHeight[num23] * zoomFactor)));
                    height -= layout.GetViewportHeight(num23);
                }
            }
            width = Math.Max(0.0, width);
            height = Math.Max(0.0, height);
            double d = width / ((double)num20);
            double num25 = height / ((double)num21);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                d = num5;
            }
            if (double.IsInfinity(num25) || double.IsNaN(num25))
            {
                num25 = num6;
            }
            for (int num26 = 0; num26 < layout.ColumnPaneCount; num26++)
            {
                if (viewportInfo.ViewportWidth[num26] < 0.0)
                {
                    layout.SetViewportWidth(num26, d);
                }
            }
            for (int num27 = 0; num27 < layout.RowPaneCount; num27++)
            {
                if (viewportInfo.ViewportHeight[num27] < 0.0)
                {
                    layout.SetViewportHeight(num27, num25);
                }
            }
            if ((num20 == 0) && (width > 0.0))
            {
                double num28 = width + viewportInfo.ViewportWidth[layout.ColumnPaneCount - 1];
                layout.SetViewportWidth(layout.ColumnPaneCount - 1, num28);
            }
            if ((num21 == 0) && (height > 0.0))
            {
                double num29 = height + viewportInfo.ViewportHeight[layout.RowPaneCount - 1];
                layout.SetViewportHeight(layout.RowPaneCount - 1, num29);
            }
            layout.SetViewportX(0, (layout.HeaderX + layout.HeaderWidth) + layout.FrozenWidth);
            for (int num30 = 1; num30 < layout.ColumnPaneCount; num30++)
            {
                layout.SetHorizontalSplitBarX(num30 - 1, layout.GetViewportX(num30 - 1) + layout.GetViewportWidth(num30 - 1));
                layout.SetViewportX(num30, layout.GetHorizontalSplitBarX(num30 - 1) + layout.GetHorizontalSplitBarWidth(num30 - 1));
            }
            layout.SetViewportY(0, (layout.HeaderY + layout.HeaderHeight) + layout.FrozenHeight);
            for (int num31 = 1; num31 < layout.RowPaneCount; num31++)
            {
                layout.SetVerticalSplitBarY(num31 - 1, layout.GetViewportY(num31 - 1) + layout.GetViewportHeight(num31 - 1));
                layout.SetViewportY(num31, layout.GetVerticalSplitBarY(num31 - 1) + layout.GetVerticalSplitBarHeight(num31 - 1));
            }
            if (layout.OrnamentHeight > 0.0)
            {
                layout.OrnamentY = (layout.GetViewportY(layout.RowPaneCount - 1) + layout.GetViewportHeight(layout.RowPaneCount - 1)) + layout.FrozenTrailingHeight;
            }
            if (layout.OrnamentWidth > 0.0)
            {
                layout.OrnamentX = (layout.GetViewportX(layout.ColumnPaneCount - 1) + layout.GetViewportWidth(layout.ColumnPaneCount - 1)) + layout.FrozenTrailingWidth;
            }
            double columnSplitBoxesWidth = GetColumnSplitBoxesWidth(layout.ColumnPaneCount);
            for (int num33 = 0; num33 < layout.ColumnPaneCount; num33++)
            {
                if (num33 == 0)
                {
                    double num34 = ((layout.HeaderX + layout.HeaderWidth) + layout.FrozenWidth) + layout.GetViewportWidth(num33);
                    double x = layout.X;
                    if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetHorizontalSplitBoxX(num33, x);
                        layout.SetHorizontalSplitBoxWidth(num33, Math.Min(num34, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarX(num33, layout.GetHorizontalSplitBoxX(num33) + layout.GetHorizontalSplitBoxWidth(num33));
                        layout.SetHorizontalScrollBarWidth(num33, Math.Max((double)0.0, (double)(num34 - layout.GetHorizontalSplitBoxWidth(num33))));
                    }
                    else
                    {
                        layout.SetHorizontalScrollBarX(num33, x);
                        layout.SetHorizontalSplitBoxWidth(num33, Math.Min(num34, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarWidth(num33, Math.Max((double)0.0, (double)(num34 - layout.GetHorizontalSplitBoxWidth(num33))));
                        layout.SetHorizontalSplitBoxX(num33, layout.GetHorizontalScrollBarX(num33) + layout.GetHorizontalScrollBarWidth(num33));
                    }
                }
                if ((num33 > 0) && (num33 < (layout.ColumnPaneCount - 1)))
                {
                    double viewportWidth = layout.GetViewportWidth(num33);
                    double viewportX = layout.GetViewportX(num33);
                    if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetHorizontalSplitBoxX(num33, viewportX);
                        layout.SetHorizontalSplitBoxWidth(num33, Math.Min(viewportWidth, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarX(num33, layout.GetHorizontalSplitBoxX(num33) + layout.GetHorizontalSplitBoxWidth(num33));
                        layout.SetHorizontalScrollBarWidth(num33, Math.Max((double)0.0, (double)(viewportWidth - layout.GetHorizontalSplitBoxWidth(num33))));
                    }
                    else
                    {
                        layout.SetHorizontalScrollBarX(num33, viewportX);
                        layout.SetHorizontalSplitBoxWidth(num33, Math.Min(viewportWidth, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarWidth(num33, Math.Max((double)0.0, (double)(viewportWidth - layout.GetHorizontalSplitBoxWidth(num33))));
                        layout.SetHorizontalSplitBoxX(num33, layout.GetHorizontalScrollBarX(num33) + layout.GetHorizontalScrollBarWidth(num33));
                    }
                }
                if (num33 == (layout.ColumnPaneCount - 1))
                {
                    double num38 = (((layout.GetViewportWidth(layout.ColumnPaneCount - 1) + layout.FrozenTrailingWidth) + ((layout.ColumnPaneCount == 1) ? layout.HeaderX : 0.0)) + ((layout.ColumnPaneCount == 1) ? layout.HeaderWidth : 0.0)) + ((layout.ColumnPaneCount == 1) ? layout.FrozenWidth : 0.0);
                    double num39 = (layout.ColumnPaneCount == 1) ? layout.X : layout.GetViewportX(layout.ColumnPaneCount - 1);
                    if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetHorizontalSplitBoxX(num33, num39);
                        layout.SetHorizontalSplitBoxWidth(num33, Math.Min(num38, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarX(num33, layout.GetHorizontalSplitBoxX(num33) + layout.GetHorizontalSplitBoxWidth(num33));
                        layout.SetHorizontalScrollBarWidth(num33, Math.Max((double)0.0, (double)(num38 - layout.GetHorizontalSplitBoxWidth(num33))));
                    }
                    else
                    {
                        layout.SetHorizontalScrollBarX(num33, num39);
                        layout.SetHorizontalSplitBoxWidth(num33, Math.Min(num38, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarWidth(num33, Math.Max((double)0.0, (double)(num38 - layout.GetHorizontalSplitBoxWidth(num33))));
                        layout.SetHorizontalSplitBoxX(num33, layout.GetHorizontalScrollBarX(num33) + layout.GetHorizontalScrollBarWidth(num33));
                    }
                }
            }
            double rowSplitBoxesHeight = GetRowSplitBoxesHeight(layout.RowPaneCount);
            for (int num41 = 0; num41 < layout.RowPaneCount; num41++)
            {
                if (num41 == 0)
                {
                    double num42 = ((layout.HeaderY + layout.HeaderHeight) + layout.FrozenHeight) + layout.GetViewportHeight(num41);
                    double y = layout.Y;
                    if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetVerticalSplitBoxY(num41, y);
                        layout.SetVerticalSplitBoxHeight(num41, Math.Min(num42, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarY(num41, layout.GetVerticalSplitBoxY(num41) + layout.GetVerticalSplitBoxHeight(num41));
                        layout.SetVerticalScrollBarHeight(num41, Math.Max((double)0.0, (double)(num42 - layout.GetVerticalSplitBoxHeight(num41))));
                    }
                    else
                    {
                        layout.SetVerticalScrollBarY(num41, y);
                        layout.SetVerticalSplitBoxHeight(num41, Math.Min(num42, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarHeight(num41, Math.Max((double)0.0, (double)(num42 - layout.GetVerticalSplitBoxHeight(num41))));
                        layout.SetVerticalSplitBoxY(num41, layout.GetVerticalScrollBarY(num41) + layout.GetVerticalScrollBarHeight(num41));
                    }
                }
                if ((num41 > 0) && (num41 < (layout.RowPaneCount - 1)))
                {
                    double viewportHeight = layout.GetViewportHeight(num41);
                    double viewportY = layout.GetViewportY(num41);
                    if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetVerticalSplitBoxY(num41, viewportY);
                        layout.SetVerticalSplitBoxHeight(num41, Math.Min(viewportHeight, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarY(num41, layout.GetVerticalSplitBoxY(num41) + layout.GetVerticalSplitBoxHeight(num41));
                        layout.SetVerticalScrollBarHeight(num41, Math.Max((double)0.0, (double)(viewportHeight - layout.GetVerticalSplitBoxHeight(num41))));
                    }
                    else
                    {
                        layout.SetVerticalScrollBarY(num41, viewportY);
                        layout.SetVerticalSplitBoxHeight(num41, Math.Min(viewportHeight, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarHeight(num41, Math.Max((double)0.0, (double)(viewportHeight - layout.GetVerticalSplitBoxHeight(num41))));
                        layout.SetVerticalSplitBoxY(num41, layout.GetVerticalScrollBarY(num41) + layout.GetVerticalScrollBarHeight(num41));
                    }
                }
                if (num41 == (layout.RowPaneCount - 1))
                {
                    double num46 = (((layout.GetViewportHeight(num41) + layout.FrozenTrailingHeight) + ((layout.RowPaneCount == 1) ? layout.HeaderY : 0.0)) + ((layout.RowPaneCount == 1) ? layout.HeaderHeight : 0.0)) + ((layout.RowPaneCount == 1) ? layout.FrozenHeight : 0.0);
                    double num47 = (layout.RowPaneCount == 1) ? layout.Y : layout.GetViewportY(layout.RowPaneCount - 1);
                    if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetVerticalSplitBoxY(num41, num47);
                        layout.SetVerticalSplitBoxHeight(num41, Math.Min(num46, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarY(num41, layout.GetVerticalSplitBoxY(num41) + layout.GetVerticalSplitBoxHeight(num41));
                        layout.SetVerticalScrollBarHeight(num41, Math.Max((double)0.0, (double)(num46 - layout.GetVerticalSplitBoxHeight(num41))));
                    }
                    else
                    {
                        layout.SetVerticalScrollBarY(num41, num47);
                        layout.SetVerticalSplitBoxHeight(num41, Math.Min(num46, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarHeight(num41, Math.Max((double)0.0, (double)(num46 - layout.GetVerticalSplitBoxHeight(num41))));
                        layout.SetVerticalSplitBoxY(num41, layout.GetVerticalScrollBarY(num41) + layout.GetVerticalScrollBarHeight(num41));
                    }
                }
            }
            if (layout.TabStripHeight > 0.0)
            {
                if ((layout.OrnamentHeight > 0.0) && flag)
                {
                    layout.TabStripX = layout.GetHorizontalScrollBarX(0);
                    layout.TabStripY = layout.OrnamentY;
                    layout.TabStripWidth = TabStripRatio * Math.Max((double)0.0, (double)(layout.GetHorizontalScrollBarWidth(0) - 16.0));
                    layout.TabSplitBoxX = layout.TabStripX + layout.TabStripWidth;
                    layout.TabSplitBoxWidth = 16.0;
                    layout.SetHorizontalScrollBarX(0, layout.TabSplitBoxX + layout.TabSplitBoxWidth);
                    layout.SetHorizontalScrollBarWidth(0, Math.Max((double)0.0, (double)((layout.GetHorizontalScrollBarWidth(0) - layout.TabStripWidth) - layout.TabSplitBoxWidth)));
                }
                else
                {
                    layout.TabStripX = layout.X;
                    layout.TabStripY = (layout.GetViewportY(layout.RowPaneCount - 1) + layout.GetViewportHeight(layout.RowPaneCount - 1)) + layout.FrozenTrailingHeight;
                    for (int num48 = 0; num48 < layout.ColumnPaneCount; num48++)
                    {
                        layout.TabStripWidth += layout.GetHorizontalScrollBarWidth(num48);
                        layout.TabStripWidth += layout.GetHorizontalSplitBoxWidth(num48);
                        if (num48 == (layout.ColumnPaneCount - 1))
                        {
                            break;
                        }
                        layout.TabStripWidth += 6.0;
                    }
                }
            }
            double num49 = base.AvailableSize.Width;
            if (double.IsInfinity(num49) || double.IsNaN(num49))
            {
                num49 = 0.0;
                if (flag3)
                {
                    num49 += ActualVerticalScrollBarWidth;
                }
                for (int num50 = 0; num50 < (layout.ColumnPaneCount - 1); num50++)
                {
                    num49 += layout.GetHorizontalSplitBarWidth(num50);
                }
                for (int num51 = 0; num51 < layout.ColumnPaneCount; num51++)
                {
                    num49 += layout.GetViewportWidth(num51);
                }
            }
            double num52 = base.AvailableSize.Height;
            if (double.IsInfinity(num52) || double.IsNaN(num52))
            {
                num52 = 0.0;
                if (flag)
                {
                    num52 += ActualHorizontalScrollBarHeight;
                }
                for (int num53 = 0; num53 < (layout.RowPaneCount - 1); num53++)
                {
                    num52 += layout.GetVerticalSplitBarHeight(num53);
                }
                for (int num54 = 0; num54 < layout.RowPaneCount; num54++)
                {
                    num52 += layout.GetViewportHeight(num54);
                }
                if (layout.TabStripHeight > 0.0)
                {
                    num52 += layout.TabStripHeight;
                }
            }
            base.AvailableSize = new Size(num49, num52);
            return layout;
        }

        /// <summary>
        /// 
        /// </summary>
        void EndColumnSplitting()
        {
            double num2;
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            SpreadLayout spreadLayout = GetSpreadLayout();
            int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
            base.IsWorking = false;
            IsTouchColumnSplitting = false;
            IsColumnSplitting = false;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (base.MousePosition.X <= spreadLayout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex))
                    {
                        num2 = spreadLayout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex) - base.MousePosition.X;
                    }
                    else
                    {
                        num2 = Math.Max((double)0.0, (double)((base.MousePosition.X - spreadLayout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex)) - spreadLayout.GetHorizontalSplitBarWidth(savedHitTestInformation.ColumnViewportIndex)));
                    }
                    if (num2 != 0.0)
                    {
                        double deltaViewportWidth = (_columnSplittingTracker.X1 - spreadLayout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex)) - (spreadLayout.GetHorizontalSplitBarWidth(savedHitTestInformation.ColumnViewportIndex) / 2.0);
                        int viewportIndex = savedHitTestInformation.ColumnViewportIndex;
                        if (!RaiseColumnViewportWidthChanging(viewportIndex, deltaViewportWidth))
                        {
                            base.AdjustColumnViewport(columnViewportIndex, deltaViewportWidth);
                            RaiseColumnViewportWidthChanged(viewportIndex, deltaViewportWidth);
                        }
                    }
                    goto Label_0258;

                case HitTestType.ColumnSplitBox:
                    if (ColumnSplitBoxAlignment != SplitBoxAlignment.Leading)
                    {
                        num2 = Math.Max((double)0.0, (double)(((spreadLayout.GetViewportX(savedHitTestInformation.ColumnViewportIndex) + spreadLayout.GetViewportWidth(savedHitTestInformation.ColumnViewportIndex)) - base.MousePosition.X) - spreadLayout.GetHorizontalSplitBoxWidth(savedHitTestInformation.ColumnViewportIndex)));
                        break;
                    }
                    num2 = Math.Max((double)0.0, (double)((base.MousePosition.X - spreadLayout.GetViewportX(savedHitTestInformation.ColumnViewportIndex)) - spreadLayout.GetHorizontalSplitBoxWidth(savedHitTestInformation.ColumnViewportIndex)));
                    break;

                default:
                    goto Label_0258;
            }
            if (num2 > 0.0)
            {
                double num3 = (_columnSplittingTracker.X1 - spreadLayout.GetViewportX(columnViewportIndex)) - (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0);
                int num4 = (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading) ? 0 : (base.GetViewportInfo().ColumnViewportCount - 1);
                if (!RaiseColumnViewportWidthChanging(num4, num3))
                {
                    AddColumnViewport(columnViewportIndex, num3);
                    RaiseColumnViewportWidthChanged(num4, num3);
                    base.ShowCell(base.GetActiveRowViewportIndex(), base.GetActiveColumnViewportIndex(), ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, VerticalPosition.Nearest, HorizontalPosition.Nearest);
                }
            }
        Label_0258:
            _columnSplittingTracker.Opacity = 0.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal override bool EndMouseClick(PointerMouseRoutedEventArgs e)
        {
            if (base.GetSavedHitTestInformation().HitTestType == HitTestType.TabStrip)
            {
                return false;
            }
            return base.EndMouseClick(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <returns></returns>
        internal int GetActiveRowViewportIndex(int sheetIndex)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SpreadSheet.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Worksheet sheet = SpreadSheet.Sheets[sheetIndex];
            return GetViewportInfo(sheet).ActiveRowViewport;
        }

        /// <summary>
        /// 
        /// </summary>
        void EndRowSplitting()
        {
            double num2;
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            SpreadLayout spreadLayout = GetSpreadLayout();
            int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
            base.IsWorking = false;
            IsRowSplitting = false;
            IsTouchRowSplitting = false;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (base.MousePosition.Y <= spreadLayout.GetVerticalSplitBarY(rowViewportIndex))
                    {
                        num2 = spreadLayout.GetVerticalSplitBarY(rowViewportIndex) - base.MousePosition.Y;
                    }
                    else
                    {
                        num2 = Math.Max((double)0.0, (double)((base.MousePosition.Y - spreadLayout.GetVerticalSplitBarY(rowViewportIndex)) - spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex)));
                    }
                    if (num2 != 0.0)
                    {
                        double deltaViewportHeight = (_rowSplittingTracker.Y1 - spreadLayout.GetVerticalSplitBarY(rowViewportIndex)) - (spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0);
                        int viewportIndex = savedHitTestInformation.RowViewportIndex;
                        if (!RaiseRowViewportHeightChanging(viewportIndex, deltaViewportHeight))
                        {
                            base.AdjustRowViewport(rowViewportIndex, deltaViewportHeight);
                            RaiseRowViewportHeightChanged(viewportIndex, deltaViewportHeight);
                        }
                    }
                    goto Label_021D;

                case HitTestType.RowSplitBox:
                    if (RowSplitBoxAlignment != SplitBoxAlignment.Leading)
                    {
                        num2 = Math.Max((double)0.0, (double)(((spreadLayout.GetViewportY(rowViewportIndex) + spreadLayout.GetViewportHeight(rowViewportIndex)) - base.MousePosition.Y) - spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex)));
                        break;
                    }
                    num2 = Math.Max((double)0.0, (double)((base.MousePosition.Y - spreadLayout.GetViewportY(rowViewportIndex)) - spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex)));
                    break;

                default:
                    goto Label_021D;
            }
            if (num2 > 0.0)
            {
                double num3 = (_rowSplittingTracker.Y1 - spreadLayout.GetViewportY(rowViewportIndex)) - (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0);
                int num4 = (RowSplitBoxAlignment == SplitBoxAlignment.Leading) ? 0 : (base.GetViewportInfo().RowViewportCount - 1);
                if (!RaiseRowViewportHeightChanging(num4, num3))
                {
                    AddRowViewport(rowViewportIndex, num3);
                    RaiseRowViewportHeightChanged(num4, num3);
                    base.ShowCell(base.GetActiveRowViewportIndex(), base.GetActiveColumnViewportIndex(), ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, VerticalPosition.Nearest, HorizontalPosition.Nearest);
                }
            }
        Label_021D:
            _rowSplittingTracker.Opacity = 0.0;
        }

        /// <summary>
        /// 
        /// </summary>
        void EndTabStripResizing()
        {
            IsTabStripResizing = false;
            base.IsTouchTabStripResizing = false;
            base.IsWorking = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal override bool EndTouchTap(Point point)
        {
            if (base.GetSavedHitTestInformation().HitTestType == HitTestType.TabStrip)
            {
                return false;
            }
            return base.EndTouchTap(point);
        }

        internal int GetActiveColumnViewportIndex(int sheetIndex)
        {
            if ((sheetIndex < 0) || (sheetIndex >= SpreadSheet.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Worksheet sheet = SpreadSheet.Sheets[sheetIndex];
            return GetViewportInfo(sheet).ActiveColumnViewport;
        }

        internal int GetColumnPaneCount()
        {
            return base.GetViewportInfo().ColumnViewportCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnPaneCount"></param>
        /// <returns></returns>
        double GetColumnSplitBoxesWidth(int columnPaneCount)
        {
            if (_columnSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                if (_columnSplitBoxPolicy == SplitBoxPolicy.AsNeeded)
                {
                    if (columnPaneCount == 1)
                    {
                        return 6.0;
                    }
                    return 0.0;
                }
                if (_columnSplitBoxPolicy == SplitBoxPolicy.Never)
                {
                    return 0.0;
                }
            }
            return 6.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        Rect GetHorizontalScrollBarRectangle(int columnViewportIndex)
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double horizontalScrollBarX = spreadLayout.GetHorizontalScrollBarX(columnViewportIndex);
            double ornamentY = spreadLayout.OrnamentY;
            double width = spreadLayout.GetHorizontalScrollBarWidth(columnViewportIndex) - 1.0;
            double height = spreadLayout.OrnamentHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(horizontalScrollBarX, ornamentY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        Rect GetHorizontalSplitBarRectangle(int columnViewportIndex)
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double horizontalSplitBarX = spreadLayout.GetHorizontalSplitBarX(columnViewportIndex);
            double headerY = spreadLayout.HeaderY;
            double width = spreadLayout.GetHorizontalSplitBarWidth(columnViewportIndex) - 1.0;
            double height = base.AvailableSize.Height - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(horizontalSplitBarX, headerY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        Rect GetHorizontalSplitBoxRectangle(int columnViewportIndex)
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double horizontalSplitBoxX = spreadLayout.GetHorizontalSplitBoxX(columnViewportIndex);
            double ornamentY = spreadLayout.OrnamentY;
            double width = spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) - 1.0;
            double height = spreadLayout.OrnamentHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(horizontalSplitBoxX, ornamentY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="beforeColumn"></param>
        /// <returns></returns>
        int GetInvisibleColumnsBeforeColumn(Worksheet sheet, int beforeColumn)
        {
            int num = 0;
            using (HashSet<int>.Enumerator enumerator = _invisibleColumns.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current >= beforeColumn)
                    {
                        return num;
                    }
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="beforeRow"></param>
        /// <returns></returns>
        int GetInvisibleRowsBeforeRow(Worksheet sheet, int beforeRow)
        {
            int num = 0;
            using (HashSet<int>.Enumerator enumerator = _invisibleRows.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current >= beforeRow)
                    {
                        return num;
                    }
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal int GetRowPaneCount()
        {
            return base.GetViewportInfo().RowViewportCount;
        }

        double GetRowSplitBoxesHeight(int rowPaneCount)
        {
            if (_rowSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                if (_rowSplitBoxPolicy == SplitBoxPolicy.AsNeeded)
                {
                    if (rowPaneCount == 1)
                    {
                        return 6.0;
                    }
                    return 0.0;
                }
                if (_rowSplitBoxPolicy == SplitBoxPolicy.Never)
                {
                    return 0.0;
                }
            }
            return 6.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        int GetSheetInvisibleColumns(Worksheet sheet)
        {
            int num = 0;
            _invisibleColumns.Clear();
            for (int i = 0; i < sheet.ColumnCount; i++)
            {
                if (!sheet.GetActualColumnVisible(i, SheetArea.Cells))
                {
                    _invisibleColumns.Add(i);
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        int GetSheetInvisibleRows(Worksheet sheet)
        {
            int num = 0;
            _invisibleRows.Clear();
            for (int i = 0; i < sheet.RowCount; i++)
            {
                if (!sheet.GetActualRowVisible(i, SheetArea.Cells))
                {
                    _invisibleRows.Add(i);
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override SheetLayout GetSheetLayout()
        {
            return GetSpreadLayout();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        double GetSmoothingScale(double scale)
        {
            if ((scale > 0.99) && (scale < 1.01))
            {
                return 1.0;
            }
            return scale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal SpreadLayout GetSpreadLayout()
        {
            if (_cachedSpreadLayout == null)
            {
                _cachedSpreadLayout = CreateSpreadLayout();
                UpdateHorizontalScrollBars();
                UpdateVerticalScrollBars();
            }
            return _cachedSpreadLayout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal SpreadView GetSpreadViewFromPoint(double x, double y)
        {
            return this;
        }

        /// <summary>
        /// Calculates the start index to bring the tab into view. 
        /// </summary>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <returns></returns>
        public int GetStartIndexToBringTabIntoView(int tabIndex)
        {
            if (TabStrip != null)
            {
                return TabStrip.GetStartIndexToBringTabIntoView(tabIndex);
            }
            return SpreadSheet.StartSheetIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Rect GetTabSplitBoxRectangle()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double tabSplitBoxX = spreadLayout.TabSplitBoxX;
            double ornamentY = spreadLayout.OrnamentY;
            double width = spreadLayout.TabSplitBoxWidth - 1.0;
            double height = spreadLayout.OrnamentHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(tabSplitBoxX, ornamentY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Rect GetTabStripRectangle()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double tabStripX = spreadLayout.TabStripX;
            double tabStripY = spreadLayout.TabStripY;
            double width = spreadLayout.TabStripWidth - 1.0;
            double height = spreadLayout.TabStripHeight - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(tabStripX, tabStripY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        Rect GetVerticalScrollBarRectangle(int rowViewportIndex)
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double ornamentX = spreadLayout.OrnamentX;
            double verticalScrollBarY = spreadLayout.GetVerticalScrollBarY(rowViewportIndex);
            double width = spreadLayout.OrnamentWidth - 1.0;
            double height = spreadLayout.GetVerticalScrollBarHeight(rowViewportIndex) - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(ornamentX, verticalScrollBarY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        Rect GetVerticalSplitBarRectangle(int rowViewportIndex)
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double headerX = spreadLayout.HeaderX;
            double verticalSplitBarY = spreadLayout.GetVerticalSplitBarY(rowViewportIndex);
            double width = base.AvailableSize.Width - 1.0;
            double height = spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex) - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(headerX, verticalSplitBarY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        Rect GetVerticalSplitBoxRectangle(int rowViewportIndex)
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            double ornamentX = spreadLayout.OrnamentX;
            double verticalSplitBoxY = spreadLayout.GetVerticalSplitBoxY(rowViewportIndex);
            double width = spreadLayout.OrnamentWidth - 1.0;
            double height = spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) - 1.0;
            if ((width >= 0.0) && (height >= 0.0))
            {
                return new Rect(ornamentX, verticalSplitBoxY, width, height);
            }
            return Rect.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <returns></returns>
        int GetViewportInvisibleColumns(int columnViewportIndex)
        {
            ColumnLayoutModel viewportColumnLayoutModel = base.GetViewportColumnLayoutModel(columnViewportIndex);
            int num = 0;
            foreach (ColumnLayout layout in viewportColumnLayoutModel)
            {
                if (!Worksheet.GetActualColumnVisible(layout.Column, SheetArea.Cells))
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <returns></returns>
        int GetViewportInvisibleRows(int rowViewportIndex)
        {
            RowLayoutModel viewportRowLayoutModel = base.GetViewportRowLayoutModel(rowViewportIndex);
            int num = 0;
            foreach (RowLayout layout in viewportRowLayoutModel)
            {
                if (!Worksheet.GetActualRowVisible(layout.Row, SheetArea.Cells))
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HideOpeningProgressRing()
        {
            if (_progressRing != null)
            {
                _progressRing.Visibility = (Visibility)1;
                _progressRing.IsActive = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HideOpeningStatus()
        {
            HideOpeningProgressRing();
            if (TabStrip != null)
            {
                TabStrip.Visibility = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal override HitTestInformation HitTest(double x, double y)
        {
            Point point = new Point(x, y);
            SpreadLayout spreadLayout = GetSpreadLayout();
            HitTestInformation information = new HitTestInformation
            {
                HitTestType = HitTestType.Empty,
                ColumnViewportIndex = -2,
                RowViewportIndex = -2,
                HitPoint = point
            };
            if (GetTabStripRectangle().Contains(point))
            {
                information.ColumnViewportIndex = 0;
                information.HitTestType = HitTestType.TabStrip;
                return information;
            }
            if (GetTabSplitBoxRectangle().Contains(point))
            {
                information.ColumnViewportIndex = 0;
                information.HitTestType = HitTestType.TabSplitBox;
                return information;
            }
            for (int i = 0; i < spreadLayout.ColumnPaneCount; i++)
            {
                if (GetHorizontalScrollBarRectangle(i).Contains(point))
                {
                    information.ColumnViewportIndex = i;
                    information.HitTestType = HitTestType.HorizontalScrollBar;
                    return information;
                }
            }
            for (int j = 0; j < spreadLayout.RowPaneCount; j++)
            {
                if (GetVerticalScrollBarRectangle(j).Contains(point))
                {
                    information.HitTestType = HitTestType.VerticalScrollBar;
                    information.RowViewportIndex = j;
                    return information;
                }
            }
            for (int k = 0; k < spreadLayout.ColumnPaneCount; k++)
            {
                if (GetHorizontalSplitBoxRectangle(k).Contains(point))
                {
                    information.HitTestType = HitTestType.ColumnSplitBox;
                    information.ColumnViewportIndex = k;
                }
            }
            for (int m = 0; m < spreadLayout.RowPaneCount; m++)
            {
                if (GetVerticalSplitBoxRectangle(m).Contains(point))
                {
                    information.HitTestType = HitTestType.RowSplitBox;
                    information.RowViewportIndex = m;
                }
            }
            for (int n = 0; n < (spreadLayout.ColumnPaneCount - 1); n++)
            {
                if (GetHorizontalSplitBarRectangle(n).Contains(point))
                {
                    information.HitTestType = HitTestType.ColumnSplitBar;
                    information.ColumnViewportIndex = n;
                }
            }
            for (int num6 = 0; num6 < (spreadLayout.RowPaneCount - 1); num6++)
            {
                if (GetVerticalSplitBarRectangle(num6).Contains(point))
                {
                    information.HitTestType = HitTestType.RowSplitBar;
                    information.RowViewportIndex = num6;
                }
            }
            if (information.HitTestType == HitTestType.Empty)
            {
                information = base.HitTest(x, y);
            }
            return information;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HorizontalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((base._touchToolbarPopup != null) && base._touchToolbarPopup.IsOpen)
            {
                base._touchToolbarPopup.IsOpen = false;
            }
            if (((ScrollBarTrackPolicy == ScrollBarTrackPolicy.Both) || (ScrollBarTrackPolicy == ScrollBarTrackPolicy.Horizontal)) || (base._isTouchScrolling || (e.ScrollEventType != (ScrollEventType)5)))
            {
                for (int i = 0; i < _horizontalScrollBar.Length; i++)
                {
                    if (sender == _horizontalScrollBar[i])
                    {
                        if (base.HorizontalScrollable)
                        {
                            ProcessHorizontalScroll(i, e);
                            return;
                        }
                        _horizontalScrollBar[i].Value = (double)base.GetViewportLeftColumn(i);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <param name="newValue"></param>
        void HorizontalScrollBarTouchSmallDecrement(int columnViewportIndex, int newValue)
        {
            int viewportLeftColumn = base.GetViewportLeftColumn(columnViewportIndex);
            int num2 = base.TryGetPreviousScrollableColumn(newValue);
            if ((viewportLeftColumn != num2) && (num2 != -1))
            {
                base.SetViewportLeftColumn(columnViewportIndex, num2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void InitCachedTransform()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if (base._cachedViewportTransform == null)
            {
                base._cachedViewportTransform = new TransformGroup[spreadLayout.RowPaneCount + 2, spreadLayout.ColumnPaneCount + 2];
                for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
                {
                    for (int j = -1; j <= spreadLayout.RowPaneCount; j++)
                    {
                        base._cachedViewportTransform[j + 1, i + 1] = InitManipulationTransforms();
                    }
                }
            }
            if (base._cachedColumnHeaderViewportTransform == null)
            {
                base._cachedColumnHeaderViewportTransform = new TransformGroup[spreadLayout.ColumnPaneCount + 2];
                for (int k = -1; k <= spreadLayout.ColumnPaneCount; k++)
                {
                    base._cachedColumnHeaderViewportTransform[k + 1] = InitManipulationTransforms();
                }
            }
            if (base._cachedRowHeaderViewportTransform == null)
            {
                base._cachedRowHeaderViewportTransform = new TransformGroup[spreadLayout.RowPaneCount + 2];
                for (int m = -1; m <= spreadLayout.RowPaneCount; m++)
                {
                    base._cachedRowHeaderViewportTransform[m + 1] = InitManipulationTransforms();
                }
            }
            if (base._cachedCornerViewportTransform == null)
            {
                base._cachedCornerViewportTransform = InitManipulationTransforms();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void InitCachedVisual()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            BitmapSource image = CreateCachedIamge(this, base.RenderSize);
            if (base._cachedViewportVisual == null)
            {
                base._cachedViewportVisual = new Image[spreadLayout.RowPaneCount + 2, spreadLayout.ColumnPaneCount + 2];
                for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
                {
                    double viewportX = spreadLayout.GetViewportX(i);
                    double viewportWidth = spreadLayout.GetViewportWidth(i);
                    for (int j = -1; j <= spreadLayout.RowPaneCount; j++)
                    {
                        double viewportY = spreadLayout.GetViewportY(j);
                        double viewportHeight = spreadLayout.GetViewportHeight(j);
                        base._cachedViewportVisual[j + 1, i + 1] = CreateCachedVisual(image, CreateClipRect(viewportX, viewportY, viewportWidth, viewportHeight, i, j));
                    }
                }
            }
            if (base._cachedColumnHeaderViewportVisual == null)
            {
                base._cachedColumnHeaderViewportVisual = new Image[spreadLayout.ColumnPaneCount + 2];
                for (int k = -1; k <= spreadLayout.ColumnPaneCount; k++)
                {
                    double columnX = spreadLayout.GetViewportX(k);
                    double headerY = spreadLayout.HeaderY;
                    double num10 = spreadLayout.GetViewportWidth(k);
                    double headerHeight = spreadLayout.HeaderHeight;
                    base._cachedColumnHeaderViewportVisual[k + 1] = CreateCachedVisual(image, CreateClipRect(columnX, headerY, num10, headerHeight, k, -2));
                }
            }
            if (base._cachedRowHeaderViewportVisual == null)
            {
                base._cachedRowHeaderViewportVisual = new Image[spreadLayout.RowPaneCount + 2];
                for (int m = -1; m <= spreadLayout.RowPaneCount; m++)
                {
                    double headerX = spreadLayout.HeaderX;
                    double rowY = spreadLayout.GetViewportY(m);
                    double headerWidth = spreadLayout.HeaderWidth;
                    double num16 = spreadLayout.GetViewportHeight(m);
                    base._cachedRowHeaderViewportVisual[m + 1] = CreateCachedVisual(image, CreateClipRect(headerX, rowY, headerWidth, num16, -2, m));
                }
            }
            if (base._cachedCornerViewportVisual == null)
            {
                base._cachedCornerViewportVisual = CreateCachedVisual(image, new Rect(spreadLayout.HeaderX, spreadLayout.HeaderY, spreadLayout.HeaderWidth, spreadLayout.HeaderHeight));
            }
            if (base._cachedBottomRightACornerVisual == null)
            {
                base._cachedBottomRightACornerVisual = CreateCachedVisual(image, new Rect(spreadLayout.OrnamentX, spreadLayout.OrnamentY, spreadLayout.OrnamentWidth, spreadLayout.OrnamentHeight));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TransformGroup InitManipulationTransforms()
        {
            TransformGroup group = new TransformGroup();
            group.Children.Add(new CompositeTransform());
            MatrixTransform transform = new MatrixTransform();
            transform.Matrix = Windows.UI.Xaml.Media.Matrix.Identity;
            group.Children.Add(transform);
            return group;
        }

        /// <summary>
        /// 
        /// </summary>
        void InitTouchCacheInfomation()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if (spreadLayout != null)
            {
                if (base._cachedViewportHeights == null)
                {
                    base._cachedViewportHeights = new double[spreadLayout.RowPaneCount + 2];
                }
                if (base._cachedViewportWidths == null)
                {
                    base._cachedViewportWidths = new double[spreadLayout.ColumnPaneCount + 2];
                }
                if (base._cachedViewportSplitBarX == null)
                {
                    base._cachedViewportSplitBarX = new double[spreadLayout.ColumnPaneCount - 1];
                }
                if (base._cachedViewportSplitBarY == null)
                {
                    base._cachedViewportSplitBarY = new double[spreadLayout.RowPaneCount - 1];
                }
                for (int i = -1; i <= spreadLayout.RowPaneCount; i++)
                {
                    base._cachedViewportHeights[i + 1] = spreadLayout.GetViewportHeight(i);
                }
                for (int j = -1; j <= spreadLayout.ColumnPaneCount; j++)
                {
                    base._cachedViewportWidths[j + 1] = spreadLayout.GetViewportWidth(j);
                }
                for (int k = 0; k < (spreadLayout.ColumnPaneCount - 1); k++)
                {
                    base._cachedViewportSplitBarX[k] = spreadLayout.GetHorizontalSplitBarX(k);
                }
                for (int m = 0; m < (spreadLayout.RowPaneCount - 1); m++)
                {
                    base._cachedViewportSplitBarY[m] = spreadLayout.GetVerticalSplitBarY(m);
                }
                base._touchStartLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
                base._touchStartTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void InvalidateLayout()
        {
            _cachedSpreadLayout = null;
            base.InvalidateLayout();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void InvalidateSheetLayout()
        {
            if (!base.IsSuspendInvalidate())
            {
                base.Children.Clear();
                base._cornerPresenter = null;
                base._rowHeaderPresenters = null;
                base._columnHeaderPresenters = null;
                base._viewportPresenters = null;
                InvalidateLayout();
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void InvalidTabStrip()
        {
            if (!base.IsSuspendInvalidate())
            {
                RefreshTabStrip();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInScrollBar()
        {
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            if ((savedHitTestInformation.HitTestType != HitTestType.HorizontalScrollBar) && (savedHitTestInformation.HitTestType != HitTestType.VerticalScrollBar))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInSplitBar()
        {
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            if ((savedHitTestInformation.HitTestType != HitTestType.RowSplitBar) && (savedHitTestInformation.HitTestType != HitTestType.ColumnSplitBar))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInSplitBox()
        {
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            if ((savedHitTestInformation.HitTestType != HitTestType.RowSplitBox) && (savedHitTestInformation.HitTestType != HitTestType.ColumnSplitBox))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInTabSplitBox()
        {
            return (base.GetSavedHitTestInformation().HitTestType == HitTestType.TabSplitBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsMouseInTabStrip()
        {
            return (base.GetSavedHitTestInformation().HitTestType == HitTestType.TabStrip);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsZero(double value)
        {
            return (Math.Abs(value) < 2.2204460492503131E-15);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="scrollValue"></param>
        /// <returns></returns>
        int MapScrollValueToColumnIndex(Worksheet sheet, int scrollValue)
        {
            int num = 0;
            for (int i = 0; i < sheet.ColumnCount; i++)
            {
                if (!_invisibleColumns.Contains(i))
                {
                    num++;
                }
                if (num == scrollValue)
                {
                    return (i + 1);
                }
            }
            return scrollValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="scrollValue"></param>
        /// <returns></returns>
        int MapScrollValueToRowIndex(Worksheet sheet, int scrollValue)
        {
            int num = 0;
            for (int i = 0; i < sheet.RowCount; i++)
            {
                if (!_invisibleRows.Contains(i))
                {
                    num++;
                }
                if (num == scrollValue)
                {
                    return (i + 1);
                }
            }
            return scrollValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool NavigationNextSheet()
        {
            SaveDataForFormulaSelection();
            if (!base.StopCellEditing(base.CanSelectFormula))
            {
                return false;
            }
            TabStrip tabStrip = TabStrip;
            if (tabStrip != null)
            {
                tabStrip.ActiveNextTab();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool NavigationPreviousSheet()
        {
            SaveDataForFormulaSelection();
            if (!base.StopCellEditing(base.CanSelectFormula))
            {
                return false;
            }
            TabStrip tabStrip = TabStrip;
            if (tabStrip != null)
            {
                tabStrip.ActivePreviousTab();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHorizontalScrollBarPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _showScrollTip = false;
            base._mouseDownPosition = e.GetCurrentPoint(this).Position;
            base.CloseTooltip();
        }

        void OnHorizontalScrollBarPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == 0)
            {
                base.InputDeviceType = InputDeviceType.Touch;
            }
            else
            {
                base.InputDeviceType = InputDeviceType.Mouse;
            }
            if ((ElementTreeHelper.GetParentOrSelf<Thumb>(e.OriginalSource as DependencyObject) != null) && ((base.ShowScrollTip == ShowScrollTip.Horizontal) || (base.ShowScrollTip == ShowScrollTip.Both)))
            {
                _showScrollTip = true;
                base._mouseDownPosition = e.GetCurrentPoint(this).Position;
                base.UpdateScrollToolTip(false, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHorizontalScrollBarPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _showScrollTip = false;
            base._mouseDownPosition = e.GetCurrentPoint(this).Position;
            base.CloseTooltip();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTabStripActiveTabChanged(object sender, EventArgs e)
        {
            TabStrip strip = sender as TabStrip;
            int sheetIndex = -1;
            if ((strip != null) && (strip.ActiveTab != null))
            {
                sheetIndex = strip.ActiveTab.SheetIndex;
            }
            if ((sheetIndex >= 0) && (sheetIndex < SpreadSheet.Sheets.Count))
            {
                base.StopCellEditing(false);
                if (sheetIndex != SpreadSheet.ActiveSheetIndex)
                {
                    SpreadSheet.Workbook.ActiveSheetIndex = sheetIndex;
                    RaiseActiveSheetIndexChanged();
                    base._currentActiveRowIndex = SpreadSheet.ActiveSheet.ActiveRowIndex;
                    base._currentActiveColumnIndex = SpreadSheet.ActiveSheet.ActiveColumnIndex;
                    base.Navigation.UpdateStartPosition(base._currentActiveRowIndex, base._currentActiveColumnIndex);
                    base.Invalidate();
                }
            }
            if (!IsEditing)
            {
                SpreadSheet.Focus(FocusState.Programmatic);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTabStripActiveTabChanging(object sender, EventArgs e)
        {
            if ((sender is TabStrip) && (e is CancelEventArgs))
            {
                ((CancelEventArgs)e).Cancel = RaiseActiveSheetIndexChanging();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTabStripNewTabNeeded(object sender, EventArgs e)
        {
            base.StopCellEditing(false);
            Worksheet item = new Worksheet();
            SpreadSheet.Sheets.Add(item);
            item.ReferenceStyle = SpreadSheet.Workbook.ReferenceStyle;
            if (item.ReferenceStyle == ReferenceStyle.R1C1)
            {
                item.ColumnHeader.AutoText = HeaderAutoText.Numbers;
            }
            else
            {
                item.ColumnHeader.AutoText = HeaderAutoText.Letters;
            }
            base._currentActiveRowIndex = item.ActiveRowIndex;
            base._currentActiveColumnIndex = item.ActiveColumnIndex;
            base.Navigation.UpdateStartPosition(base._currentActiveRowIndex, base._currentActiveColumnIndex);
            (sender as TabStrip).NewTab(SpreadSheet.Sheets.Count - 1);
            InvalidateSheetLayout();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVerticalScrollbarPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _showScrollTip = false;
            base._mouseDownPosition = e.GetCurrentPoint(this).Position;
            base.CloseTooltip();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVerticalScrollbarPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == 0)
            {
                base.InputDeviceType = InputDeviceType.Touch;
            }
            else
            {
                base.InputDeviceType = InputDeviceType.Mouse;
            }
            if ((ElementTreeHelper.GetParentOrSelf<Thumb>(e.OriginalSource as DependencyObject) != null) && ((base.ShowScrollTip == ShowScrollTip.Vertical) || (base.ShowScrollTip == ShowScrollTip.Both)))
            {
                _showScrollTip = true;
                base._mouseDownPosition = e.GetCurrentPoint(this).Position;
                base.UpdateScrollToolTip(true, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVerticalScrollbarPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _showScrollTip = false;
            base._mouseDownPosition = e.GetCurrentPoint(this).Position;
            base.CloseTooltip();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        internal override void ProcessDoubleTap(Point point)
        {
            base.UpdateTouchHitTestInfo(point);
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            HitTestInformation hi = TouchHitTest(point.X, point.Y);
            if (!IsEditing || ((hi.HitTestType != HitTestType.RowSplitBar) && (hi.HitTestType != HitTestType.ColumnSplitBar)))
            {
                if ((hi.HitTestType == HitTestType.RowSplitBar) || (hi.HitTestType == HitTestType.ColumnSplitBar))
                {
                    ProcessSplitBarDoubleTap(hi);
                }
                else if ((savedHitTestInformation.HitTestType != HitTestType.Viewport) || !savedHitTestInformation.ViewportInfo.InSelectionDrag)
                {
                    if (((savedHitTestInformation.HitTestType == HitTestType.TabStrip) && (_tabStrip != null)) && (TabStripEditable && !SpreadSheet.Workbook.Protect))
                    {
                        _tabStrip.StartTabTouchEditing(point);
                        if (_tabStrip.IsEditing)
                        {
                            int sheetTabIndex = (_tabStrip.ActiveTab != null) ? _tabStrip.ActiveTab.SheetIndex : -1;
                            base.RaiseSheetTabDoubleClick(sheetTabIndex);
                        }
                    }
                    else
                    {
                        base.ProcessDoubleTap(point);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <param name="e"></param>
        void ProcessHorizontalScroll(int columnViewportIndex, ScrollEventArgs e)
        {
            int viewportLeftColumn = base.GetViewportLeftColumn(columnViewportIndex);
            int scrollValue = (int)Math.Round(e.NewValue);
            scrollValue = MapScrollValueToColumnIndex(ActiveSheet, scrollValue);
            int num3 = scrollValue;
            if (e.ScrollEventType == (ScrollEventType)3)
            {
                if (NavigatorHelper.ScrollToNextPageOfColumns(this, columnViewportIndex))
                {
                    num3 = base.GetViewportLeftColumn(columnViewportIndex);
                }
                else
                {
                    num3 = base.TryGetNextScrollableColumn(scrollValue);
                }
            }
            else if (e.ScrollEventType == (ScrollEventType)1)
            {
                num3 = base.TryGetNextScrollableColumn(scrollValue);
            }
            else if (e.ScrollEventType == (ScrollEventType)2)
            {
                if (NavigatorHelper.ScrollToPreviousPageOfColumns(this, columnViewportIndex))
                {
                    num3 = base.GetViewportLeftColumn(columnViewportIndex);
                }
                else
                {
                    num3 = base.TryGetPreviousScrollableColumn(scrollValue);
                }
            }
            else if (e.ScrollEventType == 0)
            {
                num3 = base.TryGetPreviousScrollableColumn(scrollValue);
            }
            if ((e.ScrollEventType == (ScrollEventType)5) || (e.ScrollEventType == (ScrollEventType)8))
            {
                num3 = base.TryGetNextScrollableColumn(scrollValue);
            }
            if ((viewportLeftColumn != num3) && (num3 != -1))
            {
                base.SetViewportLeftColumn(columnViewportIndex, num3);
            }
            if (((e.ScrollEventType != (ScrollEventType)5) && (num3 != e.NewValue)) && (_horizontalScrollBar != null))
            {
                GetSpreadLayout();
                if (((columnViewportIndex > -1) && (columnViewportIndex < _horizontalScrollBar.Length)) && (_horizontalScrollBar[columnViewportIndex].Value != num3))
                {
                    int invisibleColumnsBeforeColumn = GetInvisibleColumnsBeforeColumn(ActiveSheet, num3);
                    num3 -= invisibleColumnsBeforeColumn;
                    _horizontalScrollBar[columnViewportIndex].Value = (num3 != -1) ? ((double)num3) : ((double)viewportLeftColumn);
                    _horizontalScrollBar[columnViewportIndex].InvalidateArrange();
                }
            }
            if (_showScrollTip && ((base.ShowScrollTip == ShowScrollTip.Both) || (base.ShowScrollTip == ShowScrollTip.Horizontal)))
            {
                base.UpdateScrollToolTip(false, num3 + 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal override void ProcessKeyDown(KeyRoutedEventArgs e)
        {
            if ((_tabStrip != null) && _tabStrip.IsEditing)
            {
                if ((e.Key == (VirtualKey)13) || (e.Key == (VirtualKey)9))
                {
                    _tabStrip.StopTabEditing(false);
                    e.Handled = true;
                }
                else if (e.Key == (VirtualKey)0x1b)
                {
                    _tabStrip.StopTabEditing(true);
                }
            }
            else
            {
                base.ProcessKeyDown(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal override void ProcessMouseLeftButtonDoubleClick(DoubleTappedRoutedEventArgs e)
        {
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            if ((!IsEditing || ((savedHitTestInformation.HitTestType != HitTestType.RowSplitBar) && (savedHitTestInformation.HitTestType != HitTestType.ColumnSplitBar))) && ((savedHitTestInformation.HitTestType != HitTestType.Viewport) || !savedHitTestInformation.ViewportInfo.InSelectionDrag))
            {
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.RowSplitBar:
                    case HitTestType.ColumnSplitBar:
                        ProcessSplitBarDoubleClick(savedHitTestInformation);
                        return;

                    case HitTestType.TabStrip:
                        {
                            if (((_tabStrip == null) || !TabStripEditable) || (SpreadSheet.Workbook.Protect || (base._routedEventArgs == null)))
                            {
                                break;
                            }
                            _tabStrip.StartTabEditing(base._routedEventArgs.Instance);
                            if (!_tabStrip.IsEditing)
                            {
                                break;
                            }
                            int sheetTabIndex = -1;
                            if (_tabStrip.ActiveTab != null)
                            {
                                sheetTabIndex = _tabStrip.ActiveTab.SheetIndex;
                            }
                            base.RaiseSheetTabDoubleClick(sheetTabIndex);
                            return;
                        }
                    default:
                        base.ProcessMouseLeftButtonDoubleClick(e);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal override void ProcessMouseLeftButtonDown(PointerMouseRoutedEventArgs e)
        {
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            GetSpreadLayout();
            Point position = e.GetPosition(this);
            if (position != savedHitTestInformation.HitPoint)
            {
                base.SaveHitTestInfo(savedHitTestInformation = HitTest(position.X, position.Y));
            }
            if (!base._isDoubleClick || ((savedHitTestInformation.HitTestType != HitTestType.ColumnSplitBar) && (savedHitTestInformation.HitTestType != HitTestType.RowSplitBar)))
            {
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.TabStrip:
                        if (base._routedEventArgs != null)
                        {
                            _tabStrip.ProcessMouseClickSheetTab(base._routedEventArgs.Instance);
                        }
                        return;

                    case HitTestType.RowSplitBar:
                        StartRowSplitting();
                        if (savedHitTestInformation.ColumnViewportIndex >= 0)
                        {
                            StartColumnSplitting();
                        }
                        return;

                    case HitTestType.ColumnSplitBar:
                        StartColumnSplitting();
                        if (savedHitTestInformation.RowViewportIndex >= 0)
                        {
                            StartRowSplitting();
                        }
                        return;

                    case HitTestType.RowSplitBox:
                        StartRowSplitting();
                        return;

                    case HitTestType.ColumnSplitBox:
                        StartColumnSplitting();
                        return;

                    case HitTestType.TabSplitBox:
                        StartTabStripResizing();
                        return;
                }
                base.ProcessMouseLeftButtonDown(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal override void ProcessMouseLeftButtonUp(PointerMouseRoutedEventArgs e)
        {
            if ((IsColumnSplitting || IsRowSplitting) || IsTabStripResizing)
            {
                ClearMouseLeftButtonDownStates();
                if (!IsEditing)
                {
                    base.FocusInternal();
                }
            }
            else
            {
                base.ProcessMouseLeftButtonUp(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal override void ProcessMouseMove(PointerMouseRoutedEventArgs e)
        {
            GetSpreadLayout();
            base.MousePosition = e.GetPosition(this);
            HitTestInformation hitTestInfo = HitTest(base.MousePosition.X, base.MousePosition.Y);
            if (!base.IsWorking)
            {
                base.ResetCursor();
            }
            bool flag = false;
            switch (hitTestInfo.HitTestType)
            {
                case HitTestType.RowSplitBar:
                    if ((!base.IsWorking && !IsEditing) && !SpreadSheet.Workbook.Protect)
                    {
                        if (base.InputDeviceType != InputDeviceType.Touch)
                        {
                            if (hitTestInfo.ColumnViewportIndex < 0)
                            {
                                base.SetBuiltInCursor((CoreCursorType)8);
                            }
                            else
                            {
                                base.SetBuiltInCursor((CoreCursorType)3);
                            }
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                case HitTestType.ColumnSplitBar:
                    if ((base.IsWorking || IsEditing) || SpreadSheet.Workbook.Protect)
                    {
                        goto Label_01B3;
                    }
                    if (base.InputDeviceType != InputDeviceType.Touch)
                    {
                        if (hitTestInfo.RowViewportIndex < 0)
                        {
                            base.SetBuiltInCursor((CoreCursorType)10);
                            break;
                        }
                        base.SetBuiltInCursor((CoreCursorType)3);
                    }
                    break;

                case HitTestType.RowSplitBox:
                    if ((!base.IsWorking && !IsEditing) && !SpreadSheet.Workbook.Protect)
                    {
                        if (base.InputDeviceType != InputDeviceType.Touch)
                        {
                            base.SetBuiltInCursor((CoreCursorType)8);
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                case HitTestType.ColumnSplitBox:
                    if ((!base.IsWorking && !IsEditing) && !SpreadSheet.Workbook.Protect)
                    {
                        if (base.InputDeviceType != InputDeviceType.Touch)
                        {
                            base.SetBuiltInCursor((CoreCursorType)10);
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                case HitTestType.TabSplitBox:
                    if (!base.IsWorking && !IsEditing)
                    {
                        if (base.InputDeviceType != InputDeviceType.Touch)
                        {
                            base.SetBuiltInCursor((CoreCursorType)10);
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                default:
                    goto Label_01B3;
            }
            flag = true;
        Label_01B3:
            if (IsColumnSplitting)
            {
                ContinueColumnSplitting();
            }
            if (IsRowSplitting)
            {
                ContinueRowSplitting();
            }
            if (IsTabStripResizing)
            {
                ContinueTabStripResizing();
            }
            if (flag)
            {
                if (!base.IsWorking)
                {
                    base.SaveHitTestInfo(hitTestInfo);
                    base._hoverManager.DoHover(hitTestInfo);
                }
            }
            else
            {
                base.ProcessMouseMove(e);
            }
            UpdateScrollBarIndicatorMode((ScrollingIndicatorMode)2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hi"></param>
        void ProcessSplitBarDoubleClick(HitTestInformation hi)
        {
            if (!SpreadSheet.Workbook.Protect)
            {
                int rowViewportIndex = hi.RowViewportIndex;
                int columnViewportIndex = hi.ColumnViewportIndex;
                if (rowViewportIndex >= 0)
                {
                    double viewportHeight = base.GetViewportHeight(rowViewportIndex + 1);
                    if (!RaiseRowViewportHeightChanging(rowViewportIndex, viewportHeight))
                    {
                        Worksheet.RemoveRowViewport(rowViewportIndex);
                        RaiseRowViewportHeightChanged(rowViewportIndex, viewportHeight);
                    }
                }
                if (columnViewportIndex >= 0)
                {
                    double viewportWidth = base.GetViewportWidth(columnViewportIndex + 1);
                    if (!RaiseColumnViewportWidthChanging(columnViewportIndex, viewportWidth))
                    {
                        Worksheet.RemoveColumnViewport(columnViewportIndex);
                        RaiseColumnViewportWidthChanged(columnViewportIndex, viewportWidth);
                    }
                }
                InvalidateLayout();
                base._positionInfo = null;
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hi"></param>
        void ProcessSplitBarDoubleTap(HitTestInformation hi)
        {
            ProcessSplitBarDoubleClick(hi);
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void ProcessStartSheetIndexChanged()
        {
            if (((Worksheet != null) && (Worksheet.Workbook != null)) && (_tabStrip != null))
            {
                _tabStrip.SetStartSheet(Worksheet.Workbook.StartSheetIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        internal override void ProcessTap(Point point)
        {
            base.UpdateTouchHitTestInfo(point);
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.HorizontalScrollBar:
                    {
                        int viewportLeftColumn = base.GetViewportLeftColumn(savedHitTestInformation.ColumnViewportIndex);
                        HorizontalScrollBarTouchSmallDecrement(savedHitTestInformation.ColumnViewportIndex, viewportLeftColumn - 1);
                        break;
                    }
                case HitTestType.VerticalScrollBar:
                    {
                        int viewportTopRow = base.GetViewportTopRow(savedHitTestInformation.RowViewportIndex);
                        VerticalScrollBarTouchSmallDecrement(savedHitTestInformation.RowViewportIndex, viewportTopRow - 1);
                        break;
                    }
                case HitTestType.TabStrip:
                    _tabStrip.ProcessTap(point);
                    break;
            }
            base.ProcessTap(point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        internal override void ProcessTouchHold(Point point)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <param name="e"></param>
        void ProcessVerticalScroll(int rowViewportIndex, ScrollEventArgs e)
        {
            int viewportTopRow = base.GetViewportTopRow(rowViewportIndex);
            int scrollValue = (int)Math.Round(e.NewValue);
            scrollValue = MapScrollValueToRowIndex(Worksheet, scrollValue);
            int beforeRow = scrollValue;
            if (e.ScrollEventType == (ScrollEventType)1)
            {
                beforeRow = base.TryGetNextScrollableRow(scrollValue);
            }
            else if (e.ScrollEventType == (ScrollEventType)3)
            {
                if (NavigatorHelper.ScrollToNextPageOfRows(this, rowViewportIndex))
                {
                    beforeRow = base.GetViewportTopRow(rowViewportIndex);
                }
                else
                {
                    beforeRow = base.TryGetNextScrollableRow(scrollValue);
                }
            }
            else if (e.ScrollEventType == (ScrollEventType)2)
            {
                if (NavigatorHelper.ScrollToPreviousPageOfRows(this, rowViewportIndex))
                {
                    beforeRow = base.GetViewportTopRow(rowViewportIndex);
                }
                else
                {
                    beforeRow = base.TryGetPreviousScrollableRow(scrollValue);
                }
            }
            else if (e.ScrollEventType == 0)
            {
                beforeRow = base.TryGetPreviousScrollableRow(scrollValue);
            }
            if ((e.ScrollEventType == (ScrollEventType)5) || (e.ScrollEventType == (ScrollEventType)8))
            {
                beforeRow = base.TryGetNextScrollableRow(scrollValue);
            }
            if ((viewportTopRow != beforeRow) && (beforeRow != -1))
            {
                _scrollTo = beforeRow;
                AsynSetViewportTopRow(rowViewportIndex);
            }
            if (((e.ScrollEventType != (ScrollEventType)5) && (beforeRow != e.NewValue)) && (_verticalScrollBar != null))
            {
                GetSpreadLayout();
                if (((rowViewportIndex > -1) && (rowViewportIndex < _verticalScrollBar.Length)) && (beforeRow != _verticalScrollBar[rowViewportIndex].Value))
                {
                    int invisibleRowsBeforeRow = GetInvisibleRowsBeforeRow(ActiveSheet, beforeRow);
                    beforeRow -= invisibleRowsBeforeRow;
                    _verticalScrollBar[rowViewportIndex].Value = (beforeRow != -1) ? ((double)beforeRow) : ((double)viewportTopRow);
                    _verticalScrollBar[rowViewportIndex].InvalidateMeasure();
                    _verticalScrollBar[rowViewportIndex].InvalidateArrange();
                }
            }
            if (_showScrollTip && ((base.ShowScrollTip == ShowScrollTip.Both) || (base.ShowScrollTip == ShowScrollTip.Vertical)))
            {
                base.UpdateScrollToolTip(true, _scrollTo + 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RaiseActiveSheetIndexChanged()
        {
            if ((ActiveSheetChanged != null) && (base._eventSuspended == 0))
            {
                ActiveSheetChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool RaiseActiveSheetIndexChanging()
        {
            if ((ActiveSheetChanging != null) && (base._eventSuspended == 0))
            {
                CancelEventArgs args = new CancelEventArgs();
                ActiveSheetChanging(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportIndex"></param>
        /// <param name="deltaViewportWidth"></param>
        internal void RaiseColumnViewportWidthChanged(int viewportIndex, double deltaViewportWidth)
        {
            if ((ColumnViewportWidthChanged != null) && (base._eventSuspended == 0))
            {
                ColumnViewportWidthChanged(this, new ColumnViewportWidthChangedEventArgs(viewportIndex, deltaViewportWidth));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportIndex"></param>
        /// <param name="deltaViewportWidth"></param>
        /// <returns></returns>
        internal bool RaiseColumnViewportWidthChanging(int viewportIndex, double deltaViewportWidth)
        {
            if ((ColumnViewportWidthChanging != null) && (base._eventSuspended == 0))
            {
                ColumnViewportWidthChangingEventArgs args = new ColumnViewportWidthChangingEventArgs(viewportIndex, deltaViewportWidth);
                ColumnViewportWidthChanging(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportIndex"></param>
        /// <param name="deltaViewportHeight"></param>
        internal void RaiseRowViewportHeightChanged(int viewportIndex, double deltaViewportHeight)
        {
            if ((RowViewportHeightChanged != null) && (base._eventSuspended == 0))
            {
                RowViewportHeightChanged(this, new RowViewportHeightChangedEventArgs(viewportIndex, deltaViewportHeight));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportIndex"></param>
        /// <param name="deltaViewportHeight"></param>
        /// <returns></returns>
        internal bool RaiseRowViewportHeightChanging(int viewportIndex, double deltaViewportHeight)
        {
            if ((RowViewportHeightChanging != null) && (base._eventSuspended == 0))
            {
                RowViewportHeightChangingEventArgs args = new RowViewportHeightChangingEventArgs(viewportIndex, deltaViewportHeight);
                RowViewportHeightChanging(this, args);
                if (args.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        internal override void ReadXmlInternal(XmlReader reader)
        {
            base.ReadXmlInternal(reader);
            switch (reader.Name)
            {
                case "ScrollBarTrackPolicy":
                    _scrollBarTrackPolicy = (ScrollBarTrackPolicy)Serializer.DeserializeObj(typeof(ScrollBarTrackPolicy), reader);
                    return;

                case "ColumnSplitBoxAlignment":
                    _columnSplitBoxAlignment = (SplitBoxAlignment)Serializer.DeserializeObj(typeof(SplitBoxAlignment), reader);
                    return;

                case "RowSplitBoxAlignment":
                    _rowSplitBoxAlignment = (SplitBoxAlignment)Serializer.DeserializeObj(typeof(SplitBoxAlignment), reader);
                    return;

                case "HorizontalScrollBarHeight":
                    _horizontalScrollBarHeight = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "VerticalScrollBarWidth":
                    _verticalScrollBarWidth = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "TabStripVisibility":
                    _tabStripVisibility = (Visibility)Serializer.DeserializeObj(typeof(Visibility), reader);
                    return;

                case "TabStripEditable":
                    _tabStripEditable = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "TabStripRadio":
                    _tabStripRatio = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "TabStripInsertTab":
                    {
                        bool flag = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        _tabStripInsertTab = flag;
                        if (_tabStrip == null)
                        {
                            break;
                        }
                        _tabStrip.HasInsertTab = flag;
                        return;
                    }
                case "ColumnSplitBoxPolicy":
                    _columnSplitBoxPolicy = (SplitBoxPolicy)Serializer.DeserializeObj(typeof(SplitBoxPolicy), reader);
                    return;

                case "RowSplitBoxPolicy":
                    _rowSplitBoxPolicy = (SplitBoxPolicy)Serializer.DeserializeObj(typeof(SplitBoxPolicy), reader);
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RefreshTabStrip()
        {
            if (_tabStrip != null)
            {
                _tabStrip.Refresh();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        internal void RemoveColumnViewport(int columnViewportIndex)
        {
            Worksheet.RemoveColumnViewport(columnViewportIndex);
            InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        internal void RemoveRowViewport(int rowViewportIndex)
        {
            Worksheet.RemoveRowViewport(rowViewportIndex);
            InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void Reset()
        {
            Init();
            base.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void SaveDataForFormulaSelection()
        {
            if (base.CanSelectFormula)
            {
                IsSwitchingSheet = true;
                base.EditorConnector.ClearFlickingItems();
                if (!base.EditorConnector.IsInOtherSheet)
                {
                    base.EditorConnector.IsInOtherSheet = true;
                    base.EditorConnector.SheetIndex = Worksheet.Workbook.ActiveSheetIndex;
                    base.EditorConnector.RowIndex = Worksheet.ActiveRowIndex;
                    base.EditorConnector.ColumnIndex = Worksheet.ActiveColumnIndex;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ShowOpeningProgressRing()
        {
            UIAdaptor.InvokeAsync(delegate
            {
                if (_progressRing != null)
                {
                    _progressRing.Visibility = 0;
                    _progressRing.IsActive = true;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ShowOpeningStatus()
        {
            ShowOpeningProgressRing();
            UIAdaptor.InvokeAsync(delegate
            {
                if (TabStrip != null)
                {
                    TabStrip.Visibility = (Visibility)1;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        void StartColumnSplitting()
        {
            if (!SpreadSheet.Workbook.Protect)
            {
                HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
                SpreadLayout spreadLayout = GetSpreadLayout();
                if (!base.IsTouching)
                {
                    IsColumnSplitting = true;
                }
                else
                {
                    IsTouchColumnSplitting = true;
                }
                base.IsWorking = true;
                if (_columnSplittingTracker == null)
                {
                    SolidColorBrush brush = null;
                    UIAdaptor.InvokeSync(delegate
                    {
                        brush = new SolidColorBrush(Colors.Black);
                    });
                    Line line = new Line();
                    line.Stroke = brush;
                    line.Opacity = 0.5;
                    _columnSplittingTracker = line;
                    SplittingTrackerContainer.Children.Add(_columnSplittingTracker);
                }
                int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
                int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
                _columnSplittingTracker.Opacity = 0.5;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.RowSplitBar:
                    case HitTestType.ColumnSplitBar:
                        _columnSplittingTracker.StrokeThickness = spreadLayout.GetHorizontalSplitBarWidth(columnViewportIndex);
                        _columnSplittingTracker.X1 = spreadLayout.GetHorizontalSplitBarX(columnViewportIndex) + (spreadLayout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0);
                        _columnSplittingTracker.Y1 = spreadLayout.Y;
                        _columnSplittingTracker.X2 = _columnSplittingTracker.X1;
                        _columnSplittingTracker.Y2 = spreadLayout.HeaderY + base.AvailableSize.Height;
                        return;

                    case HitTestType.RowSplitBox:
                        return;

                    case HitTestType.ColumnSplitBox:
                        _columnSplittingTracker.StrokeThickness = spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex);
                        if (ColumnSplitBoxAlignment != SplitBoxAlignment.Leading)
                        {
                            _columnSplittingTracker.X1 = (spreadLayout.GetViewportX(columnViewportIndex) + spreadLayout.GetViewportWidth(columnViewportIndex)) - (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0);
                        }
                        else
                        {
                            _columnSplittingTracker.X1 = spreadLayout.GetViewportX(columnViewportIndex) + (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0);
                        }
                        _columnSplittingTracker.Y1 = spreadLayout.Y;
                        _columnSplittingTracker.X2 = _columnSplittingTracker.X1;
                        _columnSplittingTracker.Y2 = spreadLayout.HeaderY + base.AvailableSize.Height;
                        return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal override bool StartMouseClick(PointerMouseRoutedEventArgs e)
        {
            if (IsMouseInScrollBar())
            {
                return false;
            }
            if (IsEditing && ((IsMouseInSplitBar() || IsMouseInSplitBox()) || IsMouseInTabSplitBox()))
            {
                return false;
            }
            Point position = e.GetPosition(this);
            if (!GetTabStripRectangle().Contains(position))
            {
                return base.StartMouseClick(e);
            }
            if (base.CanSelectFormula)
            {
                IsSwitchingSheet = true;
                base.EditorConnector.ClearFlickingItems();
                if (!base.EditorConnector.IsInOtherSheet)
                {
                    base.EditorConnector.IsInOtherSheet = true;
                    base.EditorConnector.SheetIndex = Worksheet.Workbook.ActiveSheetIndex;
                    base.EditorConnector.RowIndex = Worksheet.ActiveRowIndex;
                    base.EditorConnector.ColumnIndex = Worksheet.ActiveColumnIndex;
                }
            }
            base.StopCellEditing(base.CanSelectFormula);
            if (_tabStrip != null)
            {
                _tabStrip.StopTabEditing(false);
            }
            base._lastClickPoint = new Point(position.X, position.Y);
            base._routedEventArgs = e;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        void StartRowSplitting()
        {
            if (!SpreadSheet.Workbook.Protect)
            {
                HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
                SpreadLayout spreadLayout = GetSpreadLayout();
                if (!base.IsTouching)
                {
                    IsRowSplitting = true;
                }
                else
                {
                    IsTouchRowSplitting = true;
                }
                base.IsWorking = true;
                if (_rowSplittingTracker == null)
                {
                    SolidColorBrush brush = null;
                    UIAdaptor.InvokeSync(delegate
                    {
                        brush = new SolidColorBrush(Colors.Black);
                    });
                    Line line = new Line();
                    line.Stroke = brush;
                    line.Opacity = 0.5;
                    _rowSplittingTracker = line;
                    SplittingTrackerContainer.Children.Add(_rowSplittingTracker);
                }
                int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
                int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
                _rowSplittingTracker.Opacity = 0.5;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.RowSplitBar:
                    case HitTestType.ColumnSplitBar:
                        _rowSplittingTracker.StrokeThickness = spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex);
                        _rowSplittingTracker.Y1 = spreadLayout.GetVerticalSplitBarY(rowViewportIndex) + (spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0);
                        _rowSplittingTracker.X1 = spreadLayout.X;
                        _rowSplittingTracker.Y2 = _rowSplittingTracker.Y1;
                        _rowSplittingTracker.X2 = spreadLayout.X + base.AvailableSize.Width;
                        return;

                    case HitTestType.RowSplitBox:
                        _rowSplittingTracker.StrokeThickness = spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex);
                        if (RowSplitBoxAlignment != SplitBoxAlignment.Leading)
                        {
                            _rowSplittingTracker.Y1 = (spreadLayout.GetViewportY(rowViewportIndex) + spreadLayout.GetViewportHeight(rowViewportIndex)) - (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0);
                        }
                        else
                        {
                            _rowSplittingTracker.Y1 = spreadLayout.GetViewportY(rowViewportIndex) + (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0);
                        }
                        _rowSplittingTracker.X1 = spreadLayout.X;
                        _rowSplittingTracker.Y2 = _rowSplittingTracker.Y1;
                        _rowSplittingTracker.X2 = spreadLayout.X + base.AvailableSize.Width;
                        return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void StartTabStripResizing()
        {
            if (!base.IsTouching)
            {
                IsTabStripResizing = true;
            }
            else
            {
                base.IsTouchTabStripResizing = true;
            }
            base.IsWorking = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="translate"></param>
        /// <param name="scale"></param>
        void UpdateCachedImageTransform(Point currentPoint, Point translate, double scale)
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if (base._cachedCornerViewportTransform != null)
            {
                UpdateCachedImageTransform(base._cachedCornerViewportTransform, currentPoint, translate, scale, -1, -1, spreadLayout.HeaderX, spreadLayout.HeaderY);
            }
            if (base._cachedColumnHeaderViewportTransform != null)
            {
                for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
                {
                    double viewportX = spreadLayout.GetViewportX(i);
                    double headerY = spreadLayout.HeaderY;
                    UpdateCachedImageTransform(base._cachedColumnHeaderViewportTransform[i + 1], currentPoint, translate, scale, i, -1, viewportX, headerY);
                }
            }
            if (base._cachedRowHeaderViewportTransform != null)
            {
                for (int j = -1; j <= spreadLayout.RowPaneCount; j++)
                {
                    double headerX = spreadLayout.HeaderX;
                    double viewportY = spreadLayout.GetViewportY(j);
                    UpdateCachedImageTransform(base._cachedRowHeaderViewportTransform[j + 1], currentPoint, translate, scale, -1, j, headerX, viewportY);
                }
            }
            if (base._cachedViewportTransform != null)
            {
                for (int k = -1; k <= spreadLayout.ColumnPaneCount; k++)
                {
                    double columnX = spreadLayout.GetViewportX(k);
                    for (int m = -1; m <= spreadLayout.RowPaneCount; m++)
                    {
                        double rowY = spreadLayout.GetViewportY(m);
                        UpdateCachedImageTransform(base._cachedViewportTransform[m + 1, k + 1], currentPoint, translate, scale, k, m, columnX, rowY);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformGruop"></param>
        /// <param name="currentPoint"></param>
        /// <param name="translate"></param>
        /// <param name="scale"></param>
        /// <param name="columnViewportIndex"></param>
        /// <param name="rowViewportIndex"></param>
        /// <param name="columnX"></param>
        /// <param name="rowY"></param>
        void UpdateCachedImageTransform(TransformGroup transformGruop, Point currentPoint, Point translate, double scale, int columnViewportIndex, int rowViewportIndex, double columnX, double rowY)
        {
            MatrixTransform transform = null;
            CompositeTransform transform2 = null;
            double num5;
            foreach (Transform transform3 in transformGruop.Children)
            {
                if (transform3 is MatrixTransform)
                {
                    transform = transform3 as MatrixTransform;
                }
                else if (transform3 is CompositeTransform)
                {
                    transform2 = transform3 as CompositeTransform;
                }
            }
            transform.Matrix = transformGruop.Value;
            double x = currentPoint.X;
            double y = currentPoint.Y;
            double num3 = translate.X;
            double num4 = translate.Y;
            SpreadLayout spreadLayout = GetSpreadLayout();
            if ((columnViewportIndex < 0) || (columnViewportIndex < base._touchStartHitTestInfo.ColumnViewportIndex))
            {
                x = 0.0;
                num3 = 0.0;
            }
            else if (columnViewportIndex > base._touchStartHitTestInfo.ColumnViewportIndex)
            {
                x = spreadLayout.GetViewportWidth(columnViewportIndex);
            }
            if ((rowViewportIndex < 0) || (rowViewportIndex < base._touchStartHitTestInfo.RowViewportIndex))
            {
                y = 0.0;
                num4 = 0.0;
            }
            else if (rowViewportIndex > base._touchStartHitTestInfo.RowViewportIndex)
            {
                y = spreadLayout.GetViewportHeight(rowViewportIndex);
            }
            Point point = transform.TransformPoint(new Point(x, y));
            transform2.CenterX = point.X;
            transform2.CenterY = point.Y;
            transform2.ScaleY = num5 = scale;
            transform2.ScaleX = num5;
            transform2.TranslateX = num3;
            transform2.TranslateY = num4;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateCrossSplitBars()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if ((_crossSplitBar != null) && (((ActiveSheet == null) || (_crossSplitBar.GetLength(0) != (spreadLayout.RowPaneCount - 1))) || (_crossSplitBar.GetLength(1) != (spreadLayout.ColumnPaneCount - 1))))
            {
                for (int i = 0; i < _crossSplitBar.GetLength(0); i++)
                {
                    for (int j = 0; j < _crossSplitBar.GetLength(1); j++)
                    {
                        base.Children.Remove(_crossSplitBar[i, j]);
                    }
                }
                _crossSplitBar = null;
            }
            if (((ActiveSheet != null) && (_crossSplitBar == null)) && ((spreadLayout == null) || ((spreadLayout.RowPaneCount >= 1) && (spreadLayout.ColumnPaneCount >= 1))))
            {
                _crossSplitBar = new CrossSplitBar[spreadLayout.RowPaneCount - 1, spreadLayout.ColumnPaneCount - 1];
                for (int k = 0; k < _crossSplitBar.GetLength(0); k++)
                {
                    for (int m = 0; m < _crossSplitBar.GetLength(1); m++)
                    {
                        _crossSplitBar[k, m] = new CrossSplitBar();
                        Canvas.SetZIndex(_crossSplitBar[k, m], 2);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateHorizontalScrollBars()
        {
            if (ActiveSheet != null)
            {
                SpreadLayout spreadLayout = GetSpreadLayout();
                if ((_horizontalScrollBar != null) && ((ActiveSheet == null) || (_horizontalScrollBar.Length != spreadLayout.ColumnPaneCount)))
                {
                    for (int j = 0; j < _horizontalScrollBar.Length; j++)
                    {
                        _horizontalScrollBar[j].Scroll -= HorizontalScrollbar_Scroll;
                        _horizontalScrollBar[j].PointerPressed -= OnHorizontalScrollBarPointerPressed;
                        _horizontalScrollBar[j].PointerReleased -= OnHorizontalScrollBarPointerReleased;
                        _horizontalScrollBar[j].PointerExited -= OnHorizontalScrollBarPointerExited;
                        base.Children.Remove(_horizontalScrollBar[j]);
                    }
                    _horizontalScrollBar = null;
                }
                if (_horizontalScrollBar == null)
                {
                    _horizontalScrollBar = new ScrollBar[spreadLayout.ColumnPaneCount];
                    for (int k = 0; k < spreadLayout.ColumnPaneCount; k++)
                    {
                        _horizontalScrollBar[k] = new ScrollBar();
                        _horizontalScrollBar[k].Orientation = (Orientation)1;
                        _horizontalScrollBar[k].IsTabStop = false;
                        _horizontalScrollBar[k].TypeSafeSetStyle(SpreadSheet.HorizontalScrollBarStyle);
                        _horizontalScrollBar[k].Scroll += HorizontalScrollbar_Scroll;
                        _horizontalScrollBar[k].PointerPressed += OnHorizontalScrollBarPointerPressed;
                        _horizontalScrollBar[k].PointerReleased += OnHorizontalScrollBarPointerReleased;
                        _horizontalScrollBar[k].PointerExited += OnHorizontalScrollBarPointerExited;
                        Canvas.SetZIndex(_horizontalScrollBar[k], 0x62);
                    }
                }
                int sheetInvisibleColumns = GetSheetInvisibleColumns(ActiveSheet);
                for (int i = 0; i < spreadLayout.ColumnPaneCount; i++)
                {
                    double num8;
                    int viewportInvisibleColumns = GetViewportInvisibleColumns(i);
                    _horizontalScrollBar[i].Minimum = (double)ActiveSheet.FrozenColumnCount;
                    _horizontalScrollBar[i].Maximum = (double)Math.Max(ActiveSheet.FrozenColumnCount, ((ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount) - sheetInvisibleColumns) - 1);
                    _horizontalScrollBar[i].ViewportSize = num8 = base.GetViewportColumnLayoutModel(i).Count - viewportInvisibleColumns;
                    _horizontalScrollBar[i].LargeChange = num8;
                    _horizontalScrollBar[i].SmallChange = 1.0;
                    int viewportLeftColumn = base.GetViewportLeftColumn(i);
                    viewportLeftColumn = base.TryGetNextScrollableColumn(viewportLeftColumn);
                    int invisibleColumnsBeforeColumn = GetInvisibleColumnsBeforeColumn(Worksheet, viewportLeftColumn);
                    viewportLeftColumn -= invisibleColumnsBeforeColumn;
                    _horizontalScrollBar[i].Value = (double)viewportLeftColumn;
                    _horizontalScrollBar[i].InvalidateArrange();
                    _horizontalScrollBar[i].IsEnabled = HorizontalScrollBarPolicy != 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateHorizontalSplitBars()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if ((_horizontalSplitBar != null) && ((ActiveSheet == null) || (_horizontalSplitBar.Length != (spreadLayout.ColumnPaneCount - 1))))
            {
                foreach (HorizontalSplitBar bar in _horizontalSplitBar)
                {
                    base.Children.Remove(bar);
                }
                _horizontalSplitBar = null;
            }
            if (((ActiveSheet != null) && (_horizontalSplitBar == null)) && (spreadLayout.ColumnPaneCount >= 1))
            {
                _horizontalSplitBar = new HorizontalSplitBar[spreadLayout.ColumnPaneCount - 1];
                for (int i = 0; i < _horizontalSplitBar.Length; i++)
                {
                    _horizontalSplitBar[i] = new HorizontalSplitBar();
                    Canvas.SetZIndex(_horizontalSplitBar[i], 2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateHorizontalSplitBoxes()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if ((_horizontalSplitBox != null) && ((ActiveSheet == null) || (_horizontalSplitBox.Length != spreadLayout.ColumnPaneCount)))
            {
                for (int i = 0; i < _horizontalSplitBox.Length; i++)
                {
                    base.Children.Remove(_horizontalSplitBox[i]);
                }
                _horizontalSplitBox = null;
            }
            if ((ActiveSheet != null) && (_horizontalSplitBox == null))
            {
                _horizontalSplitBox = new HorizontalSplitBox[spreadLayout.ColumnPaneCount];
                for (int j = 0; j < spreadLayout.ColumnPaneCount; j++)
                {
                    _horizontalSplitBox[j] = new HorizontalSplitBox();
                    Canvas.SetZIndex(_horizontalSplitBox[j], 0x62);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScrollingIndicatorMode"></param>
        void UpdateScrollBarIndicatorMode(ScrollingIndicatorMode ScrollingIndicatorMode)
        {
            if (_horizontalScrollBar != null)
            {
                for (int i = 0; i < _horizontalScrollBar.Length; i++)
                {
                    if (_horizontalScrollBar[i] != null)
                    {
                        _horizontalScrollBar[i].IndicatorMode = ScrollingIndicatorMode;
                    }
                }
            }
            if (_verticalScrollBar != null)
            {
                for (int j = 0; j < _verticalScrollBar.Length; j++)
                {
                    if (_verticalScrollBar[j] != null)
                    {
                        _verticalScrollBar[j].IndicatorMode = ScrollingIndicatorMode;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void UpdateTabStrip()
        {
            if ((_tabStrip != null) && (_tabStrip.TabsPresenter != null))
            {
                _tabStrip.TabsPresenter.InvalidateMeasure();
                _tabStrip.TabsPresenter.InvalidateArrange();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateVerticalScrollBars()
        {
            if (ActiveSheet != null)
            {
                SpreadLayout spreadLayout = GetSpreadLayout();
                if ((_verticalScrollBar != null) && ((ActiveSheet == null) || (_verticalScrollBar.Length != spreadLayout.RowPaneCount)))
                {
                    for (int j = 0; j < _verticalScrollBar.Length; j++)
                    {
                        _verticalScrollBar[j].Scroll -= VerticalScrollbar_Scroll;
                        _verticalScrollBar[j].PointerPressed -= OnVerticalScrollbarPointerPressed;
                        _verticalScrollBar[j].PointerReleased -= OnVerticalScrollbarPointerReleased;
                        _verticalScrollBar[j].PointerExited -= OnVerticalScrollbarPointerExited;
                        base.Children.Remove(_verticalScrollBar[j]);
                    }
                    _verticalScrollBar = null;
                }
                if ((ActiveSheet != null) && (_verticalScrollBar == null))
                {
                    _verticalScrollBar = new ScrollBar[spreadLayout.RowPaneCount];
                    for (int k = 0; k < _verticalScrollBar.Length; k++)
                    {
                        _verticalScrollBar[k] = new ScrollBar();
                        _verticalScrollBar[k].IsEnabled = true;
                        _verticalScrollBar[k].Orientation = 0;
                        _verticalScrollBar[k].ViewportSize = 25.0;
                        _verticalScrollBar[k].IsTabStop = false;
                        _verticalScrollBar[k].TypeSafeSetStyle(SpreadSheet.VerticalScrollBarStyle);
                        _verticalScrollBar[k].Scroll += VerticalScrollbar_Scroll;
                        _verticalScrollBar[k].PointerPressed += OnVerticalScrollbarPointerPressed;
                        _verticalScrollBar[k].PointerReleased += OnVerticalScrollbarPointerReleased;
                        _verticalScrollBar[k].PointerExited += OnVerticalScrollbarPointerExited;
                        Canvas.SetZIndex(_verticalScrollBar[k], 0x62);
                    }
                }
                int sheetInvisibleRows = GetSheetInvisibleRows(ActiveSheet);
                for (int i = 0; i < spreadLayout.RowPaneCount; i++)
                {
                    double num8;
                    int viewportInvisibleRows = GetViewportInvisibleRows(i);
                    _verticalScrollBar[i].Minimum = (double)ActiveSheet.FrozenRowCount;
                    _verticalScrollBar[i].Maximum = (double)Math.Max(ActiveSheet.FrozenRowCount, ((ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount) - sheetInvisibleRows) - 1);
                    _verticalScrollBar[i].ViewportSize = num8 = base.GetViewportRowLayoutModel(i).Count - viewportInvisibleRows;
                    _verticalScrollBar[i].LargeChange = num8;
                    _verticalScrollBar[i].SmallChange = 1.0;
                    int viewportTopRow = base.GetViewportTopRow(i);
                    viewportTopRow = base.TryGetNextScrollableRow(viewportTopRow);
                    int invisibleRowsBeforeRow = GetInvisibleRowsBeforeRow(Worksheet, viewportTopRow);
                    viewportTopRow -= invisibleRowsBeforeRow;
                    _verticalScrollBar[i].Value = (double)viewportTopRow;
                    _verticalScrollBar[i].InvalidateArrange();
                    _verticalScrollBar[i].IsEnabled = VerticalScrollBarPolicy != 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateVerticalSplitBars()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if ((_verticalSplitBar != null) && ((ActiveSheet == null) || (_verticalSplitBar.Length != (spreadLayout.RowPaneCount - 1))))
            {
                foreach (VerticalSplitBar bar in _verticalSplitBar)
                {
                    base.Children.Remove(bar);
                }
                _verticalSplitBar = null;
            }
            if (((ActiveSheet != null) && (_verticalSplitBar == null)) && (spreadLayout.RowPaneCount >= 1))
            {
                _verticalSplitBar = new VerticalSplitBar[spreadLayout.RowPaneCount - 1];
                for (int i = 0; i < _verticalSplitBar.Length; i++)
                {
                    _verticalSplitBar[i] = new VerticalSplitBar();
                    Canvas.SetZIndex(_verticalSplitBar[i], 2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateVerticalSplitBoxes()
        {
            SpreadLayout spreadLayout = GetSpreadLayout();
            if ((_verticalSplitBox != null) && ((ActiveSheet == null) || (_verticalSplitBox.Length != spreadLayout.RowPaneCount)))
            {
                foreach (VerticalSplitBox box in _verticalSplitBox)
                {
                    base.Children.Remove(box);
                }
                _verticalSplitBox = null;
            }
            if ((ActiveSheet != null) && (_verticalSplitBox == null))
            {
                _verticalSplitBox = new VerticalSplitBox[spreadLayout.RowPaneCount];
                for (int i = 0; i < spreadLayout.RowPaneCount; i++)
                {
                    _verticalSplitBox[i] = new VerticalSplitBox();
                    Canvas.SetZIndex(_verticalSplitBox[i], 0x62);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void UpdateViewport()
        {
            bool flag = ((base._touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject) && (base._touchStartHitTestInfo.FloatingObjectInfo.FloatingObject != null)) && base._touchStartHitTestInfo.FloatingObjectInfo.FloatingObject.IsSelected;
            if (((base._touchStartHitTestInfo != null) && (base._touchStartHitTestInfo.HitTestType == HitTestType.Viewport)) || ((base._touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject) && !flag))
            {
                AdjustViewportLeftColumn();
                AdjustViewportTopRow();
                AdjustViewportSize();
                base._translateOffsetY = 0.0;
                base._translateOffsetX = 0.0;
                if (base._updateViewportAfterTouch)
                {
                    InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void VerticalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((base._touchToolbarPopup != null) && base._touchToolbarPopup.IsOpen)
            {
                base._touchToolbarPopup.IsOpen = false;
            }
            if (((ScrollBarTrackPolicy == ScrollBarTrackPolicy.Both) || (ScrollBarTrackPolicy == ScrollBarTrackPolicy.Vertical)) || (base._isTouchScrolling || (e.ScrollEventType != (ScrollEventType)5)))
            {
                for (int i = 0; i < _verticalScrollBar.Length; i++)
                {
                    if (sender == _verticalScrollBar[i])
                    {
                        if (base.VerticalScrollable)
                        {
                            ProcessVerticalScroll(i, e);
                            return;
                        }
                        _verticalScrollBar[i].Value = (double)base.GetViewportTopRow(i);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <param name="newValue"></param>
        void VerticalScrollBarTouchSmallDecrement(int rowViewportIndex, int newValue)
        {
            int viewportTopRow = base.GetViewportTopRow(rowViewportIndex);
            int num2 = base.TryGetPreviousScrollableRow(newValue);
            if ((viewportTopRow != num2) && (num2 != -1))
            {
                base.SetViewportTopRow(rowViewportIndex, num2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (_horizontalScrollBarHeight != 25.0)
            {
                Serializer.SerializeObj((double)_horizontalScrollBarHeight, "HorizontalScrollBarHeight", writer);
            }
            if (_verticalScrollBarWidth != 25.0)
            {
                Serializer.SerializeObj((double)_verticalScrollBarWidth, "VerticalScrollBarWidth", writer);
            }
            if (_scrollBarTrackPolicy != ScrollBarTrackPolicy.Both)
            {
                Serializer.SerializeObj(_scrollBarTrackPolicy, "ScrollBarTrackPolicy", writer);
            }
            if (_columnSplitBoxAlignment != SplitBoxAlignment.Leading)
            {
                Serializer.SerializeObj(_columnSplitBoxAlignment, "ColumnSplitBoxAlignment", writer);
            }
            if (_rowSplitBoxAlignment != SplitBoxAlignment.Leading)
            {
                Serializer.SerializeObj(_rowSplitBoxAlignment, "RowSplitBoxAlignment", writer);
            }
            if (_tabStripVisibility != 0)
            {
                Serializer.SerializeObj(_tabStripVisibility, "TabStripVisibility", writer);
            }
            if (!_tabStripEditable)
            {
                Serializer.SerializeObj((bool)_tabStripEditable, "TabStripEditable", writer);
            }
            if (_tabStripRatio != 0.5)
            {
                Serializer.SerializeObj((double)_tabStripRatio, "TabStripRadio", writer);
            }
            if (!_tabStripInsertTab)
            {
                Serializer.SerializeObj((bool)_tabStripInsertTab, "TabStripInsertTab", writer);
            }
            if (_columnSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                Serializer.SerializeObj(_columnSplitBoxPolicy, "ColumnSplitBoxPolicy", writer);
            }
            if (_rowSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                Serializer.SerializeObj(_rowSplitBoxPolicy, "RowSplitBoxPolicy", writer);
            }
        }
    }
}
