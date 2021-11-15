#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 单元格视口
    /// </summary>
    internal partial class CellsPanel : Panel
    {
        static Rect _rcEmpty = new Rect();
        internal int _activeCol;
        internal int _activeRow;
        BorderLayer _borderLayer;
        CellOverflowLayoutBuildEngine _buildEngine;
        internal CellRange _cachedActiveSelection;
        internal Rect _cachedActiveSelectionLayout;
        internal Rect[] _cachedChartShapeMovingRects;
        internal Rect[] _cachedChartShapeResizingRects;
        internal Rect _cachedDragFillRect;
        internal Rect _cachedFocusCellLayout;
        internal Rect _cachedSelectionFrameLayout;
        internal List<Rect> _cachedSelectionLayout;
        SpanGraph _cachedSpanGraph;
        CellCachePool _cellCachePool;
        Rectangle _dragFillLayer;
        Rect _editorBounds;
        internal EditingLayer _editorLayer;
        FloatingObjectLayer _floatingLayer;
        FloatingObjectMovingLayer _floatingEditLayer;
        RowsLayer _rowsLayer;
        SelectionLayer _selectionLayer;
        DecorationLayer _decorationLayer;

        public CellsPanel(Excel p_excel)
        {
            Excel = p_excel;

            _cachedSelectionLayout = new List<Rect>();
            _cachedActiveSelectionLayout = Rect.Empty;
            _cachedSelectionFrameLayout = new Rect();
            _cachedFocusCellLayout = new Rect();
            _editorBounds = new Rect();
            _cachedDragFillRect = _rcEmpty;
            _cellCachePool = new CellCachePool(Excel.ActiveSheet);
            _cachedSpanGraph = new SpanGraph();

            //--------------已移除 公式编辑层、图形层、数据校验层-------------------
            // 1 行数据层
            _rowsLayer = new RowsLayer(this);
            Children.Add(_rowsLayer);

            // 2 网格层，调整为只用在内容区域，行/列头不再使用
            _borderLayer = new BorderLayer(this);
            Children.Add(_borderLayer);

            // 3 选择状态层
            _selectionLayer = new SelectionLayer(this);
            Children.Add(_selectionLayer);

            // 4 拖拽复制层，点击右下角加号复制格内容
            _dragFillLayer = CreateDragFillLayer();
            Children.Add(_dragFillLayer);

            // 5 新增修饰层，打印时页面边线
            if (p_excel.ShowDecoration)
            {
                _decorationLayer = new DecorationLayer(this);
                Children.Add(_decorationLayer);
            }

            // 6 编辑层
            _editorLayer = new EditingLayer(this);
            Children.Add(_editorLayer);

            // 7 浮动对象层
            _floatingLayer = new FloatingObjectLayer(this);
            Children.Add(_floatingLayer);

            // 8 浮动对象拖拽移动层
            _floatingEditLayer = new FloatingObjectMovingLayer(this);
            Children.Add(_floatingEditLayer);

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        void BuildSelection()
        {
            _cachedSelectionLayout.Clear();
            _cachedFocusCellLayout = _rcEmpty;
            _cachedSelectionFrameLayout = _rcEmpty;
            _cachedActiveSelectionLayout = _rcEmpty;
            _cachedActiveSelection = null;
            _activeRow = Excel.ActiveSheet.ActiveRowIndex;
            _activeCol = Excel.ActiveSheet.ActiveColumnIndex;
            _selectionLayer.IsAnchorCellInSelection = false;

            var indicator = _selectionLayer.FocusIndicator;
            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            ColumnLayoutModel colLayoutModel = Excel.GetViewportColumnLayoutModel(ColumnViewportIndex);
            if (rowLayoutModel == null
                || rowLayoutModel.Count == 0
                || colLayoutModel == null
                || colLayoutModel.Count == 0)
            {
                indicator.HideAll();
                return;
            }

            CellRange activeCellRange = GetActiveCellRange();
            List<CellRange> ranges = new List<CellRange>((IEnumerable<CellRange>)Excel.ActiveSheet.Selections);
            if (ranges.Count == 0)
            {
                ranges.Add(activeCellRange);
            }

            int rangeCount = ranges.Count;
            Size viewportSize = GetViewportSize();
            Rect rectViewport = new Rect(0.0, 0.0, viewportSize.Width, viewportSize.Height);

            int topRow = rowLayoutModel[0].Row;
            int bottomRow = rowLayoutModel[rowLayoutModel.Count - 1].Row;
            int leftCol = colLayoutModel[0].Column;
            int rightCol = colLayoutModel[colLayoutModel.Count - 1].Column;

            for (int i = 0; i < rangeCount; i++)
            {
                CellRange range = ranges[i];
                if (range.Contains(_activeRow, _activeCol))
                {
                    _selectionLayer.IsAnchorCellInSelection = true;
                }

                int num7 = (range.Row < 0) ? 0 : range.Row;
                int num8 = (range.Column < 0) ? 0 : range.Column;
                int rowCount = (range.RowCount < 0) ? Excel.ActiveSheet.RowCount : range.RowCount;
                int columnCount = (range.ColumnCount < 0) ? Excel.ActiveSheet.ColumnCount : range.ColumnCount;
                range = new CellRange(num7, num8, rowCount, columnCount);

                Rect rect2 = GetRangeBounds(range);
                rect2.Intersect(rectViewport);
                if (rect2.IsEmpty)
                    continue;

                _cachedSelectionLayout.Add(new Rect(rect2.Left + 1.0, rect2.Top + 1.0, Math.Max((double)0.0, (double)(rect2.Width - 3.0)), Math.Max((double)0.0, (double)(rect2.Height - 3.0))));
                if (range.Contains(activeCellRange))
                {
                    Rect rect3 = new Rect(rect2.Left + 1.0, rect2.Top + 1.0, Math.Max((double)0.0, (double)(rect2.Width - 3.0)), Math.Max((double)0.0, (double)(rect2.Height - 3.0)));
                    if (_cachedActiveSelectionLayout.IsEmpty || (rangeCount == 1))
                    {
                        _cachedActiveSelectionLayout = rect3;
                        _cachedActiveSelection = range;
                    }
                    else
                    {
                        Rect rect4 = new Rect(rect3.Left, rect3.Top, rect3.Width, rect3.Height);
                        rect4.Intersect(_cachedActiveSelectionLayout);
                        if (rect4.IsEmpty)
                        {
                            _cachedActiveSelectionLayout = rect3;
                            _cachedActiveSelection = range;
                        }
                        else if (ContainsRect(rect3, _cachedActiveSelectionLayout))
                        {
                            _cachedActiveSelectionLayout = rect3;
                            _cachedActiveSelection = range;
                        }
                    }
                }
            }

            Rect rangeBounds = GetRangeBounds(activeCellRange);
            if (!rangeBounds.IsEmpty)
            {
                rangeBounds = new Rect(rangeBounds.Left + 1.0, rangeBounds.Top + 1.0, Math.Max((double)0.0, (double)(rangeBounds.Width - 3.0)), Math.Max((double)0.0, (double)(rangeBounds.Height - 3.0)));
            }
            _cachedFocusCellLayout = rangeBounds;

            // 只一个选择区域
            if (rangeCount == 1)
            {
                CellRange range = ranges[0];
                if (!_selectionLayer.IsAnchorCellInSelection)
                {
                    range = activeCellRange;
                }
                Rect bounds = GetRangeBounds(range);
                bounds.Intersect(rectViewport);
                if (bounds.IsEmpty)
                {
                    indicator.HideAll();
                    return;
                }

                if ((range.Row == -1) && (range.Column == -1))
                {
                    // 全选
                    indicator.Thickness = 1.0;
                    _cachedSelectionFrameLayout = bounds;
                }
                else if (!_selectionLayer.IsAnchorCellInSelection)
                {
                    indicator.Thickness = 1.0;
                    _cachedSelectionFrameLayout = new Rect(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
                }
                else
                {
                    indicator.Thickness = 3.0;
                    _cachedSelectionFrameLayout = new Rect(bounds.Left - 2.0, bounds.Top - 2.0, bounds.Width + 3.0, bounds.Height + 3.0);
                }

                if (!Excel.IsDraggingFill && range.Intersects(topRow, leftCol, rowLayoutModel.Count, colLayoutModel.Count))
                {
                    if (range.Row == -1)
                    {
                        indicator.IsTopVisible = topRow == 0;
                        indicator.IsBottomVisible = bottomRow == (Excel.ActiveSheet.RowCount - 1);
                    }
                    else
                    {
                        indicator.IsTopVisible = (range.Row >= topRow) && (range.Row <= bottomRow);
                        int num11 = (range.Row + range.RowCount) - 1;
                        indicator.IsBottomVisible = (num11 >= topRow) && (num11 <= bottomRow);
                    }

                    if (range.Column == -1)
                    {
                        indicator.IsLeftVisible = leftCol == 0;
                        indicator.IsRightVisible = rightCol == (Excel.ActiveSheet.ColumnCount - 1);
                    }
                    else
                    {
                        indicator.IsLeftVisible = (range.Column >= leftCol) && (range.Column <= rightCol);
                        int num12 = (range.Column + range.ColumnCount) - 1;
                        indicator.IsRightVisible = (num12 >= leftCol) && (num12 <= rightCol);
                    }
                }
                else
                {
                    indicator.IsTopVisible = false;
                    indicator.IsBottomVisible = false;
                    indicator.IsLeftVisible = false;
                    indicator.IsRightVisible = false;
                }

                if (Excel.CanUserDragFill)
                {
                    if (!Excel.IsDraggingFill)
                    {
                        if (((bounds.Width == 0.0) || (bounds.Height == 0.0)) || (indicator.Thickness == 1.0))
                        {
                            indicator.IsFillIndicatorVisible = false;
                        }
                        else if ((range.Row != -1) && (range.Column != -1))
                        {
                            bool flag = indicator.IsRightVisible && indicator.IsBottomVisible;
                            if (Excel.InputDeviceType == InputDeviceType.Touch)
                            {
                                flag = false;
                            }
                            indicator.IsFillIndicatorVisible = flag;
                            if (flag)
                            {
                                indicator.FillIndicatorPosition = FillIndicatorPosition.BottomRight;
                            }
                        }
                        else if ((range.Row != -1) && (range.Column == -1))
                        {
                            ViewportInfo viewportInfo = Excel.GetViewportInfo();
                            bool flag2;
                            if (Excel.ActiveSheet.FrozenColumnCount == 0)
                            {
                                flag2 = (ColumnViewportIndex >= 0) && (ColumnViewportIndex < viewportInfo.ColumnViewportCount);
                            }
                            else
                            {
                                flag2 = (ColumnViewportIndex == -1) || ((ColumnViewportIndex >= 1) && (ColumnViewportIndex < viewportInfo.ColumnViewportCount));
                            }
                            flag2 = flag2 && indicator.IsBottomVisible;
                            if (Excel.InputDeviceType == InputDeviceType.Touch)
                            {
                                flag2 = false;
                            }
                            indicator.IsFillIndicatorVisible = flag2;
                            if (flag2)
                            {
                                indicator.FillIndicatorPosition = FillIndicatorPosition.BottomLeft;
                            }
                        }
                        else if ((range.Column != -1) && (range.Row == -1))
                        {
                            ViewportInfo info2 = Excel.GetViewportInfo();
                            bool flag3;
                            if (Excel.ActiveSheet.FrozenRowCount == 0)
                            {
                                flag3 = (RowViewportIndex >= 0) && (RowViewportIndex < info2.RowViewportCount);
                            }
                            else
                            {
                                flag3 = (RowViewportIndex == -1) || ((RowViewportIndex >= 1) && (RowViewportIndex < info2.RowViewportCount));
                            }
                            flag3 = flag3 && indicator.IsRightVisible;
                            if (Excel.InputDeviceType == InputDeviceType.Touch)
                            {
                                flag3 = false;
                            }
                            indicator.IsFillIndicatorVisible = flag3;
                            if (flag3)
                            {
                                indicator.FillIndicatorPosition = FillIndicatorPosition.TopRight;
                            }
                        }
                        else
                        {
                            indicator.IsFillIndicatorVisible = false;
                        }
                    }
                }
                else
                {
                    indicator.IsFillIndicatorVisible = false;
                }
                return;
            }

            // 多个选择区域
            Rect rect7 = GetRangeBounds(activeCellRange);
            if (!rect7.IsEmpty)
            {
                indicator.Thickness = 1.0;
                _cachedSelectionFrameLayout = rect7;
                _cachedSelectionFrameLayout.Width = Math.Max((double)0.0, (double)(_cachedSelectionFrameLayout.Width - 1.0));
                _cachedSelectionFrameLayout.Height = Math.Max((double)0.0, (double)(_cachedSelectionFrameLayout.Height - 1.0));
                int num13 = 0;
                int num14 = 0;
                int num15 = 0;
                int num16 = 0;
                for (int j = 0; j < rangeCount; j++)
                {
                    CellRange range4 = ranges[j];
                    if (range4 != null)
                    {
                        if (activeCellRange.Row == range4.Row)
                        {
                            num14 = 1;
                        }
                        if (activeCellRange.Column == range4.Column)
                        {
                            num13 = 1;
                        }
                        if (activeCellRange.Row == ((range4.Row + range4.RowCount) - 1))
                        {
                            num16 = 1;
                        }
                        if (activeCellRange.Column == ((range4.Column + range4.ColumnCount) - 1))
                        {
                            num15 = 1;
                        }
                    }
                }
                _cachedSelectionFrameLayout.Y += num14;
                _cachedSelectionFrameLayout.Height = Math.Max((double)0.0, (double)(_cachedSelectionFrameLayout.Height - (num14 + num16)));
                _cachedSelectionFrameLayout.X += num13;
                _cachedSelectionFrameLayout.Width = Math.Max((double)0.0, (double)(_cachedSelectionFrameLayout.Width - (num13 + num15)));
            }
            indicator.IsBottomVisible = true;
            indicator.IsTopVisible = true;
            indicator.IsLeftVisible = true;
            indicator.IsRightVisible = true;
            indicator.IsFillIndicatorVisible = false;
        }

        bool ContainsRect(Rect rect1, Rect rect2)
        {
            return ((((rect2.Left >= rect1.Left) && (rect2.Right <= rect1.Right)) && (rect2.Top >= rect1.Top)) && (rect2.Bottom <= rect1.Bottom));
        }

        internal bool FocusContent()
        {
            // 手机上已在Excel.FocusInternal屏蔽
            if (_editorLayer.Editor != null)
            {
                _editorLayer.Editor.Focus(FocusState.Programmatic);
                return true;
            }
            return false;
        }

        CellRange GetActiveCellRange()
        {
            if ((_activeRow < 0) || (_activeCol < 0))
            {
                return new CellRange(0, 0, 0, 0);
            }
            CellRange range = new CellRange(_activeRow, _activeCol, 1, 1);
            var sheet = Excel.ActiveSheet;
            if ((((sheet != null) && (sheet.SpanModel != null)) && (!sheet.SpanModel.IsEmpty() && (range.RowCount == 1))) && (range.ColumnCount == 1))
            {
                CellRange range2 = sheet.SpanModel.Find(range.Row, range.Column);
                if (range2 != null)
                {
                    range = range2;
                }
            }
            return range;
        }

        internal Rect GetCellBounds(int p_row, int p_column, bool p_ignoreMerged)
        {
            if (Excel == null)
                return new Rect();

            // 包含合并情况
            CellLayoutModel model = null;
            if (!p_ignoreMerged)
            {
                model = Excel.GetCellLayoutModel(RowViewportIndex, ColumnViewportIndex, SheetArea.Cells);
            }
            CellLayout layout = (model == null) ? null : model.FindCell(p_row, p_column);
            if (layout != null)
            {
                return new Rect(layout.X, layout.Y, layout.Width, layout.Height);
            }

            RowLayoutModel rowLayoutModel = Excel.GetRowLayoutModel(RowViewportIndex, SheetArea.Cells);
            ColumnLayoutModel columnLayoutModel = Excel.GetColumnLayoutModel(ColumnViewportIndex, SheetArea.Cells);
            if (rowLayoutModel == null || columnLayoutModel == null)
                return new Rect();

            RowLayout layout2 = rowLayoutModel.Find(p_row);
            ColumnLayout layout3 = columnLayoutModel.Find(p_column);
            double x = -1.0;
            double y = -1.0;
            double width = 0.0;
            double height = 0.0;
            if (layout2 != null)
            {
                y = layout2.Y;
                height = layout2.Height;
            }
            if (layout3 != null)
            {
                x = layout3.X;
                width = layout3.Width;
            }
            return new Rect(x, y, width, height);
        }

        internal CellLayoutModel GetCellLayoutModel()
        {
            return Excel.GetViewportCellLayoutModel(RowViewportIndex, ColumnViewportIndex);
        }

        internal CellOverflowLayoutModel GetCellOverflowLayoutModel(int rowIndex)
        {
            return CellOverflowLayoutBuildEngine.GetModel(rowIndex);
        }

        internal ICellsSupport GetDataContext()
        {
            return Excel.ActiveSheet;
        }

        internal object GetEditorValue()
        {
            var editor = _editorLayer.Editor;
            if (editor != null)
                return editor.Text;
            return null;
        }

        public int GetFlotingObjectZIndex(string name)
        {
            return FloatingObjectsPanel.GetFlotingObjectZIndex(name);
        }

        internal Rect GetRangeBounds(CellRange range)
        {
            return GetRangeBounds(range, SheetArea.Cells);
        }

        internal Rect GetRangeBounds(CellRange range, SheetArea area)
        {
            SheetSpanModel spanModel = null;
            if (Excel.ActiveSheet != null)
            {
                switch (area)
                {
                    case SheetArea.Cells:
                        spanModel = Excel.ActiveSheet.SpanModel;
                        break;

                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                        spanModel = Excel.ActiveSheet.RowHeaderSpanModel;
                        break;

                    case SheetArea.ColumnHeader:
                        spanModel = Excel.ActiveSheet.ColumnHeaderSpanModel;
                        break;
                }
            }
            if (spanModel != null)
            {
                CellRange range2 = spanModel.Find(range.Row, range.Column);
                if (((range2 != null) && (range2.RowCount >= range.RowCount)) && (range2.ColumnCount >= range.ColumnCount))
                {
                    range = range2;
                }
            }
            RowLayoutModel rowLayoutModel = Excel.GetRowLayoutModel(RowViewportIndex, area);
            ColumnLayoutModel columnLayoutModel = Excel.GetColumnLayoutModel(ColumnViewportIndex, area);
            int row = (rowLayoutModel.Count > 0) ? rowLayoutModel[0].Row : -1;
            int num2 = (rowLayoutModel.Count > 0) ? rowLayoutModel[rowLayoutModel.Count - 1].Row : -1;
            int column = (columnLayoutModel.Count > 0) ? columnLayoutModel[0].Column : -1;
            int num4 = (columnLayoutModel.Count > 0) ? columnLayoutModel[columnLayoutModel.Count - 1].Column : -1;
            if (!range.Intersects(row, column, (num2 - row) + 1, (num4 - column) + 1))
            {
                return Rect.Empty;
            }
            int index = Math.Max(range.Row, row);
            int num6 = Math.Max(range.Column, column);
            int num7 = Math.Min((range.Row + range.RowCount) - 1, num2);
            int num8 = Math.Min((range.Column + range.ColumnCount) - 1, num4);
            RowLayout layout = rowLayoutModel.Find(index);
            RowLayout layout2 = rowLayoutModel.Find(num7);
            ColumnLayout layout3 = columnLayoutModel.Find(num6);
            ColumnLayout layout4 = columnLayoutModel.Find(num8);
            double x = -1.0;
            double y = -1.0;
            double height = 0.0;
            double width = 0.0;
            if ((layout != null) && (layout2 != null))
            {
                y = layout.Y;
                height = (layout2.Y + layout2.Height) - layout.Y;
            }
            else if (rowLayoutModel.Count > 0)
            {
                y = rowLayoutModel[0].Y;
                height = (rowLayoutModel[rowLayoutModel.Count - 1].Y + rowLayoutModel[rowLayoutModel.Count - 1].Height) - y;
            }
            if ((layout3 != null) && (layout4 != null))
            {
                x = layout3.X;
                width = (layout4.X + layout4.Width) - layout3.X;
            }
            else if (columnLayoutModel.Count > 0)
            {
                x = columnLayoutModel[0].X;
                width = (columnLayoutModel[columnLayoutModel.Count - 1].X + columnLayoutModel[columnLayoutModel.Count - 1].Width) - x;
            }
            return new Rect(PointToClient(new Point(x, y)), new Size(width, height));
        }

        internal Rect GetRangeBounds(CellRange range, out bool isLeftVisible, out bool isRightVisible, out bool isTopVisible, out bool isBottomVisible)
        {
            isLeftVisible = true;
            isRightVisible = true;
            isTopVisible = true;
            isBottomVisible = true;
            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            ColumnLayoutModel viewportColumnLayoutModel = Excel.GetViewportColumnLayoutModel(ColumnViewportIndex);
            int row = (rowLayoutModel.Count > 0) ? rowLayoutModel[0].Row : -1;
            int num2 = (rowLayoutModel.Count > 0) ? rowLayoutModel[rowLayoutModel.Count - 1].Row : -1;
            int column = (viewportColumnLayoutModel.Count > 0) ? viewportColumnLayoutModel[0].Column : -1;
            int num4 = (viewportColumnLayoutModel.Count > 0) ? viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Column : -1;
            if (!range.Intersects(row, column, (num2 - row) + 1, (num4 - column) + 1))
            {
                return Rect.Empty;
            }
            int index = Math.Max(range.Row, row);
            int num6 = Math.Max(range.Column, column);
            int num7 = Math.Min((range.Row + range.RowCount) - 1, num2);
            int num8 = Math.Min((range.Column + range.ColumnCount) - 1, num4);
            RowLayout layout = rowLayoutModel.Find(index);
            RowLayout layout2 = rowLayoutModel.Find(num7);
            ColumnLayout layout3 = viewportColumnLayoutModel.Find(num6);
            ColumnLayout layout4 = viewportColumnLayoutModel.Find(num8);
            double x = -1.0;
            double y = -1.0;
            double height = 0.0;
            double width = 0.0;
            if ((layout != null) && (layout2 != null))
            {
                y = layout.Y;
                height = (layout2.Y + layout2.Height) - layout.Y;
            }
            else if (rowLayoutModel.Count > 0)
            {
                y = rowLayoutModel[0].Y;
                height = (rowLayoutModel[rowLayoutModel.Count - 1].Y + rowLayoutModel[rowLayoutModel.Count - 1].Height) - y;
            }
            if ((layout3 != null) && (layout4 != null))
            {
                x = layout3.X;
                width = (layout4.X + layout4.Width) - layout3.X;
            }
            else if (viewportColumnLayoutModel.Count > 0)
            {
                x = viewportColumnLayoutModel[0].X;
                width = (viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].X + viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Width) - x;
            }
            if (range.Column < column)
            {
                isLeftVisible = false;
            }
            if (range.Row < row)
            {
                isTopVisible = false;
            }
            if ((num4 != -1) && (((range.Column + range.ColumnCount) - 1) > num4))
            {
                isRightVisible = false;
            }
            if ((num2 != -1) && (((range.Row + range.RowCount) - 1) > num2))
            {
                isBottomVisible = false;
            }
            return new Rect(PointToClient(new Point(x, y)), new Size(width, height));
        }

        internal RowItem GetRow(int row)
        {
            return _rowsLayer.GetRow(row);
        }

        internal RowLayoutModel GetRowLayoutModel()
        {
            return Excel.GetViewportRowLayoutModel(RowViewportIndex);
        }

        internal SheetSpanModelBase GetSpanModel()
        {
            return Excel.ActiveSheet.SpanModel;
        }

        internal SpreadChartView GetSpreadChartView(string chartName)
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectContainer floatingObjectContainer = FloatingObjectsPanel.GetFloatingObjectContainer(chartName);
                if ((floatingObjectContainer != null) && (floatingObjectContainer is SpreadChartContainer))
                {
                    return (floatingObjectContainer as SpreadChartContainer).SpreadChartView;
                }
            }
            return null;
        }

        internal Rect GetViewportBounds()
        {
            return new Rect(Location, GetViewportSize());
        }

        internal CellItem GetViewportCell(int row, int column, bool containsSpan)
        {
            CellItem cell = null;
            RowItem presenter = _rowsLayer.GetRow(row);
            if (presenter != null)
            {
                cell = presenter.GetCell(column);
            }
            if (containsSpan && (cell == null))
            {
                foreach (RowItem presenter2 in _rowsLayer.Rows)
                {
                    if (presenter2 != null)
                    {
                        foreach (CellItem base3 in presenter2.Cells.Values)
                        {
                            if (((base3 != null) && (base3.CellLayout != null)) && ((base3.CellLayout.Row == row) && (base3.CellLayout.Column == column)))
                            {
                                return base3;
                            }
                        }
                    }
                }
            }
            return cell;
        }

        internal Size GetViewportSize()
        {
            return GetViewportSize(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        internal Size GetViewportSize(Size availableSize)
        {
            double viewportWidth = Excel.GetViewportWidth(ColumnViewportIndex);
            double viewportHeight = Excel.GetViewportHeight(RowViewportIndex);
            return new Size(Math.Min(viewportWidth, availableSize.Width), Math.Min(viewportHeight, availableSize.Height));
        }

        internal void InvalidateBordersMeasureState()
        {
            _borderLayer.InvalidateMeasure();
        }

        internal void InvalidateFloatingObjectMeasureState(FloatingObject floatingObject)
        {
            InvalidateFloatingObjectsMeasureState();
        }

        internal void InvalidateFloatingObjectsMeasureState()
        {
            _floatingLayer?.InvalidateMeasure();
        }

        internal void InvalidateRowsMeasureState(bool forceInvalidateRows)
        {
            _rowsLayer.InvalidateMeasure();
            if (forceInvalidateRows)
            {
                CellCache.ClearAll();
                CellOverflowLayoutBuildEngine.Clear();
                foreach (UIElement elem in _rowsLayer.Children)
                {
                    elem.InvalidateMeasure();
                }
            }
        }

        internal void InvalidateSelectionMeasureState()
        {
            _selectionLayer?.InvalidateMeasure();
        }

        internal bool IsCurrentEditingCell(int row, int column)
        {
            if (!IsEditing())
            {
                return false;
            }
            return ((_editorLayer.EditingRowIndex == row) && (_editorLayer.EditingColumnIndex == column));
        }

        public Point PointToClient(Point point)
        {
            return new Point(point.X - Location.X, point.Y - Location.Y);
        }

        void RefreshChartsResizingFramesLayouts()
        {
            Rect[] floatingObjectsResizingRects = Excel.GetFloatingObjectsResizingRects(RowViewportIndex, ColumnViewportIndex);
            if ((floatingObjectsResizingRects != null) && (floatingObjectsResizingRects.Length != 0))
            {
                List<Rect> list = new List<Rect>();
                foreach (Rect rect in floatingObjectsResizingRects)
                {
                    list.Add(new Rect(rect.X - Location.X, rect.Y - Location.Y, rect.Width, rect.Height));
                }
                _cachedChartShapeResizingRects = list.ToArray();
            }
        }

        internal void RefreshDragFill()
        {
            CellRange dragFillFrameRange = Excel.GetDragFillFrameRange();
            _cachedDragFillRect = GetRangeBounds(dragFillFrameRange);
            if (!_cachedDragFillRect.IsEmpty)
            {
                _cachedDragFillRect = new Rect(_cachedDragFillRect.Left - 2.0, _cachedDragFillRect.Top - 2.0, _cachedDragFillRect.Width + 3.0, _cachedDragFillRect.Height + 3.0);
                if (_dragFillLayer.Width == 0.0)
                    _dragFillLayer.Width = double.NaN;
                if (_dragFillLayer.Height == 0.0)
                    _dragFillLayer.Height = double.NaN;
            }
            else
            {
                _cachedDragFillRect = _rcEmpty;
            }
            _dragFillLayer.Arrange(_cachedDragFillRect);
        }

        internal void ResetDragFill()
        {
            _cachedDragFillRect = _rcEmpty;
            _dragFillLayer.Width = _cachedDragFillRect.Width;
            _dragFillLayer.Height = _cachedDragFillRect.Height;
        }

        internal void RefreshFlaotingObjectResizingFrames()
        {
            RefreshChartsResizingFramesLayouts();
            FloatingObjectsMovingResizingContainer.InvalidateMeasure();
            FloatingObjectsMovingResizingContainer.InvalidateArrange();
        }

        internal void RefreshFloatingObject(object parameter)
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.Refresh(parameter);
            }
        }

        internal void RefreshFloatingObjectContainerIsSelected()
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.RefreshContainerIsSelected();
            }
        }

        internal void RefreshFloatingObjectContainerIsSelected(FloatingObject floatingObject)
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.RefreshContainerIsSelected(floatingObject);
            }
        }

        internal void RefreshFloatingObjectMovingFrames()
        {
            RefreshFloatingObjectMovingFramesLayouts();
            FloatingObjectsMovingResizingContainer.InvalidateMeasure();
            FloatingObjectsMovingResizingContainer.InvalidateArrange();
        }

        void RefreshFloatingObjectMovingFramesLayouts()
        {
            Rect[] floatingObjectsMovingFrameRects = Excel.GetFloatingObjectsMovingFrameRects(RowViewportIndex, ColumnViewportIndex);
            if ((floatingObjectsMovingFrameRects != null) && (floatingObjectsMovingFrameRects.Length != 0))
            {
                List<Rect> list = new List<Rect>();
                foreach (Rect rect in floatingObjectsMovingFrameRects)
                {
                    list.Add(new Rect(rect.X - Location.X, rect.Y - Location.Y, rect.Width, rect.Height));
                }
                _cachedChartShapeMovingRects = list.ToArray();
            }
        }

        internal void RefreshFloatingObjects()
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.Refresh(null);
            }
        }

        public void RefreshSelection()
        {
            if (_selectionLayer != null)
            {
                var allSelectedFloatingObjects = Excel.GetAllSelectedFloatingObjects();
                if ((allSelectedFloatingObjects != null) && (allSelectedFloatingObjects.Count > 0))
                {
                    _selectionLayer.Visibility = Visibility.Collapsed;
                    if (Excel.InputDeviceType != InputDeviceType.Touch)
                    {
                        Excel.InvalidateMeasure();
                    }
                }
                else
                {
                    _selectionLayer.Visibility = Visibility.Visible;
                    BuildSelection();
                    InvalidateSelectionMeasureState();
                }
            }
        }

        internal void RemoveCellOverflowLayoutModel(int rowIndex)
        {
            CellOverflowLayoutBuildEngine.RemoveModel(rowIndex);
        }

        internal void ResetFloatingObjectovingFrames()
        {
            _cachedChartShapeMovingRects = null;
            FloatingObjectsMovingResizingContainer.InvalidateMeasure();
            FloatingObjectsMovingResizingContainer.InvalidateArrange();
        }

        internal void ResetFloatingObjectResizingFrames()
        {
            _cachedChartShapeResizingRects = null;
            FloatingObjectsMovingResizingContainer.InvalidateMeasure();
            FloatingObjectsMovingResizingContainer.InvalidateArrange();
        }

        internal void ResumeFloatingObjectsInvalidate(bool forceInvalidate)
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.ResumeFloatingObjectInvalidate(forceInvalidate);
            }
        }

        public void RetryEditing()
        {
            var tb = _editorLayer.Editor;
            if (tb != null)
            {
                tb.SelectAll();
                tb.Focus(FocusState.Programmatic);
            }
            else
            {
                Excel.FocusInternal();
            }
        }

        internal void SendFirstKey(string c, bool replace)
        {
            foreach (char ch in c)
            {
                if (((ch != '\r') && (ch != '\n')) && char.IsControl(ch))
                {
                    return;
                }
            }
            var box = _editorLayer.Editor;
            if ((box != null) && !box.IsReadOnly)
            {
                if (replace)
                {
                    box.Text = c;
                    box.SelectionStart = box.Text.Length;
                }
                else
                {
                    int selectionStart = box.SelectionStart;
                    int startIndex = selectionStart;
                    string text = box.Text;
                    if (!string.IsNullOrEmpty(box.Text))
                    {
                        MatchCollection matchs = Regex.Matches(box.Text.Replace("\n", "").Substring(0, selectionStart), "\r");
                        if (matchs != null)
                        {
                            startIndex += matchs.Count;
                        }
                    }
                    if (startIndex <= box.Text.Length)
                    {
                        box.Text = box.Text.Insert(startIndex, c);
                        box.SelectionStart = selectionStart + 1;
                        EditingContainer.InvalidateMeasure();
                        EditingContainer.InvalidateArrange();
                    }
                }
            }
        }

        public void SetFlotingObjectZIndex(string name, int zIndex)
        {
            FloatingObjectsPanel.SetFlotingObjectZIndex(name, zIndex);
        }

        async void ShowSheetCell(int row, int column)
        {
            await Excel.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Excel.ShowCell(RowViewportIndex, ColumnViewportIndex, row, column, VerticalPosition.Nearest, HorizontalPosition.Nearest);
            });
        }

        internal void SuspendFloatingObjectsInvalidate()
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.SuspendFloatingObjectInvalidate();
            }
        }

        internal void SynChartShapeThemes()
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.SyncChartShapeTheme();
            }
        }

        internal SpanGraph CachedSpanGraph
        {
            get { return _cachedSpanGraph; }
        }

        internal CellCachePool CellCache
        {
            get { return _cellCachePool; }
        }

        internal CellOverflowLayoutBuildEngine CellOverflowLayoutBuildEngine
        {
            get
            {
                if (_buildEngine == null)
                {
                    _buildEngine = new CellOverflowLayoutBuildEngine(this);
                }
                return _buildEngine;
            }
        }

        public int ColumnViewportIndex { get; set; }

        internal EditingLayer EditingContainer
        {
            get { return _editorLayer; }
        }

        internal Rect EditorBounds
        {
            get { return _editorBounds; }
        }

        internal bool EditorDirty
        {
            get { return _editorLayer.EditorDirty; }
        }

        FloatingObjectMovingLayer FloatingObjectsMovingResizingContainer
        {
            get { return _floatingEditLayer; }
        }

        internal FloatingObjectLayer FloatingObjectsPanel
        {
            get { return _floatingLayer; }
        }

        internal bool IsActived
        {
            get { return ((Excel == null) || ((Excel.GetActiveColumnViewportIndex() == ColumnViewportIndex) && (Excel.GetActiveRowViewportIndex() == RowViewportIndex))); }
        }

        public Point Location { get; set; }

        internal RowsLayer RowsContainer
        {
            get { return _rowsLayer; }
        }

        public int RowViewportIndex { get; set; }

        internal SelectionLayer SelectionContainer
        {
            get { return _selectionLayer; }
        }

        public Excel Excel { get; private set; }

        Rectangle CreateDragFillLayer()
        {
            Rectangle rect = new Rectangle();
            rect.Stroke = BrushRes.BlackBrush;
            rect.StrokeThickness = 2.0;
            rect.StrokeDashArray = new DoubleCollection { 2.0, 1.0 };
            rect.StrokeDashOffset = 0.5;
            return rect;
        }
    }

    internal sealed class CellCachePool : ICellSupport
    {
        Dictionary<ulong, Cell> _cache = new Dictionary<ulong, Cell>();
        ICellsSupport _cellsSupport;

        public CellCachePool(ICellsSupport p_cellsSupport)
        {
            _cellsSupport = p_cellsSupport;
        }

        public void ClearAll()
        {
            foreach (Cell cell in _cache.Values)
            {
                if (cell != null)
                {
                    cell.CacheStyleObject(false);
                }
            }
            _cache.Clear();
        }

        public void ClearRow(int rowIndex)
        {
            foreach (ulong num in _cache.Keys)
            {
                int num2 = (int)(num >> 0x20);
                if (num2 == rowIndex)
                {
                    _cache[num].CacheStyleObject(false);
                    _cache.Remove(num);
                }
            }
        }

        public Cell GetCachedCell(int rowIndex, int columnIndex)
        {
            ulong num = (ulong)rowIndex;
            num = num << 0x20;
            num += (ulong)columnIndex;
            Cell cell = null;
            if (_cache.TryGetValue(num, out cell))
            {
                return cell;
            }
            return Add(rowIndex, columnIndex);
        }

        Cell Add(int rowIndex, int columnIndex)
        {
            if ((rowIndex < 0)
                || (rowIndex >= _cellsSupport.Rows.Count)
                || (columnIndex < 0)
                || (columnIndex >= _cellsSupport.Columns.Count))
            {
                return null;
            }

            Cell cell = _cellsSupport.Cells[rowIndex, columnIndex];
            ulong num = (ulong)rowIndex;
            num = num << 0x20;
            num += (ulong)columnIndex;
            _cache[num] = cell;
            cell.CacheStyleObject(true);
            return cell;
        }

        Cell ICellSupport.GetCell(int row, int column)
        {
            return GetCachedCell(row, column);
        }
    }
}

