#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        void InitLayout()
        {
            _hoverManager = new HoverManager(this);
            _dragStartRowViewport = -2;
            _dragStartColumnViewport = -2;
            _dragToColumnViewport = -2;
            _dragToRowViewport = -2;
            _dragToColumn = -2;
            _dragToRow = -2;
            _cachedResizerGipper = new Dictionary<string, BitmapImage>();
            _cachedToolbarImageSources = new Dictionary<string, ImageSource>();

            _showScrollTip = false;
            FormulaSelectionGripperContainerPanel panel = new FormulaSelectionGripperContainerPanel
            {
                Excel = this
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

            _cachedColumnResizerGripperImage = new Image { Source = GetResizerBitmapImage(false) };
            _resizerGripperContainer.Child = _cachedColumnResizerGripperImage;
            _cachedRowResizerGripperImage = new Image { Source = GetResizerBitmapImage(true) };
            _cachedResizerGipper = new Dictionary<string, BitmapImage>();
            _cachedautoFillIndicatorImage = new Image { Source = GetImageSource("AutoFillIndicator.png") };
            _autoFillIndicatorContainer.Child = _cachedautoFillIndicatorImage;
            _cachedToolbarImageSources = new Dictionary<string, ImageSource>();

            Background = BrushRes.浅灰背景;
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
                SaveHitInfo(null);

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

            //**************最底部三层为：视口 列头 行头，因可能动态增删，采用Insert到0的方式，后插入的在底层**************/
            bool reload = (_cellsPanels != null)
                && (ActiveSheet == null || _cellsPanels.GetUpperBound(0) != layout.RowPaneCount + 1 || _cellsPanels.GetUpperBound(1) != layout.ColumnPaneCount + 1);

            // 行头
            if (reload)
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
                            Children.Insert(0, header);
                        }
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
            if (reload)
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
                            Children.Insert(0, colPanel);
                        }
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

            // 多视口
            CellsPanel[,] viewportArray = null;
            if (reload)
            {
                // 视口数目变化时
                int upperRow = _cellsPanels.GetUpperBound(0);
                int upperCol = _cellsPanels.GetUpperBound(1);
                for (int i = _cellsPanels.GetLowerBound(0); i <= upperRow; i++)
                {
                    for (int j = _cellsPanels.GetLowerBound(1); j <= upperCol; j++)
                    {
                        CellsPanel viewport = _cellsPanels[i, j];
                        if (viewport != null)
                            viewport.RemoveDataValidationUI();
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
                for (int j = -1; j <= layout.RowPaneCount; j++)
                {
                    double viewportHeight = layout.GetViewportHeight(j);
                    viewportY = layout.GetViewportY(j);
                    if (((_cellsPanels[j + 1, i + 1] == null) && (viewportWidth > 0.0)) && (viewportHeight > 0.0))
                    {
                        if (((viewportArray != null) && ((j + 1) < viewportArray.GetUpperBound(0))) && (((i + 1) < viewportArray.GetUpperBound(1)) && (viewportArray[j + 1, i + 1] != null)))
                        {
                            _cellsPanels[j + 1, i + 1] = viewportArray[j + 1, i + 1];
                        }
                        else
                        {
                            _cellsPanels[j + 1, i + 1] = new CellsPanel(this);
                        }
                    }

                    CellsPanel cellPanel = _cellsPanels[j + 1, i + 1];
                    if ((viewportWidth > 0.0) && (viewportHeight > 0.0))
                    {
                        cellPanel.Location = new Point(viewportX, viewportY);
                        cellPanel.ColumnViewportIndex = i;
                        cellPanel.RowViewportIndex = j;
                        if (!Children.Contains(cellPanel))
                        {
                            // 单元格区域放最低层
                            Children.Insert(0, cellPanel);
                        }
                        cellPanel.Measure(new Size(viewportWidth, viewportHeight));
                    }
                    else if (cellPanel != null)
                    {
                        Children.Remove(cellPanel);
                        _cellsPanels[j + 1, i + 1] = null;
                    }
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
                for (int i = 0; i < layout.ColumnPaneCount; i++)
                {
                    ScrollBar bar = _horizontalScrollBar[i];
                    double barWidth = layout.GetHorizontalScrollBarWidth(i);
                    if (barWidth > 0.0)
                    {
                        if (!Children.Contains(bar))
                        {
                            Children.Add(bar);
                        }
                        bar.Measure(new Size(barWidth, HorizontalScrollBarHeight));
                    }
                    else
                    {
                        Children.Remove(bar);
                    }

                    var box = _horizontalSplitBox[i];
                    double splitBoxWidth = layout.GetHorizontalSplitBoxWidth(i);
                    if (splitBoxWidth > 0.0)
                    {
                        if (!Children.Contains(box))
                        {
                            Children.Add(box);
                        }
                        bar.Measure(new Size(splitBoxWidth, HorizontalScrollBarHeight));
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
                    foreach (var box2 in _horizontalSplitBox)
                    {
                        Children.Remove(box2);
                    }
                }
            }

            // 垂直滚动栏
            if (layout.OrnamentWidth > 0.0)
            {
                for (int i = 0; i < layout.RowPaneCount; i++)
                {
                    ScrollBar bar = _verticalScrollBar[i];
                    double barHeight = layout.GetVerticalScrollBarHeight(i);
                    if (barHeight > 0.0)
                    {
                        if (!Children.Contains(bar))
                        {
                            Children.Add(bar);
                        }
                        bar.Measure(new Size(VerticalScrollBarWidth, barHeight));
                    }
                    else
                    {
                        Children.Remove(bar);
                    }

                    var vbox = _verticalSplitBox[i];
                    double vboxHeight = layout.GetVerticalSplitBoxHeight(i);
                    if (vboxHeight > 0.0)
                    {
                        if (!Children.Contains(vbox))
                        {
                            Children.Add(vbox);
                        }
                        vbox.Measure(new Size(VerticalScrollBarWidth, vboxHeight));
                    }
                    else
                    {
                        Children.Remove(vbox);
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
                    foreach (var box4 in _verticalSplitBox)
                    {
                        Children.Remove(box4);
                    }
                }
            }

            // 水平/垂直分隔栏
            for (int j = 0; j < (layout.ColumnPaneCount - 1); j++)
            {
                var bar = _horizontalSplitBar[j];
                if (!Children.Contains(bar))
                {
                    Children.Add(bar);
                }
                bar.Measure(new Size(_defaultSplitBarSize, _availableSize.Height));
            }
            for (int k = 0; k < (layout.RowPaneCount - 1); k++)
            {
                var bar = _verticalSplitBar[k];
                if (!Children.Contains(bar))
                {
                    Children.Add(bar);
                }
                bar.Measure(new Size(_availableSize.Width, _defaultSplitBarSize));
            }
            for (int i = 0; i < (layout.RowPaneCount - 1); i++)
            {
                for (int j = 0; j < (layout.ColumnPaneCount - 1); j++)
                {
                    var bar = _crossSplitBar[i, j];
                    if (!Children.Contains(bar))
                    {
                        Children.Add(bar);
                    }
                    bar.Measure(new Size(_availableSize.Width, _availableSize.Width));
                }
            }
            MeasureRangeGroup(layout.RowPaneCount, layout.ColumnPaneCount, layout);

            // sheet标签 
            if (layout.TabStripHeight > 0.0)
            {
                if (_tabStrip == null)
                {
                    _tabStrip = new TabStrip(this);
                    _tabStrip.ActiveTabChanging += OnTabStripActiveTabChanging;
                    _tabStrip.ActiveTabChanged += OnTabStripActiveTabChanged;
                    _tabStrip.NewTabNeeded += OnTabStripNewTabNeeded;
                }

                if (!Children.Contains(_tabStrip))
                {
                    Children.Add(_tabStrip);
                }
                _tabStrip.Measure(new Size(layout.TabStripWidth, layout.TabStripHeight));
            }
            else if (_tabStrip != null)
            {
                Children.Remove(_tabStrip);
                _tabStrip.ActiveTabChanging -= OnTabStripActiveTabChanging;
                _tabStrip.ActiveTabChanged -= OnTabStripActiveTabChanged;
                _tabStrip.NewTabNeeded -= OnTabStripNewTabNeeded;
                _tabStrip = null;
            }

            // 跟踪层，调整行列宽高时的虚线，拖拽时的标志，冻结线
            if (!Children.Contains(TrackersContainer))
            {
                Children.Add(TrackersContainer);
            }
            TrackersContainer.Measure(availableSize);

            // 分隔栏层，多视口时的分割线
            if (!Children.Contains(SplittingTrackerContainer))
            {
                Children.Add(SplittingTrackerContainer);
            }
            SplittingTrackerContainer.Measure(availableSize);

            // 光标层，各种图标的png图片
            if (!Children.Contains(CursorsContainer))
            {
                Children.Add(CursorsContainer);
            }
            CursorsContainer.Measure(availableSize);

            // 触摸时调整选择范围的圈圈
            if (!Children.Contains(_topLeftGripper))
            {
                Children.Add(_topLeftGripper);
            }
            _topLeftGripper.Stroke = new SolidColorBrush(GetGripperStrokeColor());
            _topLeftGripper.Fill = new SolidColorBrush(GetGripperFillColor());
            _topLeftGripper.Measure(availableSize);

            if (!Children.Contains(_bottomRightGripper))
            {
                Children.Add(_bottomRightGripper);
            }
            _bottomRightGripper.Stroke = new SolidColorBrush(GetGripperStrokeColor());
            _bottomRightGripper.Fill = new SolidColorBrush(GetGripperFillColor());
            _bottomRightGripper.Measure(availableSize);

            if (!Children.Contains(_resizerGripperContainer))
            {
                Children.Add(_resizerGripperContainer);
            }
            _resizerGripperContainer.Measure(availableSize);

            if (!Children.Contains(_autoFillIndicatorContainer))
            {
                Children.Add(_autoFillIndicatorContainer);
            }
            _autoFillIndicatorContainer.Width = 16.0;
            _autoFillIndicatorContainer.Height = 16.0;
            _autoFillIndicatorContainer.Measure(availableSize);

            if (!Children.Contains(_formulaSelectionGripperPanel))
            {
                Children.Add(_formulaSelectionGripperPanel);
            }
            _formulaSelectionGripperPanel.Measure(availableSize);

            // 进度环
            _progressRing?.Measure(availableSize);
            return _availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rcFull = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
            TrackersContainer.Arrange(rcFull);
            SplittingTrackerContainer.Arrange(rcFull);
            ShapeDrawingContainer.Arrange(rcFull);
            CursorsContainer.Arrange(rcFull);

            double headerX;
            double headerY;
            SheetLayout layout = GetSheetLayout();

            // 左上角
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

            // 列头
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

            // 行头
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

            // 单元格区域
            if (IsTouchZooming && (_cachedViewportTransform != null))
            {
                for (int i = -1; i <= layout.ColumnPaneCount; i++)
                {
                    headerX = layout.GetViewportX(i);
                    double num20 = layout.GetViewportWidth(i);
                    for (int j = -1; j <= layout.RowPaneCount; j++)
                    {
                        headerY = layout.GetViewportY(j);
                        double num22 = layout.GetViewportHeight(j);
                        CellsPanel viewport5 = _cellsPanels[j + 1, i + 1];
                        if (viewport5 != null)
                        {
                            viewport5.Arrange(new Rect(headerX, headerY, num20, num22));
                            viewport5.RenderTransform = _cachedViewportTransform[j + 1, i + 1];
                        }
                    }
                }
            }
            else if (_cellsPanels != null)
            {
                for (int i = -1; i <= layout.ColumnPaneCount; i++)
                {
                    headerX = layout.GetViewportX(i);
                    if ((IsTouching && (i == layout.ColumnPaneCount)) && ((_translateOffsetX < 0.0) && (_touchStartHitTestInfo.ColumnViewportIndex == (layout.ColumnPaneCount - 1))))
                    {
                        headerX += _translateOffsetX;
                    }
                    double num24 = layout.GetViewportWidth(i);
                    for (int j = -1; j <= layout.RowPaneCount; j++)
                    {
                        headerY = layout.GetViewportY(j);
                        if (((IsTouching && IsTouching) && ((j == layout.RowPaneCount) && (_translateOffsetY < 0.0))) && (_touchStartHitTestInfo.RowViewportIndex == (layout.RowPaneCount - 1)))
                        {
                            headerY += _translateOffsetY;
                        }
                        double num26 = layout.GetViewportHeight(j);
                        CellsPanel viewport6 = _cellsPanels[j + 1, i + 1];
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
                                    if ((_touchStartHitTestInfo != null) && (i == _touchStartHitTestInfo.ColumnViewportIndex))
                                    {
                                        num29 += num27;
                                    }
                                    if ((_touchStartHitTestInfo != null) && (j == _touchStartHitTestInfo.RowViewportIndex))
                                    {
                                        num30 += num28;
                                    }
                                    viewport6.Arrange(new Rect(num29, num30, num24, num26));
                                    if (((headerY != num30) && (_translateOffsetY < 0.0)) || ((headerX != num29) && (_translateOffsetX < 0.0)))
                                    {
                                        RectangleGeometry geometry5 = new RectangleGeometry();
                                        geometry5.Rect = new Rect(Math.Abs((double)(headerX - num29)), Math.Abs((double)(headerY - num30)), _cachedViewportWidths[i + 1], _cachedViewportHeights[j + 1]);
                                        viewport6.Clip = geometry5;
                                    }
                                    else if ((headerX != num29) && (_translateOffsetX > 0.0))
                                    {
                                        RectangleGeometry geometry6 = new RectangleGeometry();
                                        geometry6.Rect = new Rect(0.0, 0.0, Math.Max((double)0.0, (double)(_cachedViewportWidths[i + 1] - _translateOffsetX)), _cachedViewportHeights[j + 1]);
                                        viewport6.Clip = geometry6;
                                    }
                                    else if ((headerY != num30) && (_translateOffsetY > 0.0))
                                    {
                                        RectangleGeometry geometry7 = new RectangleGeometry();
                                        geometry7.Rect = new Rect(0.0, 0.0, _cachedViewportWidths[i + 1], Math.Max((double)0.0, (double)(_cachedViewportHeights[j + 1] - _translateOffsetY)));
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
                for (int i = 0; i < layout.ColumnPaneCount; i++)
                {
                    _horizontalScrollBar[i].Arrange(new Rect(layout.GetHorizontalScrollBarX(i), layout.OrnamentY, layout.GetHorizontalScrollBarWidth(i), layout.OrnamentHeight));
                }
            }

            if (_tabStrip != null)
                _tabStrip.Arrange(new Rect(layout.TabStripX, layout.TabStripY, layout.TabStripWidth, layout.TabStripHeight));

            if (_horizontalSplitBox != null)
            {
                for (int i = 0; i < layout.ColumnPaneCount; i++)
                {
                    _horizontalSplitBox[i].Arrange(new Rect(layout.GetHorizontalSplitBoxX(i), layout.OrnamentY, layout.GetHorizontalSplitBoxWidth(i), layout.OrnamentHeight));
                }
            }

            if (_verticalScrollBar != null)
            {
                for (int i = 0; i < layout.RowPaneCount; i++)
                {
                    _verticalScrollBar[i].Arrange(new Rect(layout.OrnamentX, layout.GetVerticalScrollBarY(i), layout.OrnamentWidth, layout.GetVerticalScrollBarHeight(i)));
                }
            }
            if (_verticalSplitBox != null)
            {
                for (int i = 0; i < layout.RowPaneCount; i++)
                {
                    _verticalSplitBox[i].Arrange(new Rect(layout.OrnamentX, layout.GetVerticalSplitBoxY(i), layout.OrnamentWidth, layout.GetVerticalSplitBoxHeight(i)));
                }
            }
            if (_horizontalSplitBar != null)
            {
                for (int i = 0; i < (layout.ColumnPaneCount - 1); i++)
                {
                    if ((_horizontalSplitBar[i] != null) && (_horizontalSplitBar[i].Parent != null))
                    {
                        double horizontalSplitBarX = layout.GetHorizontalSplitBarX(i);
                        if (IsTouching && (_cachedViewportSplitBarX != null))
                        {
                            horizontalSplitBarX = _cachedViewportSplitBarX[i];
                        }
                        _horizontalSplitBar[i].Arrange(new Rect(horizontalSplitBarX, layout.Y, _defaultSplitBarSize, _availableSize.Height));
                    }
                }
            }
            if (_verticalSplitBar != null)
            {
                for (int i = 0; i < (layout.RowPaneCount - 1); i++)
                {
                    if ((_verticalSplitBar[i] != null) && (_verticalSplitBar[i].Parent != null))
                    {
                        double verticalSplitBarY = layout.GetVerticalSplitBarY(i);
                        if (IsTouching && (_cachedViewportSplitBarY != null))
                        {
                            verticalSplitBarY = _cachedViewportSplitBarY[i];
                        }
                        _verticalSplitBar[i].Arrange(new Rect(layout.X, verticalSplitBarY, _availableSize.Width, _defaultSplitBarSize));
                    }
                }
            }
            if (_crossSplitBar != null)
            {
                for (int i = 0; i < _crossSplitBar.GetLength(0); i++)
                {
                    double num70 = layout.GetVerticalSplitBarY(i);
                    if (IsTouching && (_cachedViewportSplitBarY != null))
                    {
                        num70 = _cachedViewportSplitBarY[i];
                    }
                    double num71 = layout.GetVerticalSplitBarHeight(i);
                    for (int j = 0; j < _crossSplitBar.GetLength(1); j++)
                    {
                        double num73 = layout.GetHorizontalSplitBarX(j);
                        if (IsTouching && (_cachedViewportSplitBarX != null))
                        {
                            num73 = _cachedViewportSplitBarX[j];
                        }
                        double num74 = layout.GetHorizontalSplitBarWidth(j);
                        if ((_crossSplitBar[i, j] != null) && (_crossSplitBar[i, j].Parent != null))
                        {
                            _crossSplitBar[i, j].Arrange(new Rect(num73, num70, num74, num71));
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

            _progressRing?.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

        SheetLayout CreateLayout()
        {
            var sheet = ActiveSheet;
            ViewportInfo viewportInfo = GetViewportInfo(sheet);
            double width = _availableSize.Width;
            double height = _availableSize.Height;
            SheetLayout layout = new SheetLayout(viewportInfo.RowViewportCount, viewportInfo.ColumnViewportCount);

            layout.TabStripX = 0.0;
            if (TabStripVisibility == Visibility.Visible)
                layout.TabStripHeight = _defaultTabStripHeight;
            layout.TabStripY = Math.Max(0.0, height - layout.TabStripHeight);
            layout.TabStripWidth = Math.Max(0.0, width);

            if ((sheet == null) || !sheet.Visible)
                return layout;

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
            for (int i = sheet.FrozenRowCount; (tempHeight <= height) && (i < (sheet.RowCount - sheet.FrozenTrailingRowCount)); i++)
            {
                tempHeight += Math.Ceiling((double)(sheet.GetActualRowHeight(i, SheetArea.Cells) * zoomFactor));
            }
            totalWidth += tempWidth;
            totalHeight += tempHeight;

            bool showHorScrollBar = (HorizontalScrollBarVisibility == ScrollBarVisibility.Visible) || (HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled);
            if (HorizontalScrollBarVisibility == ScrollBarVisibility.Auto)
            {
                if (layout.ColumnPaneCount > 1)
                {
                    // 多列视口
                    showHorScrollBar = true;
                }
                //else if ((VerticalScrollBarVisibility == ScrollBarVisibility.Visible) || (VerticalScrollBarVisibility == ScrollBarVisibility.Disabled))
                //{
                //    flag |= totalWidth > ((width - VerticalScrollBarWidth) - groupLayout.Width);
                //}
                //else if (VerticalScrollBarVisibility == ScrollBarVisibility.Auto)
                //{
                //    if (tempHeight > height)
                //    {
                //        flag |= totalWidth > ((width - VerticalScrollBarWidth) - groupLayout.Width);
                //    }
                //    else
                //    {
                //        flag |= totalWidth > (width - groupLayout.Width);
                //    }
                //}
                else
                {
                    // 垂直滚动栏不独立占用宽度
                    showHorScrollBar |= totalWidth > (width - groupLayout.Width);
                }
            }
            if (showHorScrollBar)
            {
                // 显示水平滚动栏
                layout.OrnamentHeight = HorizontalScrollBarHeight;

                // 水平滚动栏不独立占用高度
                //height -= layout.OrnamentHeight;
                //height = Math.Max(0.0, height);
            }

            bool showVerScrollBar = ((VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                || (VerticalScrollBarVisibility == ScrollBarVisibility.Disabled))
                || ((VerticalScrollBarVisibility == ScrollBarVisibility.Auto) && ((layout.RowPaneCount > 1) || (totalHeight > (height - groupLayout.Height))));
            if (showVerScrollBar)
            {
                // 显示垂直滚动栏
                layout.OrnamentWidth = VerticalScrollBarWidth;

                // 垂直滚动栏不独立占用宽度
                //width -= layout.OrnamentWidth;
                //width = Math.Max(0.0, width);
            }

            // 内容区的可见宽度
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

            // 内容区的可见高度
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

            // 设置多视口分隔栏的宽高，并从可见大小中扣除
            for (int i = 0; i < (layout.ColumnPaneCount - 1); i++)
            {
                layout.SetHorizontalSplitBarWidth(i, _defaultSplitBarSize);
                width -= _defaultSplitBarSize;
                width = Math.Max(0.0, width);
            }
            for (int i = 0; i < (layout.RowPaneCount - 1); i++)
            {
                layout.SetVerticalSplitBarHeight(i, _defaultSplitBarSize);
                height -= _defaultSplitBarSize;
                height = Math.Max(0.0, height);
            }

            // 为未设置大小的Viewport设置尺寸，剩余大小平均分配
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

            // 计算每个视口的左上角位置
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
                // y位置需扣除本身的高度和SheetTab的高度
                double oy = layout.GetViewportY(layout.RowPaneCount - 1) + layout.GetViewportHeight(layout.RowPaneCount - 1) + layout.FrozenTrailingHeight - HorizontalScrollBarHeight;
                if (layout.TabStripHeight > 0)
                    oy -= layout.TabStripHeight;
                layout.OrnamentY = Math.Max(oy, 0);
            }
            if (layout.OrnamentWidth > 0.0)
            {
                // 扣除ScrollBar本身宽度
                double oy = layout.GetViewportX(layout.ColumnPaneCount - 1) + layout.GetViewportWidth(layout.ColumnPaneCount - 1) + layout.FrozenTrailingWidth - VerticalScrollBarWidth;
                layout.OrnamentX = Math.Max(oy, 0);
            }

            double columnSplitBoxesWidth = GetColumnSplitBoxesWidth(layout.ColumnPaneCount);
            if (layout.ColumnPaneCount == 1)
            {
                double vWidth = layout.GetViewportWidth(0) + layout.FrozenTrailingWidth + layout.HeaderX + layout.HeaderWidth + layout.FrozenWidth;
                double boxWidth = Math.Min(vWidth, columnSplitBoxesWidth);
                double svWidth = Math.Max(0.0, vWidth - boxWidth);
                if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                {
                    layout.SetHorizontalSplitBoxX(0, layout.X);
                    layout.SetHorizontalSplitBoxWidth(0, boxWidth);
                    layout.SetHorizontalScrollBarX(0, layout.X + boxWidth);
                    layout.SetHorizontalScrollBarWidth(0, svWidth);
                }
                else
                {
                    layout.SetHorizontalScrollBarX(0, layout.X);
                    layout.SetHorizontalScrollBarWidth(0, svWidth);
                    layout.SetHorizontalSplitBoxX(0, layout.X + svWidth);
                    layout.SetHorizontalSplitBoxWidth(0, boxWidth);
                }
            }
            else if (layout.ColumnPaneCount > 1)
            {
                for (int i = 0; i < layout.ColumnPaneCount; i++)
                {
                    if (i == 0)
                    {
                        double vWidth = ((layout.HeaderX + layout.HeaderWidth) + layout.FrozenWidth) + layout.GetViewportWidth(i);
                        double x = layout.X;
                        double boxWidth = Math.Min(vWidth, columnSplitBoxesWidth);
                        double svWidth = Math.Max(0.0, vWidth - boxWidth);
                        if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                        {
                            layout.SetHorizontalSplitBoxX(i, x);
                            layout.SetHorizontalSplitBoxWidth(i, boxWidth);
                            layout.SetHorizontalScrollBarX(i, x + boxWidth);
                            layout.SetHorizontalScrollBarWidth(i, svWidth);
                        }
                        else
                        {
                            layout.SetHorizontalScrollBarX(i, x);
                            layout.SetHorizontalScrollBarWidth(i, svWidth);
                            layout.SetHorizontalSplitBoxX(i, x + svWidth);
                            layout.SetHorizontalSplitBoxWidth(i, boxWidth);
                        }
                    }
                    else
                    {
                        double viewportWidth = layout.GetViewportWidth(i);
                        if (i == layout.ColumnPaneCount - 1)
                            viewportWidth += layout.FrozenTrailingWidth;
                        double viewportX = layout.GetViewportX(i);
                        double boxWidth = Math.Min(viewportWidth, columnSplitBoxesWidth);
                        double svWidth = Math.Max(0.0, viewportWidth - boxWidth);
                        if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                        {
                            layout.SetHorizontalSplitBoxX(i, viewportX);
                            layout.SetHorizontalSplitBoxWidth(i, boxWidth);
                            layout.SetHorizontalScrollBarX(i, viewportX + boxWidth);
                            layout.SetHorizontalScrollBarWidth(i, svWidth);
                        }
                        else
                        {
                            layout.SetHorizontalScrollBarX(i, viewportX);
                            layout.SetHorizontalScrollBarWidth(i, svWidth);
                            layout.SetHorizontalSplitBoxX(i, viewportX + svWidth);
                            layout.SetHorizontalSplitBoxWidth(i, svWidth);
                        }
                    }
                }
            }

            double rowSplitBoxesHeight = GetRowSplitBoxesHeight(layout.RowPaneCount);
            if (layout.RowPaneCount == 1)
            {
                // 仅一行视口
                double vHeight = layout.GetViewportHeight(0) + layout.FrozenTrailingHeight + layout.HeaderY + layout.HeaderHeight + layout.FrozenHeight;
                double vY = layout.Y;
                double boxHeight = Math.Min(vHeight, rowSplitBoxesHeight);
                double scrollBarHeight = Math.Max(0.0, vHeight - boxHeight - layout.TabStripHeight);
                if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                {
                    layout.SetVerticalSplitBoxY(0, vY);
                    layout.SetVerticalSplitBoxHeight(0, boxHeight);
                    layout.SetVerticalScrollBarY(0, vY + boxHeight);
                    layout.SetVerticalScrollBarHeight(0, scrollBarHeight);
                }
                else
                {
                    layout.SetVerticalScrollBarY(0, vY);
                    layout.SetVerticalScrollBarHeight(0, scrollBarHeight);
                    layout.SetVerticalSplitBoxY(0, vY + scrollBarHeight);
                    layout.SetVerticalSplitBoxHeight(0, boxHeight);
                }
            }
            else if (layout.RowPaneCount > 1)
            {
                // 多行视口
                for (int i = 0; i < layout.RowPaneCount; i++)
                {
                    if (i == 0)
                    {
                        double vHeight = layout.HeaderY + layout.HeaderHeight + layout.FrozenHeight + layout.GetViewportHeight(i);
                        double boxHeight = Math.Min(vHeight, rowSplitBoxesHeight);
                        if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                        {
                            layout.SetVerticalSplitBoxY(i, layout.Y);
                            layout.SetVerticalSplitBoxHeight(i, boxHeight);
                            layout.SetVerticalScrollBarY(i, layout.Y + boxHeight);
                            layout.SetVerticalScrollBarHeight(i, Math.Max(0.0, vHeight - boxHeight));
                        }
                        else
                        {
                            layout.SetVerticalScrollBarY(i, layout.Y);
                            layout.SetVerticalScrollBarHeight(i, Math.Max(0.0, vHeight - boxHeight));
                            layout.SetVerticalSplitBoxHeight(i, boxHeight);
                            layout.SetVerticalSplitBoxY(i, layout.Y + layout.GetVerticalScrollBarHeight(i));
                        }
                    }
                    else if (i < layout.RowPaneCount - 1)
                    {
                        double vHeight = layout.GetViewportHeight(i);
                        double vY = layout.GetViewportY(i);
                        double boxHeight = Math.Min(vHeight, rowSplitBoxesHeight);
                        if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                        {
                            layout.SetVerticalSplitBoxY(i, vY);
                            layout.SetVerticalSplitBoxHeight(i, boxHeight);
                            layout.SetVerticalScrollBarY(i, vY + boxHeight);
                            layout.SetVerticalScrollBarHeight(i, Math.Max(0.0, vHeight - boxHeight));
                        }
                        else
                        {
                            layout.SetVerticalScrollBarY(i, vY);
                            layout.SetVerticalScrollBarHeight(i, Math.Max(0.0, vHeight - boxHeight));
                            layout.SetVerticalSplitBoxY(i, vY + layout.GetVerticalScrollBarHeight(i));
                            layout.SetVerticalSplitBoxHeight(i, boxHeight);
                        }
                    }
                    else if (i == layout.RowPaneCount - 1)
                    {
                        double vHeight = layout.GetViewportHeight(i) + layout.FrozenTrailingHeight;
                        double vY = layout.GetViewportY(i);
                        double boxHeight = Math.Min(vHeight, rowSplitBoxesHeight);
                        double scrollBarHeight = Math.Max(0.0, vHeight - boxHeight - layout.TabStripHeight);
                        if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                        {
                            layout.SetVerticalSplitBoxY(i, vY);
                            layout.SetVerticalSplitBoxHeight(i, boxHeight);
                            layout.SetVerticalScrollBarY(i, vY + boxHeight);
                            layout.SetVerticalScrollBarHeight(i, scrollBarHeight);
                        }
                        else
                        {
                            layout.SetVerticalScrollBarY(i, vY);
                            layout.SetVerticalScrollBarHeight(i, scrollBarHeight);
                            layout.SetVerticalSplitBoxY(i, vY + scrollBarHeight);
                            layout.SetVerticalSplitBoxHeight(i, boxHeight);
                        }
                    }
                }
            }

            double avWidth = _availableSize.Width;
            if (double.IsInfinity(avWidth) || double.IsNaN(avWidth))
            {
                avWidth = 0.0;
                for (int i = 0; i < (layout.ColumnPaneCount - 1); i++)
                {
                    avWidth += layout.GetHorizontalSplitBarWidth(i);
                }
                for (int i = 0; i < layout.ColumnPaneCount; i++)
                {
                    avWidth += layout.GetViewportWidth(i);
                }
            }
            double avHeight = _availableSize.Height;
            if (double.IsInfinity(avHeight) || double.IsNaN(avHeight))
            {
                avHeight = 0.0;
                for (int i = 0; i < (layout.RowPaneCount - 1); i++)
                {
                    avHeight += layout.GetVerticalSplitBarHeight(i);
                }
                for (int i = 0; i < layout.RowPaneCount; i++)
                {
                    avHeight += layout.GetViewportHeight(i);
                }
                if (layout.TabStripHeight > 0.0)
                {
                    avHeight += layout.TabStripHeight;
                }
            }
            _availableSize = new Size(avWidth, avHeight);
            return layout;
        }

        async Task OpenStream(DispatchedHandler p_handler)
        {
            try
            {
                ShowProgressRing();
                // 显示进度圈，不然界面混乱
                await Task.Delay(100);

                InitLayout();
                ClearValue(TabStripInsertTabProperty);
                ClearValue(TabStripVisibilityProperty);
                ClearValue(TabStripEditableProperty);
                ClearValue(CanTouchMultiSelectProperty);
                ClearValue(CanUserDragDropProperty);
                ClearValue(CanUserDragFillProperty);
                ClearValue(CanUserEditFormulaProperty);
                ClearValue(CanUserUndoProperty);
                ClearValue(CanUserZoomProperty);
                ClearValue(FreezeLineStyleProperty);
                ClearValue(HighlightInvalidDataProperty);
                ClearValue(ColumnSplitBoxAlignmentProperty);
                ClearValue(ColumnSplitBoxPolicyProperty);
                ClearValue(HorizontalScrollBarHeightProperty);
                ClearValue(DefaultAutoFillTypeProperty);
                ClearValue(HorizontalScrollBarStyleProperty);
                ClearValue(RangeGroupBackgroundProperty);
                ClearValue(RangeGroupBorderBrushProperty);
                ClearValue(RangeGroupLineStrokeProperty);
                ClearValue(ShowColumnRangeGroupProperty);
                ClearValue(ShowFreezeLineProperty);
                ClearValue(ShowRowRangeGroupProperty);
                ClearValue(VerticalScrollBarStyleProperty);
                ClearValue(VerticalScrollBarWidthProperty);
                ClearValue(HorizontalScrollableProperty);
                ClearValue(VerticalScrollableProperty);
                ClearValue(TrailingFreezeLineStyleProperty);
                ClearValue(ShowDecorationProperty);

                if (_tabStrip != null)
                {
                    _tabStrip.ActiveTabChanging -= OnTabStripActiveTabChanging;
                    _tabStrip.ActiveTabChanged -= OnTabStripActiveTabChanged;
                    _tabStrip.NewTabNeeded -= OnTabStripNewTabNeeded;
                    _tabStrip = null;
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, p_handler);
            }
            finally
            {
                HideProgressRing();
            }
        }

        void ShowProgressRing()
        {
            HideProgressRing();
            _progressRing = new Grid { Background = BrushRes.浅灰背景 };
            var ring = new ProgressRing
            {
                Foreground = BrushRes.主题蓝色,
                IsActive = true,
                Width = 100.0,
                Height = 100.0,
            };
            _progressRing.Children.Add(ring);
            Children.Add(_progressRing);
        }

        void HideProgressRing()
        {
            if (_progressRing != null)
                Children.Remove(_progressRing);
        }
    }
}
