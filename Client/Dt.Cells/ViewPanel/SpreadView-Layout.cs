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
    public partial class SpreadView : SheetView
    {

#if IOS
        new
#endif
        void Init()
        {
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
                Action action = null;
                if (action == null)
                {
                    action = delegate
                    {
                        _progressRing.Foreground = new SolidColorBrush(Colors.Black);
                    };
                }
                UIAdaptor.InvokeSync(action);
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
                AvailableSize = availableSize;
                InvalidateLayout();
            }
            if (!IsWorking)
            {
                SaveHitTestInfo(null);
            }

            SpreadLayout spreadLayout = GetSpreadLayout();
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
                    || _viewportPresenters.GetUpperBound(0) != spreadLayout.RowPaneCount + 1
                    || _viewportPresenters.GetUpperBound(1) != spreadLayout.ColumnPaneCount + 1))
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
                _viewportPresenters = new GcViewport[spreadLayout.RowPaneCount + 2, spreadLayout.ColumnPaneCount + 2];
            }
            for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
            {
                double viewportWidth = spreadLayout.GetViewportWidth(i);
                viewportX = spreadLayout.GetViewportX(i);
                for (int num5 = -1; num5 <= spreadLayout.RowPaneCount; num5++)
                {
                    double viewportHeight = spreadLayout.GetViewportHeight(num5);
                    viewportY = spreadLayout.GetViewportY(num5);
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
            if ((_rowHeaderPresenters != null) && ((ActiveSheet == null) || (_rowHeaderPresenters.Length != (spreadLayout.RowPaneCount + 2))))
            {
                foreach (GcViewport viewport3 in _rowHeaderPresenters)
                {
                    Children.Remove(viewport3);
                }
                _rowHeaderPresenters = null;
            }
            if (_rowHeaderPresenters == null)
            {
                _rowHeaderPresenters = new GcRowHeaderViewport[spreadLayout.RowPaneCount + 2];
            }
            if (spreadLayout.HeaderWidth > 0.0)
            {
                for (int num7 = -1; num7 <= spreadLayout.RowPaneCount; num7++)
                {
                    double height = spreadLayout.GetViewportHeight(num7);
                    viewportY = spreadLayout.GetViewportY(num7);
                    if ((_rowHeaderPresenters[num7 + 1] == null) && (height > 0.0))
                    {
                        _rowHeaderPresenters[num7 + 1] = new GcRowHeaderViewport(this);
                    }
                    GcViewport viewport4 = _rowHeaderPresenters[num7 + 1];
                    if (height > 0.0)
                    {
                        viewport4.Location = new Point(spreadLayout.HeaderX, viewportY);
                        viewport4.RowViewportIndex = num7;
                        if (!Children.Contains(viewport4))
                        {
                            Children.Add(viewport4);
                        }
                        viewport4.InvalidateMeasure();
                        viewport4.Measure(new Size(spreadLayout.HeaderWidth, height));
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
            if ((_columnHeaderPresenters != null) && ((ActiveSheet == null) || (_columnHeaderPresenters.Length != (spreadLayout.ColumnPaneCount + 2))))
            {
                foreach (GcViewport viewport6 in _columnHeaderPresenters)
                {
                    Children.Remove(viewport6);
                }
                _columnHeaderPresenters = null;
            }
            if (_columnHeaderPresenters == null)
            {
                _columnHeaderPresenters = new GcColumnHeaderViewport[spreadLayout.ColumnPaneCount + 2];
            }
            if (spreadLayout.HeaderHeight > 0.0)
            {
                for (int num9 = -1; num9 <= spreadLayout.ColumnPaneCount; num9++)
                {
                    viewportX = spreadLayout.GetViewportX(num9);
                    double width = spreadLayout.GetViewportWidth(num9);
                    if ((_columnHeaderPresenters[num9 + 1] == null) && (width > 0.0))
                    {
                        _columnHeaderPresenters[num9 + 1] = new GcColumnHeaderViewport(this);
                    }
                    GcViewport viewport7 = _columnHeaderPresenters[num9 + 1];
                    if (width > 0.0)
                    {
                        viewport7.Location = new Point(viewportX, spreadLayout.HeaderY);
                        viewport7.ColumnViewportIndex = num9;
                        if (!Children.Contains(viewport7))
                        {
                            Children.Add(viewport7);
                        }
                        viewport7.InvalidateMeasure();
                        viewport7.Measure(new Size(width, spreadLayout.HeaderHeight));
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
            _cornerPresenter.Location = new Point(spreadLayout.HeaderX, spreadLayout.HeaderY);
            if ((spreadLayout.HeaderWidth > 0.0) && (spreadLayout.HeaderHeight > 0.0))
            {
                if (!Children.Contains(_cornerPresenter))
                {
                    Children.Add(_cornerPresenter);
                }
                _cornerPresenter.InvalidateMeasure();
                _cornerPresenter.Measure(new Size(spreadLayout.HeaderWidth, spreadLayout.HeaderHeight));
            }
            else
            {
                Children.Remove(_cornerPresenter);
                _cornerPresenter = null;
            }

            // 水平滚动栏
            if (spreadLayout.OrnamentHeight > 0.0)
            {
                for (int num11 = 0; num11 < spreadLayout.ColumnPaneCount; num11++)
                {
                    ScrollBar bar = _horizontalScrollBar[num11];
                    if (spreadLayout.GetHorizontalScrollBarWidth(num11) > 0.0)
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
                    if (spreadLayout.GetHorizontalSplitBoxWidth(num11) > 0.0)
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
            if (spreadLayout.TabStripHeight > 0.0)
            {
                if (_tabStrip == null)
                {
                    _tabStrip = new TabStrip();
                    // hdt 应用构造前的设置
                    if (_tabStripVisibility == Visibility.Collapsed)
                        _tabStrip.Visibility = Visibility.Collapsed;
                    _tabStrip.HasInsertTab = SpreadSheet.TabStripInsertTab;
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
                _tabStrip.AddSheets(SpreadSheet.Sheets);
                if (!Children.Contains(_tabStrip))
                {
                    Children.Add(_tabStrip);
                }
                int activeSheetIndex = SpreadSheet.ActiveSheetIndex;
                if ((activeSheetIndex >= 0) && (activeSheetIndex < SpreadSheet.Sheets.Count))
                {
                    _tabStrip.ActiveSheet(activeSheetIndex, false);
                }
                _tabStrip.SetStartSheet(SpreadSheet.StartSheetIndex);
                _tabStrip.InvalidateMeasure();
                _tabStrip.Measure(new Size(spreadLayout.TabStripWidth, spreadLayout.TabStripHeight));
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
                tabStripSplitBox.Measure(new Size(spreadLayout.TabSplitBoxWidth, spreadLayout.OrnamentHeight));
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
            if (spreadLayout.OrnamentWidth > 0.0)
            {
                for (int num15 = 0; num15 < spreadLayout.RowPaneCount; num15++)
                {
                    ScrollBar bar3 = _verticalScrollBar[num15];
                    if (GetSpreadLayout().GetVerticalScrollBarHeight(num15) > 0.0)
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
                    if (spreadLayout.GetVerticalSplitBoxHeight(num15) > 0.0)
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
            for (int j = 0; j < (spreadLayout.ColumnPaneCount - 1); j++)
            {
                HorizontalSplitBar bar5 = _horizontalSplitBar[j];
                double horizontalSplitBoxWidth = spreadLayout.GetHorizontalSplitBoxWidth(j);
                double num20 = AvailableSize.Height;
                if (!Children.Contains(bar5))
                {
                    Children.Add(bar5);
                }
                bar5.Measure(new Size(horizontalSplitBoxWidth, num20));
            }
            for (int k = 0; k < (spreadLayout.RowPaneCount - 1); k++)
            {
                VerticalSplitBar bar6 = _verticalSplitBar[k];
                double num22 = AvailableSize.Width;
                double verticalSplitBarHeight = spreadLayout.GetVerticalSplitBarHeight(k);
                if (!Children.Contains(bar6))
                {
                    Children.Add(bar6);
                }
                bar6.Measure(new Size(num22, verticalSplitBarHeight));
            }
            for (int m = 0; m < (spreadLayout.RowPaneCount - 1); m++)
            {
                for (int num25 = 0; num25 < (spreadLayout.ColumnPaneCount - 1); num25++)
                {
                    CrossSplitBar bar7 = _crossSplitBar[m, num25];
                    double num26 = spreadLayout.GetHorizontalSplitBoxWidth(num25);
                    double num27 = spreadLayout.GetVerticalSplitBarHeight(m);
                    if (!Children.Contains(bar7))
                    {
                        Children.Add(bar7);
                    }
                    bar7.Measure(new Size(num26, num27));
                }
            }
            MeasureRangeGroup(spreadLayout.RowPaneCount, spreadLayout.ColumnPaneCount, spreadLayout);
            
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
            return AvailableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double headerX;
            double headerY;
            SpreadLayout spreadLayout = GetSpreadLayout();
            TrackersContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            SplittingTrackerContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            ShapeDrawingContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            CursorsContainer.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            if ((IsTouchZooming && (_cornerPresenter != null)) && (_cornerPresenter.Parent != null))
            {
                headerX = spreadLayout.HeaderX;
                headerY = spreadLayout.HeaderY;
                _cornerPresenter.Arrange(new Rect(headerX, headerY, spreadLayout.HeaderWidth, spreadLayout.HeaderHeight));
                _cornerPresenter.RenderTransform = _cachedCornerViewportTransform;
            }
            else if ((_cornerPresenter != null) && (_cornerPresenter.Parent != null))
            {
                headerX = spreadLayout.HeaderX;
                headerY = spreadLayout.HeaderY;
                if (_cornerPresenter.RenderTransform != null)
                {
                    _cornerPresenter.RenderTransform = null;
                }
                if ((_cornerPresenter.Width != spreadLayout.HeaderWidth) || (_cornerPresenter.Height != spreadLayout.HeaderHeight))
                {
                    _cornerPresenter.Arrange(new Rect(headerX, headerY, spreadLayout.HeaderWidth, spreadLayout.HeaderHeight));
                }
            }
            if (IsTouchZooming && (_cachedColumnHeaderViewportTransform != null))
            {
                for (int i = -1; i <= spreadLayout.ColumnPaneCount; i++)
                {
                    headerX = spreadLayout.GetViewportX(i);
                    headerY = spreadLayout.HeaderY;
                    double viewportWidth = spreadLayout.GetViewportWidth(i);
                    double headerHeight = spreadLayout.HeaderHeight;
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
                for (int j = -1; j <= spreadLayout.ColumnPaneCount; j++)
                {
                    headerX = spreadLayout.GetViewportX(j);
                    if ((IsTouching && (j == spreadLayout.ColumnPaneCount)) && ((_translateOffsetX < 0.0) && (_touchStartHitTestInfo.ColumnViewportIndex == (spreadLayout.ColumnPaneCount - 1))))
                    {
                        headerX += _translateOffsetX;
                    }
                    headerY = spreadLayout.HeaderY;
                    double width = spreadLayout.GetViewportWidth(j);
                    double height = spreadLayout.HeaderHeight;
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
                for (int k = -1; k <= spreadLayout.RowPaneCount; k++)
                {
                    headerX = spreadLayout.HeaderX;
                    headerY = spreadLayout.GetViewportY(k);
                    double headerWidth = spreadLayout.HeaderWidth;
                    double viewportHeight = spreadLayout.GetViewportHeight(k);
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
                for (int m = -1; m <= spreadLayout.RowPaneCount; m++)
                {
                    headerX = spreadLayout.HeaderX;
                    headerY = spreadLayout.GetViewportY(m);
                    if (((IsTouching && IsTouching) && ((m == spreadLayout.RowPaneCount) && (_translateOffsetY < 0.0))) && (_touchStartHitTestInfo.RowViewportIndex == (spreadLayout.RowPaneCount - 1)))
                    {
                        headerY += _translateOffsetY;
                    }
                    double num15 = spreadLayout.HeaderWidth;
                    double num16 = spreadLayout.GetViewportHeight(m);
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
                for (int n = -1; n <= spreadLayout.ColumnPaneCount; n++)
                {
                    headerX = spreadLayout.GetViewportX(n);
                    double num20 = spreadLayout.GetViewportWidth(n);
                    for (int num21 = -1; num21 <= spreadLayout.RowPaneCount; num21++)
                    {
                        headerY = spreadLayout.GetViewportY(num21);
                        double num22 = spreadLayout.GetViewportHeight(num21);
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
                for (int num23 = -1; num23 <= spreadLayout.ColumnPaneCount; num23++)
                {
                    headerX = spreadLayout.GetViewportX(num23);
                    if ((IsTouching && (num23 == spreadLayout.ColumnPaneCount)) && ((_translateOffsetX < 0.0) && (_touchStartHitTestInfo.ColumnViewportIndex == (spreadLayout.ColumnPaneCount - 1))))
                    {
                        headerX += _translateOffsetX;
                    }
                    double num24 = spreadLayout.GetViewportWidth(num23);
                    for (int num25 = -1; num25 <= spreadLayout.RowPaneCount; num25++)
                    {
                        headerY = spreadLayout.GetViewportY(num25);
                        if (((IsTouching && IsTouching) && ((num25 == spreadLayout.RowPaneCount) && (_translateOffsetY < 0.0))) && (_touchStartHitTestInfo.RowViewportIndex == (spreadLayout.RowPaneCount - 1)))
                        {
                            headerY += _translateOffsetY;
                        }
                        double num26 = spreadLayout.GetViewportHeight(num25);
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
                for (int num31 = 0; num31 < spreadLayout.ColumnPaneCount; num31++)
                {
                    double horizontalScrollBarX = spreadLayout.GetHorizontalScrollBarX(num31);
                    double ornamentY = spreadLayout.OrnamentY;
                    double horizontalScrollBarWidth = spreadLayout.GetHorizontalScrollBarWidth(num31);
                    double ornamentHeight = spreadLayout.OrnamentHeight;
                    horizontalScrollBarWidth = Math.Max(horizontalScrollBarWidth, 0.0);
                    ornamentHeight = Math.Max(ornamentHeight, 0.0);
                    _horizontalScrollBar[num31].Width = horizontalScrollBarWidth;
                    _horizontalScrollBar[num31].Height = ornamentHeight;
                    _horizontalScrollBar[num31].Arrange(new Rect(horizontalScrollBarX, ornamentY, horizontalScrollBarWidth, ornamentHeight));
                }
            }
            if (_tabStrip != null)
            {
                double tabStripX = spreadLayout.TabStripX;
                double tabStripY = spreadLayout.TabStripY;
                double tabStripWidth = spreadLayout.TabStripWidth;
                double tabStripHeight = spreadLayout.TabStripHeight;
                _tabStrip.Arrange(new Rect(tabStripX, tabStripY, tabStripWidth, tabStripHeight));
            }
            if (tabStripSplitBox != null)
            {
                double tabSplitBoxX = spreadLayout.TabSplitBoxX;
                double num41 = spreadLayout.OrnamentY;
                double tabSplitBoxWidth = spreadLayout.TabSplitBoxWidth;
                double num43 = spreadLayout.OrnamentHeight;
                tabStripSplitBox.Arrange(new Rect(tabSplitBoxX, num41, tabSplitBoxWidth, num43));
            }
            if (_horizontalSplitBox != null)
            {
                for (int num44 = 0; num44 < spreadLayout.ColumnPaneCount; num44++)
                {
                    double horizontalSplitBoxX = spreadLayout.GetHorizontalSplitBoxX(num44);
                    double num46 = spreadLayout.OrnamentY;
                    double horizontalSplitBoxWidth = spreadLayout.GetHorizontalSplitBoxWidth(num44);
                    double num48 = spreadLayout.OrnamentHeight;
                    _horizontalSplitBox[num44].Arrange(new Rect(horizontalSplitBoxX, num46, horizontalSplitBoxWidth, num48));
                }
            }
            if (_verticalScrollBar != null)
            {
                for (int num49 = 0; num49 < spreadLayout.RowPaneCount; num49++)
                {
                    double ornamentX = spreadLayout.OrnamentX;
                    double verticalScrollBarY = spreadLayout.GetVerticalScrollBarY(num49);
                    double ornamentWidth = spreadLayout.OrnamentWidth;
                    double num53 = spreadLayout.GetViewportHeight(num49) - spreadLayout.GetVerticalSplitBoxHeight(num49);
                    if (((IsTouching && (_touchStartHitTestInfo != null)) && ((num49 == _touchStartHitTestInfo.RowViewportIndex) && !IsZero(_translateOffsetY))) && ((_touchStartHitTestInfo != null) && (_touchStartHitTestInfo.HitTestType == HitTestType.Viewport)))
                    {
                        num53 = _cachedViewportHeights[num49 + 1] - spreadLayout.GetVerticalSplitBoxHeight(num49);
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
                    _verticalScrollBar[num49].Width = ornamentWidth;
                    _verticalScrollBar[num49].Height = num53;
                    _verticalScrollBar[num49].Arrange(new Rect(ornamentX, verticalScrollBarY, ornamentWidth, num53));
                }
            }
            if (_verticalSplitBox != null)
            {
                for (int num54 = 0; num54 < spreadLayout.RowPaneCount; num54++)
                {
                    double num55 = spreadLayout.OrnamentX;
                    double verticalSplitBoxY = spreadLayout.GetVerticalSplitBoxY(num54);
                    double num57 = spreadLayout.OrnamentWidth;
                    double verticalSplitBoxHeight = spreadLayout.GetVerticalSplitBoxHeight(num54);
                    _verticalSplitBox[num54].Arrange(new Rect(num55, verticalSplitBoxY, num57, verticalSplitBoxHeight));
                }
            }
            if (_horizontalSplitBar != null)
            {
                for (int num59 = 0; num59 < (spreadLayout.ColumnPaneCount - 1); num59++)
                {
                    if ((_horizontalSplitBar[num59] != null) && (_horizontalSplitBar[num59].Parent != null))
                    {
                        double horizontalSplitBarX = spreadLayout.GetHorizontalSplitBarX(num59);
                        if (IsTouching && (_cachedViewportSplitBarX != null))
                        {
                            horizontalSplitBarX = _cachedViewportSplitBarX[num59];
                        }
                        double num61 = spreadLayout.Y;
                        double horizontalSplitBarWidth = spreadLayout.GetHorizontalSplitBarWidth(num59);
                        double num63 = AvailableSize.Height;
                        _horizontalSplitBar[num59].Arrange(new Rect(horizontalSplitBarX, num61, horizontalSplitBarWidth, num63));
                    }
                }
            }
            if (_verticalSplitBar != null)
            {
                for (int num64 = 0; num64 < (spreadLayout.RowPaneCount - 1); num64++)
                {
                    if ((_verticalSplitBar[num64] != null) && (_verticalSplitBar[num64].Parent != null))
                    {
                        double num65 = spreadLayout.X;
                        double verticalSplitBarY = spreadLayout.GetVerticalSplitBarY(num64);
                        if (IsTouching && (_cachedViewportSplitBarY != null))
                        {
                            verticalSplitBarY = _cachedViewportSplitBarY[num64];
                        }
                        double num67 = AvailableSize.Width;
                        double verticalSplitBarHeight = spreadLayout.GetVerticalSplitBarHeight(num64);
                        _verticalSplitBar[num64].Arrange(new Rect(num65, verticalSplitBarY, num67, verticalSplitBarHeight));
                    }
                }
            }
            if (_crossSplitBar != null)
            {
                for (int num69 = 0; num69 < _crossSplitBar.GetLength(0); num69++)
                {
                    double num70 = spreadLayout.GetVerticalSplitBarY(num69);
                    if (IsTouching && (_cachedViewportSplitBarY != null))
                    {
                        num70 = _cachedViewportSplitBarY[num69];
                    }
                    double num71 = spreadLayout.GetVerticalSplitBarHeight(num69);
                    for (int num72 = 0; num72 < _crossSplitBar.GetLength(1); num72++)
                    {
                        double num73 = spreadLayout.GetHorizontalSplitBarX(num72);
                        if (IsTouching && (_cachedViewportSplitBarX != null))
                        {
                            num73 = _cachedViewportSplitBarX[num72];
                        }
                        double num74 = spreadLayout.GetHorizontalSplitBarWidth(num72);
                        if ((_crossSplitBar[num69, num72] != null) && (_crossSplitBar[num69, num72].Parent != null))
                        {
                            _crossSplitBar[num69, num72].Arrange(new Rect(num73, num70, num74, num71));
                        }
                    }
                }
            }
            ArrangeRangeGroup(spreadLayout.RowPaneCount, spreadLayout.ColumnPaneCount, spreadLayout);
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
