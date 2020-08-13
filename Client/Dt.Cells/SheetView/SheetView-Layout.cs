using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Dt.Cells.UI
{
    public partial class SheetView
    {

#if IOS
        new
#endif
        void Init()
        {
            _trailingFreezeLineStyle = null;
            _showFreezeLine = true;
            _protect = false;
            _vScrollable = true;
            _hScrollable = true;
            _cachedColumnHeaderRowLayoutModel = null;
            _cachedViewportRowLayoutModel = null;
            _cachedRowHeaderColumnLayoutModel = null;
            _cachedViewportColumnLayoutModel = null;
            _cachedColumnHeaderViewportColumnLayoutModel = null;
            _cachedRowHeaderViewportRowLayoutModel = null;
            _cachedRowHeaderCellLayoutModel = null;
            _cachedColumnHeaderCellLayoutModel = null;
            _cachedViewportCellLayoutModel = null;
            _cachedGroupLayout = null;
            _cachedFloatingObjectLayoutModel = null;
            _cornerPanel = null;
            _rowHeaders = null;
            _colHeaders = null;
            _cellsPanels = null;
            _groupCornerPresenter = null;
            _rowGroupHeaderPresenter = null;
            _columnGroupHeaderPresenter = null;
            _rowGroupPresenters = null;
            _columnGroupPresenters = null;
            _showColumnRangeGroup = true;
            _showRowRangeGroup = true;
            _shapeDrawingContainer = null;
            _trackersContainer = null;
            _columnFreezeLine = null;
            _rowFreezeLine = null;
            _columnTrailingFreezeLine = null;
            _rowTrailingFreezeLine = null;
            _resizingTracker = null;
            _currentActiveColumnIndex = 0;
            _currentActiveRowIndex = 0;
            _verticalSelectionMgr = null;
            _horizontalSelectionMgr = null;
            _keyMap = null;
            _floatingObjectsKeyMap = null;
            _availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            _isEditing = false;
            _isDoubleClick = false;
            _positionInfo = null;
            _navigation = null;
            _undoManager = null;
            _eventSuspended = 0;
            _lastClickPoint = new Point();
            _lastClickLocation = new Point(-1.0, -1.0);
            _hoverManager = new HoverManager(this);
            _dragDropIndicator = null;
            _dragDropInsertIndicator = null;
            _dragDropFromRange = null;
            _dragDropColumnOffset = 0;
            _dragDropRowOffset = 0;
            _isDragInsert = false;
            _isDragCopy = false;
            _dragStartRowViewport = -2;
            _dragStartColumnViewport = -2;
            _dragToColumnViewport = -2;
            _dragToRowViewport = -2;
            _dragToColumn = -2;
            _dragToRow = -2;
            _mouseCursor = null;
            _tooltipHelper = null;
            _filterPopupHelper = null;
            _dataValidationPopUpHelper = null;
            _inputDeviceType = InputDeviceType.Mouse;
            _resizeZeroIndicator = ResizeZeroIndicator.Default;
            _cachedResizerGipper = new Dictionary<string, BitmapImage>();
            _cachedToolbarImageSources = new Dictionary<string, ImageSource>();

            _horizontalScrollBarHeight = 25.0;
            _verticalScrollBarWidth = 25.0;
            _horizontalScrollBarStyle = null;
            _verticalScrollBarStyle = null;
            _scrollBarTrackPolicy = ScrollBarTrackPolicy.Both;
            _columnSplitBoxAlignment = SplitBoxAlignment.Leading;
            _rowSplitBoxAlignment = SplitBoxAlignment.Leading;
            _tabStripEditable = true;
            _tabStripInsertTab = true;
            _tabStripVisibility = 0;
            _tabStripRatio = 0.5;
            _cachedLastAvailableSize = new Size(0.0, 0.0);
            _columnSplitBoxPolicy = SplitBoxPolicy.Always;
            _rowSplitBoxPolicy = SplitBoxPolicy.Always;

            _progressGrid = new Grid();
            _progressRing = new ProgressRing();
            if (!_progressGrid.Children.Contains(_progressRing))
            {
                _progressRing.Foreground = new SolidColorBrush(Colors.Black);
                _progressRing.IsActive = true;
                _progressRing.Visibility = (Visibility)1;
                _progressRing.Width = 200.0;
                _progressRing.Height = 200.0;
                _progressGrid.Children.Add(_progressRing);
            }

            _showScrollTip = false;
            FormulaSelectionGripperContainerPanel panel = new FormulaSelectionGripperContainerPanel
            {
                ParentSheet = this
            };
            panel.IsHitTestVisible = false;
            _formulaSelectionGripperPanel = panel;

            _topLeftGripper = new Ellipse();
            _topLeftGripper.Stroke = new SolidColorBrush(Color.FromArgb(220, 0, 0, 0));
            _topLeftGripper.StrokeThickness = 2.0;
            _topLeftGripper.Fill = new SolidColorBrush(Colors.White);
            _topLeftGripper.Height = 16.0;
            _topLeftGripper.Width = 16.0;

            _bottomRightGripper = new Ellipse();
            _bottomRightGripper.Stroke = new SolidColorBrush(Color.FromArgb(220, 0, 0, 0));
            _bottomRightGripper.StrokeThickness = 2.0;
            _bottomRightGripper.Fill = new SolidColorBrush(Colors.White);
            _bottomRightGripper.Height = 16.0;
            _bottomRightGripper.Width = 16.0;

            _resizerGripperContainer = new Border();
            _resizerGripperContainer.Width = 16.0;
            _resizerGripperContainer.Height = 16.0;
            _autoFillIndicatorContainer = new Border();
            _autoFillIndicatorContainer.Width = 16.0;
            _autoFillIndicatorContainer.Height = 16.0;

            _cachedColumnResizerGripperImage = new Image();
            _resizerGripperContainer.Child = _cachedColumnResizerGripperImage;
            _cachedRowResizerGripperImage = new Image();
            _cachedResizerGipper = new Dictionary<string, BitmapImage>();
            _cachedautoFillIndicatorImage = new Image();
            _autoFillIndicatorContainer.Child = _cachedautoFillIndicatorImage;
            _cachedToolbarImageSources = new Dictionary<string, ImageSource>();
        }

        internal void InvalidateLayout()
        {
            if (!IsSuspendInvalidate())
            {
                _cachedLayout = null;
                _cachedViewportRowLayoutModel = null;
                _cachedViewportColumnLayoutModel = null;
                _cachedColumnHeaderRowLayoutModel = null;
                _cachedColumnHeaderViewportColumnLayoutModel = null;
                _cachedRowHeaderViewportRowLayoutModel = null;
                _cachedRowHeaderColumnLayoutModel = null;
                _cachedViewportCellLayoutModel = null;
                _cachedColumnHeaderCellLayoutModel = null;
                _cachedRowHeaderCellLayoutModel = null;
                _cachedGroupLayout = null;
                _cachedFilterButtonInfoModel = null;
                _cachedFloatingObjectLayoutModel = null;
            }
        }

        internal ViewportInfo GetViewportInfo(Worksheet p_sheet = null)
        {
            if (p_sheet != null)
                return p_sheet.GetViewportInfo();

            var sheet = ActiveSheet;
            if (sheet != null)
                return sheet.GetViewportInfo();

            return new ViewportInfo();
        }

        internal SheetLayout GetSheetLayout()
        {
            if (_cachedLayout == null)
            {
                _cachedLayout = CreateLayout();
                UpdateHorizontalScrollBars();
                UpdateVerticalScrollBars();
            }
            return _cachedLayout;
        }

        internal bool IsSuspendInvalidate()
        {
            return (_suspendViewInvalidate > 0);
        }

        internal void ResumeInvalidate()
        {
            _suspendViewInvalidate--;
            if (_suspendViewInvalidate < 0)
            {
                _suspendViewInvalidate = 0;
            }
            ResumeFloatingObjectsInvalidate();
        }

        internal void SuspendInvalidate()
        {
            _suspendViewInvalidate++;
            SuspendFloatingObjectsInvalidate();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double viewportX;
            double viewportY;
            if (_cachedLastAvailableSize != availableSize)
            {
                _cachedLastAvailableSize = availableSize;
                _availableSize = availableSize;
                InvalidateLayout();
            }
            if (!IsWorking)
            {
                SaveHitInfo(null);
            }

            SheetLayout layout = GetSheetLayout();
            List<Image> list = new List<Image>();
            foreach (var element in Children.OfType<Image>())
            {
                list.Add(element);
            }
            foreach (var element2 in list)
            {
                Children.Remove(element2);
            }

            UpdateHorizontalSplitBoxes();
            UpdateVerticalSplitBoxes();
            UpdateHorizontalSplitBars();
            UpdateVerticalSplitBars();
            UpdateCrossSplitBars();
            UpdateFreezeLines();

            // 跟踪层，调整行列宽高时的虚线，拖拽时的标志，冻结线
            if (!Children.Contains(TrackersContainer))
            {
                Children.Add(TrackersContainer);
            }
            TrackersContainer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            // 分隔栏层，多个GcViewport时的分割线
            if (!Children.Contains(SplittingTrackerContainer))
            {
                Children.Add(SplittingTrackerContainer);
            }
            SplittingTrackerContainer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            // 光标层，各种图标的png图片
            if (!Children.Contains(CursorsContainer))
            {
                Children.Add(CursorsContainer);
            }
            CursorsContainer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            // 多视口
            CellsPanel[,] viewportArray = null;
            if ((_cellsPanels != null)
                && (ActiveSheet == null
                    || _cellsPanels.GetUpperBound(0) != layout.RowPaneCount + 1
                    || _cellsPanels.GetUpperBound(1) != layout.ColumnPaneCount + 1))
            {
                CellsPanel[,] viewportArray2 = _cellsPanels;
                int upperBound = viewportArray2.GetUpperBound(0);
                int num29 = viewportArray2.GetUpperBound(1);
                for (int n = viewportArray2.GetLowerBound(0); n <= upperBound; n++)
                {
                    for (int num31 = viewportArray2.GetLowerBound(1); num31 <= num29; num31++)
                    {
                        CellsPanel viewport = viewportArray2[n, num31];
                        if (viewport != null)
                        {
                            viewport.RemoveDataValidationUI();
                        }
                        Children.Remove(viewport);
                    }
                }
                viewportArray = _cellsPanels;
                _cellsPanels = null;
            }
            if (_cellsPanels == null)
            {
                _cellsPanels = new CellsPanel[layout.RowPaneCount + 2, layout.ColumnPaneCount + 2];
            }
            for (int i = -1; i <= layout.ColumnPaneCount; i++)
            {
                double viewportWidth = layout.GetViewportWidth(i);
                viewportX = layout.GetViewportX(i);
                for (int num5 = -1; num5 <= layout.RowPaneCount; num5++)
                {
                    double viewportHeight = layout.GetViewportHeight(num5);
                    viewportY = layout.GetViewportY(num5);
                    if (((_cellsPanels[num5 + 1, i + 1] == null) && (viewportWidth > 0.0)) && (viewportHeight > 0.0))
                    {
                        if (((viewportArray != null) && ((num5 + 1) < viewportArray.GetUpperBound(0))) && (((i + 1) < viewportArray.GetUpperBound(1)) && (viewportArray[num5 + 1, i + 1] != null)))
                        {
                            _cellsPanels[num5 + 1, i + 1] = viewportArray[num5 + 1, i + 1];
                        }
                        else
                        {
                            _cellsPanels[num5 + 1, i + 1] = new CellsPanel(this);
                        }
                    }
                    CellsPanel viewport2 = _cellsPanels[num5 + 1, i + 1];
                    if ((viewportWidth > 0.0) && (viewportHeight > 0.0))
                    {
                        viewport2.Location = new Point(viewportX, viewportY);
                        viewport2.ColumnViewportIndex = i;
                        viewport2.RowViewportIndex = num5;
                        if (!Children.Contains(viewport2))
                        {
                            Children.Add(viewport2);
                        }
                        viewport2.InvalidateMeasure();
                        viewport2.Measure(new Size(viewportWidth, viewportHeight));
                    }
                    else if (viewport2 != null)
                    {
                        Children.Remove(viewport2);
                        _cellsPanels[num5 + 1, i + 1] = null;
                    }
                }
            }

            // 行头，一个GcViewport对应一个行头
            if ((_rowHeaders != null) && ((ActiveSheet == null) || (_rowHeaders.Length != (layout.RowPaneCount + 2))))
            {
                foreach (var header in _rowHeaders)
                {
                    Children.Remove(header);
                }
                _rowHeaders = null;
            }
            if (_rowHeaders == null)
            {
                _rowHeaders = new RowHeaderPanel[layout.RowPaneCount + 2];
            }
            if (layout.HeaderWidth > 0.0)
            {
                for (int i = -1; i <= layout.RowPaneCount; i++)
                {
                    double height = layout.GetViewportHeight(i);
                    viewportY = layout.GetViewportY(i);
                    if ((_rowHeaders[i + 1] == null) && (height > 0.0))
                    {
                        _rowHeaders[i + 1] = new RowHeaderPanel(this);
                    }
                    var header = _rowHeaders[i + 1];
                    if (height > 0.0)
                    {
                        header.Location = new Point(layout.HeaderX, viewportY);
                        header.RowViewportIndex = i;
                        if (!Children.Contains(header))
                        {
                            Children.Add(header);
                        }
                        header.InvalidateMeasure();
                        header.Measure(new Size(layout.HeaderWidth, height));
                    }
                    else if (header != null)
                    {
                        Children.Remove(header);
                        _rowHeaders[i + 1] = null;
                    }
                }
            }
            else if (_rowHeaders != null)
            {
                foreach (var header in _rowHeaders)
                {
                    Children.Remove(header);
                }
            }

            // 列头
            if ((_colHeaders != null) && ((ActiveSheet == null) || (_colHeaders.Length != (layout.ColumnPaneCount + 2))))
            {
                foreach (var panel in _colHeaders)
                {
                    Children.Remove(panel);
                }
                _colHeaders = null;
            }
            if (_colHeaders == null)
            {
                _colHeaders = new ColHeaderPanel[layout.ColumnPaneCount + 2];
            }
            if (layout.HeaderHeight > 0.0)
            {
                for (int i = -1; i <= layout.ColumnPaneCount; i++)
                {
                    viewportX = layout.GetViewportX(i);
                    double width = layout.GetViewportWidth(i);
                    if ((_colHeaders[i + 1] == null) && (width > 0.0))
                    {
                        _colHeaders[i + 1] = new ColHeaderPanel(this);
                    }
                    var colPanel = _colHeaders[i + 1];
                    if (width > 0.0)
                    {
                        colPanel.Location = new Point(viewportX, layout.HeaderY);
                        colPanel.ColumnViewportIndex = i;
                        if (!Children.Contains(colPanel))
                        {
                            Children.Add(colPanel);
                        }
                        colPanel.InvalidateMeasure();
                        colPanel.Measure(new Size(width, layout.HeaderHeight));
                    }
                    else if (colPanel != null)
                    {
                        Children.Remove(colPanel);
                        _colHeaders[i + 1] = null;
                    }
                }
            }
            else if (_colHeaders != null)
            {
                foreach (var colPanel in _colHeaders)
                {
                    Children.Remove(colPanel);
                }
            }

            // 左上角
            if (_cornerPanel == null)
            {
                _cornerPanel = new CornerPanel(this);
            }
            if ((layout.HeaderWidth > 0.0) && (layout.HeaderHeight > 0.0))
            {
                if (!Children.Contains(_cornerPanel))
                {
                    Children.Add(_cornerPanel);
                }
                _cornerPanel.Measure(new Size(layout.HeaderWidth, layout.HeaderHeight));
            }
            else
            {
                Children.Remove(_cornerPanel);
                _cornerPanel = null;
            }

            // 水平滚动栏
            if (layout.OrnamentHeight > 0.0)
            {
                for (int num11 = 0; num11 < layout.ColumnPaneCount; num11++)
                {
                    ScrollBar bar = _horizontalScrollBar[num11];
                    if (layout.GetHorizontalScrollBarWidth(num11) > 0.0)
                    {
                        if (!Children.Contains(bar))
                        {
                            Children.Add(bar);
                        }
                    }
                    else
                    {
                        Children.Remove(bar);
                    }
                    HorizontalSplitBox box = _horizontalSplitBox[num11];
                    if (layout.GetHorizontalSplitBoxWidth(num11) > 0.0)
                    {
                        if (!Children.Contains(box))
                        {
                            Children.Add(box);
                        }
                    }
                    else
                    {
                        Children.Remove(box);
                    }
                }
            }
            else
            {
                if (_horizontalScrollBar != null)
                {
                    foreach (ScrollBar bar2 in _horizontalScrollBar)
                    {
                        Children.Remove(bar2);
                    }
                }
                if (_horizontalSplitBox != null)
                {
                    foreach (HorizontalSplitBox box2 in _horizontalSplitBox)
                    {
                        Children.Remove(box2);
                    }
                }
            }

            // sheet标签 
            if (layout.TabStripHeight > 0.0)
            {
                if (_tabStrip == null)
                {
                    _tabStrip = new TabStrip();
                    // hdt 应用构造前的设置
                    if (_tabStripVisibility == Visibility.Collapsed)
                        _tabStrip.Visibility = Visibility.Collapsed;
                    _tabStrip.HasInsertTab = Excel.TabStripInsertTab;
                    _tabStrip.OwningView = this;
                    Canvas.SetZIndex(_tabStrip, 0x62);
                }
                else
                {
                    _tabStrip.Update();
                }
                _tabStrip.ActiveTabChanging -= new EventHandler(OnTabStripActiveTabChanging);
                _tabStrip.ActiveTabChanged -= new EventHandler(OnTabStripActiveTabChanged);
                _tabStrip.NewTabNeeded -= new EventHandler(OnTabStripNewTabNeeded);
                _tabStrip.AddSheets(Excel.Sheets);
                if (!Children.Contains(_tabStrip))
                {
                    Children.Add(_tabStrip);
                }
                int activeSheetIndex = Excel.ActiveSheetIndex;
                if ((activeSheetIndex >= 0) && (activeSheetIndex < Excel.Sheets.Count))
                {
                    _tabStrip.ActiveSheet(activeSheetIndex, false);
                }
                _tabStrip.SetStartSheet(Excel.StartSheetIndex);
                _tabStrip.InvalidateMeasure();
                _tabStrip.Measure(new Size(layout.TabStripWidth, layout.TabStripHeight));
                _tabStrip.ActiveTabChanging += new EventHandler(OnTabStripActiveTabChanging);
                _tabStrip.ActiveTabChanged += new EventHandler(OnTabStripActiveTabChanged);
                _tabStrip.NewTabNeeded += new EventHandler(OnTabStripNewTabNeeded);
                if (tabStripSplitBox == null)
                {
                    tabStripSplitBox = new TabStripSplitBox();
                    Canvas.SetZIndex(tabStripSplitBox, 0x62);
                }
                if (!Children.Contains(tabStripSplitBox))
                {
                    Children.Add(tabStripSplitBox);
                }
                tabStripSplitBox.InvalidateMeasure();
                tabStripSplitBox.Measure(new Size(layout.TabSplitBoxWidth, layout.OrnamentHeight));
            }
            else if (_tabStrip != null)
            {
                Children.Remove(_tabStrip);
                _tabStrip.ActiveTabChanging -= new EventHandler(OnTabStripActiveTabChanging);
                _tabStrip.ActiveTabChanged -= new EventHandler(OnTabStripActiveTabChanged);
                _tabStrip = null;
                Children.Remove(tabStripSplitBox);
                tabStripSplitBox = null;
            }

            // 水平/垂直多视窗分隔
            if (layout.OrnamentWidth > 0.0)
            {
                for (int num15 = 0; num15 < layout.RowPaneCount; num15++)
                {
                    ScrollBar bar3 = _verticalScrollBar[num15];
                    if (GetSheetLayout().GetVerticalScrollBarHeight(num15) > 0.0)
                    {
                        if (!Children.Contains(bar3))
                        {
                            Children.Add(bar3);
                        }
                    }
                    else
                    {
                        Children.Remove(bar3);
                    }
                    VerticalSplitBox box3 = _verticalSplitBox[num15];
                    if (layout.GetVerticalSplitBoxHeight(num15) > 0.0)
                    {
                        if (!Children.Contains(box3))
                        {
                            Children.Add(box3);
                        }
                    }
                    else
                    {
                        Children.Remove(box3);
                    }
                }
            }
            else
            {
                if (_verticalScrollBar != null)
                {
                    foreach (ScrollBar bar4 in _verticalScrollBar)
                    {
                        Children.Remove(bar4);
                    }
                }
                if (_verticalSplitBox != null)
                {
                    foreach (VerticalSplitBox box4 in _verticalSplitBox)
                    {
                        Children.Remove(box4);
                    }
                }
            }
            for (int j = 0; j < (layout.ColumnPaneCount - 1); j++)
            {
                HorizontalSplitBar bar5 = _horizontalSplitBar[j];
                double horizontalSplitBoxWidth = layout.GetHorizontalSplitBoxWidth(j);
                double num20 = _availableSize.Height;
                if (!Children.Contains(bar5))
                {
                    Children.Add(bar5);
                }
                bar5.Measure(new Size(horizontalSplitBoxWidth, num20));
            }
            for (int k = 0; k < (layout.RowPaneCount - 1); k++)
            {
                VerticalSplitBar bar6 = _verticalSplitBar[k];
                double num22 = _availableSize.Width;
                double verticalSplitBarHeight = layout.GetVerticalSplitBarHeight(k);
                if (!Children.Contains(bar6))
                {
                    Children.Add(bar6);
                }
                bar6.Measure(new Size(num22, verticalSplitBarHeight));
            }
            for (int m = 0; m < (layout.RowPaneCount - 1); m++)
            {
                for (int num25 = 0; num25 < (layout.ColumnPaneCount - 1); num25++)
                {
                    CrossSplitBar bar7 = _crossSplitBar[m, num25];
                    double num26 = layout.GetHorizontalSplitBoxWidth(num25);
                    double num27 = layout.GetVerticalSplitBarHeight(m);
                    if (!Children.Contains(bar7))
                    {
                        Children.Add(bar7);
                    }
                    bar7.Measure(new Size(num26, num27));
                }
            }
            MeasureRangeGroup(layout.RowPaneCount, layout.ColumnPaneCount, layout);

            // 进度环
            if (!Children.Contains(_progressGrid))
            {
                Children.Add(_progressGrid);
            }
            _progressGrid.Measure(availableSize);

            Children.Remove(_topLeftGripper);
            Children.Remove(_bottomRightGripper);
            Children.Remove(_resizerGripperContainer);
            Children.Remove(_autoFillIndicatorContainer);
            if (_formulaSelectionGripperPanel != null)
            {
                Children.Remove(_formulaSelectionGripperPanel);
            }
            _topLeftGripper.Stroke = new SolidColorBrush(GetGripperStrokeColor());
            _topLeftGripper.Fill = new SolidColorBrush(GetGripperFillColor());
            _bottomRightGripper.Stroke = new SolidColorBrush(GetGripperStrokeColor());
            _bottomRightGripper.Fill = new SolidColorBrush(GetGripperFillColor());
            Children.Add(_topLeftGripper);
            Children.Add(_bottomRightGripper);
            Children.Add(_resizerGripperContainer);
            _autoFillIndicatorContainer.Width = 16.0;
            _autoFillIndicatorContainer.Height = 16.0;
            Children.Add(_autoFillIndicatorContainer);
            if (_formulaSelectionGripperPanel != null)
            {
                Children.Add(_formulaSelectionGripperPanel);
            }
            _cachedColumnResizerGripperImage.Source = GetResizerBitmapImage(false);
            _cachedRowResizerGripperImage.Source = GetResizerBitmapImage(true);
            _cachedautoFillIndicatorImage.Source = GetImageSource("AutoFillIndicator.png");
            Canvas.SetZIndex(_topLeftGripper, 90);
            Canvas.SetZIndex(_bottomRightGripper, 90);
            Canvas.SetZIndex(_resizerGripperContainer, 90);
            Canvas.SetZIndex(_autoFillIndicatorContainer, 90);
            if (_formulaSelectionGripperPanel != null)
            {
                Canvas.SetZIndex(_formulaSelectionGripperPanel, 90);
            }
            if (_formulaSelectionGripperPanel != null)
            {
                _formulaSelectionGripperPanel.Measure(new Size(double.MaxValue, double.MaxValue));
            }
            return _availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double headerX;
            double headerY;
            SheetLayout layout = GetSheetLayout();
            TrackersContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            SplittingTrackerContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            ShapeDrawingContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            CursorsContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            if ((IsTouchZooming && (_cornerPanel != null)) && (_cornerPanel.Parent != null))
            {
                headerX = layout.HeaderX;
                headerY = layout.HeaderY;
                _cornerPanel.Arrange(new Rect(headerX, headerY, layout.HeaderWidth, layout.HeaderHeight));
                _cornerPanel.RenderTransform = _cachedCornerViewportTransform;
            }
            else if ((_cornerPanel != null) && (_cornerPanel.Parent != null))
            {
                headerX = layout.HeaderX;
                headerY = layout.HeaderY;
                if (_cornerPanel.RenderTransform != null)
                {
                    _cornerPanel.RenderTransform = null;
                }
                if ((_cornerPanel.Width != layout.HeaderWidth) || (_cornerPanel.Height != layout.HeaderHeight))
                {
                    _cornerPanel.Arrange(new Rect(headerX, headerY, layout.HeaderWidth, layout.HeaderHeight));
                }
            }
            if (IsTouchZooming && (_cachedColumnHeaderViewportTransform != null))
            {
                for (int i = -1; i <= layout.ColumnPaneCount; i++)
                {
                    headerX = layout.GetViewportX(i);
                    headerY = layout.HeaderY;
                    double viewportWidth = layout.GetViewportWidth(i);
                    double headerHeight = layout.HeaderHeight;
                    var viewport = _colHeaders[i + 1];
                    if ((viewport != null) && (viewport.Parent != null))
                    {
                        viewport.Arrange(new Rect(headerX, headerY, viewportWidth, headerHeight));
                        viewport.RenderTransform = _cachedColumnHeaderViewportTransform[i + 1];
                    }
                }
            }
            else if (_colHeaders != null)
            {
                for (int j = -1; j <= layout.ColumnPaneCount; j++)
                {
                    headerX = layout.GetViewportX(j);
                    if ((IsTouching && (j == layout.ColumnPaneCount)) && ((_translateOffsetX < 0.0) && (_touchStartHitTestInfo.ColumnViewportIndex == (layout.ColumnPaneCount - 1))))
                    {
                        headerX += _translateOffsetX;
                    }
                    headerY = layout.HeaderY;
                    double width = layout.GetViewportWidth(j);
                    double height = layout.HeaderHeight;
                    var viewport2 = _colHeaders[j + 1];
                    if ((viewport2 != null) && (viewport2.Parent != null))
                    {
                        if (viewport2.RenderTransform != null)
                        {
                            viewport2.RenderTransform = null;
                        }
                        if ((viewport2.Width != width) || (viewport2.Height != height))
                        {
                            if (!IsTouching)
                            {
                                viewport2.Arrange(new Rect(headerX, headerY, width, height));
                            }
                            else
                            {
                                int num9 = (int)Math.Ceiling(_translateOffsetX);
                                double x = headerX;
                                if ((_touchStartHitTestInfo != null) && (j == _touchStartHitTestInfo.ColumnViewportIndex))
                                {
                                    x += num9;
                                }
                                viewport2.Arrange(new Rect(x, headerY, width, height));
                                if ((x != headerX) && (_translateOffsetX < 0.0))
                                {
                                    RectangleGeometry geometry = new RectangleGeometry();
                                    geometry.Rect = new Rect(Math.Abs((double)(headerX - x)), 0.0, _cachedViewportWidths[j + 1], height);
                                    viewport2.Clip = geometry;
                                }
                                else if ((x != headerX) && (_translateOffsetX > 0.0))
                                {
                                    RectangleGeometry geometry2 = new RectangleGeometry();
                                    geometry2.Rect = new Rect(0.0, 0.0, Math.Max((double)0.0, (double)(_cachedViewportWidths[j + 1] - _translateOffsetX)), height);
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
            if (IsTouchZooming && (_cachedRowHeaderViewportTransform != null))
            {
                for (int k = -1; k <= layout.RowPaneCount; k++)
                {
                    headerX = layout.HeaderX;
                    headerY = layout.GetViewportY(k);
                    double headerWidth = layout.HeaderWidth;
                    double viewportHeight = layout.GetViewportHeight(k);
                    var header = _rowHeaders[k + 1];
                    if ((header != null) && (header.Parent != null))
                    {
                        header.Arrange(new Rect(headerX, headerY, headerWidth, viewportHeight));
                        header.RenderTransform = _cachedRowHeaderViewportTransform[k + 1];
                    }
                }
            }
            else if (_rowHeaders != null)
            {
                for (int m = -1; m <= layout.RowPaneCount; m++)
                {
                    headerX = layout.HeaderX;
                    headerY = layout.GetViewportY(m);
                    if (((IsTouching && IsTouching) && ((m == layout.RowPaneCount) && (_translateOffsetY < 0.0))) && (_touchStartHitTestInfo.RowViewportIndex == (layout.RowPaneCount - 1)))
                    {
                        headerY += _translateOffsetY;
                    }
                    double num15 = layout.HeaderWidth;
                    double num16 = layout.GetViewportHeight(m);
                    var header = _rowHeaders[m + 1];
                    if ((header != null) && (header.Parent != null))
                    {
                        if (header.RenderTransform != null)
                        {
                            header.RenderTransform = null;
                        }
                        if ((header.Width != num15) || (header.Height != num16))
                        {
                            if (!IsTouching)
                            {
                                header.Arrange(new Rect(headerX, headerY, num15, num16));
                            }
                            else
                            {
                                int num17 = (int)Math.Ceiling(_translateOffsetY);
                                double y = headerY;
                                if ((_touchStartHitTestInfo != null) && (m == _touchStartHitTestInfo.RowViewportIndex))
                                {
                                    y += num17;
                                }
                                header.Arrange(new Rect(headerX, y, num15, num16));
                                if ((y != headerY) && (_translateOffsetY < 0.0))
                                {
                                    RectangleGeometry geometry3 = new RectangleGeometry();
                                    geometry3.Rect = new Rect(0.0, Math.Abs((double)(headerY - y)), num15, _cachedViewportHeights[m + 1]);
                                    header.Clip = geometry3;
                                }
                                else if ((y != headerY) && (_translateOffsetY > 0.0))
                                {
                                    RectangleGeometry geometry4 = new RectangleGeometry();
                                    geometry4.Rect = new Rect(0.0, 0.0, num15, Math.Max((double)0.0, (double)(_cachedViewportHeights[m + 1] - _translateOffsetY)));
                                    header.Clip = geometry4;
                                }
                                else
                                {
                                    header.Clip = null;
                                }
                            }
                        }
                    }
                }
            }
            if (IsTouchZooming && (_cachedViewportTransform != null))
            {
                for (int n = -1; n <= layout.ColumnPaneCount; n++)
                {
                    headerX = layout.GetViewportX(n);
                    double num20 = layout.GetViewportWidth(n);
                    for (int num21 = -1; num21 <= layout.RowPaneCount; num21++)
                    {
                        headerY = layout.GetViewportY(num21);
                        double num22 = layout.GetViewportHeight(num21);
                        CellsPanel viewport5 = _cellsPanels[num21 + 1, n + 1];
                        if (viewport5 != null)
                        {
                            viewport5.Arrange(new Rect(headerX, headerY, num20, num22));
                            viewport5.RenderTransform = _cachedViewportTransform[num21 + 1, n + 1];
                        }
                    }
                }
            }
            else if (_cellsPanels != null)
            {
                for (int num23 = -1; num23 <= layout.ColumnPaneCount; num23++)
                {
                    headerX = layout.GetViewportX(num23);
                    if ((IsTouching && (num23 == layout.ColumnPaneCount)) && ((_translateOffsetX < 0.0) && (_touchStartHitTestInfo.ColumnViewportIndex == (layout.ColumnPaneCount - 1))))
                    {
                        headerX += _translateOffsetX;
                    }
                    double num24 = layout.GetViewportWidth(num23);
                    for (int num25 = -1; num25 <= layout.RowPaneCount; num25++)
                    {
                        headerY = layout.GetViewportY(num25);
                        if (((IsTouching && IsTouching) && ((num25 == layout.RowPaneCount) && (_translateOffsetY < 0.0))) && (_touchStartHitTestInfo.RowViewportIndex == (layout.RowPaneCount - 1)))
                        {
                            headerY += _translateOffsetY;
                        }
                        double num26 = layout.GetViewportHeight(num25);
                        CellsPanel viewport6 = _cellsPanels[num25 + 1, num23 + 1];
                        if (viewport6 != null)
                        {
                            if (viewport6.RenderTransform != null)
                            {
                                viewport6.RenderTransform = null;
                            }
                            if ((viewport6.Width != num24) || (viewport6.Height != num26))
                            {
                                if (!IsTouching)
                                {
                                    viewport6.Arrange(new Rect(headerX, headerY, num24, num26));
                                }
                                else
                                {
                                    int num27 = (int)Math.Ceiling(_translateOffsetX);
                                    int num28 = (int)Math.Ceiling(_translateOffsetY);
                                    double num29 = headerX;
                                    double num30 = headerY;
                                    if ((_touchStartHitTestInfo != null) && (num23 == _touchStartHitTestInfo.ColumnViewportIndex))
                                    {
                                        num29 += num27;
                                    }
                                    if ((_touchStartHitTestInfo != null) && (num25 == _touchStartHitTestInfo.RowViewportIndex))
                                    {
                                        num30 += num28;
                                    }
                                    viewport6.Arrange(new Rect(num29, num30, num24, num26));
                                    if (((headerY != num30) && (_translateOffsetY < 0.0)) || ((headerX != num29) && (_translateOffsetX < 0.0)))
                                    {
                                        RectangleGeometry geometry5 = new RectangleGeometry();
                                        geometry5.Rect = new Rect(Math.Abs((double)(headerX - num29)), Math.Abs((double)(headerY - num30)), _cachedViewportWidths[num23 + 1], _cachedViewportHeights[num25 + 1]);
                                        viewport6.Clip = geometry5;
                                    }
                                    else if ((headerX != num29) && (_translateOffsetX > 0.0))
                                    {
                                        RectangleGeometry geometry6 = new RectangleGeometry();
                                        geometry6.Rect = new Rect(0.0, 0.0, Math.Max((double)0.0, (double)(_cachedViewportWidths[num23 + 1] - _translateOffsetX)), _cachedViewportHeights[num25 + 1]);
                                        viewport6.Clip = geometry6;
                                    }
                                    else if ((headerY != num30) && (_translateOffsetY > 0.0))
                                    {
                                        RectangleGeometry geometry7 = new RectangleGeometry();
                                        geometry7.Rect = new Rect(0.0, 0.0, _cachedViewportWidths[num23 + 1], Math.Max((double)0.0, (double)(_cachedViewportHeights[num25 + 1] - _translateOffsetY)));
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
            if (_horizontalScrollBar != null)
            {
                for (int num31 = 0; num31 < layout.ColumnPaneCount; num31++)
                {
                    double horizontalScrollBarX = layout.GetHorizontalScrollBarX(num31);
                    double ornamentY = layout.OrnamentY;
                    double horizontalScrollBarWidth = layout.GetHorizontalScrollBarWidth(num31);
                    double ornamentHeight = layout.OrnamentHeight;
                    horizontalScrollBarWidth = Math.Max(horizontalScrollBarWidth, 0.0);
                    ornamentHeight = Math.Max(ornamentHeight, 0.0);
                    _horizontalScrollBar[num31].Width = horizontalScrollBarWidth;
                    _horizontalScrollBar[num31].Height = ornamentHeight;
                    _horizontalScrollBar[num31].Arrange(new Rect(horizontalScrollBarX, ornamentY, horizontalScrollBarWidth, ornamentHeight));
                }
            }
            if (_tabStrip != null)
            {
                double tabStripX = layout.TabStripX;
                double tabStripY = layout.TabStripY;
                double tabStripWidth = layout.TabStripWidth;
                double tabStripHeight = layout.TabStripHeight;
                _tabStrip.Arrange(new Rect(tabStripX, tabStripY, tabStripWidth, tabStripHeight));
            }
            if (tabStripSplitBox != null)
            {
                double tabSplitBoxX = layout.TabSplitBoxX;
                double num41 = layout.OrnamentY;
                double tabSplitBoxWidth = layout.TabSplitBoxWidth;
                double num43 = layout.OrnamentHeight;
                tabStripSplitBox.Arrange(new Rect(tabSplitBoxX, num41, tabSplitBoxWidth, num43));
            }
            if (_horizontalSplitBox != null)
            {
                for (int num44 = 0; num44 < layout.ColumnPaneCount; num44++)
                {
                    double horizontalSplitBoxX = layout.GetHorizontalSplitBoxX(num44);
                    double num46 = layout.OrnamentY;
                    double horizontalSplitBoxWidth = layout.GetHorizontalSplitBoxWidth(num44);
                    double num48 = layout.OrnamentHeight;
                    _horizontalSplitBox[num44].Arrange(new Rect(horizontalSplitBoxX, num46, horizontalSplitBoxWidth, num48));
                }
            }
            if (_verticalScrollBar != null)
            {
                for (int num49 = 0; num49 < layout.RowPaneCount; num49++)
                {
                    double ornamentX = layout.OrnamentX;
                    double verticalScrollBarY = layout.GetVerticalScrollBarY(num49);
                    double ornamentWidth = layout.OrnamentWidth;
                    double num53 = layout.GetViewportHeight(num49) - layout.GetVerticalSplitBoxHeight(num49);
                    if (((IsTouching && (_touchStartHitTestInfo != null)) && ((num49 == _touchStartHitTestInfo.RowViewportIndex) && !IsZero(_translateOffsetY))) && ((_touchStartHitTestInfo != null) && (_touchStartHitTestInfo.HitTestType == HitTestType.Viewport)))
                    {
                        num53 = _cachedViewportHeights[num49 + 1] - layout.GetVerticalSplitBoxHeight(num49);
                    }
                    if (num49 == 0)
                    {
                        num53 += (layout.HeaderY + layout.HeaderHeight) + layout.FrozenHeight;
                    }
                    if (num49 == (layout.RowPaneCount - 1))
                    {
                        num53 += layout.FrozenTrailingHeight;
                    }
                    ornamentWidth = Math.Max(ornamentWidth, 0.0);
                    num53 = Math.Max(num53, 0.0);
                    _verticalScrollBar[num49].Width = ornamentWidth;
                    _verticalScrollBar[num49].Height = num53;
                    _verticalScrollBar[num49].Arrange(new Rect(ornamentX, verticalScrollBarY, ornamentWidth, num53));
                }
            }
            if (_verticalSplitBox != null)
            {
                for (int num54 = 0; num54 < layout.RowPaneCount; num54++)
                {
                    double num55 = layout.OrnamentX;
                    double verticalSplitBoxY = layout.GetVerticalSplitBoxY(num54);
                    double num57 = layout.OrnamentWidth;
                    double verticalSplitBoxHeight = layout.GetVerticalSplitBoxHeight(num54);
                    _verticalSplitBox[num54].Arrange(new Rect(num55, verticalSplitBoxY, num57, verticalSplitBoxHeight));
                }
            }
            if (_horizontalSplitBar != null)
            {
                for (int num59 = 0; num59 < (layout.ColumnPaneCount - 1); num59++)
                {
                    if ((_horizontalSplitBar[num59] != null) && (_horizontalSplitBar[num59].Parent != null))
                    {
                        double horizontalSplitBarX = layout.GetHorizontalSplitBarX(num59);
                        if (IsTouching && (_cachedViewportSplitBarX != null))
                        {
                            horizontalSplitBarX = _cachedViewportSplitBarX[num59];
                        }
                        double num61 = layout.Y;
                        double horizontalSplitBarWidth = layout.GetHorizontalSplitBarWidth(num59);
                        double num63 = _availableSize.Height;
                        _horizontalSplitBar[num59].Arrange(new Rect(horizontalSplitBarX, num61, horizontalSplitBarWidth, num63));
                    }
                }
            }
            if (_verticalSplitBar != null)
            {
                for (int num64 = 0; num64 < (layout.RowPaneCount - 1); num64++)
                {
                    if ((_verticalSplitBar[num64] != null) && (_verticalSplitBar[num64].Parent != null))
                    {
                        double num65 = layout.X;
                        double verticalSplitBarY = layout.GetVerticalSplitBarY(num64);
                        if (IsTouching && (_cachedViewportSplitBarY != null))
                        {
                            verticalSplitBarY = _cachedViewportSplitBarY[num64];
                        }
                        double num67 = _availableSize.Width;
                        double verticalSplitBarHeight = layout.GetVerticalSplitBarHeight(num64);
                        _verticalSplitBar[num64].Arrange(new Rect(num65, verticalSplitBarY, num67, verticalSplitBarHeight));
                    }
                }
            }
            if (_crossSplitBar != null)
            {
                for (int num69 = 0; num69 < _crossSplitBar.GetLength(0); num69++)
                {
                    double num70 = layout.GetVerticalSplitBarY(num69);
                    if (IsTouching && (_cachedViewportSplitBarY != null))
                    {
                        num70 = _cachedViewportSplitBarY[num69];
                    }
                    double num71 = layout.GetVerticalSplitBarHeight(num69);
                    for (int num72 = 0; num72 < _crossSplitBar.GetLength(1); num72++)
                    {
                        double num73 = layout.GetHorizontalSplitBarX(num72);
                        if (IsTouching && (_cachedViewportSplitBarX != null))
                        {
                            num73 = _cachedViewportSplitBarX[num72];
                        }
                        double num74 = layout.GetHorizontalSplitBarWidth(num72);
                        if ((_crossSplitBar[num69, num72] != null) && (_crossSplitBar[num69, num72].Parent != null))
                        {
                            _crossSplitBar[num69, num72].Arrange(new Rect(num73, num70, num74, num71));
                        }
                    }
                }
            }
            ArrangeRangeGroup(layout.RowPaneCount, layout.ColumnPaneCount, layout);
            Rect rect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
            if (_rowFreezeLine != null)
            {
                _rowFreezeLine.Arrange(rect);
            }
            if (_rowTrailingFreezeLine != null)
            {
                _rowTrailingFreezeLine.Arrange(rect);
            }
            if (_columnFreezeLine != null)
            {
                _columnFreezeLine.Arrange(rect);
            }
            if (_columnTrailingFreezeLine != null)
            {
                _columnTrailingFreezeLine.Arrange(rect);
            }
            RectangleGeometry geometry9 = new RectangleGeometry();
            geometry9.Rect = rect;
            Clip = geometry9;
            if (_formulaSelectionGripperPanel != null)
            {
                _formulaSelectionGripperPanel.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            }
            UpdateTouchSelectionGripper();
            _progressGrid.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

        SheetLayout CreateLayout()
        {
            var sheet = ActiveSheet;
            ViewportInfo viewportInfo = GetViewportInfo(sheet);
            double width = _availableSize.Width;
            double height = _availableSize.Height;
            SheetLayout layout = new SheetLayout(viewportInfo.RowViewportCount, viewportInfo.ColumnViewportCount)
            {
                X = 0.0,
                Y = 0.0
            };
            if ((sheet == null) || !sheet.Visible)
            {
                layout.TabStripX = 0.0;
                layout.TabStripHeight = 25.0;
                layout.TabStripY = Math.Max((double)0.0, (double)(height - layout.TabStripHeight));
                layout.TabStripWidth = Math.Max(0.0, width);
                return layout;
            }

            GroupLayout groupLayout = GetGroupLayout();
            layout.HeaderX = layout.X + groupLayout.Width;
            layout.HeaderY = layout.Y + groupLayout.Height;
            float zoomFactor = ZoomFactor;

            // 行头宽度 列头高度
            double totalWidth = 0.0;
            double totalHeight = 0.0;
            if (sheet.RowHeader.IsVisible)
            {
                for (int i = 0; i < sheet.RowHeader.Columns.Count; i++)
                {
                    layout.HeaderWidth += Math.Ceiling((double)(sheet.GetActualColumnWidth(i, SheetArea.CornerHeader | SheetArea.RowHeader) * zoomFactor));
                }
                totalWidth += layout.HeaderWidth;
            }
            if (sheet.ColumnHeader.IsVisible)
            {
                for (int i = 0; i < sheet.ColumnHeader.Rows.Count; i++)
                {
                    layout.HeaderHeight += Math.Ceiling((double)(sheet.GetActualRowHeight(i, SheetArea.ColumnHeader) * zoomFactor));
                }
                totalHeight += layout.HeaderHeight;
            }

            // 冻结列的宽度高度
            layout.FrozenX = layout.HeaderX + layout.HeaderWidth;
            layout.FrozenY = layout.HeaderY + layout.HeaderHeight;
            for (int i = 0; i < sheet.FrozenColumnCount; i++)
            {
                layout.FrozenWidth += Math.Ceiling((double)(sheet.GetActualColumnWidth(i, SheetArea.Cells) * zoomFactor));
            }
            for (int j = 0; j < sheet.FrozenRowCount; j++)
            {
                layout.FrozenHeight += Math.Ceiling((double)(sheet.GetActualRowHeight(j, SheetArea.Cells) * zoomFactor));
            }
            for (int k = Math.Max(sheet.FrozenColumnCount, sheet.ColumnCount - sheet.FrozenTrailingColumnCount); k < sheet.ColumnCount; k++)
            {
                layout.FrozenTrailingWidth += Math.Ceiling((double)(sheet.GetActualColumnWidth(k, SheetArea.Cells) * zoomFactor));
            }
            for (int m = Math.Max(sheet.FrozenRowCount, sheet.RowCount - sheet.FrozenTrailingRowCount); m < sheet.RowCount; m++)
            {
                layout.FrozenTrailingHeight += Math.Ceiling((double)(sheet.GetActualRowHeight(m, SheetArea.Cells) * zoomFactor));
            }
            totalWidth += layout.FrozenWidth + layout.FrozenTrailingWidth;
            totalHeight += layout.FrozenHeight + layout.FrozenTrailingHeight;

            // 普通可视列的宽度高度
            double tempWidth = 0.0;
            double tempHeight = 0.0;
            for (int i = sheet.FrozenColumnCount; (tempWidth <= width) && (i < (sheet.ColumnCount - sheet.FrozenTrailingColumnCount)); i++)
            {
                tempWidth += Math.Ceiling((double)(sheet.GetActualColumnWidth(i, SheetArea.Cells) * zoomFactor));
            }
            for (int num15 = sheet.FrozenRowCount; (tempHeight <= height) && (num15 < (sheet.RowCount - sheet.FrozenTrailingRowCount)); num15++)
            {
                tempHeight += Math.Ceiling((double)(sheet.GetActualRowHeight(num15, SheetArea.Cells) * zoomFactor));
            }
            totalWidth += tempWidth;
            totalHeight += tempHeight;

            bool flag = (HorizontalScrollBarPolicy == ScrollBarVisibility.Visible) || (HorizontalScrollBarPolicy == ScrollBarVisibility.Disabled);
            if (HorizontalScrollBarPolicy == ScrollBarVisibility.Auto)
            {
                if (layout.ColumnPaneCount > 1)
                {
                    flag = true;
                }
                else if ((VerticalScrollBarPolicy == (ScrollBarVisibility)3) || (VerticalScrollBarPolicy == 0))
                {
                    flag |= totalWidth > ((width - ActualVerticalScrollBarWidth) - groupLayout.Width);
                }
                else if (VerticalScrollBarPolicy == (ScrollBarVisibility)1)
                {
                    if (tempHeight > height)
                    {
                        flag |= totalWidth > ((width - ActualVerticalScrollBarWidth) - groupLayout.Width);
                    }
                    else
                    {
                        flag |= totalWidth > (width - groupLayout.Width);
                    }
                }
                else
                {
                    flag |= totalWidth > (width - groupLayout.Width);
                }
            }
            if (flag)
            {
                // 显示水平滚动栏
                layout.OrnamentHeight = ActualHorizontalScrollBarHeight;
                height -= layout.OrnamentHeight;
                height = Math.Max(0.0, height);
            }

            if (TabStripVisibility == Visibility.Visible)
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

            bool flag3 = ((VerticalScrollBarPolicy == (ScrollBarVisibility)3) || (VerticalScrollBarPolicy == 0)) || ((VerticalScrollBarPolicy == (ScrollBarVisibility)1) && ((layout.RowPaneCount > 1) || (totalHeight > (height - groupLayout.Height))));
            if (flag3)
            {
                // 显示垂直滚动栏
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

            for (int i = 0; i < (layout.ColumnPaneCount - 1); i++)
            {
                layout.SetHorizontalSplitBarWidth(i, 6.0);
                width -= layout.GetHorizontalSplitBarWidth(i);
                width = Math.Max(0.0, width);
            }
            for (int i = 0; i < (layout.RowPaneCount - 1); i++)
            {
                layout.SetVerticalSplitBarHeight(i, 6.0);
                height -= layout.GetVerticalSplitBarHeight(i);
                height = Math.Max(0.0, height);
            }

            // 为未设置大小的Viewport设置尺寸
            int cntNotSettingWidth = 0;
            int cntNotSettingHeight = 0;
            for (int i = 0; i < layout.ColumnPaneCount; i++)
            {
                if (viewportInfo.ViewportWidth[i] < 0.0)
                {
                    cntNotSettingWidth++;
                }
                else
                {
                    layout.SetViewportWidth(i, Math.Max(0.0, Math.Min(width, viewportInfo.ViewportWidth[i] * zoomFactor)));
                    width -= layout.GetViewportWidth(i);
                }
            }
            for (int i = 0; i < layout.RowPaneCount; i++)
            {
                if (viewportInfo.ViewportHeight[i] < 0.0)
                {
                    cntNotSettingHeight++;
                }
                else
                {
                    layout.SetViewportHeight(i, Math.Max(0.0, Math.Min(height, viewportInfo.ViewportHeight[i] * zoomFactor)));
                    height -= layout.GetViewportHeight(i);
                }
            }
            width = Math.Max(0.0, width);
            height = Math.Max(0.0, height);
            double perWidth = width / ((double)cntNotSettingWidth);
            double perHeight = height / ((double)cntNotSettingHeight);
            if (double.IsInfinity(perWidth) || double.IsNaN(perWidth))
            {
                perWidth = totalWidth;
            }
            if (double.IsInfinity(perHeight) || double.IsNaN(perHeight))
            {
                perHeight = totalHeight;
            }
            for (int i = 0; i < layout.ColumnPaneCount; i++)
            {
                if (viewportInfo.ViewportWidth[i] < 0.0)
                {
                    layout.SetViewportWidth(i, perWidth);
                }
            }
            for (int i = 0; i < layout.RowPaneCount; i++)
            {
                if (viewportInfo.ViewportHeight[i] < 0.0)
                {
                    layout.SetViewportHeight(i, perHeight);
                }
            }

            if (cntNotSettingWidth == 0 && width > 0.0)
            {
                double num28 = width + viewportInfo.ViewportWidth[layout.ColumnPaneCount - 1];
                layout.SetViewportWidth(layout.ColumnPaneCount - 1, num28);
            }
            if ((cntNotSettingHeight == 0) && (height > 0.0) && layout.RowPaneCount > 0)
            {
                double num29 = height + viewportInfo.ViewportHeight[layout.RowPaneCount - 1];
                layout.SetViewportHeight(layout.RowPaneCount - 1, num29);
            }


            layout.SetViewportX(0, (layout.HeaderX + layout.HeaderWidth) + layout.FrozenWidth);
            for (int i = 1; i < layout.ColumnPaneCount; i++)
            {
                layout.SetHorizontalSplitBarX(i - 1, layout.GetViewportX(i - 1) + layout.GetViewportWidth(i - 1));
                layout.SetViewportX(i, layout.GetHorizontalSplitBarX(i - 1) + layout.GetHorizontalSplitBarWidth(i - 1));
            }

            layout.SetViewportY(0, (layout.HeaderY + layout.HeaderHeight) + layout.FrozenHeight);
            for (int i = 1; i < layout.RowPaneCount; i++)
            {
                layout.SetVerticalSplitBarY(i - 1, layout.GetViewportY(i - 1) + layout.GetViewportHeight(i - 1));
                layout.SetViewportY(i, layout.GetVerticalSplitBarY(i - 1) + layout.GetVerticalSplitBarHeight(i - 1));
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
            for (int i = 0; i < layout.ColumnPaneCount; i++)
            {
                if (i == 0)
                {
                    double num34 = ((layout.HeaderX + layout.HeaderWidth) + layout.FrozenWidth) + layout.GetViewportWidth(i);
                    double x = layout.X;
                    if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetHorizontalSplitBoxX(i, x);
                        layout.SetHorizontalSplitBoxWidth(i, Math.Min(num34, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarX(i, layout.GetHorizontalSplitBoxX(i) + layout.GetHorizontalSplitBoxWidth(i));
                        layout.SetHorizontalScrollBarWidth(i, Math.Max((double)0.0, (double)(num34 - layout.GetHorizontalSplitBoxWidth(i))));
                    }
                    else
                    {
                        layout.SetHorizontalScrollBarX(i, x);
                        layout.SetHorizontalSplitBoxWidth(i, Math.Min(num34, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarWidth(i, Math.Max((double)0.0, (double)(num34 - layout.GetHorizontalSplitBoxWidth(i))));
                        layout.SetHorizontalSplitBoxX(i, layout.GetHorizontalScrollBarX(i) + layout.GetHorizontalScrollBarWidth(i));
                    }
                }
                if ((i > 0) && (i < (layout.ColumnPaneCount - 1)))
                {
                    double viewportWidth = layout.GetViewportWidth(i);
                    double viewportX = layout.GetViewportX(i);
                    if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetHorizontalSplitBoxX(i, viewportX);
                        layout.SetHorizontalSplitBoxWidth(i, Math.Min(viewportWidth, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarX(i, layout.GetHorizontalSplitBoxX(i) + layout.GetHorizontalSplitBoxWidth(i));
                        layout.SetHorizontalScrollBarWidth(i, Math.Max((double)0.0, (double)(viewportWidth - layout.GetHorizontalSplitBoxWidth(i))));
                    }
                    else
                    {
                        layout.SetHorizontalScrollBarX(i, viewportX);
                        layout.SetHorizontalSplitBoxWidth(i, Math.Min(viewportWidth, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarWidth(i, Math.Max((double)0.0, (double)(viewportWidth - layout.GetHorizontalSplitBoxWidth(i))));
                        layout.SetHorizontalSplitBoxX(i, layout.GetHorizontalScrollBarX(i) + layout.GetHorizontalScrollBarWidth(i));
                    }
                }
                if (i == (layout.ColumnPaneCount - 1))
                {
                    double num38 = (((layout.GetViewportWidth(layout.ColumnPaneCount - 1) + layout.FrozenTrailingWidth) + ((layout.ColumnPaneCount == 1) ? layout.HeaderX : 0.0)) + ((layout.ColumnPaneCount == 1) ? layout.HeaderWidth : 0.0)) + ((layout.ColumnPaneCount == 1) ? layout.FrozenWidth : 0.0);
                    double num39 = (layout.ColumnPaneCount == 1) ? layout.X : layout.GetViewportX(layout.ColumnPaneCount - 1);
                    if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetHorizontalSplitBoxX(i, num39);
                        layout.SetHorizontalSplitBoxWidth(i, Math.Min(num38, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarX(i, layout.GetHorizontalSplitBoxX(i) + layout.GetHorizontalSplitBoxWidth(i));
                        layout.SetHorizontalScrollBarWidth(i, Math.Max((double)0.0, (double)(num38 - layout.GetHorizontalSplitBoxWidth(i))));
                    }
                    else
                    {
                        layout.SetHorizontalScrollBarX(i, num39);
                        layout.SetHorizontalSplitBoxWidth(i, Math.Min(num38, columnSplitBoxesWidth));
                        layout.SetHorizontalScrollBarWidth(i, Math.Max((double)0.0, (double)(num38 - layout.GetHorizontalSplitBoxWidth(i))));
                        layout.SetHorizontalSplitBoxX(i, layout.GetHorizontalScrollBarX(i) + layout.GetHorizontalScrollBarWidth(i));
                    }
                }
            }
            double rowSplitBoxesHeight = GetRowSplitBoxesHeight(layout.RowPaneCount);
            for (int i = 0; i < layout.RowPaneCount; i++)
            {
                if (i == 0)
                {
                    double num42 = ((layout.HeaderY + layout.HeaderHeight) + layout.FrozenHeight) + layout.GetViewportHeight(i);
                    double y = layout.Y;
                    if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetVerticalSplitBoxY(i, y);
                        layout.SetVerticalSplitBoxHeight(i, Math.Min(num42, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarY(i, layout.GetVerticalSplitBoxY(i) + layout.GetVerticalSplitBoxHeight(i));
                        layout.SetVerticalScrollBarHeight(i, Math.Max((double)0.0, (double)(num42 - layout.GetVerticalSplitBoxHeight(i))));
                    }
                    else
                    {
                        layout.SetVerticalScrollBarY(i, y);
                        layout.SetVerticalSplitBoxHeight(i, Math.Min(num42, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarHeight(i, Math.Max((double)0.0, (double)(num42 - layout.GetVerticalSplitBoxHeight(i))));
                        layout.SetVerticalSplitBoxY(i, layout.GetVerticalScrollBarY(i) + layout.GetVerticalScrollBarHeight(i));
                    }
                }
                if ((i > 0) && (i < (layout.RowPaneCount - 1)))
                {
                    double viewportHeight = layout.GetViewportHeight(i);
                    double viewportY = layout.GetViewportY(i);
                    if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetVerticalSplitBoxY(i, viewportY);
                        layout.SetVerticalSplitBoxHeight(i, Math.Min(viewportHeight, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarY(i, layout.GetVerticalSplitBoxY(i) + layout.GetVerticalSplitBoxHeight(i));
                        layout.SetVerticalScrollBarHeight(i, Math.Max((double)0.0, (double)(viewportHeight - layout.GetVerticalSplitBoxHeight(i))));
                    }
                    else
                    {
                        layout.SetVerticalScrollBarY(i, viewportY);
                        layout.SetVerticalSplitBoxHeight(i, Math.Min(viewportHeight, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarHeight(i, Math.Max((double)0.0, (double)(viewportHeight - layout.GetVerticalSplitBoxHeight(i))));
                        layout.SetVerticalSplitBoxY(i, layout.GetVerticalScrollBarY(i) + layout.GetVerticalScrollBarHeight(i));
                    }
                }
                if (i == (layout.RowPaneCount - 1))
                {
                    double num46 = (((layout.GetViewportHeight(i) + layout.FrozenTrailingHeight) + ((layout.RowPaneCount == 1) ? layout.HeaderY : 0.0)) + ((layout.RowPaneCount == 1) ? layout.HeaderHeight : 0.0)) + ((layout.RowPaneCount == 1) ? layout.FrozenHeight : 0.0);
                    double num47 = (layout.RowPaneCount == 1) ? layout.Y : layout.GetViewportY(layout.RowPaneCount - 1);
                    if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        layout.SetVerticalSplitBoxY(i, num47);
                        layout.SetVerticalSplitBoxHeight(i, Math.Min(num46, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarY(i, layout.GetVerticalSplitBoxY(i) + layout.GetVerticalSplitBoxHeight(i));
                        layout.SetVerticalScrollBarHeight(i, Math.Max((double)0.0, (double)(num46 - layout.GetVerticalSplitBoxHeight(i))));
                    }
                    else
                    {
                        layout.SetVerticalScrollBarY(i, num47);
                        layout.SetVerticalSplitBoxHeight(i, Math.Min(num46, rowSplitBoxesHeight));
                        layout.SetVerticalScrollBarHeight(i, Math.Max((double)0.0, (double)(num46 - layout.GetVerticalSplitBoxHeight(i))));
                        layout.SetVerticalSplitBoxY(i, layout.GetVerticalScrollBarY(i) + layout.GetVerticalScrollBarHeight(i));
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
            double num49 = _availableSize.Width;
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
            double num52 = _availableSize.Height;
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
            _availableSize = new Size(num49, num52);
            return layout;
        }

    }
}
