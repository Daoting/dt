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
            _cornerPresenter = null;
            _rowHeaderPresenters = null;
            _columnHeaderPresenters = null;
            _viewportPresenters = null;
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
            _highlightDataValidationInvalidData = false;
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

            // 多GcViewport
            GcViewport[,] viewportArray = null;
            if ((_viewportPresenters != null)
                && (ActiveSheet == null
                    || _viewportPresenters.GetUpperBound(0) != layout.RowPaneCount + 1
                    || _viewportPresenters.GetUpperBound(1) != layout.ColumnPaneCount + 1))
            {
                GcViewport[,] viewportArray2 = _viewportPresenters;
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
                        Children.Remove(viewport);
                    }
                }
                viewportArray = _viewportPresenters;
                _viewportPresenters = null;
            }
            if (_viewportPresenters == null)
            {
                _viewportPresenters = new GcViewport[layout.RowPaneCount + 2, layout.ColumnPaneCount + 2];
            }
            for (int i = -1; i <= layout.ColumnPaneCount; i++)
            {
                double viewportWidth = layout.GetViewportWidth(i);
                viewportX = layout.GetViewportX(i);
                for (int num5 = -1; num5 <= layout.RowPaneCount; num5++)
                {
                    double viewportHeight = layout.GetViewportHeight(num5);
                    viewportY = layout.GetViewportY(num5);
                    if (((_viewportPresenters[num5 + 1, i + 1] == null) && (viewportWidth > 0.0)) && (viewportHeight > 0.0))
                    {
                        if (((viewportArray != null) && ((num5 + 1) < viewportArray.GetUpperBound(0))) && (((i + 1) < viewportArray.GetUpperBound(1)) && (viewportArray[num5 + 1, i + 1] != null)))
                        {
                            _viewportPresenters[num5 + 1, i + 1] = viewportArray[num5 + 1, i + 1];
                        }
                        else
                        {
                            _viewportPresenters[num5 + 1, i + 1] = new GcViewport(this);
                        }
                    }
                    GcViewport viewport2 = _viewportPresenters[num5 + 1, i + 1];
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
                        _viewportPresenters[num5 + 1, i + 1] = null;
                    }
                }
            }

            // 行头，一个GcViewport对应一个行头
            if ((_rowHeaderPresenters != null) && ((ActiveSheet == null) || (_rowHeaderPresenters.Length != (layout.RowPaneCount + 2))))
            {
                foreach (GcViewport viewport3 in _rowHeaderPresenters)
                {
                    Children.Remove(viewport3);
                }
                _rowHeaderPresenters = null;
            }
            if (_rowHeaderPresenters == null)
            {
                _rowHeaderPresenters = new GcRowHeaderViewport[layout.RowPaneCount + 2];
            }
            if (layout.HeaderWidth > 0.0)
            {
                for (int num7 = -1; num7 <= layout.RowPaneCount; num7++)
                {
                    double height = layout.GetViewportHeight(num7);
                    viewportY = layout.GetViewportY(num7);
                    if ((_rowHeaderPresenters[num7 + 1] == null) && (height > 0.0))
                    {
                        _rowHeaderPresenters[num7 + 1] = new GcRowHeaderViewport(this);
                    }
                    GcViewport viewport4 = _rowHeaderPresenters[num7 + 1];
                    if (height > 0.0)
                    {
                        viewport4.Location = new Point(layout.HeaderX, viewportY);
                        viewport4.RowViewportIndex = num7;
                        if (!Children.Contains(viewport4))
                        {
                            Children.Add(viewport4);
                        }
                        viewport4.InvalidateMeasure();
                        viewport4.Measure(new Size(layout.HeaderWidth, height));
                    }
                    else if (viewport4 != null)
                    {
                        Children.Remove(viewport4);
                        _rowHeaderPresenters[num7 + 1] = null;
                    }
                }
            }
            else if (_rowHeaderPresenters != null)
            {
                foreach (GcViewport viewport5 in _rowHeaderPresenters)
                {
                    Children.Remove(viewport5);
                }
            }

            // 列头
            if ((_columnHeaderPresenters != null) && ((ActiveSheet == null) || (_columnHeaderPresenters.Length != (layout.ColumnPaneCount + 2))))
            {
                foreach (GcViewport viewport6 in _columnHeaderPresenters)
                {
                    Children.Remove(viewport6);
                }
                _columnHeaderPresenters = null;
            }
            if (_columnHeaderPresenters == null)
            {
                _columnHeaderPresenters = new GcColumnHeaderViewport[layout.ColumnPaneCount + 2];
            }
            if (layout.HeaderHeight > 0.0)
            {
                for (int num9 = -1; num9 <= layout.ColumnPaneCount; num9++)
                {
                    viewportX = layout.GetViewportX(num9);
                    double width = layout.GetViewportWidth(num9);
                    if ((_columnHeaderPresenters[num9 + 1] == null) && (width > 0.0))
                    {
                        _columnHeaderPresenters[num9 + 1] = new GcColumnHeaderViewport(this);
                    }
                    GcViewport viewport7 = _columnHeaderPresenters[num9 + 1];
                    if (width > 0.0)
                    {
                        viewport7.Location = new Point(viewportX, layout.HeaderY);
                        viewport7.ColumnViewportIndex = num9;
                        if (!Children.Contains(viewport7))
                        {
                            Children.Add(viewport7);
                        }
                        viewport7.InvalidateMeasure();
                        viewport7.Measure(new Size(width, layout.HeaderHeight));
                    }
                    else if (viewport7 != null)
                    {
                        Children.Remove(viewport7);
                        _columnHeaderPresenters[num9 + 1] = null;
                    }
                }
            }
            else if (_columnHeaderPresenters != null)
            {
                foreach (GcViewport viewport8 in _columnHeaderPresenters)
                {
                    Children.Remove(viewport8);
                }
            }

            // 左上角
            if (_cornerPresenter == null)
            {
                _cornerPresenter = new GcHeaderCornerViewport(this);
            }
            _cornerPresenter.Location = new Point(layout.HeaderX, layout.HeaderY);
            if ((layout.HeaderWidth > 0.0) && (layout.HeaderHeight > 0.0))
            {
                if (!Children.Contains(_cornerPresenter))
                {
                    Children.Add(_cornerPresenter);
                }
                _cornerPresenter.InvalidateMeasure();
                _cornerPresenter.Measure(new Size(layout.HeaderWidth, layout.HeaderHeight));
            }
            else
            {
                Children.Remove(_cornerPresenter);
                _cornerPresenter = null;
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
            if ((IsTouchZooming && (_cornerPresenter != null)) && (_cornerPresenter.Parent != null))
            {
                headerX = layout.HeaderX;
                headerY = layout.HeaderY;
                _cornerPresenter.Arrange(new Rect(headerX, headerY, layout.HeaderWidth, layout.HeaderHeight));
                _cornerPresenter.RenderTransform = _cachedCornerViewportTransform;
            }
            else if ((_cornerPresenter != null) && (_cornerPresenter.Parent != null))
            {
                headerX = layout.HeaderX;
                headerY = layout.HeaderY;
                if (_cornerPresenter.RenderTransform != null)
                {
                    _cornerPresenter.RenderTransform = null;
                }
                if ((_cornerPresenter.Width != layout.HeaderWidth) || (_cornerPresenter.Height != layout.HeaderHeight))
                {
                    _cornerPresenter.Arrange(new Rect(headerX, headerY, layout.HeaderWidth, layout.HeaderHeight));
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
                    GcViewport viewport = _columnHeaderPresenters[i + 1];
                    if ((viewport != null) && (viewport.Parent != null))
                    {
                        viewport.Arrange(new Rect(headerX, headerY, viewportWidth, headerHeight));
                        viewport.RenderTransform = _cachedColumnHeaderViewportTransform[i + 1];
                    }
                }
            }
            else if (_columnHeaderPresenters != null)
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
                    GcViewport viewport2 = _columnHeaderPresenters[j + 1];
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
                    GcViewport viewport3 = _rowHeaderPresenters[k + 1];
                    if ((viewport3 != null) && (viewport3.Parent != null))
                    {
                        viewport3.Arrange(new Rect(headerX, headerY, headerWidth, viewportHeight));
                        viewport3.RenderTransform = _cachedRowHeaderViewportTransform[k + 1];
                    }
                }
            }
            else if (_rowHeaderPresenters != null)
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
                    GcViewport viewport4 = _rowHeaderPresenters[m + 1];
                    if ((viewport4 != null) && (viewport4.Parent != null))
                    {
                        if (viewport4.RenderTransform != null)
                        {
                            viewport4.RenderTransform = null;
                        }
                        if ((viewport4.Width != num15) || (viewport4.Height != num16))
                        {
                            if (!IsTouching)
                            {
                                viewport4.Arrange(new Rect(headerX, headerY, num15, num16));
                            }
                            else
                            {
                                int num17 = (int)Math.Ceiling(_translateOffsetY);
                                double y = headerY;
                                if ((_touchStartHitTestInfo != null) && (m == _touchStartHitTestInfo.RowViewportIndex))
                                {
                                    y += num17;
                                }
                                viewport4.Arrange(new Rect(headerX, y, num15, num16));
                                if ((y != headerY) && (_translateOffsetY < 0.0))
                                {
                                    RectangleGeometry geometry3 = new RectangleGeometry();
                                    geometry3.Rect = new Rect(0.0, Math.Abs((double)(headerY - y)), num15, _cachedViewportHeights[m + 1]);
                                    viewport4.Clip = geometry3;
                                }
                                else if ((y != headerY) && (_translateOffsetY > 0.0))
                                {
                                    RectangleGeometry geometry4 = new RectangleGeometry();
                                    geometry4.Rect = new Rect(0.0, 0.0, num15, Math.Max((double)0.0, (double)(_cachedViewportHeights[m + 1] - _translateOffsetY)));
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
                        GcViewport viewport5 = _viewportPresenters[num21 + 1, n + 1];
                        if (viewport5 != null)
                        {
                            viewport5.Arrange(new Rect(headerX, headerY, num20, num22));
                            viewport5.RenderTransform = _cachedViewportTransform[num21 + 1, n + 1];
                        }
                    }
                }
            }
            else if (_viewportPresenters != null)
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
                        GcViewport viewport6 = _viewportPresenters[num25 + 1, num23 + 1];
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
    }
}
