using Dt.Base;
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
        private Size _cachedLastAvailableSize;
        private SpreadLayout _cachedSpreadLayout;
        private SplitBoxAlignment _columnSplitBoxAlignment;
        private SplitBoxPolicy _columnSplitBoxPolicy;
        private Line _columnSplittingTracker;
        private CrossSplitBar[,] _crossSplitBar;
        private ScrollBar[] _horizontalScrollBar;
        private double _horizontalScrollBarHeight;
        private Style _horizontalScrollBarStyle;
        private HorizontalSplitBar[] _horizontalSplitBar;
        private HorizontalSplitBox[] _horizontalSplitBox;
        private HashSet<int> _invisibleColumns;
        private HashSet<int> _invisibleRows;
        private bool _pendinging;
        private Grid _progressGrid;
        private ProgressRing _progressRing;
        private SplitBoxAlignment _rowSplitBoxAlignment;
        private SplitBoxPolicy _rowSplitBoxPolicy;
        private Line _rowSplittingTracker;
        private ScrollBarTrackPolicy _scrollBarTrackPolicy;
        private int _scrollTo;
        private bool _showScrollTip;
        private Canvas _splittingTrackerContainer;
        private TabStrip _tabStrip;
        private bool _tabStripEditable;
        private bool _tabStripInsertTab;
        private double _tabStripRatio;
        private Visibility _tabStripVisibility;
        private ScrollBar[] _verticalScrollBar;
        private Style _verticalScrollBarStyle;
        private double _verticalScrollBarWidth;
        private VerticalSplitBar[] _verticalSplitBar;
        private VerticalSplitBox[] _verticalSplitBox;
        internal const double GCSPREAD_HorizontalScrollBarDefaultHeight = 25.0;
        internal const ScrollBarTrackPolicy GCSPREAD_ScrollBarTrackPolicy = ScrollBarTrackPolicy.Both;
        internal const double GCSPREAD_TabStripRatio = 0.5;
        internal const double GCSPREAD_VerticalScrollBarDefaultWidth = 25.0;
        internal static bool IsSwitchingSheet;
        private bool IsTouchColumnSplitting;
        private bool IsTouchRowSplitting;
        private bool IsTouchTabStripScrolling;
        private HorizontalSplitBox tabStripSplitBox;
        private const double TABSTRIPSPLITBOX_WIDTH = 16.0;

        internal SpreadView(Excel owningSpread)
            : base(owningSpread)
        {
            this._cachedLastAvailableSize = new Size(0.0, 0.0);
            this._invisibleRows = new HashSet<int>();
            this._invisibleColumns = new HashSet<int>();
            this.SpreadSheet = owningSpread;
            this.Init();
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
            get { return this.SpreadSheet.ActiveSheet; }
        }

        /// <summary>
        /// 
        /// </summary>
        private double ActualHorizontalScrollBarHeight
        {
            get
            {
                if (this.SpreadSheet.HorizontalScrollBarHeight >= 0.0)
                {
                    return this.SpreadSheet.HorizontalScrollBarHeight;
                }
                return 25.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private double ActualVerticalScrollBarWidth
        {
            get
            {
                if (this.SpreadSheet.VerticalScrollBarWidth >= 0.0)
                {
                    return this.SpreadSheet.VerticalScrollBarWidth;
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
            get { return this._columnSplitBoxAlignment; }
            set
            {
                this._columnSplitBoxAlignment = value;
                this.InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets what conditions under which the GcSpreadSheet component permits column splits. 
        /// </summary>
        [DefaultValue(0)]
        public SplitBoxPolicy ColumnSplitBoxPolicy
        {
            get { return this._columnSplitBoxPolicy; }
            set
            {
                if (value != this._columnSplitBoxPolicy)
                {
                    this._columnSplitBoxPolicy = value;
                    this.InvalidateLayout();
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
            get { return this._horizontalScrollBarHeight; }
            set
            {
                this._horizontalScrollBarHeight = value;
                this.InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private ScrollBarVisibility HorizontalScrollBarPolicy
        {
            get { return this.SpreadSheet.HorizontalScrollBarVisibility; }
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
            get { return this._horizontalScrollBarStyle; }
            set
            {
                this._horizontalScrollBarStyle = value;
                this.InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsColumnSplitting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal override bool IsEditing
        {
            get
            {
                if ((this._tabStrip != null) && this._tabStrip.IsEditing)
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
        private bool IsRowSplitting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private bool IsTabStripResizing { get; set; }

        /// <summary>
        /// Gets or sets the row split box alignment. 
        /// </summary>
        /// <value>
        /// The row split box alignment. 
        /// </value>
        [DefaultValue(0)]
        public SplitBoxAlignment RowSplitBoxAlignment
        {
            get { return this._rowSplitBoxAlignment; }
            set
            {
                this._rowSplitBoxAlignment = value;
                this.InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets what conditions under which the GcSpreadSheet component permits row splits. 
        /// </summary>
        [DefaultValue(0)]
        public SplitBoxPolicy RowSplitBoxPolicy
        {
            get { return this._rowSplitBoxPolicy; }
            set
            {
                if (value != this._rowSplitBoxPolicy)
                {
                    this._rowSplitBoxPolicy = value;
                    this.InvalidateLayout();
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
            get { return this._scrollBarTrackPolicy; }
            set
            {
                this._scrollBarTrackPolicy = value;
                this.InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Canvas SplittingTrackerContainer
        {
            get
            {
                if (this._splittingTrackerContainer == null)
                {
                    this._splittingTrackerContainer = new Canvas();
                    Canvas.SetZIndex(this._splittingTrackerContainer, 0x63);
                }
                return this._splittingTrackerContainer;
            }
        }

        /// <summary>
        /// Gets the Excel control that is associated with the view. 
        /// </summary>
        public Excel SpreadSheet
        {
            get { return (base._host as Excel); }
            private set { base._host = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal TabStrip TabStrip
        {
            get { return this._tabStrip; }
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
            get { return this._tabStripEditable; }
            set
            {
                this._tabStripEditable = value;
                if ((!value && (this._tabStrip != null)) && this._tabStrip.IsEditing)
                {
                    this._tabStrip.StopTabEditing(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether a special tab is displayed to allow the user to insert new sheets. 
        /// </summary>
        [DefaultValue(true)]
        public bool TabStripInsertTab
        {
            get { return this._tabStripInsertTab; }
            set
            {
                if (value != this._tabStripInsertTab)
                {
                    this._tabStripInsertTab = value;
                    if ((this._tabStrip != null) && (value != this._tabStrip.HasInsertTab))
                    {
                        this._tabStrip.HasInsertTab = value;
                        this.InvalidateLayout();
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
            get { return this._tabStripRatio; }
            set
            {
                if (((value >= 0.0) && (value <= 1.0)) && (value != this._tabStripRatio))
                {
                    this._tabStripRatio = value;
                    this.InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        ///Gets or sets the display policy for the sheet tab strip for this component.  
        /// </summary>
        public Visibility TabStripVisibility
        {
            get { return this._tabStripVisibility; }
            set
            {
                if (value != this._tabStripVisibility)
                {
                    this._tabStripVisibility = value;
                    this.InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private ScrollBarVisibility VerticalScrollBarPolicy
        {
            get { return this.SpreadSheet.VerticalScrollBarVisibility; }
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
            get { return this._verticalScrollBarStyle; }
            set
            {
                this._verticalScrollBarStyle = value;
                this.InvalidateLayout();
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
            get { return this._verticalScrollBarWidth; }
            set
            {
                this._verticalScrollBarWidth = value;
                this.InvalidateLayout();
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets the worksheet associated with the view.
        /// </summary>
        public override Worksheet Worksheet
        {
            get { return this.ActiveSheet; }
        }

        /// <summary>
        ///Positions child elements and determines a size for a FrameworkElement derived class, when overridden in a derived class.  
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element uses to arrange itself and its children.
        /// </param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double headerX;
            double headerY;
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            base.TrackersContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            this.SplittingTrackerContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            base.ShapeDrawingContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            base.CursorsContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            if ((base.IsTouchZooming && (base._cornerPresenter != null)) && (base._cornerPresenter.Parent != null))
            {
                headerX = spreadLayout.HeaderX;
                headerY = spreadLayout.HeaderY;
                base._cornerPresenter.Arrange(new Rect(headerX, headerY, spreadLayout.HeaderWidth, spreadLayout.HeaderHeight));
                base._cornerPresenter.RenderTransform = base._cachedCornerViewportTransform;
            }
            else if ((base._cornerPresenter != null) && (base._cornerPresenter.Parent != null))
            {
                headerX = spreadLayout.HeaderX;
                headerY = spreadLayout.HeaderY;
                if (base._cornerPresenter.RenderTransform != null)
                {
                    base._cornerPresenter.RenderTransform = null;
                }
                if ((base._cornerPresenter.Width != spreadLayout.HeaderWidth) || (base._cornerPresenter.Height != spreadLayout.HeaderHeight))
                {
                    base._cornerPresenter.Arrange(new Rect(headerX, headerY, spreadLayout.HeaderWidth, spreadLayout.HeaderHeight));
                }
            }
            if (base.IsTouchZooming && (base._cachedColumnHeaderViewportTransform != null))
            {
                for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
                {
                    headerX = spreadLayout.GetViewportX(i);
                    headerY = spreadLayout.HeaderY;
                    double viewportWidth = spreadLayout.GetViewportWidth(i);
                    double headerHeight = spreadLayout.HeaderHeight;
                    GcViewport viewport = base._columnHeaderPresenters[i + 1];
                    if ((viewport != null) && (viewport.Parent != null))
                    {
                        viewport.Arrange(new Rect(headerX, headerY, viewportWidth, headerHeight));
                        viewport.RenderTransform = base._cachedColumnHeaderViewportTransform[i + 1];
                    }
                }
            }
            else if (base._columnHeaderPresenters != null)
            {
                for (int j = -1; j <= spreadLayout.ColumnPaneCount; j++)
                {
                    headerX = spreadLayout.GetViewportX(j);
                    if ((base.IsTouching && (j == spreadLayout.ColumnPaneCount)) && ((base._translateOffsetX < 0.0) && (base._touchStartHitTestInfo.ColumnViewportIndex == (spreadLayout.ColumnPaneCount - 1))))
                    {
                        headerX += base._translateOffsetX;
                    }
                    headerY = spreadLayout.HeaderY;
                    double width = spreadLayout.GetViewportWidth(j);
                    double height = spreadLayout.HeaderHeight;
                    GcViewport viewport2 = base._columnHeaderPresenters[j + 1];
                    if ((viewport2 != null) && (viewport2.Parent != null))
                    {
                        if (viewport2.RenderTransform != null)
                        {
                            viewport2.RenderTransform = null;
                        }
                        if ((viewport2.Width != width) || (viewport2.Height != height))
                        {
                            if (!base.IsTouching)
                            {
                                viewport2.Arrange(new Rect(headerX, headerY, width, height));
                            }
                            else
                            {
                                int num9 = (int)Math.Ceiling(base._translateOffsetX);
                                double x = headerX;
                                if ((base._touchStartHitTestInfo != null) && (j == base._touchStartHitTestInfo.ColumnViewportIndex))
                                {
                                    x += num9;
                                }
                                viewport2.Arrange(new Rect(x, headerY, width, height));
                                if ((x != headerX) && (base._translateOffsetX < 0.0))
                                {
                                    RectangleGeometry geometry = new RectangleGeometry();
                                    geometry.Rect = new Rect(Math.Abs((double)(headerX - x)), 0.0, base._cachedViewportWidths[j + 1], height);
                                    viewport2.Clip = geometry;
                                }
                                else if ((x != headerX) && (base._translateOffsetX > 0.0))
                                {
                                    RectangleGeometry geometry2 = new RectangleGeometry();
                                    geometry2.Rect = new Rect(0.0, 0.0, Math.Max((double)0.0, (double)(base._cachedViewportWidths[j + 1] - base._translateOffsetX)), height);
                                    viewport2.Clip = geometry2;
                                }
                                else
                                {
                                    viewport2.Clip = null;
                                }
                            }
                        }
                    }
                }
            }
            if (base.IsTouchZooming && (base._cachedRowHeaderViewportTransform != null))
            {
                for (int k = -1; k <= spreadLayout.RowPaneCount; k++)
                {
                    headerX = spreadLayout.HeaderX;
                    headerY = spreadLayout.GetViewportY(k);
                    double headerWidth = spreadLayout.HeaderWidth;
                    double viewportHeight = spreadLayout.GetViewportHeight(k);
                    GcViewport viewport3 = base._rowHeaderPresenters[k + 1];
                    if ((viewport3 != null) && (viewport3.Parent != null))
                    {
                        viewport3.Arrange(new Rect(headerX, headerY, headerWidth, viewportHeight));
                        viewport3.RenderTransform = base._cachedRowHeaderViewportTransform[k + 1];
                    }
                }
            }
            else if (base._rowHeaderPresenters != null)
            {
                for (int m = -1; m <= spreadLayout.RowPaneCount; m++)
                {
                    headerX = spreadLayout.HeaderX;
                    headerY = spreadLayout.GetViewportY(m);
                    if (((base.IsTouching && base.IsTouching) && ((m == spreadLayout.RowPaneCount) && (base._translateOffsetY < 0.0))) && (base._touchStartHitTestInfo.RowViewportIndex == (spreadLayout.RowPaneCount - 1)))
                    {
                        headerY += base._translateOffsetY;
                    }
                    double num15 = spreadLayout.HeaderWidth;
                    double num16 = spreadLayout.GetViewportHeight(m);
                    GcViewport viewport4 = base._rowHeaderPresenters[m + 1];
                    if ((viewport4 != null) && (viewport4.Parent != null))
                    {
                        if (viewport4.RenderTransform != null)
                        {
                            viewport4.RenderTransform = null;
                        }
                        if ((viewport4.Width != num15) || (viewport4.Height != num16))
                        {
                            if (!base.IsTouching)
                            {
                                viewport4.Arrange(new Rect(headerX, headerY, num15, num16));
                            }
                            else
                            {
                                int num17 = (int)Math.Ceiling(base._translateOffsetY);
                                double y = headerY;
                                if ((base._touchStartHitTestInfo != null) && (m == base._touchStartHitTestInfo.RowViewportIndex))
                                {
                                    y += num17;
                                }
                                viewport4.Arrange(new Rect(headerX, y, num15, num16));
                                if ((y != headerY) && (base._translateOffsetY < 0.0))
                                {
                                    RectangleGeometry geometry3 = new RectangleGeometry();
                                    geometry3.Rect = new Rect(0.0, Math.Abs((double)(headerY - y)), num15, base._cachedViewportHeights[m + 1]);
                                    viewport4.Clip = geometry3;
                                }
                                else if ((y != headerY) && (base._translateOffsetY > 0.0))
                                {
                                    RectangleGeometry geometry4 = new RectangleGeometry();
                                    geometry4.Rect = new Rect(0.0, 0.0, num15, Math.Max((double)0.0, (double)(base._cachedViewportHeights[m + 1] - base._translateOffsetY)));
                                    viewport4.Clip = geometry4;
                                }
                                else
                                {
                                    viewport4.Clip = null;
                                }
                            }
                        }
                    }
                }
            }
            if (base.IsTouchZooming && (base._cachedViewportTransform != null))
            {
                for (int n = -1; n <= spreadLayout.ColumnPaneCount; n++)
                {
                    headerX = spreadLayout.GetViewportX(n);
                    double num20 = spreadLayout.GetViewportWidth(n);
                    for (int num21 = -1; num21 <= spreadLayout.RowPaneCount; num21++)
                    {
                        headerY = spreadLayout.GetViewportY(num21);
                        double num22 = spreadLayout.GetViewportHeight(num21);
                        GcViewport viewport5 = base._viewportPresenters[num21 + 1, n + 1];
                        if (viewport5 != null)
                        {
                            viewport5.Arrange(new Rect(headerX, headerY, num20, num22));
                            viewport5.RenderTransform = base._cachedViewportTransform[num21 + 1, n + 1];
                        }
                    }
                }
            }
            else if (base._viewportPresenters != null)
            {
                for (int num23 = -1; num23 <= spreadLayout.ColumnPaneCount; num23++)
                {
                    headerX = spreadLayout.GetViewportX(num23);
                    if ((base.IsTouching && (num23 == spreadLayout.ColumnPaneCount)) && ((base._translateOffsetX < 0.0) && (base._touchStartHitTestInfo.ColumnViewportIndex == (spreadLayout.ColumnPaneCount - 1))))
                    {
                        headerX += base._translateOffsetX;
                    }
                    double num24 = spreadLayout.GetViewportWidth(num23);
                    for (int num25 = -1; num25 <= spreadLayout.RowPaneCount; num25++)
                    {
                        headerY = spreadLayout.GetViewportY(num25);
                        if (((base.IsTouching && base.IsTouching) && ((num25 == spreadLayout.RowPaneCount) && (base._translateOffsetY < 0.0))) && (base._touchStartHitTestInfo.RowViewportIndex == (spreadLayout.RowPaneCount - 1)))
                        {
                            headerY += base._translateOffsetY;
                        }
                        double num26 = spreadLayout.GetViewportHeight(num25);
                        GcViewport viewport6 = base._viewportPresenters[num25 + 1, num23 + 1];
                        if (viewport6 != null)
                        {
                            if (viewport6.RenderTransform != null)
                            {
                                viewport6.RenderTransform = null;
                            }
                            if ((viewport6.Width != num24) || (viewport6.Height != num26))
                            {
                                if (!base.IsTouching)
                                {
                                    viewport6.Arrange(new Rect(headerX, headerY, num24, num26));
                                }
                                else
                                {
                                    int num27 = (int)Math.Ceiling(base._translateOffsetX);
                                    int num28 = (int)Math.Ceiling(base._translateOffsetY);
                                    double num29 = headerX;
                                    double num30 = headerY;
                                    if ((base._touchStartHitTestInfo != null) && (num23 == base._touchStartHitTestInfo.ColumnViewportIndex))
                                    {
                                        num29 += num27;
                                    }
                                    if ((base._touchStartHitTestInfo != null) && (num25 == base._touchStartHitTestInfo.RowViewportIndex))
                                    {
                                        num30 += num28;
                                    }
                                    viewport6.Arrange(new Rect(num29, num30, num24, num26));
                                    if (((headerY != num30) && (base._translateOffsetY < 0.0)) || ((headerX != num29) && (base._translateOffsetX < 0.0)))
                                    {
                                        RectangleGeometry geometry5 = new RectangleGeometry();
                                        geometry5.Rect = new Rect(Math.Abs((double)(headerX - num29)), Math.Abs((double)(headerY - num30)), base._cachedViewportWidths[num23 + 1], base._cachedViewportHeights[num25 + 1]);
                                        viewport6.Clip = geometry5;
                                    }
                                    else if ((headerX != num29) && (base._translateOffsetX > 0.0))
                                    {
                                        RectangleGeometry geometry6 = new RectangleGeometry();
                                        geometry6.Rect = new Rect(0.0, 0.0, Math.Max((double)0.0, (double)(base._cachedViewportWidths[num23 + 1] - base._translateOffsetX)), base._cachedViewportHeights[num25 + 1]);
                                        viewport6.Clip = geometry6;
                                    }
                                    else if ((headerY != num30) && (base._translateOffsetY > 0.0))
                                    {
                                        RectangleGeometry geometry7 = new RectangleGeometry();
                                        geometry7.Rect = new Rect(0.0, 0.0, base._cachedViewportWidths[num23 + 1], Math.Max((double)0.0, (double)(base._cachedViewportHeights[num25 + 1] - base._translateOffsetY)));
                                        viewport6.Clip = geometry7;
                                    }
                                    else
                                    {
                                        RectangleGeometry geometry8 = new RectangleGeometry();
                                        geometry8.Rect = new Rect(0.0, 0.0, num24, num26);
                                        viewport6.Clip = geometry8;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (this._horizontalScrollBar != null)
            {
                for (int num31 = 0; num31 < spreadLayout.ColumnPaneCount; num31++)
                {
                    double horizontalScrollBarX = spreadLayout.GetHorizontalScrollBarX(num31);
                    double ornamentY = spreadLayout.OrnamentY;
                    double horizontalScrollBarWidth = spreadLayout.GetHorizontalScrollBarWidth(num31);
                    double ornamentHeight = spreadLayout.OrnamentHeight;
                    horizontalScrollBarWidth = Math.Max(horizontalScrollBarWidth, 0.0);
                    ornamentHeight = Math.Max(ornamentHeight, 0.0);
                    this._horizontalScrollBar[num31].Width = horizontalScrollBarWidth;
                    this._horizontalScrollBar[num31].Height = ornamentHeight;
                    this._horizontalScrollBar[num31].Arrange(new Rect(horizontalScrollBarX, ornamentY, horizontalScrollBarWidth, ornamentHeight));
                }
            }
            if (this._tabStrip != null)
            {
                double tabStripX = spreadLayout.TabStripX;
                double tabStripY = spreadLayout.TabStripY;
                double tabStripWidth = spreadLayout.TabStripWidth;
                double tabStripHeight = spreadLayout.TabStripHeight;
                this._tabStrip.Arrange(new Rect(tabStripX, tabStripY, tabStripWidth, tabStripHeight));
            }
            if (this.tabStripSplitBox != null)
            {
                double tabSplitBoxX = spreadLayout.TabSplitBoxX;
                double num41 = spreadLayout.OrnamentY;
                double tabSplitBoxWidth = spreadLayout.TabSplitBoxWidth;
                double num43 = spreadLayout.OrnamentHeight;
                this.tabStripSplitBox.Arrange(new Rect(tabSplitBoxX, num41, tabSplitBoxWidth, num43));
            }
            if (this._horizontalSplitBox != null)
            {
                for (int num44 = 0; num44 < spreadLayout.ColumnPaneCount; num44++)
                {
                    double horizontalSplitBoxX = spreadLayout.GetHorizontalSplitBoxX(num44);
                    double num46 = spreadLayout.OrnamentY;
                    double horizontalSplitBoxWidth = spreadLayout.GetHorizontalSplitBoxWidth(num44);
                    double num48 = spreadLayout.OrnamentHeight;
                    this._horizontalSplitBox[num44].Arrange(new Rect(horizontalSplitBoxX, num46, horizontalSplitBoxWidth, num48));
                }
            }
            if (this._verticalScrollBar != null)
            {
                for (int num49 = 0; num49 < spreadLayout.RowPaneCount; num49++)
                {
                    double ornamentX = spreadLayout.OrnamentX;
                    double verticalScrollBarY = spreadLayout.GetVerticalScrollBarY(num49);
                    double ornamentWidth = spreadLayout.OrnamentWidth;
                    double num53 = spreadLayout.GetViewportHeight(num49) - spreadLayout.GetVerticalSplitBoxHeight(num49);
                    if (((base.IsTouching && (base._touchStartHitTestInfo != null)) && ((num49 == base._touchStartHitTestInfo.RowViewportIndex) && !this.IsZero(base._translateOffsetY))) && ((base._touchStartHitTestInfo != null) && (base._touchStartHitTestInfo.HitTestType == HitTestType.Viewport)))
                    {
                        num53 = base._cachedViewportHeights[num49 + 1] - spreadLayout.GetVerticalSplitBoxHeight(num49);
                    }
                    if (num49 == 0)
                    {
                        num53 += (spreadLayout.HeaderY + spreadLayout.HeaderHeight) + spreadLayout.FrozenHeight;
                    }
                    if (num49 == (spreadLayout.RowPaneCount - 1))
                    {
                        num53 += spreadLayout.FrozenTrailingHeight;
                    }
                    ornamentWidth = Math.Max(ornamentWidth, 0.0);
                    num53 = Math.Max(num53, 0.0);
                    this._verticalScrollBar[num49].Width = ornamentWidth;
                    this._verticalScrollBar[num49].Height = num53;
                    this._verticalScrollBar[num49].Arrange(new Rect(ornamentX, verticalScrollBarY, ornamentWidth, num53));
                }
            }
            if (this._verticalSplitBox != null)
            {
                for (int num54 = 0; num54 < spreadLayout.RowPaneCount; num54++)
                {
                    double num55 = spreadLayout.OrnamentX;
                    double verticalSplitBoxY = spreadLayout.GetVerticalSplitBoxY(num54);
                    double num57 = spreadLayout.OrnamentWidth;
                    double verticalSplitBoxHeight = spreadLayout.GetVerticalSplitBoxHeight(num54);
                    this._verticalSplitBox[num54].Arrange(new Rect(num55, verticalSplitBoxY, num57, verticalSplitBoxHeight));
                }
            }
            if (this._horizontalSplitBar != null)
            {
                for (int num59 = 0; num59 < (spreadLayout.ColumnPaneCount - 1); num59++)
                {
                    if ((this._horizontalSplitBar[num59] != null) && (this._horizontalSplitBar[num59].Parent != null))
                    {
                        double horizontalSplitBarX = spreadLayout.GetHorizontalSplitBarX(num59);
                        if (base.IsTouching && (base._cachedViewportSplitBarX != null))
                        {
                            horizontalSplitBarX = base._cachedViewportSplitBarX[num59];
                        }
                        double num61 = spreadLayout.Y;
                        double horizontalSplitBarWidth = spreadLayout.GetHorizontalSplitBarWidth(num59);
                        double num63 = base.AvailableSize.Height;
                        this._horizontalSplitBar[num59].Arrange(new Rect(horizontalSplitBarX, num61, horizontalSplitBarWidth, num63));
                    }
                }
            }
            if (this._verticalSplitBar != null)
            {
                for (int num64 = 0; num64 < (spreadLayout.RowPaneCount - 1); num64++)
                {
                    if ((this._verticalSplitBar[num64] != null) && (this._verticalSplitBar[num64].Parent != null))
                    {
                        double num65 = spreadLayout.X;
                        double verticalSplitBarY = spreadLayout.GetVerticalSplitBarY(num64);
                        if (base.IsTouching && (base._cachedViewportSplitBarY != null))
                        {
                            verticalSplitBarY = base._cachedViewportSplitBarY[num64];
                        }
                        double num67 = base.AvailableSize.Width;
                        double verticalSplitBarHeight = spreadLayout.GetVerticalSplitBarHeight(num64);
                        this._verticalSplitBar[num64].Arrange(new Rect(num65, verticalSplitBarY, num67, verticalSplitBarHeight));
                    }
                }
            }
            if (this._crossSplitBar != null)
            {
                for (int num69 = 0; num69 < this._crossSplitBar.GetLength(0); num69++)
                {
                    double num70 = spreadLayout.GetVerticalSplitBarY(num69);
                    if (base.IsTouching && (base._cachedViewportSplitBarY != null))
                    {
                        num70 = base._cachedViewportSplitBarY[num69];
                    }
                    double num71 = spreadLayout.GetVerticalSplitBarHeight(num69);
                    for (int num72 = 0; num72 < this._crossSplitBar.GetLength(1); num72++)
                    {
                        double num73 = spreadLayout.GetHorizontalSplitBarX(num72);
                        if (base.IsTouching && (base._cachedViewportSplitBarX != null))
                        {
                            num73 = base._cachedViewportSplitBarX[num72];
                        }
                        double num74 = spreadLayout.GetHorizontalSplitBarWidth(num72);
                        if ((this._crossSplitBar[num69, num72] != null) && (this._crossSplitBar[num69, num72].Parent != null))
                        {
                            this._crossSplitBar[num69, num72].Arrange(new Rect(num73, num70, num74, num71));
                        }
                    }
                }
            }
            base.ArrangeRangeGroup(spreadLayout.RowPaneCount, spreadLayout.ColumnPaneCount, spreadLayout);
            Rect rect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
            if (base._rowFreezeLine != null)
            {
                base._rowFreezeLine.Arrange(rect);
            }
            if (base._rowTrailingFreezeLine != null)
            {
                base._rowTrailingFreezeLine.Arrange(rect);
            }
            if (base._columnFreezeLine != null)
            {
                base._columnFreezeLine.Arrange(rect);
            }
            if (base._columnTrailingFreezeLine != null)
            {
                base._columnTrailingFreezeLine.Arrange(rect);
            }
            RectangleGeometry geometry9 = new RectangleGeometry();
            geometry9.Rect = rect;
            base.Clip = geometry9;
            if (base._formulaSelectionGripperPanel != null)
            {
                base._formulaSelectionGripperPanel.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            }
            base.UpdateTouchSelectionGripper();
            this._progressGrid.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void ClearMouseLeftButtonDownStates()
        {
            if (this.IsColumnSplitting)
            {
                this.EndColumnSplitting();
            }
            if (this.IsRowSplitting)
            {
                this.EndRowSplitting();
            }
            if (this.IsTabStripResizing)
            {
                this.EndTabStripResizing();
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
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            Worksheet activeSheet = this.ActiveSheet;
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
                return this.CreateViewportColumnLayoutModel(columnViewportIndex);
            }
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            ColumnLayoutModel model = new ColumnLayoutModel();
            Worksheet activeSheet = this.ActiveSheet;
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
                    if ((base.IsTouching && (this.Worksheet.FrozenTrailingColumnCount > 0)) && ((base._touchStartHitTestInfo.ColumnViewportIndex == (spreadLayout.ColumnPaneCount - 1)) && (base._translateOffsetX < 0.0)))
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
                return this.CreateViewportRowLayoutModel(rowViewportIndex);
            }
            RowLayoutModel model = new RowLayoutModel();
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            Worksheet activeSheet = this.ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = base.ZoomFactor;
                if (rowViewportIndex == -1)
                {
                    double y = spreadLayout.HeaderY + spreadLayout.HeaderHeight;
                    int frozenRowCount = this.Worksheet.FrozenRowCount;
                    if (this.Worksheet.RowCount < frozenRowCount)
                    {
                        frozenRowCount = this.Worksheet.RowCount;
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
                    if ((base.IsTouching && (this.Worksheet.FrozenTrailingColumnCount > 0)) && ((base._touchStartHitTestInfo.RowViewportIndex == (spreadLayout.RowPaneCount - 1)) && (base._translateOffsetY < 0.0)))
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
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            Worksheet activeSheet = this.ActiveSheet;
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
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            ColumnLayoutModel model = new ColumnLayoutModel();
            Worksheet activeSheet = this.ActiveSheet;
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
                    if ((base.IsTouching && (this.Worksheet.FrozenTrailingColumnCount > 0)) && ((base._touchStartHitTestInfo.ColumnViewportIndex == (spreadLayout.ColumnPaneCount - 1)) && (base._translateOffsetX < 0.0)))
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
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            Worksheet activeSheet = this.ActiveSheet;
            if (activeSheet != null)
            {
                float zoomFactor = base.ZoomFactor;
                if (rowViewportIndex == -1)
                {
                    double y = spreadLayout.HeaderY + spreadLayout.HeaderHeight;
                    int frozenRowCount = this.Worksheet.FrozenRowCount;
                    if (this.Worksheet.RowCount < frozenRowCount)
                    {
                        frozenRowCount = this.Worksheet.RowCount;
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
                    if ((base.IsTouching && (this.Worksheet.FrozenTrailingColumnCount > 0)) && ((base._touchStartHitTestInfo.RowViewportIndex == (spreadLayout.RowPaneCount - 1)) && (base._translateOffsetY < 0.0)))
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
        /// Measures the layout size required for child elements and determines a size for the FrameworkElement-derived class, when overridden in a derived class. 
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element sizes to whatever content is available.
        /// </param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes. 
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double viewportX;
            double viewportY;
            if (this._cachedLastAvailableSize != availableSize)
            {
                this._cachedLastAvailableSize = availableSize;
                base.AvailableSize = availableSize;
                this.InvalidateLayout();
            }
            if (!base.IsWorking)
            {
                base.SaveHitTestInfo(null);
            }
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            List<Image> list = new List<Image>();
            foreach (var element in base.Children.OfType<Image>())
            {
                list.Add(element);
            }
            foreach (var element2 in list)
            {
                base.Children.Remove(element2);
            }
            this.UpdateHorizontalSplitBoxes();
            this.UpdateVerticalSplitBoxes();
            this.UpdateHorizontalSplitBars();
            this.UpdateVerticalSplitBars();
            this.UpdateCrossSplitBars();
            base.UpdateFreezeLines();
            if (!base.Children.Contains(base.TrackersContainer))
            {
                base.Children.Add(base.TrackersContainer);
            }
            base.TrackersContainer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (!base.Children.Contains(this.SplittingTrackerContainer))
            {
                base.Children.Add(this.SplittingTrackerContainer);
            }
            this.SplittingTrackerContainer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (!base.Children.Contains(base.CursorsContainer))
            {
                base.Children.Add(base.CursorsContainer);
            }
            base.CursorsContainer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            GcViewport[,] viewportArray = null;
            if ((base._viewportPresenters != null) && (((this.ActiveSheet == null) || (base._viewportPresenters.GetUpperBound(0) != (spreadLayout.RowPaneCount + 1))) || (base._viewportPresenters.GetUpperBound(1) != (spreadLayout.ColumnPaneCount + 1))))
            {
                GcViewport[,] viewportArray2 = base._viewportPresenters;
                int upperBound = viewportArray2.GetUpperBound(0);
                int num29 = viewportArray2.GetUpperBound(1);
                for (int n = viewportArray2.GetLowerBound(0); n <= upperBound; n++)
                {
                    for (int num31 = viewportArray2.GetLowerBound(1); num31 <= num29; num31++)
                    {
                        GcViewport viewport = viewportArray2[n, num31];
                        if (viewport != null)
                        {
                            viewport.RemoveDataValidationUI();
                        }
                        base.Children.Remove(viewport);
                    }
                }
                viewportArray = base._viewportPresenters;
                base._viewportPresenters = null;
            }
            if (base._viewportPresenters == null)
            {
                base._viewportPresenters = new GcViewport[spreadLayout.RowPaneCount + 2, spreadLayout.ColumnPaneCount + 2];
            }
            for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
            {
                double viewportWidth = spreadLayout.GetViewportWidth(i);
                viewportX = spreadLayout.GetViewportX(i);
                for (int num5 = -1; num5 <= spreadLayout.RowPaneCount; num5++)
                {
                    double viewportHeight = spreadLayout.GetViewportHeight(num5);
                    viewportY = spreadLayout.GetViewportY(num5);
                    if (((base._viewportPresenters[num5 + 1, i + 1] == null) && (viewportWidth > 0.0)) && (viewportHeight > 0.0))
                    {
                        if (((viewportArray != null) && ((num5 + 1) < viewportArray.GetUpperBound(0))) && (((i + 1) < viewportArray.GetUpperBound(1)) && (viewportArray[num5 + 1, i + 1] != null)))
                        {
                            base._viewportPresenters[num5 + 1, i + 1] = viewportArray[num5 + 1, i + 1];
                        }
                        else
                        {
                            base._viewportPresenters[num5 + 1, i + 1] = new GcViewport(this);
                        }
                    }
                    GcViewport viewport2 = base._viewportPresenters[num5 + 1, i + 1];
                    if ((viewportWidth > 0.0) && (viewportHeight > 0.0))
                    {
                        viewport2.Location = new Point(viewportX, viewportY);
                        viewport2.ColumnViewportIndex = i;
                        viewport2.RowViewportIndex = num5;
                        if (!base.Children.Contains(viewport2))
                        {
                            base.Children.Add(viewport2);
                        }
                        viewport2.InvalidateMeasure();
                        viewport2.Measure(new Size(viewportWidth, viewportHeight));
                    }
                    else if (viewport2 != null)
                    {
                        base.Children.Remove(viewport2);
                        base._viewportPresenters[num5 + 1, i + 1] = null;
                    }
                }
            }
            if ((base._rowHeaderPresenters != null) && ((this.ActiveSheet == null) || (base._rowHeaderPresenters.Length != (spreadLayout.RowPaneCount + 2))))
            {
                foreach (GcViewport viewport3 in base._rowHeaderPresenters)
                {
                    base.Children.Remove(viewport3);
                }
                base._rowHeaderPresenters = null;
            }
            if (base._rowHeaderPresenters == null)
            {
                base._rowHeaderPresenters = new GcRowHeaderViewport[spreadLayout.RowPaneCount + 2];
            }
            if (spreadLayout.HeaderWidth > 0.0)
            {
                for (int num7 = -1; num7 <= spreadLayout.RowPaneCount; num7++)
                {
                    double height = spreadLayout.GetViewportHeight(num7);
                    viewportY = spreadLayout.GetViewportY(num7);
                    if ((base._rowHeaderPresenters[num7 + 1] == null) && (height > 0.0))
                    {
                        base._rowHeaderPresenters[num7 + 1] = new GcRowHeaderViewport(this);
                    }
                    GcViewport viewport4 = base._rowHeaderPresenters[num7 + 1];
                    if (height > 0.0)
                    {
                        viewport4.Location = new Point(spreadLayout.HeaderX, viewportY);
                        viewport4.RowViewportIndex = num7;
                        if (!base.Children.Contains(viewport4))
                        {
                            base.Children.Add(viewport4);
                        }
                        viewport4.InvalidateMeasure();
                        viewport4.Measure(new Size(spreadLayout.HeaderWidth, height));
                    }
                    else if (viewport4 != null)
                    {
                        base.Children.Remove(viewport4);
                        base._rowHeaderPresenters[num7 + 1] = null;
                    }
                }
            }
            else if (base._rowHeaderPresenters != null)
            {
                foreach (GcViewport viewport5 in base._rowHeaderPresenters)
                {
                    base.Children.Remove(viewport5);
                }
            }
            if ((base._columnHeaderPresenters != null) && ((this.ActiveSheet == null) || (base._columnHeaderPresenters.Length != (spreadLayout.ColumnPaneCount + 2))))
            {
                foreach (GcViewport viewport6 in base._columnHeaderPresenters)
                {
                    base.Children.Remove(viewport6);
                }
                base._columnHeaderPresenters = null;
            }
            if (base._columnHeaderPresenters == null)
            {
                base._columnHeaderPresenters = new GcColumnHeaderViewport[spreadLayout.ColumnPaneCount + 2];
            }
            if (spreadLayout.HeaderHeight > 0.0)
            {
                for (int num9 = -1; num9 <= spreadLayout.ColumnPaneCount; num9++)
                {
                    viewportX = spreadLayout.GetViewportX(num9);
                    double width = spreadLayout.GetViewportWidth(num9);
                    if ((base._columnHeaderPresenters[num9 + 1] == null) && (width > 0.0))
                    {
                        base._columnHeaderPresenters[num9 + 1] = new GcColumnHeaderViewport(this);
                    }
                    GcViewport viewport7 = base._columnHeaderPresenters[num9 + 1];
                    if (width > 0.0)
                    {
                        viewport7.Location = new Point(viewportX, spreadLayout.HeaderY);
                        viewport7.ColumnViewportIndex = num9;
                        if (!base.Children.Contains(viewport7))
                        {
                            base.Children.Add(viewport7);
                        }
                        viewport7.InvalidateMeasure();
                        viewport7.Measure(new Size(width, spreadLayout.HeaderHeight));
                    }
                    else if (viewport7 != null)
                    {
                        base.Children.Remove(viewport7);
                        base._columnHeaderPresenters[num9 + 1] = null;
                    }
                }
            }
            else if (base._columnHeaderPresenters != null)
            {
                foreach (GcViewport viewport8 in base._columnHeaderPresenters)
                {
                    base.Children.Remove(viewport8);
                }
            }
            if (base._cornerPresenter == null)
            {
                base._cornerPresenter = new GcHeaderCornerViewport(this);
            }
            base._cornerPresenter.Location = new Point(spreadLayout.HeaderX, spreadLayout.HeaderY);
            if ((spreadLayout.HeaderWidth > 0.0) && (spreadLayout.HeaderHeight > 0.0))
            {
                if (!base.Children.Contains(base._cornerPresenter))
                {
                    base.Children.Add(base._cornerPresenter);
                }
                base._cornerPresenter.InvalidateMeasure();
                base._cornerPresenter.Measure(new Size(spreadLayout.HeaderWidth, spreadLayout.HeaderHeight));
            }
            else
            {
                base.Children.Remove(base._cornerPresenter);
                base._cornerPresenter = null;
            }
            if (spreadLayout.OrnamentHeight > 0.0)
            {
                for (int num11 = 0; num11 < spreadLayout.ColumnPaneCount; num11++)
                {
                    ScrollBar bar = this._horizontalScrollBar[num11];
                    if (spreadLayout.GetHorizontalScrollBarWidth(num11) > 0.0)
                    {
                        if (!base.Children.Contains(bar))
                        {
                            base.Children.Add(bar);
                        }
                    }
                    else
                    {
                        base.Children.Remove(bar);
                    }
                    HorizontalSplitBox box = this._horizontalSplitBox[num11];
                    if (spreadLayout.GetHorizontalSplitBoxWidth(num11) > 0.0)
                    {
                        if (!base.Children.Contains(box))
                        {
                            base.Children.Add(box);
                        }
                    }
                    else
                    {
                        base.Children.Remove(box);
                    }
                }
            }
            else
            {
                if (this._horizontalScrollBar != null)
                {
                    foreach (ScrollBar bar2 in this._horizontalScrollBar)
                    {
                        base.Children.Remove(bar2);
                    }
                }
                if (this._horizontalSplitBox != null)
                {
                    foreach (HorizontalSplitBox box2 in this._horizontalSplitBox)
                    {
                        base.Children.Remove(box2);
                    }
                }
            }
            if (spreadLayout.TabStripHeight > 0.0)
            {
                if (this._tabStrip == null)
                {
                    this._tabStrip = new TabStrip();
                    // hdt 应用构造前的设置
                    if (_tabStripVisibility == Visibility.Collapsed)
                        _tabStrip.Visibility = Visibility.Collapsed;
                    this._tabStrip.HasInsertTab = this.SpreadSheet.TabStripInsertTab;
                    this._tabStrip.OwningView = this;
                    Canvas.SetZIndex(this._tabStrip, 0x62);
                }
                else
                {
                    this._tabStrip.Update();
                }
                this._tabStrip.ActiveTabChanging -= new EventHandler(this.OnTabStripActiveTabChanging);
                this._tabStrip.ActiveTabChanged -= new EventHandler(this.OnTabStripActiveTabChanged);
                this._tabStrip.NewTabNeeded -= new EventHandler(this.OnTabStripNewTabNeeded);
                this._tabStrip.AddSheets(this.SpreadSheet.Sheets);
                if (!base.Children.Contains(this._tabStrip))
                {
                    base.Children.Add(this._tabStrip);
                }
                int activeSheetIndex = this.SpreadSheet.ActiveSheetIndex;
                if ((activeSheetIndex >= 0) && (activeSheetIndex < this.SpreadSheet.Sheets.Count))
                {
                    this._tabStrip.ActiveSheet(activeSheetIndex, false);
                }
                this._tabStrip.SetStartSheet(this.SpreadSheet.StartSheetIndex);
                this._tabStrip.InvalidateMeasure();
                this._tabStrip.Measure(new Size(spreadLayout.TabStripWidth, spreadLayout.TabStripHeight));
                this._tabStrip.ActiveTabChanging += new EventHandler(this.OnTabStripActiveTabChanging);
                this._tabStrip.ActiveTabChanged += new EventHandler(this.OnTabStripActiveTabChanged);
                this._tabStrip.NewTabNeeded += new EventHandler(this.OnTabStripNewTabNeeded);
                if (this.tabStripSplitBox == null)
                {
                    this.tabStripSplitBox = new TabStripSplitBox();
                    Canvas.SetZIndex(this.tabStripSplitBox, 0x62);
                }
                if (!base.Children.Contains(this.tabStripSplitBox))
                {
                    base.Children.Add(this.tabStripSplitBox);
                }
                this.tabStripSplitBox.InvalidateMeasure();
                this.tabStripSplitBox.Measure(new Size(spreadLayout.TabSplitBoxWidth, spreadLayout.OrnamentHeight));
            }
            else if (this._tabStrip != null)
            {
                base.Children.Remove(this._tabStrip);
                this._tabStrip.ActiveTabChanging -= new EventHandler(this.OnTabStripActiveTabChanging);
                this._tabStrip.ActiveTabChanged -= new EventHandler(this.OnTabStripActiveTabChanged);
                this._tabStrip = null;
                base.Children.Remove(this.tabStripSplitBox);
                this.tabStripSplitBox = null;
            }
            if (spreadLayout.OrnamentWidth > 0.0)
            {
                for (int num15 = 0; num15 < spreadLayout.RowPaneCount; num15++)
                {
                    ScrollBar bar3 = this._verticalScrollBar[num15];
                    if (this.GetSpreadLayout().GetVerticalScrollBarHeight(num15) > 0.0)
                    {
                        if (!base.Children.Contains(bar3))
                        {
                            base.Children.Add(bar3);
                        }
                    }
                    else
                    {
                        base.Children.Remove(bar3);
                    }
                    VerticalSplitBox box3 = this._verticalSplitBox[num15];
                    if (spreadLayout.GetVerticalSplitBoxHeight(num15) > 0.0)
                    {
                        if (!base.Children.Contains(box3))
                        {
                            base.Children.Add(box3);
                        }
                    }
                    else
                    {
                        base.Children.Remove(box3);
                    }
                }
            }
            else
            {
                if (this._verticalScrollBar != null)
                {
                    foreach (ScrollBar bar4 in this._verticalScrollBar)
                    {
                        base.Children.Remove(bar4);
                    }
                }
                if (this._verticalSplitBox != null)
                {
                    foreach (VerticalSplitBox box4 in this._verticalSplitBox)
                    {
                        base.Children.Remove(box4);
                    }
                }
            }
            for (int j = 0; j < (spreadLayout.ColumnPaneCount - 1); j++)
            {
                HorizontalSplitBar bar5 = this._horizontalSplitBar[j];
                double horizontalSplitBoxWidth = spreadLayout.GetHorizontalSplitBoxWidth(j);
                double num20 = base.AvailableSize.Height;
                if (!base.Children.Contains(bar5))
                {
                    base.Children.Add(bar5);
                }
                bar5.Measure(new Size(horizontalSplitBoxWidth, num20));
            }
            for (int k = 0; k < (spreadLayout.RowPaneCount - 1); k++)
            {
                VerticalSplitBar bar6 = this._verticalSplitBar[k];
                double num22 = base.AvailableSize.Width;
                double verticalSplitBarHeight = spreadLayout.GetVerticalSplitBarHeight(k);
                if (!base.Children.Contains(bar6))
                {
                    base.Children.Add(bar6);
                }
                bar6.Measure(new Size(num22, verticalSplitBarHeight));
            }
            for (int m = 0; m < (spreadLayout.RowPaneCount - 1); m++)
            {
                for (int num25 = 0; num25 < (spreadLayout.ColumnPaneCount - 1); num25++)
                {
                    CrossSplitBar bar7 = this._crossSplitBar[m, num25];
                    double num26 = spreadLayout.GetHorizontalSplitBoxWidth(num25);
                    double num27 = spreadLayout.GetVerticalSplitBarHeight(m);
                    if (!base.Children.Contains(bar7))
                    {
                        base.Children.Add(bar7);
                    }
                    bar7.Measure(new Size(num26, num27));
                }
            }
            base.MeasureRangeGroup(spreadLayout.RowPaneCount, spreadLayout.ColumnPaneCount, spreadLayout);
            if (!base.Children.Contains(this._progressGrid))
            {
                base.Children.Add(this._progressGrid);
            }
            this._progressGrid.Measure(availableSize);
            base.Children.Remove(base._topLeftGripper);
            base.Children.Remove(base._bottomRightGripper);
            base.Children.Remove(base._resizerGripperContainer);
            base.Children.Remove(base._autoFillIndicatorContainer);
            if (base._formulaSelectionGripperPanel != null)
            {
                base.Children.Remove(base._formulaSelectionGripperPanel);
            }
            base._topLeftGripper.Stroke = new SolidColorBrush(base.GetGripperStrokeColor());
            base._topLeftGripper.Fill = new SolidColorBrush(base.GetGripperFillColor());
            base._bottomRightGripper.Stroke = new SolidColorBrush(base.GetGripperStrokeColor());
            base._bottomRightGripper.Fill = new SolidColorBrush(base.GetGripperFillColor());
            base.Children.Add(base._topLeftGripper);
            base.Children.Add(base._bottomRightGripper);
            base.Children.Add(base._resizerGripperContainer);
            base._autoFillIndicatorContainer.Width = 16.0;
            base._autoFillIndicatorContainer.Height = 16.0;
            base.Children.Add(base._autoFillIndicatorContainer);
            if (base._formulaSelectionGripperPanel != null)
            {
                base.Children.Add(base._formulaSelectionGripperPanel);
            }
            base._cachedColumnResizerGripperImage.Source = base.GetResizerBitmapImage(false);
            base._cachedRowResizerGripperImage.Source = base.GetResizerBitmapImage(true);
            base._cachedautoFillIndicatorImage.Source = base.GetImageSource("AutoFillIndicator.png");
            Canvas.SetZIndex(base._topLeftGripper, 90);
            Canvas.SetZIndex(base._bottomRightGripper, 90);
            Canvas.SetZIndex(base._resizerGripperContainer, 90);
            Canvas.SetZIndex(base._autoFillIndicatorContainer, 90);
            if (base._formulaSelectionGripperPanel != null)
            {
                Canvas.SetZIndex(base._formulaSelectionGripperPanel, 90);
            }
            if (base._formulaSelectionGripperPanel != null)
            {
                base._formulaSelectionGripperPanel.Measure(new Size(double.MaxValue, double.MaxValue));
            }
            return base.AvailableSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPoint">currentPoint</param>
        internal override void OnManipulationComplete(Point currentPoint)
        {
            base.IsContinueTouchOperation = false;
            base.CachedGripperLocation = null;
            this.ClearViewportsClip();
            this.UpdateViewport();
            if (this.IsTouchTabStripScrolling)
            {
                this.IsTouchTabStripScrolling = false;
                this.TabStrip.TabsPresenter.Offset = 0.0;
                this.TabStrip.TabsPresenter.InvalidateMeasure();
                this.TabStrip.TabsPresenter.InvalidateArrange();
            }
            if (base.IsTouchZooming)
            {
                base.IsTouchZooming = false;
                base._cachedViewportVisual = null;
                base._cachedColumnHeaderViewportVisual = null;
                base._cachedRowHeaderViewportVisual = null;
                base._cachedCornerViewportVisual = null;
                base._cachedBottomRightACornerVisual = null;
                if ((base._zoomOriginHitTestInfo != null) && (base._zoomOriginHitTestInfo.HitTestType == HitTestType.Viewport))
                {
                    TransformGroup group = base._cachedViewportTransform[base._zoomOriginHitTestInfo.RowViewportIndex + 1, base._zoomOriginHitTestInfo.ColumnViewportIndex + 1];
                    if (group != null)
                    {
                        SpreadLayout spreadLayout = this.GetSpreadLayout();
                        Point newLocation = new Point(spreadLayout.HeaderWidth, spreadLayout.HeaderHeight);
                        Point reference = group.TransformPoint(newLocation);
                        int viewportLeftColumn = base.GetViewportLeftColumn(base._zoomOriginHitTestInfo.ColumnViewportIndex);
                        Point point3 = reference.Delta(newLocation);
                        int num2 = viewportLeftColumn;
                        double x = point3.X;
                        if (x > 0.0)
                        {
                            while ((x > 0.0) && (num2 < this.Worksheet.ColumnCount))
                            {
                                x -= Math.Floor((double)(this.Worksheet.Columns[num2].ActualWidth * base._touchZoomNewFactor));
                                num2++;
                            }
                        }
                        else if (x < 0.0)
                        {
                            while (((x < 0.0) && (num2 > 0)) && (num2 < this.Worksheet.ColumnCount))
                            {
                                x += Math.Floor((double)(this.Worksheet.Columns[num2].ActualWidth * base._touchZoomNewFactor));
                                num2--;
                            }
                        }
                        if (num2 != viewportLeftColumn)
                        {
                            this.SetViewportLeftColumn(base._zoomOriginHitTestInfo.ColumnViewportIndex, num2);
                        }
                        int viewportTopRow = base.GetViewportTopRow(base._zoomOriginHitTestInfo.RowViewportIndex);
                        int num5 = viewportTopRow;
                        double y = point3.Y;
                        if (y > 0.0)
                        {
                            while ((y > 0.0) && (num5 < this.Worksheet.RowCount))
                            {
                                y -= Math.Floor((double)(this.Worksheet.Rows[num5].ActualHeight * base._touchZoomNewFactor));
                                num5++;
                            }
                        }
                        else if (y < 0.0)
                        {
                            while (((y < 0.0) && (num5 > 0)) && (num5 < this.Worksheet.RowCount))
                            {
                                y += Math.Floor((double)(this.Worksheet.Rows[num5].ActualHeight * base._touchZoomNewFactor));
                                num5--;
                            }
                        }
                        if (num5 != viewportTopRow)
                        {
                            this.SetViewportTopRow(base._zoomOriginHitTestInfo.RowViewportIndex, num5);
                        }
                    }
                }
                base._cachedViewportTransform = null;
                base._cachedRowHeaderViewportTransform = null;
                base._cachedColumnHeaderViewportTransform = null;
                base._cachedCornerViewportTransform = null;
                float zoomFactor = this.Worksheet.ZoomFactor;
                this.Worksheet.ZoomFactor = (float)base._touchZoomNewFactor;
                base.RaiseUserZooming(zoomFactor, this.Worksheet.ZoomFactor);
                base.InvalidateViewportColumnsLayout();
                base.InvalidateViewportRowsLayout();
                base.InvalidateFloatingObjects();
                base.InvalidateMeasure();
                if (((base._touchStartTopRow >= 0) && (base._touchStartTopRow < this.Worksheet.RowCount)) && (base._touchStartLeftColumn >= 0))
                {
                    int columnCount = this.Worksheet.ColumnCount;
                    int num14 = base._touchStartLeftColumn;
                }
            }
            if (this.IsTouchColumnSplitting)
            {
                this.EndColumnSplitting();
            }
            if (this.IsTouchRowSplitting)
            {
                this.EndRowSplitting();
            }
            if (base.IsTouchTabStripResizing)
            {
                this.EndTabStripResizing();
            }
            base.fastScroll = false;
            base.GetViewportInfo();
            if (base._viewportPresenters != null)
            {
                GcViewport[,] viewportArray = base._viewportPresenters;
                int upperBound = viewportArray.GetUpperBound(0);
                int num9 = viewportArray.GetUpperBound(1);
                for (int i = viewportArray.GetLowerBound(0); i <= upperBound; i++)
                {
                    for (int j = viewportArray.GetLowerBound(1); j <= num9; j++)
                    {
                        GcViewport viewport = viewportArray[i, j];
                        if (viewport != null)
                        {
                            viewport.InvalidateBordersMeasureState();
                        }
                    }
                }
            }
            if (base._rowHeaderPresenters != null)
            {
                foreach (GcViewport viewport2 in base._rowHeaderPresenters)
                {
                    if (viewport2 != null)
                    {
                        viewport2.InvalidateBordersMeasureState();
                    }
                }
            }
            if (base._columnHeaderPresenters != null)
            {
                foreach (GcViewport viewport3 in base._columnHeaderPresenters)
                {
                    if (viewport3 != null)
                    {
                        viewport3.InvalidateBordersMeasureState();
                    }
                }
            }
            base.OnManipulationComplete(currentPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">point</param>
        internal override void OnManipulationStarted(Point point)
        {
            base.UpdateTouchHitTestInfo(new Point(point.X, point.Y));
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            base._touchStartHitTestInfo = savedHitTestInformation;
            base._touchZoomNewFactor = this.Worksheet.ZoomFactor;
            if ((savedHitTestInformation != null) && (savedHitTestInformation.HitTestType != HitTestType.Empty))
            {
                this.InitTouchCacheInfomation();
            }
            base.OnManipulationStarted(point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal override void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == 0)
            {
                this.UpdateScrollBarIndicatorMode((ScrollingIndicatorMode)1);
            }
            base.OnPointerMoved(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal override void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == 0)
            {
                IList<PointerPoint> intermediatePoints = e.GetIntermediatePoints(this);
                if ((this._primaryTouchDeviceId.HasValue && (intermediatePoints != null)) && (intermediatePoints.Count > 0))
                {
                    using (IEnumerator<PointerPoint> enumerator = intermediatePoints.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.PointerId == this._primaryTouchDeviceId.Value)
                            {
                                if (this.IsTouchColumnSplitting)
                                {
                                    this.EndColumnSplitting();
                                }
                                if (this.IsTouchRowSplitting)
                                {
                                    this.EndRowSplitting();
                                }
                            }
                        }
                    }
                }
            }
            if ((this.TabStrip != null) && this.TabStripEditable)
            {
                Point point = e.GetCurrentPoint(this).Position;
                if ((this.HitTest(point.X, point.Y).HitTestType == HitTestType.TabStrip) && this.TabStrip.StayInEditing(point))
                {
                    e.Handled = true;
                    return;
                }
            }
            base.OnPointerReleased(sender, e);
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
                value = Math.Max(this.Worksheet.FrozenColumnCount, value);
                value = Math.Min((this.Worksheet.ColumnCount - this.Worksheet.FrozenTrailingColumnCount) - 1, value);
                value = base.TryGetNextScrollableColumn(value);
                base.SetViewportLeftColumn(columnViewportIndex, value);
                if (this._horizontalScrollBar != null)
                {
                    this.GetSpreadLayout();
                    if (((columnViewportIndex > -1) && (columnViewportIndex < this._horizontalScrollBar.Length)) && (this._horizontalScrollBar[columnViewportIndex].Value != value))
                    {
                        int invisibleColumnsBeforeColumn = this.GetInvisibleColumnsBeforeColumn(this.ActiveSheet, value);
                        int num2 = value - invisibleColumnsBeforeColumn;
                        this._horizontalScrollBar[columnViewportIndex].Value = (double)num2;
                        this._horizontalScrollBar[columnViewportIndex].InvalidateArrange();
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
                value = Math.Max(this.Worksheet.FrozenRowCount, value);
                value = Math.Min((this.Worksheet.RowCount - this.Worksheet.FrozenTrailingRowCount) - 1, value);
                value = base.TryGetNextScrollableRow(value);
                if (this._verticalScrollBar != null)
                {
                    this.GetSpreadLayout();
                    if (action == null)
                    {
                        action = delegate
                        {
                            if (((rowViewportIndex > -1) && (rowViewportIndex < this._verticalScrollBar.Length)) && (value != this._verticalScrollBar[rowViewportIndex].Value))
                            {
                                int invisibleRowsBeforeRow = this.GetInvisibleRowsBeforeRow(this.ActiveSheet, value);
                                int num2 = value - invisibleRowsBeforeRow;
                                this._verticalScrollBar[rowViewportIndex].Value = (double)num2;
                                this._verticalScrollBar[rowViewportIndex].InvalidateArrange();
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
            this.Worksheet.AddColumnViewport(columnViewportIndex, viewportWidth / ((double)base.ZoomFactor));
            this.InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        /// <param name="viewportHeight"></param>
        internal void AddRowViewport(int rowViewportIndex, double viewportHeight)
        {
            this.Worksheet.AddRowViewport(rowViewportIndex, viewportHeight / ((double)base.ZoomFactor));
            this.InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AdjustViewportLeftColumn()
        {
            if (base._translateOffsetX != 0.0)
            {
                int viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
                if (viewportLeftColumn >= this.Worksheet.FrozenColumnCount)
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
                                    this.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, nextScrollableColumn);
                                }
                            }
                        }
                        else if (Math.Abs(base._translateOffsetX) >= (width / 2.0))
                        {
                            int num5 = base.GetNextScrollableColumn(viewportLeftColumn);
                            if (num5 != -1)
                            {
                                this.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num5);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AdjustViewportSize()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private void AdjustViewportTopRow()
        {
            if (base._translateOffsetY != 0.0)
            {
                int viewportTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
                if (viewportTopRow >= this.Worksheet.FrozenRowCount)
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
                                    this.SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, nextScrollableRow);
                                }
                            }
                        }
                        else if (Math.Abs(base._translateOffsetY) >= (height / 2.0))
                        {
                            int num5 = base.GetNextScrollableRow(viewportTopRow);
                            if (num5 != -1)
                            {
                                this.SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, num5);
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
        private async void AsynSetViewportTopRow(int rowViewportIndex)
        {
            if (!this._pendinging)
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
        private void ClearViewportsClip()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private void ContinueColumnSplitting()
        {
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (base.MousePosition.X <= this._columnSplittingTracker.X1)
                    {
                        this._columnSplittingTracker.X1 = Math.Max(base.MousePosition.X, spreadLayout.GetViewportX(columnViewportIndex) + (spreadLayout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0));
                        break;
                    }
                    this._columnSplittingTracker.X1 = Math.Min(base.MousePosition.X, (spreadLayout.GetViewportX(columnViewportIndex + 1) + spreadLayout.GetViewportWidth(columnViewportIndex + 1)) - (spreadLayout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0));
                    break;

                case HitTestType.ColumnSplitBox:
                    if (base.MousePosition.X <= this._columnSplittingTracker.X1)
                    {
                        this._columnSplittingTracker.X1 = Math.Max(base.MousePosition.X, spreadLayout.GetViewportX(columnViewportIndex) + (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0));
                        break;
                    }
                    this._columnSplittingTracker.X1 = Math.Min(base.MousePosition.X, (spreadLayout.GetViewportX(columnViewportIndex) + spreadLayout.GetViewportWidth(columnViewportIndex)) - (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0));
                    break;
            }
            this._columnSplittingTracker.X2 = this._columnSplittingTracker.X1;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ContinueRowSplitting()
        {
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (base.MousePosition.Y <= this._rowSplittingTracker.Y1)
                    {
                        this._rowSplittingTracker.Y1 = Math.Max(base.MousePosition.Y, spreadLayout.GetViewportY(rowViewportIndex) + (spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0));
                        break;
                    }
                    this._rowSplittingTracker.Y1 = Math.Min(base.MousePosition.Y, (spreadLayout.GetViewportY(rowViewportIndex + 1) + spreadLayout.GetViewportHeight(rowViewportIndex + 1)) - (spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0));
                    break;

                case HitTestType.RowSplitBox:
                    if (base.MousePosition.Y <= this._rowSplittingTracker.Y1)
                    {
                        this._rowSplittingTracker.Y1 = Math.Max(base.MousePosition.Y, spreadLayout.GetViewportY(rowViewportIndex) + (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0));
                        break;
                    }
                    this._rowSplittingTracker.Y1 = Math.Min(base.MousePosition.Y, (spreadLayout.GetViewportY(rowViewportIndex) + spreadLayout.GetViewportHeight(rowViewportIndex)) - (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0));
                    break;
            }
            this._rowSplittingTracker.Y2 = this._rowSplittingTracker.Y1;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ContinueTabStripResizing()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            double tabStripX = spreadLayout.TabStripX;
            double tabStripHeight = spreadLayout.TabStripHeight;
            double num2 = spreadLayout.GetHorizontalScrollBarWidth(0) + spreadLayout.TabStripWidth;
            double num3 = Math.Min(Math.Max((double)0.0, (double)(base.MousePosition.X - tabStripX)), num2);
            this._tabStripRatio = num3 / num2;
            this.InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="renderSize"></param>
        /// <returns></returns>
        private BitmapSource CreateCachedIamge(UIElement child, Size renderSize)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private Image CreateCachedVisual(ImageSource image, Rect bounds)
        {
#if UWP
            if (!this.IsZero(bounds.Width) && !this.IsZero(bounds.Height))
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
        private Rect CreateClipRect(double columnX, double rowY, double viewportWidth, double viewportHeight, int viewportColumnIndex, int viewportRowIndex)
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            double num = columnX;
            double num2 = rowY;
            double num3 = viewportWidth;
            double num4 = viewportHeight;
            if ((this.Worksheet.FrozenColumnCount > 0) && base.ShowFreezeLine)
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
            if ((this.Worksheet.FrozenRowCount > 0) && base.ShowFreezeLine)
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
            if ((this.Worksheet.FrozenTrailingColumnCount > 0) && base.ShowFreezeLine)
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
            if ((this.Worksheet.FrozenTrailingRowCount > 0) && base.ShowFreezeLine)
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
            if (((viewportTopRow >= 0) && (viewportBottomRow < this.Worksheet.RowCount)) && (viewportTopRow <= viewportBottomRow))
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
            if (((viewportLeftColumn >= 0) && (viewportRightColumn < this.Worksheet.ColumnCount)) && (viewportLeftColumn <= viewportRightColumn))
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
        private SpreadLayout CreateSpreadLayout()
        {
            Worksheet activeSheet = this.ActiveSheet;
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
            bool flag = (this.HorizontalScrollBarPolicy == (ScrollBarVisibility)3) || (this.HorizontalScrollBarPolicy == 0);
            if (this.HorizontalScrollBarPolicy == (ScrollBarVisibility)1)
            {
                if (layout.ColumnPaneCount > 1)
                {
                    flag = true;
                }
                else if ((this.VerticalScrollBarPolicy == (ScrollBarVisibility)3) || (this.VerticalScrollBarPolicy == 0))
                {
                    flag |= num5 > ((width - this.ActualVerticalScrollBarWidth) - groupLayout.Width);
                }
                else if (this.VerticalScrollBarPolicy == (ScrollBarVisibility)1)
                {
                    if (num4 > height)
                    {
                        flag |= num5 > ((width - this.ActualVerticalScrollBarWidth) - groupLayout.Width);
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
                layout.OrnamentHeight = this.ActualHorizontalScrollBarHeight;
                height -= layout.OrnamentHeight;
                height = Math.Max(0.0, height);
            }
            if (this.TabStripVisibility == 0)
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
            bool flag3 = ((this.VerticalScrollBarPolicy == (ScrollBarVisibility)3) || (this.VerticalScrollBarPolicy == 0)) || ((this.VerticalScrollBarPolicy == (ScrollBarVisibility)1) && ((layout.RowPaneCount > 1) || (num6 > (height - groupLayout.Height))));
            if (flag3)
            {
                layout.OrnamentWidth = this.ActualVerticalScrollBarWidth;
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
            double columnSplitBoxesWidth = this.GetColumnSplitBoxesWidth(layout.ColumnPaneCount);
            for (int num33 = 0; num33 < layout.ColumnPaneCount; num33++)
            {
                if (num33 == 0)
                {
                    double num34 = ((layout.HeaderX + layout.HeaderWidth) + layout.FrozenWidth) + layout.GetViewportWidth(num33);
                    double x = layout.X;
                    if (this.ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
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
                    if (this.ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
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
                    if (this.ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
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
            double rowSplitBoxesHeight = this.GetRowSplitBoxesHeight(layout.RowPaneCount);
            for (int num41 = 0; num41 < layout.RowPaneCount; num41++)
            {
                if (num41 == 0)
                {
                    double num42 = ((layout.HeaderY + layout.HeaderHeight) + layout.FrozenHeight) + layout.GetViewportHeight(num41);
                    double y = layout.Y;
                    if (this.RowSplitBoxAlignment == SplitBoxAlignment.Leading)
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
                    if (this.RowSplitBoxAlignment == SplitBoxAlignment.Leading)
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
                    if (this.RowSplitBoxAlignment == SplitBoxAlignment.Leading)
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
                    layout.TabStripWidth = this.TabStripRatio * Math.Max((double)0.0, (double)(layout.GetHorizontalScrollBarWidth(0) - 16.0));
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
                    num49 += this.ActualVerticalScrollBarWidth;
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
                    num52 += this.ActualHorizontalScrollBarHeight;
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
        private void EndColumnSplitting()
        {
            double num2;
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
            base.IsWorking = false;
            this.IsTouchColumnSplitting = false;
            this.IsColumnSplitting = false;
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
                        double deltaViewportWidth = (this._columnSplittingTracker.X1 - spreadLayout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex)) - (spreadLayout.GetHorizontalSplitBarWidth(savedHitTestInformation.ColumnViewportIndex) / 2.0);
                        int viewportIndex = savedHitTestInformation.ColumnViewportIndex;
                        if (!this.RaiseColumnViewportWidthChanging(viewportIndex, deltaViewportWidth))
                        {
                            base.AdjustColumnViewport(columnViewportIndex, deltaViewportWidth);
                            this.RaiseColumnViewportWidthChanged(viewportIndex, deltaViewportWidth);
                        }
                    }
                    goto Label_0258;

                case HitTestType.ColumnSplitBox:
                    if (this.ColumnSplitBoxAlignment != SplitBoxAlignment.Leading)
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
                double num3 = (this._columnSplittingTracker.X1 - spreadLayout.GetViewportX(columnViewportIndex)) - (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0);
                int num4 = (this.ColumnSplitBoxAlignment == SplitBoxAlignment.Leading) ? 0 : (base.GetViewportInfo().ColumnViewportCount - 1);
                if (!this.RaiseColumnViewportWidthChanging(num4, num3))
                {
                    this.AddColumnViewport(columnViewportIndex, num3);
                    this.RaiseColumnViewportWidthChanged(num4, num3);
                    base.ShowCell(base.GetActiveRowViewportIndex(), base.GetActiveColumnViewportIndex(), this.ActiveSheet.ActiveRowIndex, this.ActiveSheet.ActiveColumnIndex, VerticalPosition.Nearest, HorizontalPosition.Nearest);
                }
            }
        Label_0258:
            this._columnSplittingTracker.Opacity = 0.0;
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
            if ((sheetIndex < 0) || (sheetIndex >= this.SpreadSheet.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Worksheet sheet = this.SpreadSheet.Sheets[sheetIndex];
            return this.GetViewportInfo(sheet).ActiveRowViewport;
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndRowSplitting()
        {
            double num2;
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
            base.IsWorking = false;
            this.IsRowSplitting = false;
            this.IsTouchRowSplitting = false;
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
                        double deltaViewportHeight = (this._rowSplittingTracker.Y1 - spreadLayout.GetVerticalSplitBarY(rowViewportIndex)) - (spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0);
                        int viewportIndex = savedHitTestInformation.RowViewportIndex;
                        if (!this.RaiseRowViewportHeightChanging(viewportIndex, deltaViewportHeight))
                        {
                            base.AdjustRowViewport(rowViewportIndex, deltaViewportHeight);
                            this.RaiseRowViewportHeightChanged(viewportIndex, deltaViewportHeight);
                        }
                    }
                    goto Label_021D;

                case HitTestType.RowSplitBox:
                    if (this.RowSplitBoxAlignment != SplitBoxAlignment.Leading)
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
                double num3 = (this._rowSplittingTracker.Y1 - spreadLayout.GetViewportY(rowViewportIndex)) - (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0);
                int num4 = (this.RowSplitBoxAlignment == SplitBoxAlignment.Leading) ? 0 : (base.GetViewportInfo().RowViewportCount - 1);
                if (!this.RaiseRowViewportHeightChanging(num4, num3))
                {
                    this.AddRowViewport(rowViewportIndex, num3);
                    this.RaiseRowViewportHeightChanged(num4, num3);
                    base.ShowCell(base.GetActiveRowViewportIndex(), base.GetActiveColumnViewportIndex(), this.ActiveSheet.ActiveRowIndex, this.ActiveSheet.ActiveColumnIndex, VerticalPosition.Nearest, HorizontalPosition.Nearest);
                }
            }
        Label_021D:
            this._rowSplittingTracker.Opacity = 0.0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndTabStripResizing()
        {
            this.IsTabStripResizing = false;
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
            if ((sheetIndex < 0) || (sheetIndex >= this.SpreadSheet.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Worksheet sheet = this.SpreadSheet.Sheets[sheetIndex];
            return this.GetViewportInfo(sheet).ActiveColumnViewport;
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
        private double GetColumnSplitBoxesWidth(int columnPaneCount)
        {
            if (this._columnSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                if (this._columnSplitBoxPolicy == SplitBoxPolicy.AsNeeded)
                {
                    if (columnPaneCount == 1)
                    {
                        return 6.0;
                    }
                    return 0.0;
                }
                if (this._columnSplitBoxPolicy == SplitBoxPolicy.Never)
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
        private Rect GetHorizontalScrollBarRectangle(int columnViewportIndex)
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private Rect GetHorizontalSplitBarRectangle(int columnViewportIndex)
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private Rect GetHorizontalSplitBoxRectangle(int columnViewportIndex)
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private int GetInvisibleColumnsBeforeColumn(Worksheet sheet, int beforeColumn)
        {
            int num = 0;
            using (HashSet<int>.Enumerator enumerator = this._invisibleColumns.GetEnumerator())
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
        private int GetInvisibleRowsBeforeRow(Worksheet sheet, int beforeRow)
        {
            int num = 0;
            using (HashSet<int>.Enumerator enumerator = this._invisibleRows.GetEnumerator())
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

        private double GetRowSplitBoxesHeight(int rowPaneCount)
        {
            if (this._rowSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                if (this._rowSplitBoxPolicy == SplitBoxPolicy.AsNeeded)
                {
                    if (rowPaneCount == 1)
                    {
                        return 6.0;
                    }
                    return 0.0;
                }
                if (this._rowSplitBoxPolicy == SplitBoxPolicy.Never)
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
        private int GetSheetInvisibleColumns(Worksheet sheet)
        {
            int num = 0;
            this._invisibleColumns.Clear();
            for (int i = 0; i < sheet.ColumnCount; i++)
            {
                if (!sheet.GetActualColumnVisible(i, SheetArea.Cells))
                {
                    this._invisibleColumns.Add(i);
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
        private int GetSheetInvisibleRows(Worksheet sheet)
        {
            int num = 0;
            this._invisibleRows.Clear();
            for (int i = 0; i < sheet.RowCount; i++)
            {
                if (!sheet.GetActualRowVisible(i, SheetArea.Cells))
                {
                    this._invisibleRows.Add(i);
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
            return this.GetSpreadLayout();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        private double GetSmoothingScale(double scale)
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
            if (this._cachedSpreadLayout == null)
            {
                this._cachedSpreadLayout = this.CreateSpreadLayout();
                this.UpdateHorizontalScrollBars();
                this.UpdateVerticalScrollBars();
            }
            return this._cachedSpreadLayout;
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
            if (this.TabStrip != null)
            {
                return this.TabStrip.GetStartIndexToBringTabIntoView(tabIndex);
            }
            return this.SpreadSheet.StartSheetIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Rect GetTabSplitBoxRectangle()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private Rect GetTabStripRectangle()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private Rect GetVerticalScrollBarRectangle(int rowViewportIndex)
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private Rect GetVerticalSplitBarRectangle(int rowViewportIndex)
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private Rect GetVerticalSplitBoxRectangle(int rowViewportIndex)
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
        private int GetViewportInvisibleColumns(int columnViewportIndex)
        {
            ColumnLayoutModel viewportColumnLayoutModel = base.GetViewportColumnLayoutModel(columnViewportIndex);
            int num = 0;
            foreach (ColumnLayout layout in viewportColumnLayoutModel)
            {
                if (!this.Worksheet.GetActualColumnVisible(layout.Column, SheetArea.Cells))
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
        private int GetViewportInvisibleRows(int rowViewportIndex)
        {
            RowLayoutModel viewportRowLayoutModel = base.GetViewportRowLayoutModel(rowViewportIndex);
            int num = 0;
            foreach (RowLayout layout in viewportRowLayoutModel)
            {
                if (!this.Worksheet.GetActualRowVisible(layout.Row, SheetArea.Cells))
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
            if (this._progressRing != null)
            {
                this._progressRing.Visibility = (Visibility)1;
                this._progressRing.IsActive = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HideOpeningStatus()
        {
            this.HideOpeningProgressRing();
            if (this.TabStrip != null)
            {
                this.TabStrip.Visibility = 0;
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
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            HitTestInformation information = new HitTestInformation
            {
                HitTestType = HitTestType.Empty,
                ColumnViewportIndex = -2,
                RowViewportIndex = -2,
                HitPoint = point
            };
            if (this.GetTabStripRectangle().Contains(point))
            {
                information.ColumnViewportIndex = 0;
                information.HitTestType = HitTestType.TabStrip;
                return information;
            }
            if (this.GetTabSplitBoxRectangle().Contains(point))
            {
                information.ColumnViewportIndex = 0;
                information.HitTestType = HitTestType.TabSplitBox;
                return information;
            }
            for (int i = 0; i < spreadLayout.ColumnPaneCount; i++)
            {
                if (this.GetHorizontalScrollBarRectangle(i).Contains(point))
                {
                    information.ColumnViewportIndex = i;
                    information.HitTestType = HitTestType.HorizontalScrollBar;
                    return information;
                }
            }
            for (int j = 0; j < spreadLayout.RowPaneCount; j++)
            {
                if (this.GetVerticalScrollBarRectangle(j).Contains(point))
                {
                    information.HitTestType = HitTestType.VerticalScrollBar;
                    information.RowViewportIndex = j;
                    return information;
                }
            }
            for (int k = 0; k < spreadLayout.ColumnPaneCount; k++)
            {
                if (this.GetHorizontalSplitBoxRectangle(k).Contains(point))
                {
                    information.HitTestType = HitTestType.ColumnSplitBox;
                    information.ColumnViewportIndex = k;
                }
            }
            for (int m = 0; m < spreadLayout.RowPaneCount; m++)
            {
                if (this.GetVerticalSplitBoxRectangle(m).Contains(point))
                {
                    information.HitTestType = HitTestType.RowSplitBox;
                    information.RowViewportIndex = m;
                }
            }
            for (int n = 0; n < (spreadLayout.ColumnPaneCount - 1); n++)
            {
                if (this.GetHorizontalSplitBarRectangle(n).Contains(point))
                {
                    information.HitTestType = HitTestType.ColumnSplitBar;
                    information.ColumnViewportIndex = n;
                }
            }
            for (int num6 = 0; num6 < (spreadLayout.RowPaneCount - 1); num6++)
            {
                if (this.GetVerticalSplitBarRectangle(num6).Contains(point))
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
        private void HorizontalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((base._touchToolbarPopup != null) && base._touchToolbarPopup.IsOpen)
            {
                base._touchToolbarPopup.IsOpen = false;
            }
            if (((this.ScrollBarTrackPolicy == ScrollBarTrackPolicy.Both) || (this.ScrollBarTrackPolicy == ScrollBarTrackPolicy.Horizontal)) || (base._isTouchScrolling || (e.ScrollEventType != (ScrollEventType)5)))
            {
                for (int i = 0; i < this._horizontalScrollBar.Length; i++)
                {
                    if (sender == this._horizontalScrollBar[i])
                    {
                        if (base.HorizontalScrollable)
                        {
                            this.ProcessHorizontalScroll(i, e);
                            return;
                        }
                        this._horizontalScrollBar[i].Value = (double)base.GetViewportLeftColumn(i);
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
        private void HorizontalScrollBarTouchSmallDecrement(int columnViewportIndex, int newValue)
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
#if IOS
        new
#endif
        private void Init()
        {
            Action action = null;
            this._horizontalScrollBarHeight = 25.0;
            this._verticalScrollBarWidth = 25.0;
            this._horizontalScrollBarStyle = null;
            this._verticalScrollBarStyle = null;
            this._scrollBarTrackPolicy = ScrollBarTrackPolicy.Both;
            this._columnSplitBoxAlignment = SplitBoxAlignment.Leading;
            this._rowSplitBoxAlignment = SplitBoxAlignment.Leading;
            this._tabStripEditable = true;
            this._tabStripInsertTab = true;
            this._tabStripVisibility = 0;
            this._tabStripRatio = 0.5;
            this._cachedLastAvailableSize = new Size(0.0, 0.0);
            this._columnSplitBoxPolicy = SplitBoxPolicy.Always;
            this._rowSplitBoxPolicy = SplitBoxPolicy.Always;
            this._progressGrid = new Grid();
            this._progressRing = new ProgressRing();
            if (!this._progressGrid.Children.Contains(this._progressRing))
            {
                if (action == null)
                {
                    action = delegate
                    {
                        this._progressRing.Foreground = new SolidColorBrush(Colors.Black);
                    };
                }
                UIAdaptor.InvokeSync(action);
                this._progressRing.IsActive = true;
                this._progressRing.Visibility = (Visibility)1;
                this._progressRing.Width = 200.0;
                this._progressRing.Height = 200.0;
                this._progressGrid.Children.Add(this._progressRing);
            }
            this._showScrollTip = false;
            FormulaSelectionGripperContainerPanel panel = new FormulaSelectionGripperContainerPanel
            {
                ParentSheet = this
            };
            panel.IsHitTestVisible = false;
            base._formulaSelectionGripperPanel = panel;
            base._topLeftGripper = new Ellipse();
            base._topLeftGripper.Stroke = new SolidColorBrush(Color.FromArgb(220, 0, 0, 0));
            base._topLeftGripper.StrokeThickness = 2.0;
            base._topLeftGripper.Fill = new SolidColorBrush(Colors.White);
            base._topLeftGripper.Height = 16.0;
            base._topLeftGripper.Width = 16.0;
            base._bottomRightGripper = new Ellipse();
            base._bottomRightGripper.Stroke = new SolidColorBrush(Color.FromArgb(220, 0, 0, 0));
            base._bottomRightGripper.StrokeThickness = 2.0;
            base._bottomRightGripper.Fill = new SolidColorBrush(Colors.White);
            base._bottomRightGripper.Height = 16.0;
            base._bottomRightGripper.Width = 16.0;
            base._resizerGripperContainer = new Border();
            base._resizerGripperContainer.Width = 16.0;
            base._resizerGripperContainer.Height = 16.0;
            base._autoFillIndicatorContainer = new Border();
            base._autoFillIndicatorContainer.Width = 16.0;
            base._autoFillIndicatorContainer.Height = 16.0;
            base._cachedColumnResizerGripperImage = new Image();
            base._resizerGripperContainer.Child = base._cachedColumnResizerGripperImage;
            base._cachedRowResizerGripperImage = new Image();
            base._cachedResizerGipper = new Dictionary<string, BitmapImage>();
            base._cachedautoFillIndicatorImage = new Image();
            base._autoFillIndicatorContainer.Child = base._cachedautoFillIndicatorImage;
            base._cachedToolbarImageSources = new Dictionary<string, ImageSource>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitCachedTransform()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            if (base._cachedViewportTransform == null)
            {
                base._cachedViewportTransform = new TransformGroup[spreadLayout.RowPaneCount + 2, spreadLayout.ColumnPaneCount + 2];
                for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
                {
                    for (int j = -1; j <= spreadLayout.RowPaneCount; j++)
                    {
                        base._cachedViewportTransform[j + 1, i + 1] = this.InitManipulationTransforms();
                    }
                }
            }
            if (base._cachedColumnHeaderViewportTransform == null)
            {
                base._cachedColumnHeaderViewportTransform = new TransformGroup[spreadLayout.ColumnPaneCount + 2];
                for (int k = -1; k <= spreadLayout.ColumnPaneCount; k++)
                {
                    base._cachedColumnHeaderViewportTransform[k + 1] = this.InitManipulationTransforms();
                }
            }
            if (base._cachedRowHeaderViewportTransform == null)
            {
                base._cachedRowHeaderViewportTransform = new TransformGroup[spreadLayout.RowPaneCount + 2];
                for (int m = -1; m <= spreadLayout.RowPaneCount; m++)
                {
                    base._cachedRowHeaderViewportTransform[m + 1] = this.InitManipulationTransforms();
                }
            }
            if (base._cachedCornerViewportTransform == null)
            {
                base._cachedCornerViewportTransform = this.InitManipulationTransforms();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitCachedVisual()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            BitmapSource image = this.CreateCachedIamge(this, base.RenderSize);
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
                        base._cachedViewportVisual[j + 1, i + 1] = this.CreateCachedVisual(image, this.CreateClipRect(viewportX, viewportY, viewportWidth, viewportHeight, i, j));
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
                    base._cachedColumnHeaderViewportVisual[k + 1] = this.CreateCachedVisual(image, this.CreateClipRect(columnX, headerY, num10, headerHeight, k, -2));
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
                    base._cachedRowHeaderViewportVisual[m + 1] = this.CreateCachedVisual(image, this.CreateClipRect(headerX, rowY, headerWidth, num16, -2, m));
                }
            }
            if (base._cachedCornerViewportVisual == null)
            {
                base._cachedCornerViewportVisual = this.CreateCachedVisual(image, new Rect(spreadLayout.HeaderX, spreadLayout.HeaderY, spreadLayout.HeaderWidth, spreadLayout.HeaderHeight));
            }
            if (base._cachedBottomRightACornerVisual == null)
            {
                base._cachedBottomRightACornerVisual = this.CreateCachedVisual(image, new Rect(spreadLayout.OrnamentX, spreadLayout.OrnamentY, spreadLayout.OrnamentWidth, spreadLayout.OrnamentHeight));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private TransformGroup InitManipulationTransforms()
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
        private void InitTouchCacheInfomation()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
            this._cachedSpreadLayout = null;
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
                this.InvalidateLayout();
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
                this.RefreshTabStrip();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsMouseInScrollBar()
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
        private bool IsMouseInSplitBar()
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
        private bool IsMouseInSplitBox()
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
        private bool IsMouseInTabSplitBox()
        {
            return (base.GetSavedHitTestInformation().HitTestType == HitTestType.TabSplitBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsMouseInTabStrip()
        {
            return (base.GetSavedHitTestInformation().HitTestType == HitTestType.TabStrip);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsZero(double value)
        {
            return (Math.Abs(value) < 2.2204460492503131E-15);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="scrollValue"></param>
        /// <returns></returns>
        private int MapScrollValueToColumnIndex(Worksheet sheet, int scrollValue)
        {
            int num = 0;
            for (int i = 0; i < sheet.ColumnCount; i++)
            {
                if (!this._invisibleColumns.Contains(i))
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
        private int MapScrollValueToRowIndex(Worksheet sheet, int scrollValue)
        {
            int num = 0;
            for (int i = 0; i < sheet.RowCount; i++)
            {
                if (!this._invisibleRows.Contains(i))
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
            this.SaveDataForFormulaSelection();
            if (!base.StopCellEditing(base.CanSelectFormula))
            {
                return false;
            }
            TabStrip tabStrip = this.TabStrip;
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
            this.SaveDataForFormulaSelection();
            if (!base.StopCellEditing(base.CanSelectFormula))
            {
                return false;
            }
            TabStrip tabStrip = this.TabStrip;
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
        private void OnHorizontalScrollBarPointerExited(object sender, PointerRoutedEventArgs e)
        {
            this._showScrollTip = false;
            base._mouseDownPosition = e.GetCurrentPoint(this).Position;
            base.CloseTooltip();
        }

        private void OnHorizontalScrollBarPointerPressed(object sender, PointerRoutedEventArgs e)
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
                this._showScrollTip = true;
                base._mouseDownPosition = e.GetCurrentPoint(this).Position;
                base.UpdateScrollToolTip(false, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHorizontalScrollBarPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this._showScrollTip = false;
            base._mouseDownPosition = e.GetCurrentPoint(this).Position;
            base.CloseTooltip();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTabStripActiveTabChanged(object sender, EventArgs e)
        {
            TabStrip strip = sender as TabStrip;
            int sheetIndex = -1;
            if ((strip != null) && (strip.ActiveTab != null))
            {
                sheetIndex = strip.ActiveTab.SheetIndex;
            }
            if ((sheetIndex >= 0) && (sheetIndex < this.SpreadSheet.Sheets.Count))
            {
                base.StopCellEditing(false);
                if (sheetIndex != this.SpreadSheet.ActiveSheetIndex)
                {
                    this.SpreadSheet.Workbook.ActiveSheetIndex = sheetIndex;
                    this.RaiseActiveSheetIndexChanged();
                    base._currentActiveRowIndex = this.SpreadSheet.ActiveSheet.ActiveRowIndex;
                    base._currentActiveColumnIndex = this.SpreadSheet.ActiveSheet.ActiveColumnIndex;
                    base.Navigation.UpdateStartPosition(base._currentActiveRowIndex, base._currentActiveColumnIndex);
                    base.Invalidate();
                }
            }
            if (!this.IsEditing)
            {
                this.SpreadSheet.Focus(FocusState.Programmatic);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTabStripActiveTabChanging(object sender, EventArgs e)
        {
            if ((sender is TabStrip) && (e is CancelEventArgs))
            {
                ((CancelEventArgs)e).Cancel = this.RaiseActiveSheetIndexChanging();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTabStripNewTabNeeded(object sender, EventArgs e)
        {
            base.StopCellEditing(false);
            Worksheet item = new Worksheet();
            this.SpreadSheet.Sheets.Add(item);
            item.ReferenceStyle = this.SpreadSheet.Workbook.ReferenceStyle;
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
            (sender as TabStrip).NewTab(this.SpreadSheet.Sheets.Count - 1);
            this.InvalidateSheetLayout();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVerticalScrollbarPointerExited(object sender, PointerRoutedEventArgs e)
        {
            this._showScrollTip = false;
            base._mouseDownPosition = e.GetCurrentPoint(this).Position;
            base.CloseTooltip();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVerticalScrollbarPointerPressed(object sender, PointerRoutedEventArgs e)
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
                this._showScrollTip = true;
                base._mouseDownPosition = e.GetCurrentPoint(this).Position;
                base.UpdateScrollToolTip(true, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVerticalScrollbarPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this._showScrollTip = false;
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
            HitTestInformation hi = this.TouchHitTest(point.X, point.Y);
            if (!this.IsEditing || ((hi.HitTestType != HitTestType.RowSplitBar) && (hi.HitTestType != HitTestType.ColumnSplitBar)))
            {
                if ((hi.HitTestType == HitTestType.RowSplitBar) || (hi.HitTestType == HitTestType.ColumnSplitBar))
                {
                    this.ProcessSplitBarDoubleTap(hi);
                }
                else if ((savedHitTestInformation.HitTestType != HitTestType.Viewport) || !savedHitTestInformation.ViewportInfo.InSelectionDrag)
                {
                    if (((savedHitTestInformation.HitTestType == HitTestType.TabStrip) && (this._tabStrip != null)) && (this.TabStripEditable && !this.SpreadSheet.Workbook.Protect))
                    {
                        this._tabStrip.StartTabTouchEditing(point);
                        if (this._tabStrip.IsEditing)
                        {
                            int sheetTabIndex = (this._tabStrip.ActiveTab != null) ? this._tabStrip.ActiveTab.SheetIndex : -1;
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
        /// <param name="e"></param>
        internal override void ProcessGestrueRecognizerManipulationUpdated(ManipulationUpdatedEventArgs e)
        {
            if ((base._currentGestureAction != null) && base._currentGestureAction.IsValid)
            {
                if (this.IsZero((double)(e.Cumulative.Scale - 1f)) || (base._touchProcessedPointIds.Count == 1))
                {
                    base._currentGestureAction.HandleSingleManipulationDelta(e.Position, new Point(e.Cumulative.Translation.X, e.Cumulative.Translation.Y));
                }
                else if ((!this.IsZero((double)(e.Cumulative.Scale - 1f)) && !base.IsTouchZooming) && base.CanUserZoom)
                {
                    base.IsContinueTouchOperation = true;
                    base._touchZoomInitFactor = this.Worksheet.ZoomFactor;
                    base.IsTouchZooming = true;
                    base._touchZoomOrigin = e.Position;
                    base.CloseTouchToolbar();
                    if (base._touchStartHitTestInfo == null)
                    {
                        base._touchStartHitTestInfo = this.HitTest(e.Position.X, e.Position.Y);
                    }
                    if (base._zoomOriginHitTestInfo == null)
                    {
                        base._zoomOriginHitTestInfo = this.HitTest(e.Position.X, e.Position.Y);
                    }
                    base._touchZoomOrigin = e.Position;
                    this.InitCachedTransform();
                }
                else if (base.IsTouchZooming && (((!base.IsTouchDragFilling && !base.IsTouchDrapDropping) && (!this.IsEditing && !base.IsTouchSelectingCells)) && ((!base.IsTouchSelectingColumns && !base.IsTouchSelectingRows) && ((!base.IsTouchResizingColumns && !base.IsTouchResizingRows) && base.CanUserZoom))))
                {
                    double num = ((base._touchZoomInitFactor * ((float)e.Cumulative.Scale)) * 100.0) / 100.0;
                    float scale = e.Delta.Scale;
                    if ((num < 0.5) || (num > 4.0))
                    {
                        scale = 1f;
                    }
                    this.UpdateCachedImageTransform(e.Position, e.Delta.Translation, (double)scale);
                    if (num < 0.5)
                    {
                        num = 0.5;
                    }
                    if (num > 4.0)
                    {
                        num = 4.0;
                    }
                    base._touchZoomNewFactor = num;
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        /// <param name="e"></param>
        private void ProcessHorizontalScroll(int columnViewportIndex, ScrollEventArgs e)
        {
            int viewportLeftColumn = base.GetViewportLeftColumn(columnViewportIndex);
            int scrollValue = (int)Math.Round(e.NewValue);
            scrollValue = this.MapScrollValueToColumnIndex(this.ActiveSheet, scrollValue);
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
            if (((e.ScrollEventType != (ScrollEventType)5) && (num3 != e.NewValue)) && (this._horizontalScrollBar != null))
            {
                this.GetSpreadLayout();
                if (((columnViewportIndex > -1) && (columnViewportIndex < this._horizontalScrollBar.Length)) && (this._horizontalScrollBar[columnViewportIndex].Value != num3))
                {
                    int invisibleColumnsBeforeColumn = this.GetInvisibleColumnsBeforeColumn(this.ActiveSheet, num3);
                    num3 -= invisibleColumnsBeforeColumn;
                    this._horizontalScrollBar[columnViewportIndex].Value = (num3 != -1) ? ((double)num3) : ((double)viewportLeftColumn);
                    this._horizontalScrollBar[columnViewportIndex].InvalidateArrange();
                }
            }
            if (this._showScrollTip && ((base.ShowScrollTip == ShowScrollTip.Both) || (base.ShowScrollTip == ShowScrollTip.Horizontal)))
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
            if ((this._tabStrip != null) && this._tabStrip.IsEditing)
            {
                if ((e.Key == (VirtualKey)13) || (e.Key == (VirtualKey)9))
                {
                    this._tabStrip.StopTabEditing(false);
                    e.Handled = true;
                }
                else if (e.Key == (VirtualKey)0x1b)
                {
                    this._tabStrip.StopTabEditing(true);
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
            if ((!this.IsEditing || ((savedHitTestInformation.HitTestType != HitTestType.RowSplitBar) && (savedHitTestInformation.HitTestType != HitTestType.ColumnSplitBar))) && ((savedHitTestInformation.HitTestType != HitTestType.Viewport) || !savedHitTestInformation.ViewportInfo.InSelectionDrag))
            {
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.RowSplitBar:
                    case HitTestType.ColumnSplitBar:
                        this.ProcessSplitBarDoubleClick(savedHitTestInformation);
                        return;

                    case HitTestType.TabStrip:
                        {
                            if (((this._tabStrip == null) || !this.TabStripEditable) || (this.SpreadSheet.Workbook.Protect || (base._routedEventArgs == null)))
                            {
                                break;
                            }
                            this._tabStrip.StartTabEditing(base._routedEventArgs.Instance);
                            if (!this._tabStrip.IsEditing)
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
            this.GetSpreadLayout();
            Point position = e.GetPosition(this);
            if (position != savedHitTestInformation.HitPoint)
            {
                base.SaveHitTestInfo(savedHitTestInformation = this.HitTest(position.X, position.Y));
            }
            if (!base._isDoubleClick || ((savedHitTestInformation.HitTestType != HitTestType.ColumnSplitBar) && (savedHitTestInformation.HitTestType != HitTestType.RowSplitBar)))
            {
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.TabStrip:
                        if (base._routedEventArgs != null)
                        {
                            this._tabStrip.ProcessMouseClickSheetTab(base._routedEventArgs.Instance);
                        }
                        return;

                    case HitTestType.RowSplitBar:
                        this.StartRowSplitting();
                        if (savedHitTestInformation.ColumnViewportIndex >= 0)
                        {
                            this.StartColumnSplitting();
                        }
                        return;

                    case HitTestType.ColumnSplitBar:
                        this.StartColumnSplitting();
                        if (savedHitTestInformation.RowViewportIndex >= 0)
                        {
                            this.StartRowSplitting();
                        }
                        return;

                    case HitTestType.RowSplitBox:
                        this.StartRowSplitting();
                        return;

                    case HitTestType.ColumnSplitBox:
                        this.StartColumnSplitting();
                        return;

                    case HitTestType.TabSplitBox:
                        this.StartTabStripResizing();
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
            if ((this.IsColumnSplitting || this.IsRowSplitting) || this.IsTabStripResizing)
            {
                this.ClearMouseLeftButtonDownStates();
                if (!this.IsEditing)
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
            this.GetSpreadLayout();
            base.MousePosition = e.GetPosition(this);
            HitTestInformation hitTestInfo = this.HitTest(base.MousePosition.X, base.MousePosition.Y);
            if (!base.IsWorking)
            {
                base.ResetCursor();
            }
            bool flag = false;
            switch (hitTestInfo.HitTestType)
            {
                case HitTestType.RowSplitBar:
                    if ((!base.IsWorking && !this.IsEditing) && !this.SpreadSheet.Workbook.Protect)
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
                    if ((base.IsWorking || this.IsEditing) || this.SpreadSheet.Workbook.Protect)
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
                    if ((!base.IsWorking && !this.IsEditing) && !this.SpreadSheet.Workbook.Protect)
                    {
                        if (base.InputDeviceType != InputDeviceType.Touch)
                        {
                            base.SetBuiltInCursor((CoreCursorType)8);
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                case HitTestType.ColumnSplitBox:
                    if ((!base.IsWorking && !this.IsEditing) && !this.SpreadSheet.Workbook.Protect)
                    {
                        if (base.InputDeviceType != InputDeviceType.Touch)
                        {
                            base.SetBuiltInCursor((CoreCursorType)10);
                        }
                        flag = true;
                    }
                    goto Label_01B3;

                case HitTestType.TabSplitBox:
                    if (!base.IsWorking && !this.IsEditing)
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
            if (this.IsColumnSplitting)
            {
                this.ContinueColumnSplitting();
            }
            if (this.IsRowSplitting)
            {
                this.ContinueRowSplitting();
            }
            if (this.IsTabStripResizing)
            {
                this.ContinueTabStripResizing();
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
            this.UpdateScrollBarIndicatorMode((ScrollingIndicatorMode)2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hi"></param>
        private void ProcessSplitBarDoubleClick(HitTestInformation hi)
        {
            if (!this.SpreadSheet.Workbook.Protect)
            {
                int rowViewportIndex = hi.RowViewportIndex;
                int columnViewportIndex = hi.ColumnViewportIndex;
                if (rowViewportIndex >= 0)
                {
                    double viewportHeight = base.GetViewportHeight(rowViewportIndex + 1);
                    if (!this.RaiseRowViewportHeightChanging(rowViewportIndex, viewportHeight))
                    {
                        this.Worksheet.RemoveRowViewport(rowViewportIndex);
                        this.RaiseRowViewportHeightChanged(rowViewportIndex, viewportHeight);
                    }
                }
                if (columnViewportIndex >= 0)
                {
                    double viewportWidth = base.GetViewportWidth(columnViewportIndex + 1);
                    if (!this.RaiseColumnViewportWidthChanging(columnViewportIndex, viewportWidth))
                    {
                        this.Worksheet.RemoveColumnViewport(columnViewportIndex);
                        this.RaiseColumnViewportWidthChanged(columnViewportIndex, viewportWidth);
                    }
                }
                this.InvalidateLayout();
                base._positionInfo = null;
                base.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hi"></param>
        private void ProcessSplitBarDoubleTap(HitTestInformation hi)
        {
            this.ProcessSplitBarDoubleClick(hi);
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void ProcessStartSheetIndexChanged()
        {
            if (((this.Worksheet != null) && (this.Worksheet.Workbook != null)) && (this._tabStrip != null))
            {
                this._tabStrip.SetStartSheet(this.Worksheet.Workbook.StartSheetIndex);
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
                        this.HorizontalScrollBarTouchSmallDecrement(savedHitTestInformation.ColumnViewportIndex, viewportLeftColumn - 1);
                        break;
                    }
                case HitTestType.VerticalScrollBar:
                    {
                        int viewportTopRow = base.GetViewportTopRow(savedHitTestInformation.RowViewportIndex);
                        this.VerticalScrollBarTouchSmallDecrement(savedHitTestInformation.RowViewportIndex, viewportTopRow - 1);
                        break;
                    }
                case HitTestType.TabStrip:
                    this._tabStrip.ProcessTap(point);
                    break;
            }
            base.ProcessTap(point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="currentPoint"></param>
        /// <param name="deltaPoint"></param>
        /// <param name="orientation"></param>
        internal override void ProcessTouchFreeDrag(Point startPoint, Point currentPoint, Point deltaPoint, DragOrientation orientation)
        {
            if (!base.IsWorking)
            {
                base.UpdateTouchHitTestInfo(currentPoint);
            }
            HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
            base.MousePosition = currentPoint;
            if (base.IsTouching)
            {
                bool flag = ((base._touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject) && (base._touchStartHitTestInfo.FloatingObjectInfo.FloatingObject != null)) && base._touchStartHitTestInfo.FloatingObjectInfo.FloatingObject.IsSelected;
                if ((((!base.IsTouchDragFilling && !base.IsTouchDrapDropping) && (!base.IsTouchSelectingCells && !base.IsTouchTabStripResizing)) && ((!this.IsRowSplitting && !this.IsColumnSplitting) && ((!base.IsTouchSelectingColumns && !base.IsTouchSelectingRows) && (base._touchStartHitTestInfo.HitTestType == HitTestType.Viewport)))) || ((base._touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject) && !flag))
                {
                    base.IsContinueTouchOperation = true;
                    base.CloseTouchToolbar();
                    base._updateViewportAfterTouch = true;
                    if ((deltaPoint.X != 0.0) && ((orientation & DragOrientation.Horizontal) == DragOrientation.Horizontal))
                    {
                        if ((orientation & DragOrientation.Vertical) == DragOrientation.None)
                        {
                            base._translateOffsetY = 0.0;
                        }
                        if (deltaPoint.X > 0.0)
                        {
                            base._isTouchScrolling = true;
                            this.TouchScrollLeft(startPoint, currentPoint, deltaPoint);
                        }
                        if (deltaPoint.X < 0.0)
                        {
                            base._isTouchScrolling = true;
                            this.TouchScrollRight(startPoint, currentPoint, deltaPoint);
                        }
                    }
                    if ((deltaPoint.Y != 0.0) && ((orientation & DragOrientation.Vertical) == DragOrientation.Vertical))
                    {
                        if ((orientation & DragOrientation.Horizontal) == DragOrientation.None)
                        {
                            base._translateOffsetX = 0.0;
                        }
                        if (deltaPoint.Y > 0.0)
                        {
                            base._isTouchScrolling = true;
                            this.TouchScrollUp(startPoint, currentPoint, deltaPoint);
                        }
                        if (deltaPoint.Y < 0.0)
                        {
                            base._isTouchScrolling = true;
                            this.TouchScrollBottom(startPoint, currentPoint, deltaPoint);
                        }
                    }
                    SpreadLayout spreadLayout = this.GetSpreadLayout();
                    if (base._translateOffsetX < 0.0)
                    {
                        if ((base._touchStartHitTestInfo.ColumnViewportIndex == -1) || (base._touchStartHitTestInfo.ColumnViewportIndex == spreadLayout.ColumnPaneCount))
                        {
                            base._translateOffsetX = 0.0;
                        }
                        else
                        {
                            spreadLayout.SetViewportWidth(base._touchStartHitTestInfo.ColumnViewportIndex, base._cachedViewportWidths[base._touchStartHitTestInfo.ColumnViewportIndex + 1] + Math.Abs(base._translateOffsetX));
                            base.InvalidateViewportColumnsLayout();
                        }
                    }
                    else if ((base._touchStartHitTestInfo.ColumnViewportIndex == -1) || (base._touchStartHitTestInfo.ColumnViewportIndex == spreadLayout.ColumnPaneCount))
                    {
                        base._translateOffsetX = 0.0;
                    }
                    else
                    {
                        spreadLayout.SetViewportWidth(base._touchStartHitTestInfo.ColumnViewportIndex, base._cachedViewportWidths[base._touchStartHitTestInfo.ColumnViewportIndex + 1]);
                        base.InvalidateViewportColumnsLayout();
                    }
                    if (base._translateOffsetY < 0.0)
                    {
                        if ((base._touchStartHitTestInfo.RowViewportIndex == -1) || (base._touchStartHitTestInfo.RowViewportIndex == spreadLayout.RowPaneCount))
                        {
                            base._translateOffsetY = 0.0;
                        }
                        else
                        {
                            spreadLayout.SetViewportHeight(base._touchStartHitTestInfo.RowViewportIndex, base._cachedViewportHeights[base._touchStartHitTestInfo.RowViewportIndex + 1] + Math.Abs(base._translateOffsetY));
                            base.InvalidateViewportRowsLayout();
                        }
                    }
                    else if ((base._touchStartHitTestInfo.RowViewportIndex == -1) || (base._touchStartHitTestInfo.RowViewportIndex == spreadLayout.RowPaneCount))
                    {
                        base._translateOffsetY = 0.0;
                    }
                    else
                    {
                        spreadLayout.SetViewportHeight(base._touchStartHitTestInfo.RowViewportIndex, base._cachedViewportHeights[base._touchStartHitTestInfo.RowViewportIndex + 1]);
                        base.InvalidateViewportColumnsLayout();
                    }
                    base.InvalidateMeasure();
                }
                if ((savedHitTestInformation.HitTestType == HitTestType.ColumnSplitBox) && this.IsTouchColumnSplitting)
                {
                    this.ContinueColumnSplitting();
                }
                else if ((savedHitTestInformation.HitTestType == HitTestType.RowSplitBox) && this.IsTouchRowSplitting)
                {
                    this.ContinueRowSplitting();
                }
                else if ((savedHitTestInformation.HitTestType == HitTestType.RowSplitBar) || (savedHitTestInformation.HitTestType == HitTestType.ColumnSplitBar))
                {
                    if (this.IsTouchRowSplitting)
                    {
                        this.ContinueRowSplitting();
                    }
                    if (this.IsTouchColumnSplitting)
                    {
                        this.ContinueColumnSplitting();
                    }
                }
                else if (savedHitTestInformation.HitTestType == HitTestType.TabStrip)
                {
                    this.IsTouchTabStripScrolling = true;
                    if ((deltaPoint.X != 0.0) && ((orientation & DragOrientation.Horizontal) == DragOrientation.Horizontal))
                    {
                        if (deltaPoint.X > 0.0)
                        {
                            this.TouchTabStripScrollLeft(startPoint, currentPoint, deltaPoint);
                        }
                        if (deltaPoint.X < 0.0)
                        {
                            this.TouchTabStripScrollRight(startPoint, currentPoint, deltaPoint);
                        }
                        this.TabStrip.TabsPresenter.InvalidateMeasure();
                        this.TabStrip.TabsPresenter.InvalidateArrange();
                    }
                }
                else if ((savedHitTestInformation.HitTestType == HitTestType.TabSplitBox) && base.IsTouchTabStripResizing)
                {
                    this.ContinueTabStripResizing();
                }
                else
                {
                    base.ProcessTouchFreeDrag(startPoint, currentPoint, deltaPoint, orientation);
                }
            }
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
        private void ProcessVerticalScroll(int rowViewportIndex, ScrollEventArgs e)
        {
            int viewportTopRow = base.GetViewportTopRow(rowViewportIndex);
            int scrollValue = (int)Math.Round(e.NewValue);
            scrollValue = this.MapScrollValueToRowIndex(this.Worksheet, scrollValue);
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
                this._scrollTo = beforeRow;
                this.AsynSetViewportTopRow(rowViewportIndex);
            }
            if (((e.ScrollEventType != (ScrollEventType)5) && (beforeRow != e.NewValue)) && (this._verticalScrollBar != null))
            {
                this.GetSpreadLayout();
                if (((rowViewportIndex > -1) && (rowViewportIndex < this._verticalScrollBar.Length)) && (beforeRow != this._verticalScrollBar[rowViewportIndex].Value))
                {
                    int invisibleRowsBeforeRow = this.GetInvisibleRowsBeforeRow(this.ActiveSheet, beforeRow);
                    beforeRow -= invisibleRowsBeforeRow;
                    this._verticalScrollBar[rowViewportIndex].Value = (beforeRow != -1) ? ((double)beforeRow) : ((double)viewportTopRow);
                    this._verticalScrollBar[rowViewportIndex].InvalidateMeasure();
                    this._verticalScrollBar[rowViewportIndex].InvalidateArrange();
                }
            }
            if (this._showScrollTip && ((base.ShowScrollTip == ShowScrollTip.Both) || (base.ShowScrollTip == ShowScrollTip.Vertical)))
            {
                base.UpdateScrollToolTip(true, this._scrollTo + 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RaiseActiveSheetIndexChanged()
        {
            if ((this.ActiveSheetChanged != null) && (base._eventSuspended == 0))
            {
                this.ActiveSheetChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool RaiseActiveSheetIndexChanging()
        {
            if ((this.ActiveSheetChanging != null) && (base._eventSuspended == 0))
            {
                CancelEventArgs args = new CancelEventArgs();
                this.ActiveSheetChanging(this, args);
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
            if ((this.ColumnViewportWidthChanged != null) && (base._eventSuspended == 0))
            {
                this.ColumnViewportWidthChanged(this, new ColumnViewportWidthChangedEventArgs(viewportIndex, deltaViewportWidth));
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
            if ((this.ColumnViewportWidthChanging != null) && (base._eventSuspended == 0))
            {
                ColumnViewportWidthChangingEventArgs args = new ColumnViewportWidthChangingEventArgs(viewportIndex, deltaViewportWidth);
                this.ColumnViewportWidthChanging(this, args);
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
            if ((this.RowViewportHeightChanged != null) && (base._eventSuspended == 0))
            {
                this.RowViewportHeightChanged(this, new RowViewportHeightChangedEventArgs(viewportIndex, deltaViewportHeight));
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
            if ((this.RowViewportHeightChanging != null) && (base._eventSuspended == 0))
            {
                RowViewportHeightChangingEventArgs args = new RowViewportHeightChangingEventArgs(viewportIndex, deltaViewportHeight);
                this.RowViewportHeightChanging(this, args);
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
                    this._scrollBarTrackPolicy = (ScrollBarTrackPolicy)Serializer.DeserializeObj(typeof(ScrollBarTrackPolicy), reader);
                    return;

                case "ColumnSplitBoxAlignment":
                    this._columnSplitBoxAlignment = (SplitBoxAlignment)Serializer.DeserializeObj(typeof(SplitBoxAlignment), reader);
                    return;

                case "RowSplitBoxAlignment":
                    this._rowSplitBoxAlignment = (SplitBoxAlignment)Serializer.DeserializeObj(typeof(SplitBoxAlignment), reader);
                    return;

                case "HorizontalScrollBarHeight":
                    this._horizontalScrollBarHeight = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "VerticalScrollBarWidth":
                    this._verticalScrollBarWidth = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "TabStripVisibility":
                    this._tabStripVisibility = (Visibility)Serializer.DeserializeObj(typeof(Visibility), reader);
                    return;

                case "TabStripEditable":
                    this._tabStripEditable = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "TabStripRadio":
                    this._tabStripRatio = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "TabStripInsertTab":
                    {
                        bool flag = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        this._tabStripInsertTab = flag;
                        if (this._tabStrip == null)
                        {
                            break;
                        }
                        this._tabStrip.HasInsertTab = flag;
                        return;
                    }
                case "ColumnSplitBoxPolicy":
                    this._columnSplitBoxPolicy = (SplitBoxPolicy)Serializer.DeserializeObj(typeof(SplitBoxPolicy), reader);
                    return;

                case "RowSplitBoxPolicy":
                    this._rowSplitBoxPolicy = (SplitBoxPolicy)Serializer.DeserializeObj(typeof(SplitBoxPolicy), reader);
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
            if (this._tabStrip != null)
            {
                this._tabStrip.Refresh();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnViewportIndex"></param>
        internal void RemoveColumnViewport(int columnViewportIndex)
        {
            this.Worksheet.RemoveColumnViewport(columnViewportIndex);
            this.InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowViewportIndex"></param>
        internal void RemoveRowViewport(int rowViewportIndex)
        {
            this.Worksheet.RemoveRowViewport(rowViewportIndex);
            this.InvalidateLayout();
            base.InvalidateMeasure();
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void Reset()
        {
            this.Init();
            base.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ps"></param>
        internal override void ResetTouchStates(IList<PointerPoint> ps)
        {
            if (this.IsTouchColumnSplitting)
            {
                this.EndColumnSplitting();
            }
            if (this.IsTouchRowSplitting)
            {
                this.EndRowSplitting();
            }
            if (base.IsTouchTabStripResizing)
            {
                this.EndTabStripResizing();
            }
            base.ResetTouchStates(ps);
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
                    base.EditorConnector.SheetIndex = this.Worksheet.Workbook.ActiveSheetIndex;
                    base.EditorConnector.RowIndex = this.Worksheet.ActiveRowIndex;
                    base.EditorConnector.ColumnIndex = this.Worksheet.ActiveColumnIndex;
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
                if (this._progressRing != null)
                {
                    this._progressRing.Visibility = 0;
                    this._progressRing.IsActive = true;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ShowOpeningStatus()
        {
            this.ShowOpeningProgressRing();
            UIAdaptor.InvokeAsync(delegate
            {
                if (this.TabStrip != null)
                {
                    this.TabStrip.Visibility = (Visibility)1;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartColumnSplitting()
        {
            if (!this.SpreadSheet.Workbook.Protect)
            {
                HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
                SpreadLayout spreadLayout = this.GetSpreadLayout();
                if (!base.IsTouching)
                {
                    this.IsColumnSplitting = true;
                }
                else
                {
                    this.IsTouchColumnSplitting = true;
                }
                base.IsWorking = true;
                if (this._columnSplittingTracker == null)
                {
                    SolidColorBrush brush = null;
                    UIAdaptor.InvokeSync(delegate
                    {
                        brush = new SolidColorBrush(Colors.Black);
                    });
                    Line line = new Line();
                    line.Stroke = brush;
                    line.Opacity = 0.5;
                    this._columnSplittingTracker = line;
                    this.SplittingTrackerContainer.Children.Add(this._columnSplittingTracker);
                }
                int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
                int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
                this._columnSplittingTracker.Opacity = 0.5;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.RowSplitBar:
                    case HitTestType.ColumnSplitBar:
                        this._columnSplittingTracker.StrokeThickness = spreadLayout.GetHorizontalSplitBarWidth(columnViewportIndex);
                        this._columnSplittingTracker.X1 = spreadLayout.GetHorizontalSplitBarX(columnViewportIndex) + (spreadLayout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0);
                        this._columnSplittingTracker.Y1 = spreadLayout.Y;
                        this._columnSplittingTracker.X2 = this._columnSplittingTracker.X1;
                        this._columnSplittingTracker.Y2 = spreadLayout.HeaderY + base.AvailableSize.Height;
                        return;

                    case HitTestType.RowSplitBox:
                        return;

                    case HitTestType.ColumnSplitBox:
                        this._columnSplittingTracker.StrokeThickness = spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex);
                        if (this.ColumnSplitBoxAlignment != SplitBoxAlignment.Leading)
                        {
                            this._columnSplittingTracker.X1 = (spreadLayout.GetViewportX(columnViewportIndex) + spreadLayout.GetViewportWidth(columnViewportIndex)) - (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0);
                        }
                        else
                        {
                            this._columnSplittingTracker.X1 = spreadLayout.GetViewportX(columnViewportIndex) + (spreadLayout.GetHorizontalSplitBoxWidth(columnViewportIndex) / 2.0);
                        }
                        this._columnSplittingTracker.Y1 = spreadLayout.Y;
                        this._columnSplittingTracker.X2 = this._columnSplittingTracker.X1;
                        this._columnSplittingTracker.Y2 = spreadLayout.HeaderY + base.AvailableSize.Height;
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
            if (this.IsMouseInScrollBar())
            {
                return false;
            }
            if (this.IsEditing && ((this.IsMouseInSplitBar() || this.IsMouseInSplitBox()) || this.IsMouseInTabSplitBox()))
            {
                return false;
            }
            Point position = e.GetPosition(this);
            if (!this.GetTabStripRectangle().Contains(position))
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
                    base.EditorConnector.SheetIndex = this.Worksheet.Workbook.ActiveSheetIndex;
                    base.EditorConnector.RowIndex = this.Worksheet.ActiveRowIndex;
                    base.EditorConnector.ColumnIndex = this.Worksheet.ActiveColumnIndex;
                }
            }
            base.StopCellEditing(base.CanSelectFormula);
            if (this._tabStrip != null)
            {
                this._tabStrip.StopTabEditing(false);
            }
            base._lastClickPoint = new Point(position.X, position.Y);
            base._routedEventArgs = e;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartRowSplitting()
        {
            if (!this.SpreadSheet.Workbook.Protect)
            {
                HitTestInformation savedHitTestInformation = base.GetSavedHitTestInformation();
                SpreadLayout spreadLayout = this.GetSpreadLayout();
                if (!base.IsTouching)
                {
                    this.IsRowSplitting = true;
                }
                else
                {
                    this.IsTouchRowSplitting = true;
                }
                base.IsWorking = true;
                if (this._rowSplittingTracker == null)
                {
                    SolidColorBrush brush = null;
                    UIAdaptor.InvokeSync(delegate
                    {
                        brush = new SolidColorBrush(Colors.Black);
                    });
                    Line line = new Line();
                    line.Stroke = brush;
                    line.Opacity = 0.5;
                    this._rowSplittingTracker = line;
                    this.SplittingTrackerContainer.Children.Add(this._rowSplittingTracker);
                }
                int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
                int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
                this._rowSplittingTracker.Opacity = 0.5;
                switch (savedHitTestInformation.HitTestType)
                {
                    case HitTestType.RowSplitBar:
                    case HitTestType.ColumnSplitBar:
                        this._rowSplittingTracker.StrokeThickness = spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex);
                        this._rowSplittingTracker.Y1 = spreadLayout.GetVerticalSplitBarY(rowViewportIndex) + (spreadLayout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0);
                        this._rowSplittingTracker.X1 = spreadLayout.X;
                        this._rowSplittingTracker.Y2 = this._rowSplittingTracker.Y1;
                        this._rowSplittingTracker.X2 = spreadLayout.X + base.AvailableSize.Width;
                        return;

                    case HitTestType.RowSplitBox:
                        this._rowSplittingTracker.StrokeThickness = spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex);
                        if (this.RowSplitBoxAlignment != SplitBoxAlignment.Leading)
                        {
                            this._rowSplittingTracker.Y1 = (spreadLayout.GetViewportY(rowViewportIndex) + spreadLayout.GetViewportHeight(rowViewportIndex)) - (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0);
                        }
                        else
                        {
                            this._rowSplittingTracker.Y1 = spreadLayout.GetViewportY(rowViewportIndex) + (spreadLayout.GetVerticalSplitBoxHeight(rowViewportIndex) / 2.0);
                        }
                        this._rowSplittingTracker.X1 = spreadLayout.X;
                        this._rowSplittingTracker.Y2 = this._rowSplittingTracker.Y1;
                        this._rowSplittingTracker.X2 = spreadLayout.X + base.AvailableSize.Width;
                        return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartTabStripResizing()
        {
            if (!base.IsTouching)
            {
                this.IsTabStripResizing = true;
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
        /// <param name="point"></param>
        /// <returns></returns>
        internal override bool StartTouchTap(Point point)
        {
            if (this.IsEditing && ((this.IsMouseInSplitBar() || this.IsMouseInSplitBox()) || this.IsMouseInTabSplitBox()))
            {
                return false;
            }
            if (!this.GetTabStripRectangle().Contains(point) || base.CanSelectFormula)
            {
                return base.StartTouchTap(point);
            }
            base.StopCellEditing(false);
            if (this._tabStrip != null)
            {
                this._tabStrip.StopTabEditing(false);
            }
            base._lastClickPoint = point;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal override HitTestInformation TouchHitTest(double x, double y)
        {
            Point point = new Point(x, y);
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            HitTestInformation information = new HitTestInformation
            {
                HitTestType = HitTestType.Empty,
                ColumnViewportIndex = -2,
                RowViewportIndex = -2,
                HitPoint = point
            };
            bool flag = (this.RowSplitBoxAlignment == SplitBoxAlignment.Trailing) && (this.ColumnSplitBoxAlignment == SplitBoxAlignment.Trailing);
            for (int i = 0; i < spreadLayout.ColumnPaneCount; i++)
            {
                Rect horizontalSplitBoxRectangle = this.GetHorizontalSplitBoxRectangle(i);
                if (flag)
                {
                    horizontalSplitBoxRectangle = horizontalSplitBoxRectangle.Expand(30, 0);
                }
                else
                {
                    horizontalSplitBoxRectangle = horizontalSplitBoxRectangle.Expand(30, 15);
                }
                if (horizontalSplitBoxRectangle.Contains(point))
                {
                    information.HitTestType = HitTestType.ColumnSplitBox;
                    information.ColumnViewportIndex = i;
                    return information;
                }
            }
            for (int j = 0; j < spreadLayout.RowPaneCount; j++)
            {
                Rect verticalSplitBoxRectangle = this.GetVerticalSplitBoxRectangle(j);
                if (flag)
                {
                    verticalSplitBoxRectangle = verticalSplitBoxRectangle.Expand(0, 30);
                }
                else
                {
                    verticalSplitBoxRectangle = verticalSplitBoxRectangle.Expand(15, 30);
                }
                if (verticalSplitBoxRectangle.Expand(15, 30).Contains(point))
                {
                    information.HitTestType = HitTestType.RowSplitBox;
                    information.RowViewportIndex = j;
                    return information;
                }
            }
            for (int k = 0; k < (spreadLayout.ColumnPaneCount - 1); k++)
            {
                if (this.GetHorizontalSplitBarRectangle(k).Expand(10, 10).Contains(point) && (information.HitTestType != HitTestType.ColumnSplitBox))
                {
                    information.HitTestType = HitTestType.ColumnSplitBar;
                    information.ColumnViewportIndex = k;
                }
            }
            for (int m = 0; m < (spreadLayout.RowPaneCount - 1); m++)
            {
                if (this.GetVerticalSplitBarRectangle(m).Expand(10, 10).Contains(point) && (information.HitTestType != HitTestType.RowSplitBox))
                {
                    information.HitTestType = HitTestType.RowSplitBar;
                    information.RowViewportIndex = m;
                }
            }
            if (this.GetTabSplitBoxRectangle().Expand(40, 10).Contains(point))
            {
                information.ColumnViewportIndex = 0;
                information.HitTestType = HitTestType.TabSplitBox;
                return information;
            }
            if (information.HitTestType == HitTestType.Empty)
            {
                if (this.GetTabStripRectangle().Contains(point))
                {
                    information.ColumnViewportIndex = 0;
                    information.HitTestType = HitTestType.TabStrip;
                    return information;
                }
                for (int n = 0; n < spreadLayout.ColumnPaneCount; n++)
                {
                    if (this.GetHorizontalScrollBarRectangle(n).Contains(point))
                    {
                        information.ColumnViewportIndex = n;
                        information.HitTestType = HitTestType.HorizontalScrollBar;
                        return information;
                    }
                }
                for (int num6 = 0; num6 < spreadLayout.RowPaneCount; num6++)
                {
                    if (this.GetVerticalScrollBarRectangle(num6).Contains(point))
                    {
                        information.HitTestType = HitTestType.VerticalScrollBar;
                        information.RowViewportIndex = num6;
                        return information;
                    }
                }
                if (information.HitTestType == HitTestType.Empty)
                {
                    information = base.TouchHitTest(x, y);
                }
            }
            return information;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="currentPoint"></param>
        /// <param name="deltaPoint"></param>
        private void TouchScrollBottom(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            int maxTopScrollableRow = base.GetMaxTopScrollableRow();
            int maxBottomScrollableRow = base.GetMaxBottomScrollableRow();
            RowLayoutModel rowLayoutModel = base.GetRowLayoutModel(base._touchStartHitTestInfo.RowViewportIndex, SheetArea.Cells);
            double num3 = Math.Abs(deltaPoint.Y);
            int viewportTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow <= maxTopScrollableRow)
            {
                Point point = currentPoint.Delta(startPoint);
                base._translateOffsetY = -1.0 * point.Y;
                base._translateOffsetY = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetY), 80.0) * Math.Sign(base._translateOffsetY);
                return;
            }
            if ((viewportTopRow >= maxBottomScrollableRow) && (currentPoint.Y < startPoint.Y))
            {
                return;
            }
            double num5 = 0.0;
            if (rowLayoutModel.FindRow(viewportTopRow) != null)
            {
                if ((base._translateOffsetY + Math.Abs(deltaPoint.Y)) < 0.0)
                {
                    num5 = (rowLayoutModel.FindRow(viewportTopRow).Height + base._translateOffsetY) + Math.Abs(deltaPoint.Y);
                }
                else
                {
                    num5 = 0.0;
                }
            }
            if (num5 >= num3)
            {
                base._translateOffsetY += num3;
                return;
            }
            num3 -= num5;
            if ((num3 + base._translateOffsetY) >= 0.0)
            {
                num3 += base._translateOffsetY;
                base._translateOffsetY = 0.0;
            }
            else
            {
                return;
            }
            int num6 = viewportTopRow - 1;
            while (true)
            {
                if ((num6 < maxTopScrollableRow) || this.Worksheet.Rows[num6].ActualVisible)
                {
                    break;
                }
                num6--;
            }
            if (num6 < maxTopScrollableRow)
            {
                Point point2 = currentPoint.Delta(startPoint);
                base._translateOffsetY = -1.0 * point2.Y;
                base._translateOffsetY = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetY), 80.0) * Math.Sign(base._translateOffsetY);
                return;
            }
            base.SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, num6);
            viewportTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow <= maxTopScrollableRow)
            {
                return;
            }
            int num7 = viewportTopRow;
        Label_0201:
            if (!this.Worksheet.Rows[num7].ActualVisible)
            {
                num7--;
            }
            if (((num7 >= maxTopScrollableRow) && (num7 >= 0)) && (num7 < this.Worksheet.RowCount))
            {
                Row row = this.Worksheet.Rows[num7];
                if (row.ActualVisible)
                {
                    double num8 = row.Height * base.ZoomFactor;
                    if (num8 >= num3)
                    {
                        base._translateOffsetY = num3 - num8;
                        this.GetSpreadLayout();
                        return;
                    }
                    num3 -= num8;
                }
                num7--;
                if (num7 >= maxTopScrollableRow)
                {
                    if (row.ActualVisible)
                    {
                        base.SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, num7);
                    }
                    goto Label_0201;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="currentPoint"></param>
        /// <param name="deltaPoint"></param>
        private void TouchScrollLeft(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            int maxRightScrollableColumn = base.GetMaxRightScrollableColumn();
            ColumnLayoutModel columnLayoutModel = base.GetColumnLayoutModel(base._touchStartHitTestInfo.ColumnViewportIndex, SheetArea.Cells);
            double num2 = Math.Abs(deltaPoint.X);
            int viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn > maxRightScrollableColumn)
            {
                return;
            }
            double num4 = 0.0;
            if (columnLayoutModel.FindColumn(viewportLeftColumn) != null)
            {
                num4 = columnLayoutModel.FindColumn(viewportLeftColumn).Width + base._translateOffsetX;
            }
            if (num4 > num2)
            {
                base._translateOffsetX += -1.0 * num2;
                return;
            }
            num2 -= num4;
            int num5 = viewportLeftColumn + 1;
            while (true)
            {
                if ((num5 > maxRightScrollableColumn) || this.Worksheet.Columns[num5].ActualVisible)
                {
                    break;
                }
                num5++;
            }
            if (num5 > maxRightScrollableColumn)
            {
                base._translateOffsetX += -1.0 * Math.Abs(deltaPoint.X);
                return;
            }
            base.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num5);
            viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn >= maxRightScrollableColumn)
            {
                return;
            }
            int num6 = viewportLeftColumn;
        Label_010B:
            if (num6 >= this.Worksheet.ColumnCount)
            {
                return;
            }
            if (num6 >= 0)
            {
                Column column = this.Worksheet.Columns[num6];
                if (column.ActualVisible)
                {
                    double num7 = column.Width * base.ZoomFactor;
                    if (num7 > num2)
                    {
                        base._translateOffsetX = -1.0 * num2;
                        return;
                    }
                    num2 -= num7;
                }
                num6++;
                if (num6 <= maxRightScrollableColumn)
                {
                    if (column.ActualVisible)
                    {
                        base.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num6);
                    }
                    goto Label_010B;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="currentPoint"></param>
        /// <param name="deltaPoint"></param>
        private void TouchScrollRight(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            int maxLeftScrollableColumn = base.GetMaxLeftScrollableColumn();
            int maxRightScrollableColumn = base.GetMaxRightScrollableColumn();
            ColumnLayoutModel columnLayoutModel = base.GetColumnLayoutModel(base._touchStartHitTestInfo.ColumnViewportIndex, SheetArea.Cells);
            double num3 = Math.Abs(deltaPoint.X);
            int viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn <= maxLeftScrollableColumn)
            {
                Point point = currentPoint.Delta(startPoint);
                base._translateOffsetX = -1.0 * point.X;
                base._translateOffsetX = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetX), 120.0) * Math.Sign(base._translateOffsetX);
                return;
            }
            if ((viewportLeftColumn >= maxRightScrollableColumn) && (currentPoint.X < startPoint.X))
            {
                return;
            }
            double num5 = 0.0;
            if (columnLayoutModel.FindColumn(viewportLeftColumn) != null)
            {
                if ((base._translateOffsetX + Math.Abs(deltaPoint.X)) < 0.0)
                {
                    num5 = (columnLayoutModel.FindColumn(viewportLeftColumn).Width + base._translateOffsetX) + Math.Abs(deltaPoint.X);
                }
                else
                {
                    num5 = 0.0;
                }
            }
            if (num5 >= num3)
            {
                base._translateOffsetX += num3;
                return;
            }
            num3 -= num5;
            if ((num3 + base._translateOffsetX) >= 0.0)
            {
                num3 += base._translateOffsetX;
                base._translateOffsetX = 0.0;
            }
            else
            {
                return;
            }
            int num6 = viewportLeftColumn - 1;
            while (true)
            {
                if ((num6 < maxLeftScrollableColumn) || this.Worksheet.Columns[num6].ActualVisible)
                {
                    break;
                }
                num6--;
            }
            if (num6 < maxLeftScrollableColumn)
            {
                Point point2 = currentPoint.Delta(startPoint);
                base._translateOffsetX = -1.0 * point2.X;
                base._translateOffsetX = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(base._translateOffsetX), 120.0) * Math.Sign(base._translateOffsetX);
                return;
            }
            base.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num6);
            viewportLeftColumn = base.GetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex);
            if (viewportLeftColumn <= maxLeftScrollableColumn)
            {
                return;
            }
            int num7 = viewportLeftColumn;
        Label_0201:
            if (num7 >= this.Worksheet.ColumnCount)
            {
                return;
            }
            if (num7 >= 0)
            {
                Column column = this.Worksheet.Columns[num7];
                if (column.ActualVisible)
                {
                    double num8 = column.Width * base.ZoomFactor;
                    if (num8 >= num3)
                    {
                        base._translateOffsetX = num3 - num8;
                        this.GetSpreadLayout();
                        return;
                    }
                    num3 -= num8;
                }
                num7--;
                if (num7 < maxLeftScrollableColumn)
                {
                    base._translateOffsetX = num3;
                }
                else
                {
                    if (column.ActualVisible)
                    {
                        base.SetViewportLeftColumn(base._touchStartHitTestInfo.ColumnViewportIndex, num7);
                    }
                    goto Label_0201;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="currentPoint"></param>
        /// <param name="deltaPoint"></param>
        private void TouchScrollUp(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            base.GetMaxTopScrollableRow();
            int maxBottomScrollableRow = base.GetMaxBottomScrollableRow();
            RowLayoutModel rowLayoutModel = base.GetRowLayoutModel(base._touchStartHitTestInfo.RowViewportIndex, SheetArea.Cells);
            double num2 = Math.Abs(deltaPoint.Y);
            int viewportTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow > maxBottomScrollableRow)
            {
                return;
            }
            double num4 = 0.0;
            if (rowLayoutModel.FindRow(viewportTopRow) != null)
            {
                num4 = rowLayoutModel.FindRow(viewportTopRow).Height + base._translateOffsetY;
            }
            if (num4 > num2)
            {
                base._translateOffsetY += -1.0 * num2;
                return;
            }
            num2 -= num4;
            int num5 = viewportTopRow + 1;
            while (true)
            {
                if ((num5 > maxBottomScrollableRow) || this.Worksheet.Rows[num5].ActualVisible)
                {
                    break;
                }
                num5++;
            }
            if (num5 > maxBottomScrollableRow)
            {
                base._translateOffsetY += -1.0 * Math.Abs(deltaPoint.Y);
                return;
            }
            base.SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, num5);
            viewportTopRow = base.GetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex);
            if (viewportTopRow >= maxBottomScrollableRow)
            {
                return;
            }
            int num6 = viewportTopRow;
        Label_0112:
            if (num6 < 0)
            {
                return;
            }
            if (num6 < this.Worksheet.RowCount)
            {
                Row row = this.Worksheet.Rows[num6];
                if (row.ActualVisible)
                {
                    double num7 = row.ActualHeight * base.ZoomFactor;
                    if (num7 > num2)
                    {
                        base._translateOffsetY = -1.0 * num2;
                        return;
                    }
                    num2 -= num7;
                }
                num6++;
                if (num6 < maxBottomScrollableRow)
                {
                    if (row.ActualVisible)
                    {
                        base.SetViewportTopRow(base._touchStartHitTestInfo.RowViewportIndex, num6);
                    }
                    goto Label_0112;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="currentPoint"></param>
        /// <param name="deltaPoint"></param>
        private void TouchTabStripScrollLeft(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            TabsPresenter tabsPresenter = this.TabStrip.TabsPresenter;
            int firstScrollableSheetIndex = tabsPresenter.FirstScrollableSheetIndex;
            int lastScrollableSheetIndex = tabsPresenter.LastScrollableSheetIndex;
            int startIndex = tabsPresenter.StartIndex;
            double num2 = Math.Abs(deltaPoint.X);
            if (tabsPresenter.IsLastSheetVisible)
            {
                double num3 = -1.0 * currentPoint.Delta(startPoint).X;
                num3 = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(num3), 120.0) * Math.Sign(num3);
                tabsPresenter.Offset = num3;
                return;
            }
            double num4 = tabsPresenter.FirstSheetTabWidth - Math.Abs(tabsPresenter.Offset);
            if (num4 >= num2)
            {
                tabsPresenter.Offset += -1.0 * num2;
                return;
            }
            num2 -= num4;
            int num5 = startIndex + 1;
            if (num5 > tabsPresenter.LastScrollableSheetIndex)
            {
                tabsPresenter.Offset = -1.0 * Math.Abs(deltaPoint.X);
            }
            if (num5 >= tabsPresenter.LastScrollableSheetIndex)
            {
                return;
            }
            this.SpreadSheet.StartSheetIndex = num5;
        Label_00F0:
            if (num5 >= tabsPresenter.LastScrollableSheetIndex)
            {
                return;
            }
            if (num5 >= 0)
            {
                double num6 = tabsPresenter.FirstSheetTabWidth * base.ZoomFactor;
                if (num6 > num2)
                {
                    tabsPresenter.Offset = -1.0 * num2;
                }
                else
                {
                    num2 -= num6;
                    num5++;
                    if (num5 <= tabsPresenter.LastScrollableSheetIndex)
                    {
                        this.SpreadSheet.StartSheetIndex = num5;
                        goto Label_00F0;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="currentPoint"></param>
        /// <param name="deltaPoint"></param>
        private void TouchTabStripScrollRight(Point startPoint, Point currentPoint, Point deltaPoint)
        {
            int num7;
            double num9;
            TabsPresenter tabsPresenter = this.TabStrip.TabsPresenter;
            int firstScrollableSheetIndex = tabsPresenter.FirstScrollableSheetIndex;
            int lastScrollableSheetIndex = tabsPresenter.LastScrollableSheetIndex;
            int startIndex = tabsPresenter.StartIndex;
            double num4 = Math.Abs(deltaPoint.X);
            if (startIndex <= firstScrollableSheetIndex)
            {
                Point point = currentPoint.Delta(startPoint);
                double num5 = -1.0 * point.X;
                num5 = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(num5), 120.0) * Math.Sign(num5);
                tabsPresenter.Offset = num5;
                return;
            }
            if ((startIndex < lastScrollableSheetIndex) || (currentPoint.X >= startPoint.X))
            {
                double num6 = Math.Abs(tabsPresenter.Offset);
                if (num6 >= num4)
                {
                    tabsPresenter.Offset += num4;
                    return;
                }
                num4 -= num6;
                if ((num4 + tabsPresenter.Offset) >= 0.0)
                {
                    num4 += tabsPresenter.Offset;
                    tabsPresenter.Offset = 0.0;
                    num7 = startIndex - 1;
                    if (num7 < tabsPresenter.FirstScrollableSheetIndex)
                    {
                        Point point2 = currentPoint.Delta(startPoint);
                        double num8 = -1.0 * point2.X;
                        num8 = ManipulationAlgorithm.GetBoundaryFactor(Math.Abs(num8), 120.0) * Math.Sign(num8);
                        tabsPresenter.Offset = num8;
                        return;
                    }
                    if (num7 < tabsPresenter.FirstScrollableSheetIndex)
                    {
                        return;
                    }
                    this.SpreadSheet.StartSheetIndex = num7;
                    goto Label_0154;
                }
            }
            return;
        Label_0154:
            num9 = tabsPresenter.FirstSheetTabWidth * base.ZoomFactor;
            if (num9 >= num4)
            {
                tabsPresenter.Offset = num4 - num9;
            }
            else
            {
                num4 -= num9;
                num7--;
                if (num7 < tabsPresenter.FirstScrollableSheetIndex)
                {
                    tabsPresenter.Offset = num4;
                }
                else
                {
                    this.SpreadSheet.StartSheetIndex = num7;
                    goto Label_0154;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPoint"></param>
        /// <param name="translate"></param>
        /// <param name="scale"></param>
        private void UpdateCachedImageTransform(Point currentPoint, Point translate, double scale)
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            if (base._cachedCornerViewportTransform != null)
            {
                this.UpdateCachedImageTransform(base._cachedCornerViewportTransform, currentPoint, translate, scale, -1, -1, spreadLayout.HeaderX, spreadLayout.HeaderY);
            }
            if (base._cachedColumnHeaderViewportTransform != null)
            {
                for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
                {
                    double viewportX = spreadLayout.GetViewportX(i);
                    double headerY = spreadLayout.HeaderY;
                    this.UpdateCachedImageTransform(base._cachedColumnHeaderViewportTransform[i + 1], currentPoint, translate, scale, i, -1, viewportX, headerY);
                }
            }
            if (base._cachedRowHeaderViewportTransform != null)
            {
                for (int j = -1; j <= spreadLayout.RowPaneCount; j++)
                {
                    double headerX = spreadLayout.HeaderX;
                    double viewportY = spreadLayout.GetViewportY(j);
                    this.UpdateCachedImageTransform(base._cachedRowHeaderViewportTransform[j + 1], currentPoint, translate, scale, -1, j, headerX, viewportY);
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
                        this.UpdateCachedImageTransform(base._cachedViewportTransform[m + 1, k + 1], currentPoint, translate, scale, k, m, columnX, rowY);
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
        private void UpdateCachedImageTransform(TransformGroup transformGruop, Point currentPoint, Point translate, double scale, int columnViewportIndex, int rowViewportIndex, double columnX, double rowY)
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
            SpreadLayout spreadLayout = this.GetSpreadLayout();
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
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            if ((this._crossSplitBar != null) && (((this.ActiveSheet == null) || (this._crossSplitBar.GetLength(0) != (spreadLayout.RowPaneCount - 1))) || (this._crossSplitBar.GetLength(1) != (spreadLayout.ColumnPaneCount - 1))))
            {
                for (int i = 0; i < this._crossSplitBar.GetLength(0); i++)
                {
                    for (int j = 0; j < this._crossSplitBar.GetLength(1); j++)
                    {
                        base.Children.Remove(this._crossSplitBar[i, j]);
                    }
                }
                this._crossSplitBar = null;
            }
            if (((this.ActiveSheet != null) && (this._crossSplitBar == null)) && ((spreadLayout == null) || ((spreadLayout.RowPaneCount >= 1) && (spreadLayout.ColumnPaneCount >= 1))))
            {
                this._crossSplitBar = new CrossSplitBar[spreadLayout.RowPaneCount - 1, spreadLayout.ColumnPaneCount - 1];
                for (int k = 0; k < this._crossSplitBar.GetLength(0); k++)
                {
                    for (int m = 0; m < this._crossSplitBar.GetLength(1); m++)
                    {
                        this._crossSplitBar[k, m] = new CrossSplitBar();
                        Canvas.SetZIndex(this._crossSplitBar[k, m], 2);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateHorizontalScrollBars()
        {
            if (this.ActiveSheet != null)
            {
                SpreadLayout spreadLayout = this.GetSpreadLayout();
                if ((this._horizontalScrollBar != null) && ((this.ActiveSheet == null) || (this._horizontalScrollBar.Length != spreadLayout.ColumnPaneCount)))
                {
                    for (int j = 0; j < this._horizontalScrollBar.Length; j++)
                    {
                        _horizontalScrollBar[j].Scroll -= HorizontalScrollbar_Scroll;
                        _horizontalScrollBar[j].PointerPressed -= OnHorizontalScrollBarPointerPressed;
                        _horizontalScrollBar[j].PointerReleased -= OnHorizontalScrollBarPointerReleased;
                        _horizontalScrollBar[j].PointerExited -= OnHorizontalScrollBarPointerExited;
                        base.Children.Remove(this._horizontalScrollBar[j]);
                    }
                    this._horizontalScrollBar = null;
                }
                if (this._horizontalScrollBar == null)
                {
                    this._horizontalScrollBar = new ScrollBar[spreadLayout.ColumnPaneCount];
                    for (int k = 0; k < spreadLayout.ColumnPaneCount; k++)
                    {
                        this._horizontalScrollBar[k] = new ScrollBar();
                        this._horizontalScrollBar[k].Orientation = (Orientation)1;
                        this._horizontalScrollBar[k].IsTabStop = false;
                        this._horizontalScrollBar[k].TypeSafeSetStyle(this.SpreadSheet.HorizontalScrollBarStyle);
                        _horizontalScrollBar[k].Scroll += HorizontalScrollbar_Scroll;
                        _horizontalScrollBar[k].PointerPressed += OnHorizontalScrollBarPointerPressed;
                        _horizontalScrollBar[k].PointerReleased += OnHorizontalScrollBarPointerReleased;
                        _horizontalScrollBar[k].PointerExited += OnHorizontalScrollBarPointerExited;
                        Canvas.SetZIndex(this._horizontalScrollBar[k], 0x62);
                    }
                }
                int sheetInvisibleColumns = this.GetSheetInvisibleColumns(this.ActiveSheet);
                for (int i = 0; i < spreadLayout.ColumnPaneCount; i++)
                {
                    double num8;
                    int viewportInvisibleColumns = this.GetViewportInvisibleColumns(i);
                    this._horizontalScrollBar[i].Minimum = (double)this.ActiveSheet.FrozenColumnCount;
                    this._horizontalScrollBar[i].Maximum = (double)Math.Max(this.ActiveSheet.FrozenColumnCount, ((this.ActiveSheet.ColumnCount - this.ActiveSheet.FrozenTrailingColumnCount) - sheetInvisibleColumns) - 1);
                    this._horizontalScrollBar[i].ViewportSize = num8 = base.GetViewportColumnLayoutModel(i).Count - viewportInvisibleColumns;
                    this._horizontalScrollBar[i].LargeChange = num8;
                    this._horizontalScrollBar[i].SmallChange = 1.0;
                    int viewportLeftColumn = base.GetViewportLeftColumn(i);
                    viewportLeftColumn = base.TryGetNextScrollableColumn(viewportLeftColumn);
                    int invisibleColumnsBeforeColumn = this.GetInvisibleColumnsBeforeColumn(this.Worksheet, viewportLeftColumn);
                    viewportLeftColumn -= invisibleColumnsBeforeColumn;
                    this._horizontalScrollBar[i].Value = (double)viewportLeftColumn;
                    this._horizontalScrollBar[i].InvalidateArrange();
                    this._horizontalScrollBar[i].IsEnabled = this.HorizontalScrollBarPolicy != 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateHorizontalSplitBars()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            if ((this._horizontalSplitBar != null) && ((this.ActiveSheet == null) || (this._horizontalSplitBar.Length != (spreadLayout.ColumnPaneCount - 1))))
            {
                foreach (HorizontalSplitBar bar in this._horizontalSplitBar)
                {
                    base.Children.Remove(bar);
                }
                this._horizontalSplitBar = null;
            }
            if (((this.ActiveSheet != null) && (this._horizontalSplitBar == null)) && (spreadLayout.ColumnPaneCount >= 1))
            {
                this._horizontalSplitBar = new HorizontalSplitBar[spreadLayout.ColumnPaneCount - 1];
                for (int i = 0; i < this._horizontalSplitBar.Length; i++)
                {
                    this._horizontalSplitBar[i] = new HorizontalSplitBar();
                    Canvas.SetZIndex(this._horizontalSplitBar[i], 2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateHorizontalSplitBoxes()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            if ((this._horizontalSplitBox != null) && ((this.ActiveSheet == null) || (this._horizontalSplitBox.Length != spreadLayout.ColumnPaneCount)))
            {
                for (int i = 0; i < this._horizontalSplitBox.Length; i++)
                {
                    base.Children.Remove(this._horizontalSplitBox[i]);
                }
                this._horizontalSplitBox = null;
            }
            if ((this.ActiveSheet != null) && (this._horizontalSplitBox == null))
            {
                this._horizontalSplitBox = new HorizontalSplitBox[spreadLayout.ColumnPaneCount];
                for (int j = 0; j < spreadLayout.ColumnPaneCount; j++)
                {
                    this._horizontalSplitBox[j] = new HorizontalSplitBox();
                    Canvas.SetZIndex(this._horizontalSplitBox[j], 0x62);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScrollingIndicatorMode"></param>
        private void UpdateScrollBarIndicatorMode(ScrollingIndicatorMode ScrollingIndicatorMode)
        {
            if (this._horizontalScrollBar != null)
            {
                for (int i = 0; i < this._horizontalScrollBar.Length; i++)
                {
                    if (this._horizontalScrollBar[i] != null)
                    {
                        this._horizontalScrollBar[i].IndicatorMode = ScrollingIndicatorMode;
                    }
                }
            }
            if (this._verticalScrollBar != null)
            {
                for (int j = 0; j < this._verticalScrollBar.Length; j++)
                {
                    if (this._verticalScrollBar[j] != null)
                    {
                        this._verticalScrollBar[j].IndicatorMode = ScrollingIndicatorMode;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void UpdateTabStrip()
        {
            if ((this._tabStrip != null) && (this._tabStrip.TabsPresenter != null))
            {
                this._tabStrip.TabsPresenter.InvalidateMeasure();
                this._tabStrip.TabsPresenter.InvalidateArrange();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateVerticalScrollBars()
        {
            if (this.ActiveSheet != null)
            {
                SpreadLayout spreadLayout = this.GetSpreadLayout();
                if ((this._verticalScrollBar != null) && ((this.ActiveSheet == null) || (this._verticalScrollBar.Length != spreadLayout.RowPaneCount)))
                {
                    for (int j = 0; j < this._verticalScrollBar.Length; j++)
                    {
                        _verticalScrollBar[j].Scroll -= VerticalScrollbar_Scroll;
                        _verticalScrollBar[j].PointerPressed -= OnVerticalScrollbarPointerPressed;
                        _verticalScrollBar[j].PointerReleased -= OnVerticalScrollbarPointerReleased;
                        _verticalScrollBar[j].PointerExited -= OnVerticalScrollbarPointerExited;
                        base.Children.Remove(this._verticalScrollBar[j]);
                    }
                    this._verticalScrollBar = null;
                }
                if ((this.ActiveSheet != null) && (this._verticalScrollBar == null))
                {
                    this._verticalScrollBar = new ScrollBar[spreadLayout.RowPaneCount];
                    for (int k = 0; k < this._verticalScrollBar.Length; k++)
                    {
                        this._verticalScrollBar[k] = new ScrollBar();
                        this._verticalScrollBar[k].IsEnabled = true;
                        this._verticalScrollBar[k].Orientation = 0;
                        this._verticalScrollBar[k].ViewportSize = 25.0;
                        this._verticalScrollBar[k].IsTabStop = false;
                        this._verticalScrollBar[k].TypeSafeSetStyle(this.SpreadSheet.VerticalScrollBarStyle);
                        _verticalScrollBar[k].Scroll += VerticalScrollbar_Scroll;
                        _verticalScrollBar[k].PointerPressed += OnVerticalScrollbarPointerPressed;
                        _verticalScrollBar[k].PointerReleased += OnVerticalScrollbarPointerReleased;
                        _verticalScrollBar[k].PointerExited += OnVerticalScrollbarPointerExited;
                        Canvas.SetZIndex(this._verticalScrollBar[k], 0x62);
                    }
                }
                int sheetInvisibleRows = this.GetSheetInvisibleRows(this.ActiveSheet);
                for (int i = 0; i < spreadLayout.RowPaneCount; i++)
                {
                    double num8;
                    int viewportInvisibleRows = this.GetViewportInvisibleRows(i);
                    this._verticalScrollBar[i].Minimum = (double)this.ActiveSheet.FrozenRowCount;
                    this._verticalScrollBar[i].Maximum = (double)Math.Max(this.ActiveSheet.FrozenRowCount, ((this.ActiveSheet.RowCount - this.ActiveSheet.FrozenTrailingRowCount) - sheetInvisibleRows) - 1);
                    this._verticalScrollBar[i].ViewportSize = num8 = base.GetViewportRowLayoutModel(i).Count - viewportInvisibleRows;
                    this._verticalScrollBar[i].LargeChange = num8;
                    this._verticalScrollBar[i].SmallChange = 1.0;
                    int viewportTopRow = base.GetViewportTopRow(i);
                    viewportTopRow = base.TryGetNextScrollableRow(viewportTopRow);
                    int invisibleRowsBeforeRow = this.GetInvisibleRowsBeforeRow(this.Worksheet, viewportTopRow);
                    viewportTopRow -= invisibleRowsBeforeRow;
                    this._verticalScrollBar[i].Value = (double)viewportTopRow;
                    this._verticalScrollBar[i].InvalidateArrange();
                    this._verticalScrollBar[i].IsEnabled = this.VerticalScrollBarPolicy != 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateVerticalSplitBars()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            if ((this._verticalSplitBar != null) && ((this.ActiveSheet == null) || (this._verticalSplitBar.Length != (spreadLayout.RowPaneCount - 1))))
            {
                foreach (VerticalSplitBar bar in this._verticalSplitBar)
                {
                    base.Children.Remove(bar);
                }
                this._verticalSplitBar = null;
            }
            if (((this.ActiveSheet != null) && (this._verticalSplitBar == null)) && (spreadLayout.RowPaneCount >= 1))
            {
                this._verticalSplitBar = new VerticalSplitBar[spreadLayout.RowPaneCount - 1];
                for (int i = 0; i < this._verticalSplitBar.Length; i++)
                {
                    this._verticalSplitBar[i] = new VerticalSplitBar();
                    Canvas.SetZIndex(this._verticalSplitBar[i], 2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateVerticalSplitBoxes()
        {
            SpreadLayout spreadLayout = this.GetSpreadLayout();
            if ((this._verticalSplitBox != null) && ((this.ActiveSheet == null) || (this._verticalSplitBox.Length != spreadLayout.RowPaneCount)))
            {
                foreach (VerticalSplitBox box in this._verticalSplitBox)
                {
                    base.Children.Remove(box);
                }
                this._verticalSplitBox = null;
            }
            if ((this.ActiveSheet != null) && (this._verticalSplitBox == null))
            {
                this._verticalSplitBox = new VerticalSplitBox[spreadLayout.RowPaneCount];
                for (int i = 0; i < spreadLayout.RowPaneCount; i++)
                {
                    this._verticalSplitBox[i] = new VerticalSplitBox();
                    Canvas.SetZIndex(this._verticalSplitBox[i], 0x62);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateViewport()
        {
            bool flag = ((base._touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject) && (base._touchStartHitTestInfo.FloatingObjectInfo.FloatingObject != null)) && base._touchStartHitTestInfo.FloatingObjectInfo.FloatingObject.IsSelected;
            if (((base._touchStartHitTestInfo != null) && (base._touchStartHitTestInfo.HitTestType == HitTestType.Viewport)) || ((base._touchStartHitTestInfo.HitTestType == HitTestType.FloatingObject) && !flag))
            {
                this.AdjustViewportLeftColumn();
                this.AdjustViewportTopRow();
                this.AdjustViewportSize();
                base._translateOffsetY = 0.0;
                base._translateOffsetX = 0.0;
                if (base._updateViewportAfterTouch)
                {
                    this.InvalidateLayout();
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerticalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            if ((base._touchToolbarPopup != null) && base._touchToolbarPopup.IsOpen)
            {
                base._touchToolbarPopup.IsOpen = false;
            }
            if (((this.ScrollBarTrackPolicy == ScrollBarTrackPolicy.Both) || (this.ScrollBarTrackPolicy == ScrollBarTrackPolicy.Vertical)) || (base._isTouchScrolling || (e.ScrollEventType != (ScrollEventType)5)))
            {
                for (int i = 0; i < this._verticalScrollBar.Length; i++)
                {
                    if (sender == this._verticalScrollBar[i])
                    {
                        if (base.VerticalScrollable)
                        {
                            this.ProcessVerticalScroll(i, e);
                            return;
                        }
                        this._verticalScrollBar[i].Value = (double)base.GetViewportTopRow(i);
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
        private void VerticalScrollBarTouchSmallDecrement(int rowViewportIndex, int newValue)
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
            if (this._horizontalScrollBarHeight != 25.0)
            {
                Serializer.SerializeObj((double)this._horizontalScrollBarHeight, "HorizontalScrollBarHeight", writer);
            }
            if (this._verticalScrollBarWidth != 25.0)
            {
                Serializer.SerializeObj((double)this._verticalScrollBarWidth, "VerticalScrollBarWidth", writer);
            }
            if (this._scrollBarTrackPolicy != ScrollBarTrackPolicy.Both)
            {
                Serializer.SerializeObj(this._scrollBarTrackPolicy, "ScrollBarTrackPolicy", writer);
            }
            if (this._columnSplitBoxAlignment != SplitBoxAlignment.Leading)
            {
                Serializer.SerializeObj(this._columnSplitBoxAlignment, "ColumnSplitBoxAlignment", writer);
            }
            if (this._rowSplitBoxAlignment != SplitBoxAlignment.Leading)
            {
                Serializer.SerializeObj(this._rowSplitBoxAlignment, "RowSplitBoxAlignment", writer);
            }
            if (this._tabStripVisibility != 0)
            {
                Serializer.SerializeObj(this._tabStripVisibility, "TabStripVisibility", writer);
            }
            if (!this._tabStripEditable)
            {
                Serializer.SerializeObj((bool)this._tabStripEditable, "TabStripEditable", writer);
            }
            if (this._tabStripRatio != 0.5)
            {
                Serializer.SerializeObj((double)this._tabStripRatio, "TabStripRadio", writer);
            }
            if (!this._tabStripInsertTab)
            {
                Serializer.SerializeObj((bool)this._tabStripInsertTab, "TabStripInsertTab", writer);
            }
            if (this._columnSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                Serializer.SerializeObj(this._columnSplitBoxPolicy, "ColumnSplitBoxPolicy", writer);
            }
            if (this._rowSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                Serializer.SerializeObj(this._rowSplitBoxPolicy, "RowSplitBoxPolicy", writer);
            }
        }
    }
}
