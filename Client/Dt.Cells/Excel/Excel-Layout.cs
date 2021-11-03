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
        #region 测量
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_cachedLastAvailableSize != availableSize)
            {
                _cachedLastAvailableSize = availableSize;
                _availableSize = availableSize;
                InvalidateLayout();
            }
            if (!IsWorking)
                SaveHitInfo(null);

            //**************最底部三层为：视口 列头 行头，因可能动态增删，采用Insert到0的方式，后插入的在底层**************/
            SheetLayout layout = GetSheetLayout();
            bool reload = (_cellsPanels != null)
                && (ActiveSheet == null || _cellsPanels.GetUpperBound(0) != layout.RowPaneCount + 1 || _cellsPanels.GetUpperBound(1) != layout.ColumnPaneCount + 1);

            // 行头
            MeasureRowHeaders(reload, layout);
            // 列头
            MeasureColHeaders(reload, layout);
            // 多视口
            MeasureCellPanels(reload, layout);
            // 左上角
            MeasureCornerPanel(layout);
            // 滚动栏
            MeasureScrollBar(layout);
            // 水平/垂直分隔栏
            MeasureSplitBars(layout);
            // 区域分组
            MeasureRangeGroup(layout);
            // sheet标签
            MeasureTabStrip(layout);
            // 触摸时调整选择范围的圈圈、调整列宽行高的标志、自动填充标志
            MeasureSelectionGripper();

            // 跟踪层：调整行列宽高时的虚线，拖拽时的标志，冻结线，动态调整时的分隔栏，各种png图片的光标
            UpdateFreezeLines();
            if (!Children.Contains(_trackersPanel))
                Children.Add(_trackersPanel);
            _trackersPanel.Measure(availableSize);

            // 进度环
            if (_progressRing != null)
            {
                if (!Children.Contains(_progressRing))
                    Children.Add(_progressRing);
                _progressRing.Measure(availableSize);
            }
            return _availableSize;
        }

        void MeasureRowHeaders(bool p_reload, SheetLayout p_layout)
        {
            if (p_reload)
            {
                foreach (var header in _rowHeaders)
                {
                    Children.Remove(header);
                }
                _rowHeaders = null;
            }

            if (_rowHeaders == null)
                _rowHeaders = new RowHeaderPanel[p_layout.RowPaneCount + 2];

            if (p_layout.HeaderWidth > 0.0)
            {
                for (int i = -1; i <= p_layout.RowPaneCount; i++)
                {
                    double height = p_layout.GetViewportHeight(i);
                    if ((_rowHeaders[i + 1] == null) && (height > 0.0))
                    {
                        _rowHeaders[i + 1] = new RowHeaderPanel(this);
                    }

                    var header = _rowHeaders[i + 1];
                    if (height > 0.0)
                    {
                        header.Location = new Point(p_layout.HeaderX, p_layout.GetViewportY(i));
                        header.RowViewportIndex = i;
                        if (!Children.Contains(header))
                        {
                            Children.Insert(0, header);
                        }
                        header.Measure(new Size(p_layout.HeaderWidth, height));
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
        }

        void MeasureColHeaders(bool p_reload, SheetLayout p_layout)
        {
            if (p_reload)
            {
                foreach (var panel in _colHeaders)
                {
                    Children.Remove(panel);
                }
                _colHeaders = null;
            }

            if (_colHeaders == null)
                _colHeaders = new ColHeaderPanel[p_layout.ColumnPaneCount + 2];

            if (p_layout.HeaderHeight > 0.0)
            {
                for (int i = -1; i <= p_layout.ColumnPaneCount; i++)
                {
                    double width = p_layout.GetViewportWidth(i);
                    if ((_colHeaders[i + 1] == null) && (width > 0.0))
                    {
                        _colHeaders[i + 1] = new ColHeaderPanel(this);
                    }

                    var colPanel = _colHeaders[i + 1];
                    if (width > 0.0)
                    {
                        colPanel.Location = new Point(p_layout.GetViewportX(i), p_layout.HeaderY);
                        colPanel.ColumnViewportIndex = i;
                        if (!Children.Contains(colPanel))
                        {
                            Children.Insert(0, colPanel);
                        }
                        colPanel.Measure(new Size(width, p_layout.HeaderHeight));
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
        }

        void MeasureCellPanels(bool p_reload, SheetLayout p_layout)
        {
            CellsPanel[,] viewportArray = null;
            if (p_reload)
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
                            Children.Remove(viewport);
                    }
                }
                viewportArray = _cellsPanels;
                _cellsPanels = null;
            }

            if (_cellsPanels == null)
                _cellsPanels = new CellsPanel[p_layout.RowPaneCount + 2, p_layout.ColumnPaneCount + 2];

            for (int i = -1; i <= p_layout.ColumnPaneCount; i++)
            {
                double viewportWidth = p_layout.GetViewportWidth(i);
                double viewportX = p_layout.GetViewportX(i);
                for (int j = -1; j <= p_layout.RowPaneCount; j++)
                {
                    double viewportHeight = p_layout.GetViewportHeight(j);
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
                        cellPanel.Location = new Point(viewportX, p_layout.GetViewportY(j));
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
        }

        void MeasureCornerPanel(SheetLayout p_layout)
        {
            if (_cornerPanel == null)
            {
                _cornerPanel = new CornerPanel(this);
            }
            if ((p_layout.HeaderWidth > 0.0) && (p_layout.HeaderHeight > 0.0))
            {
                if (!Children.Contains(_cornerPanel))
                {
                    Children.Add(_cornerPanel);
                }
                _cornerPanel.Measure(new Size(p_layout.HeaderWidth, p_layout.HeaderHeight));
            }
            else
            {
                Children.Remove(_cornerPanel);
                _cornerPanel = null;
            }
        }

        void MeasureScrollBar(SheetLayout p_layout)
        {
            UpdateHorizontalSplitBoxes();
            if (p_layout.OrnamentHeight > 0.0)
            {
                for (int i = 0; i < p_layout.ColumnPaneCount; i++)
                {
                    ScrollBar bar = _horizontalScrollBar[i];
                    double barWidth = p_layout.GetHorizontalScrollBarWidth(i);
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
                    double splitBoxWidth = p_layout.GetHorizontalSplitBoxWidth(i);
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

            UpdateVerticalSplitBoxes();
            if (p_layout.OrnamentWidth > 0.0)
            {
                for (int i = 0; i < p_layout.RowPaneCount; i++)
                {
                    ScrollBar bar = _verticalScrollBar[i];
                    double barHeight = p_layout.GetVerticalScrollBarHeight(i);
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
                    double vboxHeight = p_layout.GetVerticalSplitBoxHeight(i);
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
        }

        void MeasureSplitBars(SheetLayout p_layout)
        {
            UpdateHorizontalSplitBars();
            UpdateVerticalSplitBars();
            UpdateCrossSplitBars();

            for (int j = 0; j < (p_layout.ColumnPaneCount - 1); j++)
            {
                var bar = _horizontalSplitBar[j];
                if (!Children.Contains(bar))
                {
                    Children.Add(bar);
                }
                bar.Measure(new Size(_defaultSplitBarSize, _availableSize.Height));
            }
            for (int k = 0; k < (p_layout.RowPaneCount - 1); k++)
            {
                var bar = _verticalSplitBar[k];
                if (!Children.Contains(bar))
                {
                    Children.Add(bar);
                }
                bar.Measure(new Size(_availableSize.Width, _defaultSplitBarSize));
            }
            for (int i = 0; i < (p_layout.RowPaneCount - 1); i++)
            {
                for (int j = 0; j < (p_layout.ColumnPaneCount - 1); j++)
                {
                    var bar = _crossSplitBar[i, j];
                    if (!Children.Contains(bar))
                    {
                        Children.Add(bar);
                    }
                    bar.Measure(new Size(_availableSize.Width, _availableSize.Width));
                }
            }
        }

        void MeasureRangeGroup(SheetLayout p_layout)
        {
            GroupLayout groupLayout = GetGroupLayout();
            if (groupLayout.Width == 0.0 && groupLayout.Height == 0.0)
            {
                if (_rowGroupPresenters != null)
                    ClearRowGroups();

                if (_columnGroupPresenters != null)
                    ClearColumnGroups();

                if (_rowGroupHeaderPresenter != null)
                {
                    Children.Remove(_rowGroupHeaderPresenter);
                    _rowGroupHeaderPresenter = null;
                }

                if (_columnGroupHeaderPresenter != null)
                {
                    Children.Remove(_columnGroupHeaderPresenter);
                    _columnGroupHeaderPresenter = null;
                }

                if (_groupCornerPresenter != null)
                {
                    Children.Remove(_groupCornerPresenter);
                    _groupCornerPresenter = null;
                }
                return;
            }

            if ((_rowGroupPresenters != null)
                && ((ActiveSheet == null) || (_rowGroupPresenters.Length != p_layout.RowPaneCount + 2)))
            {
                ClearRowGroups();
            }

            if (_rowGroupPresenters == null)
            {
                _rowGroupPresenters = new GcRangeGroup[p_layout.RowPaneCount + 2];
            }
            if (groupLayout.Width > 0.0)
            {
                for (int i = -1; i <= p_layout.RowPaneCount; i++)
                {
                    double viewportY = p_layout.GetViewportY(i);
                    double viewportHeight = p_layout.GetViewportHeight(i);
                    if (_rowGroupPresenters[i + 1] == null)
                    {
                        GcRangeGroup group2 = new GcRangeGroup(this);
                        _rowGroupPresenters[i + 1] = group2;
                    }
                    GcRangeGroup group3 = _rowGroupPresenters[i + 1];
                    group3.Orientation = Orientation.Horizontal;
                    group3.ViewportIndex = i;
                    group3.Location = new Point(groupLayout.X, viewportY);
                    if (viewportHeight > 0.0)
                    {
                        if (!Children.Contains(group3))
                        {
                            Children.Add(group3);
                        }
                        group3.Measure(new Size(groupLayout.Width, viewportHeight));
                    }
                    else
                    {
                        Children.Remove(group3);
                        _rowGroupPresenters[i + 1] = null;
                    }
                }
            }
            else
            {
                ClearRowGroups();
            }

            if ((_columnGroupPresenters != null)
                && ((ActiveSheet == null) || (_columnGroupPresenters.Length != p_layout.ColumnPaneCount)))
            {
                ClearColumnGroups();
            }
            if (_columnGroupPresenters == null)
            {
                _columnGroupPresenters = new GcRangeGroup[p_layout.ColumnPaneCount + 2];
            }
            if (groupLayout.Height > 0.0)
            {
                for (int k = -1; k <= p_layout.ColumnPaneCount; k++)
                {
                    double viewportX = p_layout.GetViewportX(k);
                    double viewportWidth = p_layout.GetViewportWidth(k);
                    if (_columnGroupPresenters[k + 1] == null)
                    {
                        GcRangeGroup group5 = new GcRangeGroup(this);
                        _columnGroupPresenters[k + 1] = group5;
                    }
                    GcRangeGroup group6 = _columnGroupPresenters[k + 1];
                    group6.Orientation = Orientation.Vertical;
                    group6.ViewportIndex = k;
                    group6.Location = new Point(viewportX, groupLayout.Y);
                    if (viewportWidth > 0.0)
                    {
                        if (!Children.Contains(group6))
                        {
                            Children.Add(group6);
                        }
                        group6.Measure(new Size(viewportWidth, groupLayout.Height));
                    }
                    else
                    {
                        Children.Remove(group6);
                        _columnGroupPresenters[k + 1] = null;
                    }
                }
            }
            else
            {
                ClearColumnGroups();
            }

            if (_rowGroupHeaderPresenter == null)
            {
                _rowGroupHeaderPresenter = new GcRangeGroupHeader(this);
            }
            _rowGroupHeaderPresenter.Orientation = Orientation.Horizontal;
            _rowGroupHeaderPresenter.Location = new Point(groupLayout.X, groupLayout.Y + groupLayout.Height);
            if (groupLayout.Width > 0.0)
            {
                if (!Children.Contains(_rowGroupHeaderPresenter))
                {
                    Children.Add(_rowGroupHeaderPresenter);
                }
                _rowGroupHeaderPresenter.Measure(new Size(groupLayout.Width, p_layout.HeaderHeight));
            }
            else
            {
                Children.Remove(_rowGroupHeaderPresenter);
                _rowGroupHeaderPresenter = null;
            }

            if (_columnGroupHeaderPresenter == null)
            {
                _columnGroupHeaderPresenter = new GcRangeGroupHeader(this);
            }
            _columnGroupHeaderPresenter.Orientation = Orientation.Vertical;
            _columnGroupHeaderPresenter.Location = new Point(groupLayout.X + groupLayout.Width, groupLayout.Y);
            if (groupLayout.Height > 0.0)
            {
                if (!Children.Contains(_columnGroupHeaderPresenter))
                {
                    Children.Add(_columnGroupHeaderPresenter);
                }
                _columnGroupHeaderPresenter.Measure(new Size(p_layout.HeaderWidth, groupLayout.Height));
            }
            else
            {
                Children.Remove(_columnGroupHeaderPresenter);
                _columnGroupHeaderPresenter = null;
            }

            if (_groupCornerPresenter == null)
            {
                _groupCornerPresenter = new GcRangeGroupCorner(this);
            }
            _groupCornerPresenter.Location = new Point(groupLayout.X, groupLayout.Y);
            if ((groupLayout.Width > 0.0) && (groupLayout.Height > 0.0))
            {
                if (!Children.Contains(_groupCornerPresenter))
                {
                    Children.Add(_groupCornerPresenter);
                }
                _groupCornerPresenter.Measure(new Size(groupLayout.Width, groupLayout.Height));
            }
            else
            {
                Children.Remove(_groupCornerPresenter);
                _groupCornerPresenter = null;
            }
        }

        void MeasureSelectionGripper()
        {
            Size size = new Size(16, 16);

            // 触摸时调整选择范围的圈圈
            if (!Children.Contains(_topLeftGripper))
                Children.Add(_topLeftGripper);
            _topLeftGripper.Measure(size);

            if (!Children.Contains(_bottomRightGripper))
                Children.Add(_bottomRightGripper);
            _bottomRightGripper.Measure(size);

            // 触摸时调整列宽/行高的标志
            if (!Children.Contains(_rowResizeGripper))
                Children.Add(_rowResizeGripper);
            _rowResizeGripper.Measure(size);

            if (!Children.Contains(_colResizeGripper))
                Children.Add(_colResizeGripper);
            _colResizeGripper.Measure(size);

            // 自动填充格标志
            if (!Children.Contains(_autoFillIndicator))
                Children.Add(_autoFillIndicator);
            _autoFillIndicator.Measure(size);
        }

        void ClearRowGroups()
        {
            foreach (GcRangeGroup group in _rowGroupPresenters)
            {
                Children.Remove(group);
            }
            _rowGroupPresenters = null;
        }

        void ClearColumnGroups()
        {
            foreach (GcRangeGroup group4 in _columnGroupPresenters)
            {
                Children.Remove(group4);
            }
            _columnGroupPresenters = null;
        }

        void MeasureTabStrip(SheetLayout p_layout)
        {
            if (p_layout.TabStripHeight > 0.0)
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
                _tabStrip.Measure(new Size(p_layout.TabStripWidth, p_layout.TabStripHeight));
            }
            else if (_tabStrip != null)
            {
                Children.Remove(_tabStrip);
                _tabStrip.ActiveTabChanging -= OnTabStripActiveTabChanging;
                _tabStrip.ActiveTabChanged -= OnTabStripActiveTabChanged;
                _tabStrip.NewTabNeeded -= OnTabStripNewTabNeeded;
                _tabStrip = null;
            }
        }
        #endregion

        #region 布局
        protected override Size ArrangeOverride(Size finalSize)
        {
            SheetLayout layout = GetSheetLayout();
            // 行头
            ArrangeRowHeaders(layout);
            // 列头
            ArrangeColHeaders(layout);
            // 单元格区域
            ArrangeCellPanels(layout);
            // 左上角
            ArrangeCornerPanel(layout);
            // 滚动栏
            ArrangeScrollBar(layout);
            // 分隔栏
            ArrangeSplitBar(layout);
            // 区域分组
            ArrangeRangeGroup(layout);
            // sheet标签
            _tabStrip?.Arrange(new Rect(layout.TabStripX, layout.TabStripY, layout.TabStripWidth, layout.TabStripHeight));
            // 触摸时调整选择范围的圈圈、调整列宽行高的标志、自动填充标志
            ArrangeSelectionGripper();

            // 跟踪层
            Rect rcFull = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
            _trackersPanel.Arrange(rcFull);
            _rowFreezeLine?.Arrange(rcFull);
            _rowTrailingFreezeLine?.Arrange(rcFull);
            _columnFreezeLine?.Arrange(rcFull);
            _columnTrailingFreezeLine?.Arrange(rcFull);
            // 进度环
            _progressRing?.Arrange(rcFull);

            Clip = new RectangleGeometry { Rect = rcFull };
            return finalSize;
        }

        void ArrangeCornerPanel(SheetLayout p_layout)
        {
            double headerX;
            double headerY;
            if ((IsTouchZooming && (_cornerPanel != null)) && (_cornerPanel.Parent != null))
            {
                headerX = p_layout.HeaderX;
                headerY = p_layout.HeaderY;
                _cornerPanel.Arrange(new Rect(headerX, headerY, p_layout.HeaderWidth, p_layout.HeaderHeight));
                _cornerPanel.RenderTransform = _cachedCornerViewportTransform;
            }
            else if ((_cornerPanel != null) && (_cornerPanel.Parent != null))
            {
                headerX = p_layout.HeaderX;
                headerY = p_layout.HeaderY;
                if (_cornerPanel.RenderTransform != null)
                {
                    _cornerPanel.RenderTransform = null;
                }
                if ((_cornerPanel.Width != p_layout.HeaderWidth) || (_cornerPanel.Height != p_layout.HeaderHeight))
                {
                    _cornerPanel.Arrange(new Rect(headerX, headerY, p_layout.HeaderWidth, p_layout.HeaderHeight));
                }
            }
        }

        void ArrangeColHeaders(SheetLayout p_layout)
        {
            double headerX;
            double headerY;
            if (IsTouchZooming && (_cachedColumnHeaderViewportTransform != null))
            {
                for (int i = -1; i <= p_layout.ColumnPaneCount; i++)
                {
                    headerX = p_layout.GetViewportX(i);
                    headerY = p_layout.HeaderY;
                    double viewportWidth = p_layout.GetViewportWidth(i);
                    double headerHeight = p_layout.HeaderHeight;
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
                for (int j = -1; j <= p_layout.ColumnPaneCount; j++)
                {
                    headerX = p_layout.GetViewportX(j);
                    if ((IsTouching && (j == p_layout.ColumnPaneCount)) && ((_translateOffsetX < 0.0) && (_touchStartHitTestInfo.ColumnViewportIndex == (p_layout.ColumnPaneCount - 1))))
                    {
                        headerX += _translateOffsetX;
                    }
                    headerY = p_layout.HeaderY;
                    double width = p_layout.GetViewportWidth(j);
                    double height = p_layout.HeaderHeight;
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
        }

        void ArrangeRowHeaders(SheetLayout p_layout)
        {
            double headerX;
            double headerY;
            if (IsTouchZooming && (_cachedRowHeaderViewportTransform != null))
            {
                for (int k = -1; k <= p_layout.RowPaneCount; k++)
                {
                    headerX = p_layout.HeaderX;
                    headerY = p_layout.GetViewportY(k);
                    double headerWidth = p_layout.HeaderWidth;
                    double viewportHeight = p_layout.GetViewportHeight(k);
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
                for (int m = -1; m <= p_layout.RowPaneCount; m++)
                {
                    headerX = p_layout.HeaderX;
                    headerY = p_layout.GetViewportY(m);
                    if (((IsTouching && IsTouching) && ((m == p_layout.RowPaneCount) && (_translateOffsetY < 0.0))) && (_touchStartHitTestInfo.RowViewportIndex == (p_layout.RowPaneCount - 1)))
                    {
                        headerY += _translateOffsetY;
                    }
                    double num15 = p_layout.HeaderWidth;
                    double num16 = p_layout.GetViewportHeight(m);
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
        }

        void ArrangeCellPanels(SheetLayout p_layout)
        {
            double headerX;
            double headerY;
            if (IsTouchZooming && (_cachedViewportTransform != null))
            {
                for (int i = -1; i <= p_layout.ColumnPaneCount; i++)
                {
                    headerX = p_layout.GetViewportX(i);
                    double num20 = p_layout.GetViewportWidth(i);
                    for (int j = -1; j <= p_layout.RowPaneCount; j++)
                    {
                        headerY = p_layout.GetViewportY(j);
                        double num22 = p_layout.GetViewportHeight(j);
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
                for (int i = -1; i <= p_layout.ColumnPaneCount; i++)
                {
                    headerX = p_layout.GetViewportX(i);
                    if ((IsTouching && (i == p_layout.ColumnPaneCount)) && ((_translateOffsetX < 0.0) && (_touchStartHitTestInfo.ColumnViewportIndex == (p_layout.ColumnPaneCount - 1))))
                    {
                        headerX += _translateOffsetX;
                    }
                    double num24 = p_layout.GetViewportWidth(i);
                    for (int j = -1; j <= p_layout.RowPaneCount; j++)
                    {
                        headerY = p_layout.GetViewportY(j);
                        if (((IsTouching && IsTouching) && ((j == p_layout.RowPaneCount) && (_translateOffsetY < 0.0))) && (_touchStartHitTestInfo.RowViewportIndex == (p_layout.RowPaneCount - 1)))
                        {
                            headerY += _translateOffsetY;
                        }
                        double num26 = p_layout.GetViewportHeight(j);
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
        }

        void ArrangeScrollBar(SheetLayout p_layout)
        {
            if (_horizontalScrollBar != null)
            {
                for (int i = 0; i < p_layout.ColumnPaneCount; i++)
                {
                    _horizontalScrollBar[i].Arrange(new Rect(p_layout.GetHorizontalScrollBarX(i), p_layout.OrnamentY, p_layout.GetHorizontalScrollBarWidth(i), p_layout.OrnamentHeight));
                }
            }

            if (_verticalScrollBar != null)
            {
                for (int i = 0; i < p_layout.RowPaneCount; i++)
                {
                    _verticalScrollBar[i].Arrange(new Rect(p_layout.OrnamentX, p_layout.GetVerticalScrollBarY(i), p_layout.OrnamentWidth, p_layout.GetVerticalScrollBarHeight(i)));
                }
            }

            if (_horizontalSplitBox != null)
            {
                for (int i = 0; i < p_layout.ColumnPaneCount; i++)
                {
                    _horizontalSplitBox[i].Arrange(new Rect(p_layout.GetHorizontalSplitBoxX(i), p_layout.OrnamentY, p_layout.GetHorizontalSplitBoxWidth(i), p_layout.OrnamentHeight));
                }
            }

            if (_verticalSplitBox != null)
            {
                for (int i = 0; i < p_layout.RowPaneCount; i++)
                {
                    _verticalSplitBox[i].Arrange(new Rect(p_layout.OrnamentX, p_layout.GetVerticalSplitBoxY(i), p_layout.OrnamentWidth, p_layout.GetVerticalSplitBoxHeight(i)));
                }
            }
        }

        void ArrangeSplitBar(SheetLayout p_layout)
        {
            if (_horizontalSplitBar != null)
            {
                for (int i = 0; i < (p_layout.ColumnPaneCount - 1); i++)
                {
                    if ((_horizontalSplitBar[i] != null) && (_horizontalSplitBar[i].Parent != null))
                    {
                        double horizontalSplitBarX = p_layout.GetHorizontalSplitBarX(i);
                        if (IsTouching && (_cachedViewportSplitBarX != null))
                        {
                            horizontalSplitBarX = _cachedViewportSplitBarX[i];
                        }
                        _horizontalSplitBar[i].Arrange(new Rect(horizontalSplitBarX, p_layout.Y, _defaultSplitBarSize, _availableSize.Height));
                    }
                }
            }

            if (_verticalSplitBar != null)
            {
                for (int i = 0; i < (p_layout.RowPaneCount - 1); i++)
                {
                    if ((_verticalSplitBar[i] != null) && (_verticalSplitBar[i].Parent != null))
                    {
                        double verticalSplitBarY = p_layout.GetVerticalSplitBarY(i);
                        if (IsTouching && (_cachedViewportSplitBarY != null))
                        {
                            verticalSplitBarY = _cachedViewportSplitBarY[i];
                        }
                        _verticalSplitBar[i].Arrange(new Rect(p_layout.X, verticalSplitBarY, _availableSize.Width, _defaultSplitBarSize));
                    }
                }
            }

            if (_crossSplitBar != null)
            {
                for (int i = 0; i < _crossSplitBar.GetLength(0); i++)
                {
                    double num70 = p_layout.GetVerticalSplitBarY(i);
                    if (IsTouching && (_cachedViewportSplitBarY != null))
                    {
                        num70 = _cachedViewportSplitBarY[i];
                    }
                    double num71 = p_layout.GetVerticalSplitBarHeight(i);
                    for (int j = 0; j < _crossSplitBar.GetLength(1); j++)
                    {
                        double num73 = p_layout.GetHorizontalSplitBarX(j);
                        if (IsTouching && (_cachedViewportSplitBarX != null))
                        {
                            num73 = _cachedViewportSplitBarX[j];
                        }
                        double num74 = p_layout.GetHorizontalSplitBarWidth(j);
                        if ((_crossSplitBar[i, j] != null) && (_crossSplitBar[i, j].Parent != null))
                        {
                            _crossSplitBar[i, j].Arrange(new Rect(num73, num70, num74, num71));
                        }
                    }
                }
            }
        }

        void ArrangeRangeGroup(SheetLayout p_layout)
        {
            double x;
            double y;
            GroupLayout groupLayout = GetGroupLayout();
            if ((_groupCornerPresenter != null) && (_groupCornerPresenter.Parent != null))
            {
                x = groupLayout.X;
                y = groupLayout.Y;
                if ((_groupCornerPresenter.Width != groupLayout.Width) || (_groupCornerPresenter.Height != groupLayout.Height))
                {
                    _groupCornerPresenter.Arrange(new Rect(x, y, groupLayout.Width, groupLayout.Height));
                }
            }

            if ((_rowGroupHeaderPresenter != null) && (_rowGroupHeaderPresenter.Parent != null))
            {
                x = groupLayout.X;
                y = groupLayout.Y + groupLayout.Height;
                double width = groupLayout.Width;
                double headerHeight = p_layout.HeaderHeight;
                _rowGroupHeaderPresenter.Arrange(new Rect(x, y, width, headerHeight));
            }

            if ((_columnGroupHeaderPresenter != null) && (_columnGroupHeaderPresenter.Parent != null))
            {
                x = groupLayout.X + groupLayout.Width;
                y = groupLayout.Y;
                double headerWidth = p_layout.HeaderWidth;
                double height = groupLayout.Height;
                _columnGroupHeaderPresenter.Arrange(new Rect(x, y, headerWidth, height));
            }

            if (_rowGroupPresenters != null)
            {
                for (int i = -1; i <= p_layout.RowPaneCount; i++)
                {
                    GcRangeGroup group = _rowGroupPresenters[i + 1];
                    if (group != null)
                    {
                        x = groupLayout.X;
                        y = p_layout.GetViewportY(i);
                        double num8 = groupLayout.Width;
                        double viewportHeight = p_layout.GetViewportHeight(i);
                        if (!IsTouching || (i != _touchStartHitTestInfo.RowViewportIndex))
                        {
                            group.Arrange(new Rect(x, y, num8, viewportHeight));
                            group.Clip = null;
                        }
                        else
                        {
                            group.Arrange(new Rect(x, y + _translateOffsetY, num8, viewportHeight));
                            if (_translateOffsetY < 0.0)
                            {
                                RectangleGeometry geometry = new RectangleGeometry();
                                geometry.Rect = new Rect(x, Math.Abs(_translateOffsetY), num8, viewportHeight);
                                group.Clip = geometry;
                            }
                            else if (_translateOffsetY > 0.0)
                            {
                                RectangleGeometry geometry2 = new RectangleGeometry();
                                geometry2.Rect = new Rect(x, 0.0, num8, Math.Max((double)0.0, (double)(viewportHeight - Math.Abs(_translateOffsetY))));
                                group.Clip = geometry2;
                            }
                        }
                    }
                }
            }

            if (_columnGroupPresenters != null)
            {
                for (int j = -1; j <= p_layout.ColumnPaneCount; j++)
                {
                    GcRangeGroup group2 = _columnGroupPresenters[j + 1];
                    if (group2 != null)
                    {
                        x = p_layout.GetViewportX(j);
                        y = groupLayout.Y;
                        double viewportWidth = p_layout.GetViewportWidth(j);
                        double num12 = groupLayout.Height;
                        if (!IsTouching || (j != _touchStartHitTestInfo.ColumnViewportIndex))
                        {
                            group2.Arrange(new Rect(x, y, viewportWidth, num12));
                            group2.Clip = null;
                        }
                        else
                        {
                            group2.Arrange(new Rect(x + _translateOffsetX, y, viewportWidth, num12));
                            if (_translateOffsetX < 0.0)
                            {
                                RectangleGeometry geometry3 = new RectangleGeometry();
                                geometry3.Rect = new Rect(Math.Abs(_translateOffsetX), y, viewportWidth, num12);
                                group2.Clip = geometry3;
                            }
                            else if (_translateOffsetX > 0.0)
                            {
                                RectangleGeometry geometry4 = new RectangleGeometry();
                                geometry4.Rect = new Rect(0.0, y, Math.Max((double)0.0, (double)(viewportWidth - Math.Abs(_translateOffsetX))), num12);
                                group2.Clip = geometry4;
                            }
                        }
                    }
                }
            }
        }

        internal void ArrangeSelectionGripper()
        {
            if (InputDeviceType != InputDeviceType.Touch || IsTouchPromotedMouseMessage)
            {
                ArrangeMouseSelectionGripper();
                return;
            }

            CellsPanel cellsPanel = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex());
            if (cellsPanel == null || cellsPanel.Excel.ActiveSheet.Selections.Count <= 0)
                return;

            if (IsContinueTouchOperation
                || IsEditing
                || ActiveSheet.SelectionPolicy == SelectionPolicy.Single
                || GetAllSelectedFloatingObjects().Count > 0)
            {
                HideSelectionGripper();
                return;
            }

            CellRange activeSelection = GetActiveSelection();
            if ((activeSelection == null) && (ActiveSheet.Selections.Count > 0))
                activeSelection = ActiveSheet.Selections[0];
            if (activeSelection == null)
            {
                HideSelectionGripper();
                return;
            }

            if (_autoFillIndicatorRect.HasValue)
            {
                HideSelectionGripper();
                Rect autoFillIndicatorRect = GetAutoFillIndicatorRect(cellsPanel, activeSelection);
                _autoFillIndicator.Arrange(autoFillIndicatorRect);
                _autoFillIndicatorRect = new Rect?(autoFillIndicatorRect);
                return;
            }

            if (IsEntrieSheetSelection())
            {
                // 全选
                HideSelectionAndResizeGripper();
                return;
            }

            SheetLayout sheetLayout = GetSheetLayout();
            Rect rangeBounds = cellsPanel._cachedSelectionFrameLayout;
            if (!cellsPanel.SelectionContainer.IsAnchorCellInSelection)
            {
                rangeBounds = cellsPanel._cachedFocusCellLayout;
            }
            if (cellsPanel.Excel.ActiveSheet.Selections.Count > 0)
            {
                rangeBounds = cellsPanel.GetRangeBounds(activeSelection);
            }

            List<Tuple<Point, double>> list = new List<Tuple<Point, double>>();
            double viewportY;
            bool flag2;
            if (!IsEntrieColumnSelection())
            {
                // 非整列非整行
                if (!IsEntrieRowSelection())
                {
                    double num27 = sheetLayout.GetViewportX(cellsPanel.ColumnViewportIndex);
                    double num28 = sheetLayout.GetViewportY(cellsPanel.RowViewportIndex);
                    int num29 = GetActiveRowViewportIndex();
                    int activeColumnViewportIndex = GetActiveColumnViewportIndex();
                    int viewportLeftColumn = GetViewportLeftColumn(activeColumnViewportIndex);
                    int num32 = GetViewportTopRow(num29);
                    int num33 = GetViewportBottomRow(num29);
                    int viewportRightColumn = GetViewportRightColumn(activeColumnViewportIndex);
                    int num35 = -7;
                    int num36 = -7;
                    if ((activeSelection.Column < viewportLeftColumn) || (activeSelection.Row < num32))
                    {
                        list.Add(Tuple.Create<Point, double>(new Point(-2147483648.0, -2147483648.0), 0.0));
                    }
                    else
                    {
                        list.Add(Tuple.Create<Point, double>(new Point((num27 + rangeBounds.X) + num35, (num28 + rangeBounds.Y) + num36), 16.0));
                    }
                    num35 = (int)(rangeBounds.Width - 9.0);
                    num36 = (int)(rangeBounds.Height - 9.0);
                    int num37 = (activeSelection.Row + activeSelection.RowCount) - 1;
                    int num38 = (activeSelection.Column + activeSelection.ColumnCount) - 1;
                    if (num37 > num33)
                    {
                        num36 = 0x7fffffff;
                    }
                    if (num38 > viewportRightColumn)
                    {
                        num35 = 0x7fffffff;
                    }
                    int num39 = GetActiveRowViewportIndex();
                    int num40 = GetActiveColumnViewportIndex();
                    ActiveSheet.GetViewportInfo();
                    if ((num35 == 0x7fffffff) || (num36 == 0x7fffffff))
                    {
                        for (int i = num39; i <= GetViewportInfo(ActiveSheet).RowViewportCount; i++)
                        {
                            for (int j = num40; j <= GetViewportInfo(ActiveSheet).ColumnViewportCount; j++)
                            {
                                num33 = GetViewportBottomRow(i);
                                viewportRightColumn = GetViewportRightColumn(j);
                                if ((num33 >= num37) && (viewportRightColumn >= num38))
                                {
                                    CellsPanel viewport8 = _cellsPanels[i + 1, j + 1];
                                    if (viewport8 != null)
                                    {
                                        Rect rect13 = viewport8._cachedSelectionFrameLayout;
                                        if (!viewport8.SelectionContainer.IsAnchorCellInSelection)
                                        {
                                            rect13 = viewport8._cachedFocusCellLayout;
                                        }
                                        num35 = (int)(((sheetLayout.GetViewportX(j) + rect13.X) + rect13.Width) - 9.0);
                                        num36 = (int)(((sheetLayout.GetViewportY(i) + rect13.Y) + rect13.Height) - 9.0);
                                        if (list.Count == 1)
                                        {
                                            if ((num35 > (sheetLayout.GetViewportX(j) + sheetLayout.GetViewportWidth(j))) || (num36 > (sheetLayout.GetViewportY(i) + sheetLayout.GetViewportHeight(i))))
                                            {
                                                list.Add(Tuple.Create<Point, double>(new Point(2147483647.0, 2147483647.0), 0.0));
                                            }
                                            else
                                            {
                                                list.Add(Tuple.Create<Point, double>(new Point((double)num35, (double)num36), 16.0));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (list.Count == 1)
                        {
                            list.Add(Tuple.Create<Point, double>(new Point(2147483647.0, 2147483647.0), 0.0));
                        }
                    }
                    else
                    {
                        num35 = (int)((num35 + num27) + rangeBounds.X);
                        num36 = (int)((num36 + num28) + rangeBounds.Y);
                        if ((num35 > (sheetLayout.GetViewportX(activeColumnViewportIndex) + sheetLayout.GetViewportWidth(activeColumnViewportIndex))) || (num36 > (sheetLayout.GetViewportY(num29) + sheetLayout.GetViewportHeight(num29))))
                        {
                            list.Add(Tuple.Create<Point, double>(new Point(2147483647.0, 2147483647.0), 0.0));
                        }
                        else
                        {
                            list.Add(Tuple.Create<Point, double>(new Point((double)num35, (double)num36), 16.0));
                        }
                    }
                    goto Label_10BF;
                }

                // 选择整行
                double viewportX = sheetLayout.GetViewportX(cellsPanel.ColumnViewportIndex);
                viewportY = sheetLayout.GetViewportY(cellsPanel.RowViewportIndex);
                int viewportTopRow = GetViewportTopRow(cellsPanel.RowViewportIndex);
                int viewportBottomRow = GetViewportBottomRow(cellsPanel.RowViewportIndex);
                if (ActiveSheet.FrozenColumnCount > 0)
                {
                    CellsPanel viewport5 = GetViewportRowsPresenter(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex() + 1);
                    Rect rect9 = viewport5._cachedSelectionFrameLayout;
                    if (!viewport5.SelectionContainer.IsAnchorCellInSelection)
                    {
                        rect9 = cellsPanel._cachedFocusCellLayout;
                    }
                    rangeBounds = new Rect(rangeBounds.X, rangeBounds.Y, rangeBounds.Width + rect9.Width, rangeBounds.Height);
                }

                if (activeSelection.Row >= viewportTopRow)
                {
                    list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, (viewportY + rangeBounds.Y) - 16.0), 16.0));
                }
                else
                {
                    list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, -2147483648.0), 0.0));
                }

                int num18 = (int)(rangeBounds.Height - 9.0);
                int num19 = (activeSelection.Row + activeSelection.RowCount) - 1;
                if (num19 > viewportBottomRow)
                {
                    num18 = 0x7fffffff;
                }
                int activeRowViewportIndex = GetActiveRowViewportIndex();
                ActiveSheet.GetViewportInfo();
                flag2 = true;
                int rowViewportIndex = activeRowViewportIndex;

                if (num18 == 0x7fffffff)
                {
                    while (rowViewportIndex <= GetViewportInfo(ActiveSheet).RowViewportCount)
                    {
                        if (GetViewportBottomRow(rowViewportIndex) >= num19)
                        {
                            CellsPanel viewport6 = _cellsPanels[rowViewportIndex + 1, cellsPanel.ColumnViewportIndex + 1];
                            if (viewport6 != null)
                            {
                                Rect rect10 = viewport6._cachedSelectionFrameLayout;
                                if (!viewport6.SelectionContainer.IsAnchorCellInSelection)
                                {
                                    rect10 = viewport6._cachedFocusCellLayout;
                                }
                                num18 = (int)((sheetLayout.GetViewportY(rowViewportIndex) + rect10.Y) + rect10.Height);
                                if (list.Count == 1)
                                {
                                    if (num18 <= (sheetLayout.GetViewportY(rowViewportIndex) + sheetLayout.GetViewportHeight(rowViewportIndex)))
                                    {
                                        list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, (double)num18), 16.0));
                                    }
                                    else
                                    {
                                        list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, 2147483647.0), 0.0));
                                        flag2 = false;
                                    }
                                    break;
                                }
                            }
                        }
                        rowViewportIndex++;
                    }
                }
                else
                {
                    double viewportHeight = sheetLayout.GetViewportHeight(cellsPanel.RowViewportIndex);
                    double y = (viewportY + rangeBounds.Y) + rangeBounds.Height;
                    if (y <= (viewportY + viewportHeight))
                    {
                        list.Add(Tuple.Create<Point, double>(new Point(((viewportX + rangeBounds.X) + (rangeBounds.Width / 2.0)) - 7.0, y), 16.0));
                    }
                    else
                    {
                        list.Add(Tuple.Create<Point, double>(new Point(viewportX, 2147483647.0), 0.0));
                        flag2 = false;
                    }
                }
            }
            else
            {
                // 选择整列
                double num = sheetLayout.GetViewportX(cellsPanel.ColumnViewportIndex);
                double num2 = sheetLayout.GetViewportY(cellsPanel.RowViewportIndex);
                int num3 = GetViewportLeftColumn(cellsPanel.ColumnViewportIndex);
                int num4 = GetViewportRightColumn(cellsPanel.ColumnViewportIndex);
                if (ActiveSheet.FrozenRowCount > 0)
                {
                    CellsPanel viewport2 = GetViewportRowsPresenter(GetActiveRowViewportIndex() + 1, GetActiveColumnViewportIndex());
                    Rect rect5 = viewport2._cachedSelectionFrameLayout;
                    if (!viewport2.SelectionContainer.IsAnchorCellInSelection)
                    {
                        rect5 = viewport2._cachedFocusCellLayout;
                    }
                    rangeBounds = new Rect(rangeBounds.X, rangeBounds.Y, rangeBounds.Width, rangeBounds.Height + rect5.Height);
                }
                if (activeSelection.Column >= num3)
                {
                    list.Add(Tuple.Create<Point, double>(new Point((num + rangeBounds.X) - 16.0, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 16.0));
                }
                else
                {
                    list.Add(Tuple.Create<Point, double>(new Point(-2147483648.0, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 0.0));
                }
                int num5 = (int)(rangeBounds.Width - 9.0);
                int num6 = (activeSelection.Column + activeSelection.ColumnCount) - 1;
                if (num6 > num4)
                {
                    num5 = 0x7fffffff;
                }
                int num7 = GetActiveColumnViewportIndex();
                ActiveSheet.GetViewportInfo();
                bool flag = true;
                int columnViewportIndex = num7;
                if (num5 == 0x7fffffff)
                {
                    while (columnViewportIndex <= GetViewportInfo(ActiveSheet).ColumnViewportCount)
                    {
                        if (GetViewportRightColumn(columnViewportIndex) >= num6)
                        {
                            CellsPanel viewport3 = _cellsPanels[cellsPanel.RowViewportIndex + 1, columnViewportIndex + 1];
                            if (viewport3 != null)
                            {
                                Rect rect6 = viewport3._cachedSelectionFrameLayout;
                                if (!viewport3.SelectionContainer.IsAnchorCellInSelection)
                                {
                                    rect6 = viewport3._cachedFocusCellLayout;
                                }
                                num5 = (int)((sheetLayout.GetViewportX(columnViewportIndex) + rect6.X) + rect6.Width);
                                if (list.Count == 1)
                                {
                                    if (num5 <= (sheetLayout.GetViewportX(columnViewportIndex) + sheetLayout.GetViewportWidth(columnViewportIndex)))
                                    {
                                        list.Add(Tuple.Create<Point, double>(new Point((double)num5, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 16.0));
                                    }
                                    else
                                    {
                                        list.Add(Tuple.Create<Point, double>(new Point(-2147483648.0, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 0.0));
                                        flag = false;
                                    }
                                    break;
                                }
                            }
                        }
                        columnViewportIndex++;
                    }
                }
                else
                {
                    double viewportWidth = sheetLayout.GetViewportWidth(cellsPanel.ColumnViewportIndex);
                    double x = (num + rangeBounds.X) + rangeBounds.Width;
                    if (x <= (num + viewportWidth))
                    {
                        list.Add(Tuple.Create<Point, double>(new Point(x, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 16.0));
                    }
                    else
                    {
                        list.Add(Tuple.Create<Point, double>(new Point(2147483647.0, ((num2 + rangeBounds.Y) + (rangeBounds.Height / 2.0)) - 9.0), 0.0));
                        flag = false;
                    }
                }
                var colHeader = _colHeaders[cellsPanel.ColumnViewportIndex + 1];
                CellRange range2 = new CellRange(ActiveSheet.ColumnHeader.RowCount - 1, (activeSelection.Column + activeSelection.ColumnCount) - 1, 1, 1);
                Rect rect7 = colHeader.GetRangeBounds(range2, SheetArea.ColumnHeader);
                int column = (activeSelection.Column + activeSelection.ColumnCount) - 1;
                if ((ActiveSheet.GetColumnResizable(column) && !rect7.IsEmpty) && flag)
                {
                    double num12 = 0.0;
                    for (int k = 0; k < ActiveSheet.ColumnHeader.RowCount; k++)
                    {
                        num12 += ActiveSheet.GetActualRowHeight(k, SheetArea.ColumnHeader) * ActiveSheet.ZoomFactor;
                    }
                    Rect rect8 = new Rect(((num + rect7.X) + rect7.Width) - 8.0, (colHeader.Location.Y + num12) - 16.0, 16.0, 16.0);
                    _colResizeGripper.Arrange(rect8);
                    ResizerGripperRect = new Rect?(rect8);
                }
                else
                {
                    _colResizeGripper.Arrange(_rcEmpty);
                    ResizerGripperRect = null;
                }
                _rowResizeGripper.Arrange(_rcEmpty);
                goto Label_10BF;
            }

            var header = _rowHeaders[cellsPanel.RowViewportIndex + 1];
            CellRange range = new CellRange((activeSelection.Row + activeSelection.RowCount) - 1, ActiveSheet.RowHeader.ColumnCount - 1, 1, 1);
            Rect rect11 = header.GetRangeBounds(range, SheetArea.CornerHeader | SheetArea.RowHeader);
            int row = (activeSelection.Row + activeSelection.RowCount) - 1;
            if ((ActiveSheet.GetRowResizable(row) && !rect11.IsEmpty) && flag2)
            {
                double num25 = 0.0;
                for (int m = 0; m < ActiveSheet.RowHeader.ColumnCount; m++)
                {
                    num25 += ActiveSheet.GetActualColumnWidth(m, SheetArea.CornerHeader | SheetArea.RowHeader) * ActiveSheet.ZoomFactor;
                }
                Rect rect12 = new Rect((header.Location.X + num25) - 16.0, ((viewportY + rect11.Y) + rect11.Height) - 8.0, 16.0, 16.0);
                _rowResizeGripper.Arrange(rect12);
                ResizerGripperRect = new Rect?(rect12);
            }
            else
            {
                _rowResizeGripper.Arrange(_rcEmpty);
                ResizerGripperRect = null;
            }
            _colResizeGripper.Arrange(_rcEmpty);

        Label_10BF:
            if (list.Count == 2)
            {
                Point point = list[0].Item1;
                double width = list[0].Item2;
                Rect rect14 = new Rect((double)((int)point.X), (double)((int)point.Y), width, width);
                _topLeftGripper.Arrange(rect14);
                point = list[1].Item1;
                width = list[1].Item2;
                Rect rect15 = new Rect((double)((int)point.X), (double)((int)point.Y), width, width);
                _bottomRightGripper.Arrange(rect15);
                _gripperLocations = new GripperLocationsStruct
                {
                    TopLeft = rect14,
                    BottomRight = rect15
                };
                CachedGripperLocation = _gripperLocations;

                if (!IsEntrieRowSelection() && !IsEntrieColumnSelection())
                {
                    _rowResizeGripper.Arrange(_rcEmpty);
                    _colResizeGripper.Arrange(_rcEmpty);
                    ResizerGripperRect = null;
                    return;
                }
            }
            else
            {
                HideSelectionAndResizeGripper();
            }
        }

        void HideSelectionGripper()
        {
            if (_gripperLocations != null)
                CachedGripperLocation = _gripperLocations;
            _gripperLocations = null;
            _topLeftGripper.Arrange(_rcEmpty);
            _bottomRightGripper.Arrange(_rcEmpty);
            _rowResizeGripper.Arrange(_rcEmpty);
            _colResizeGripper.Arrange(_rcEmpty);
        }

        void HideSelectionAndResizeGripper()
        {
            _gripperLocations = null;
            _topLeftGripper.Arrange(_rcEmpty);
            _bottomRightGripper.Arrange(_rcEmpty);
            _rowResizeGripper.Arrange(_rcEmpty);
            _colResizeGripper.Arrange(_rcEmpty);
            ResizerGripperRect = null;
        }

        void ArrangeMouseSelectionGripper()
        {
            _gripperLocations = null;
            ResizerGripperRect = null;
            _topLeftGripper.Arrange(_rcEmpty);
            _bottomRightGripper.Arrange(_rcEmpty);
            _rowResizeGripper.Arrange(_rcEmpty);
            _colResizeGripper.Arrange(_rcEmpty);
            if (_autoFillIndicatorRect.HasValue)
            {
                _autoFillIndicator.Arrange(_rcEmpty);
                _autoFillIndicatorRect = null;
            }
            if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
            {
                _touchToolbarPopup.IsOpen = false;
            }
        }
        #endregion

        #region 内部方法
        void InitLayout()
        {
            _hoverManager = new HoverManager(this);
            _dragStartRowViewport = -2;
            _dragStartColumnViewport = -2;
            _dragToColumnViewport = -2;
            _dragToRowViewport = -2;
            _dragToColumn = -2;
            _dragToRow = -2;
            _showScrollTip = false;

            _trackersPanel = new Canvas();
            _topLeftGripper = new Ellipse();
            _topLeftGripper.Stroke = BrushRes.BlackBrush;
            _topLeftGripper.StrokeThickness = 2.0;
            _topLeftGripper.Fill = BrushRes.WhiteBrush;
            _topLeftGripper.Height = 16.0;
            _topLeftGripper.Width = 16.0;

            _bottomRightGripper = new Ellipse();
            _bottomRightGripper.Stroke = BrushRes.BlackBrush;
            _bottomRightGripper.StrokeThickness = 2.0;
            _bottomRightGripper.Fill = BrushRes.WhiteBrush;
            _bottomRightGripper.Height = 16.0;
            _bottomRightGripper.Width = 16.0;

            // iOS暂时不支持程序集中的内容图片，3.1将支持
            _rowResizeGripper = new Image { Width = 16.0, Height = 16.0 };
            _colResizeGripper = new Image { Width = 16.0, Height = 16.0 };

            _autoFillIndicator = new Rectangle
            {
                Stroke = BrushRes.BlackBrush,
                StrokeThickness = 2.0,
                Fill = BrushRes.WhiteBrush,
                Height = 16.0,
                Width = 16.0,
            };

            Background = BrushRes.浅灰1;
            LoadResizeGripper();
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

        internal void InvalidateRangeGroup()
        {
            if (_rowGroupPresenters != null)
            {
                foreach (var group in _rowGroupPresenters)
                {
                    group?.InvalidateMeasure();
                }
            }

            if (_columnGroupPresenters != null)
            {
                foreach (var group in _columnGroupPresenters)
                {
                    group?.InvalidateMeasure();
                }
            }
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

        void UpdateFreezeLines()
        {
            if (IsTouchZooming)
                return;

            // 原程序造成iOS滚动时非常慢，每次都创建 FreezeLine 再移除，不易发现！
            SheetLayout sheetLayout = GetSheetLayout();
            if ((sheetLayout.FrozenWidth > 0.0) && ShowFreezeLine)
            {
                if (_columnFreezeLine == null)
                    _columnFreezeLine = CreateFreezeLine();

                if (!_trackersPanel.Children.Contains(_columnFreezeLine))
                {
                    _trackersPanel.Children.Add(_columnFreezeLine);
                }
                int frozenColumnCount = ActiveSheet.FrozenColumnCount;
                if (frozenColumnCount > ActiveSheet.ColumnCount)
                {
                    frozenColumnCount = ActiveSheet.ColumnCount;
                }
                ColumnLayout layout2 = GetViewportColumnLayoutModel(-1).FindColumn(frozenColumnCount - 1);
                if (layout2 != null)
                {
                    _columnFreezeLine.X1 = layout2.X + layout2.Width;
                    _columnFreezeLine.X2 = _columnFreezeLine.X1;
                    _columnFreezeLine.Y1 = 0.0;
                    _columnFreezeLine.Y2 = sheetLayout.FrozenTrailingY + sheetLayout.FrozenTrailingHeight;
                }
                else
                {
                    _trackersPanel.Children.Remove(_columnFreezeLine);
                }
            }
            else if (_columnFreezeLine != null)
            {
                _trackersPanel.Children.Remove(_columnFreezeLine);
                _columnFreezeLine = null;
            }

            ViewportInfo viewportInfo = GetViewportInfo();
            if ((sheetLayout.FrozenTrailingWidth > 0.0) && ShowFreezeLine)
            {
                if (_columnTrailingFreezeLine == null)
                    _columnTrailingFreezeLine = CreateFreezeLine();

                if (!_trackersPanel.Children.Contains(_columnTrailingFreezeLine))
                {
                    _trackersPanel.Children.Add(_columnTrailingFreezeLine);
                }
                ColumnLayout layout3 = GetViewportColumnLayoutModel(viewportInfo.ColumnViewportCount).FindColumn(Math.Max(ActiveSheet.FrozenColumnCount, ActiveSheet.ColumnCount - ActiveSheet.FrozenTrailingColumnCount));
                if (layout3 != null)
                {
                    _columnTrailingFreezeLine.X1 = layout3.X;
                    _columnTrailingFreezeLine.X2 = _columnTrailingFreezeLine.X1;
                    _columnTrailingFreezeLine.Y1 = 0.0;
                    _columnTrailingFreezeLine.Y2 = sheetLayout.FrozenTrailingY + sheetLayout.FrozenTrailingHeight;
                }
                else
                {
                    _trackersPanel.Children.Remove(_columnTrailingFreezeLine);
                }
            }
            else if (_columnTrailingFreezeLine != null)
            {
                _trackersPanel.Children.Remove(_columnTrailingFreezeLine);
                _columnTrailingFreezeLine = null;
            }

            if ((sheetLayout.FrozenHeight > 0.0) && ShowFreezeLine)
            {
                if (_rowFreezeLine == null)
                    _rowFreezeLine = CreateFreezeLine();

                if (!_trackersPanel.Children.Contains(_rowFreezeLine))
                {
                    _trackersPanel.Children.Add(_rowFreezeLine);
                }
                int frozenRowCount = ActiveSheet.FrozenRowCount;
                if (ActiveSheet.RowCount < frozenRowCount)
                {
                    frozenRowCount = ActiveSheet.RowCount;
                }
                RowLayout layout4 = GetViewportRowLayoutModel(-1).FindRow(frozenRowCount - 1);
                if (layout4 != null)
                {
                    _rowFreezeLine.X1 = 0.0;
                    if (_translateOffsetX >= 0.0)
                    {
                        _rowFreezeLine.X2 = sheetLayout.FrozenTrailingX + sheetLayout.FrozenTrailingWidth;
                    }
                    else
                    {
                        _rowFreezeLine.X2 = (sheetLayout.FrozenTrailingX + _translateOffsetX) + sheetLayout.FrozenTrailingWidth;
                    }
                    _rowFreezeLine.Y1 = layout4.Y + layout4.Height;
                    _rowFreezeLine.Y2 = _rowFreezeLine.Y1;
                }
                else
                {
                    _trackersPanel.Children.Remove(_rowFreezeLine);
                }
            }
            else if (_rowFreezeLine != null)
            {
                _trackersPanel.Children.Remove(_rowFreezeLine);
                _rowFreezeLine = null;
            }

            if ((sheetLayout.FrozenTrailingHeight > 0.0) && ShowFreezeLine)
            {
                if (_rowTrailingFreezeLine == null)
                    _rowTrailingFreezeLine = CreateFreezeLine();

                if (!_trackersPanel.Children.Contains(_rowTrailingFreezeLine))
                {
                    _trackersPanel.Children.Add(_rowTrailingFreezeLine);
                }
                RowLayout layout5 = GetViewportRowLayoutModel(viewportInfo.RowViewportCount).FindRow(Math.Max(ActiveSheet.FrozenRowCount, ActiveSheet.RowCount - ActiveSheet.FrozenTrailingRowCount));
                if (layout5 != null)
                {
                    _rowTrailingFreezeLine.X1 = 0.0;
                    _rowTrailingFreezeLine.X2 = sheetLayout.FrozenTrailingX + sheetLayout.FrozenTrailingWidth;
                    _rowTrailingFreezeLine.Y1 = layout5.Y + ((_translateOffsetY < 0.0) ? _translateOffsetY : 0.0);
                    _rowTrailingFreezeLine.Y2 = _rowTrailingFreezeLine.Y1;
                }
                else
                {
                    _trackersPanel.Children.Remove(_rowTrailingFreezeLine);
                }
            }
            else if (_rowTrailingFreezeLine != null)
            {
                _trackersPanel.Children.Remove(_rowTrailingFreezeLine);
                _rowTrailingFreezeLine = null;
            }
        }

        async Task OpenStream(DispatchedHandler p_handler)
        {
            Workbook.Sheets.CollectionChanged -= OnSheetsCollectionChanged;
            Workbook.PropertyChanged -= OnWorkbookPropertyChanged;

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

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, p_handler);
            }
            finally
            {
                foreach (Worksheet worksheet in Workbook.Sheets)
                {
                    AttachSheet(worksheet);
                }
                Workbook.Sheets.CollectionChanged += OnSheetsCollectionChanged;
                Workbook.PropertyChanged += OnWorkbookPropertyChanged;

                _progressRing = null;
                RefreshAll();
            }
        }

        void ShowProgressRing()
        {
            HideProgressRing();
            _progressRing = new Grid { Background = BrushRes.浅灰1 };
            // 屏蔽交互事件
            _progressRing.PointerPressed += (s, e) => e.Handled = true;
            _progressRing.PointerMoved += (s, e) => e.Handled = true;
            _progressRing.PointerReleased += (s, e) => e.Handled = true;
            var ring = new ProgressRing
            {
                Foreground = BrushRes.主蓝,
                IsActive = true,
                Width = 100.0,
                Height = 100.0,
            };
            _progressRing.Children.Add(ring);
            InvalidateMeasure();
        }

        void HideProgressRing()
        {
            if (_progressRing != null)
            {
                Children.Remove(_progressRing);
                _progressRing = null;
            }
        }

        async void LoadResizeGripper()
        {
            _rowResizeGripper.Source = await SR.GetImage("ResizeGripperVer.png");
            _colResizeGripper.Source = await SR.GetImage("ResizeGripperHor.png");
        }
        #endregion
    }
}
