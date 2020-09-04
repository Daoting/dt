#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 单元格行面板
    /// </summary>
    internal partial class RowItem : Panel
    {
        #region 成员变量
        const int _flowCellZIndexBase = 0x7530;
        const int _normalCellZIndexBase = 0x2710;
        const int _spanCellZIndexBase = 0x4e20;
        static Rect _rcEmpty = new Rect();
        static Size _szEmpty = new Size();
        CellItem _headingOverflowCell;
        CellItem _trailingOverflowCell;
        readonly List<CellItem> _recycledCells;
        #endregion

        #region 构造方法
        public RowItem(CellsPanel p_panel)
        {
            OwnPanel = p_panel;
            Row = -1;
            Cells = new Dictionary<int, CellItem>();
            _recycledCells = new List<CellItem>();
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }
        #endregion

        #region 属性
        public CellsPanel OwnPanel { get; }

        public Dictionary<int, CellItem> Cells { get; }

        public bool ContainsSpanCell { get; set; }

        public Point Location { get; set; }

        /// <summary>
        /// -1表示被回收
        /// </summary>
        public int Row { get; set; }

        public double RowWidth { get; set; }

        public ColumnLayoutModel GetColumnLayoutModel()
        {
            return OwnPanel.Excel.GetViewportColumnLayoutModel(OwnPanel.ColumnViewportIndex);
        }

        public void CleanUpBeforeDiscard()
        {
            foreach (var cell in Cells.Values)
            {
                cell.CleanUpBeforeDiscard();
            }
        }

        public CellItem GetCell(int column)
        {
            if (Cells.TryGetValue(column, out var cell))
                return cell;
            return null;
        }
        #endregion

        #region 测量布局
        //*** CellsPanel.Measure -> RowsLayer.Measure -> RowItem.UpdateChildren -> 行列改变时 CellItem.UpdateChildren -> RowItem.Measure -> CellItem.Measure ***//

        public void UpdateChildren(bool p_updateAllCell)
        {
            // 频繁增删Children子元素会出现卡顿现象！
            // Children = Cells + _recycledCells
            ColumnLayoutModel colLayoutModel = GetColumnLayoutModel();
            int less = colLayoutModel.Count - Children.Count + (_headingOverflowCell == null ? 0 : 1) + (_trailingOverflowCell == null ? 0 : 1);
            if (less > 0)
            {
                for (int i = 0; i < less; i++)
                {
                    var cell = new CellItem(this);
                    Children.Add(cell);
                    _recycledCells.Add(cell);
                }
            }

            // 先回收不可见格
            var cols = Cells.Values.ToList();
            foreach (var cell in cols)
            {
                ColumnLayout layout = colLayoutModel.FindColumn(cell.Column);
                if (layout == null || layout.Width <= 0.0)
                {
                    PushRecycledCell(cell);
                }
            }

            int updateCells = 0;
            ContainsSpanCell = false;
            SpanGraph cachedSpanGraph = OwnPanel.CachedSpanGraph;
            for (int i = 0; i < colLayoutModel.Count; i++)
            {
                ColumnLayout colLayout = colLayoutModel[i];
                if (colLayout.Width <= 0.0)
                    continue;

                byte state = cachedSpanGraph.GetState(Row, colLayout.Column);
                CellLayout layout = null;
                if (state > 0)
                {
                    CellLayoutModel cellLayoutModel = OwnPanel.GetCellLayoutModel();
                    if (cellLayoutModel != null)
                    {
                        layout = cellLayoutModel.FindCell(Row, colLayout.Column);
                        if (layout != null)
                            ContainsSpanCell = true;
                    }
                }

                CellItem cell = GetCell(colLayout.Column);
                bool rangeChanged = false;
                if (layout != null && layout.Width > 0.0 && layout.Height > 0.0)
                {
                    CellRange range = ClipCellRange(layout.GetCellRange());
                    rangeChanged = (Row != range.Row) || (range.Column != colLayout.Column);

                    // 跨多列
                    if (layout.ColumnCount > 1)
                    {
                        // 移除跨度区域内的所有格
                        int maxCol = (layout.Column + layout.ColumnCount) - 1;
                        for (int j = i + 1; j < colLayoutModel.Count; j++)
                        {
                            int curCol = colLayoutModel[j].Column;
                            if (curCol > maxCol)
                                break;

                            i = j;
                            CellItem ci = GetCell(curCol);
                            if (ci != null)
                                PushRecycledCell(ci);
                        }
                    }
                }

                // 跨度不同不显示
                if (rangeChanged)
                {
                    if (cell != null)
                        PushRecycledCell(cell);
                    continue;
                }

                bool updated = false;
                if (cell == null)
                {
                    // 重新利用回收的格
                    cell = _recycledCells[0];
                    _recycledCells.RemoveAt(0);
                    cell.Column = colLayout.Column;
                    Cells.Add(colLayout.Column, cell);
                    updated = true;
                }
                cell.CellLayout = layout;

                if (p_updateAllCell || updated)
                {
                    cell.UpdateChildren();
                    updateCells++;
                }
            }

            CellOverflowLayoutModel cellOverflowLayoutModel = OwnPanel.GetCellOverflowLayoutModel(Row);
            if ((cellOverflowLayoutModel != null) && !cellOverflowLayoutModel.IsEmpty)
            {
                if (cellOverflowLayoutModel.HeadingOverflowlayout != null)
                {
                    if (HeadingOverflowCell == null)
                        HeadingOverflowCell = new CellItem(this);
                    if (HeadingOverflowCell.Column != cellOverflowLayoutModel.HeadingOverflowlayout.Column)
                    {
                        HeadingOverflowCell.Column = cellOverflowLayoutModel.HeadingOverflowlayout.Column;
                        HeadingOverflowCell.UpdateChildren();
                        updateCells++;
                    }
                }
                else
                {
                    HeadingOverflowCell = null;
                }

                if (cellOverflowLayoutModel.TrailingOverflowlayout != null)
                {
                    if (TrailingOverflowCell == null)
                        TrailingOverflowCell = new CellItem(this);
                    if (TrailingOverflowCell.Column != cellOverflowLayoutModel.TrailingOverflowlayout.Column)
                    {
                        TrailingOverflowCell.Column = cellOverflowLayoutModel.TrailingOverflowlayout.Column;
                        TrailingOverflowCell.UpdateChildren();
                        updateCells++;
                    }
                }
                else
                {
                    TrailingOverflowCell = null;
                }
            }
            else
            {
                HeadingOverflowCell = null;
                TrailingOverflowCell = null;
            }

#if !IOS
            // iOS在 MeasureOverride 内部调用子元素的 InvalidateMeasure 会造成死循环！
            // 不重新测量会造成如：迷你图忽大忽小的情况
            if (updateCells > 0)
                InvalidateMeasure();
#endif
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Row == -1 || availableSize.Width == 0.0 || availableSize.Height == 0.0)
            {
                foreach (UIElement elem in Children)
                {
                    elem.Measure(_szEmpty);
                }
                return _szEmpty;
            }

            ColumnLayoutModel colLayoutModel = GetColumnLayoutModel();
            RowLayout layout = OwnPanel.GetRowLayoutModel().FindRow(Row);
            CellOverflowLayoutModel cellOverflowLayoutModel = OwnPanel.GetCellOverflowLayoutModel(Row);
            RowWidth = 0.0;
            double height = 0.0;

            foreach (ColumnLayout colLayout in colLayoutModel)
            {
                RowWidth += colLayout.Width;
                CellItem cell = GetCell(colLayout.Column);
                if (cell == null)
                    continue;

                double w = colLayout.Width;
                double h = layout.Height;
                if (cell.CellLayout != null)
                {
                    w = cell.CellLayout.Width;
                    h = cell.CellLayout.Height;
                }

                cell.CellOverflowLayout = null;
                if (cell.CellLayout == null && cellOverflowLayoutModel != null)
                {
                    var cellOverflowLayout = cellOverflowLayoutModel.GetCellOverflowLayout(colLayout.Column);
                    if ((cellOverflowLayout != null) && (cellOverflowLayout.Column == colLayout.Column))
                    {
                        cell.CellOverflowLayout = cellOverflowLayout;
                    }
                }
                cell.Measure(new Size(w, h));
                if (h > height)
                    height = h;
            }

            if (_recycledCells.Count > 0)
            {
                foreach (var cell in _recycledCells)
                {
                    cell.Measure(_szEmpty);
                }
            }

            double width = Math.Min(RowWidth, OwnPanel.GetViewportSize().Width);
            if (cellOverflowLayoutModel != null)
            {
                Worksheet worksheet = OwnPanel.Excel.ActiveSheet;
                float zoomFactor = OwnPanel.Excel.ZoomFactor;
                if (cellOverflowLayoutModel.HeadingOverflowlayout != null)
                {
                    HeadingOverflowCell.CellOverflowLayout = cellOverflowLayoutModel.HeadingOverflowlayout;
                    double num8 = worksheet.GetActualColumnWidth(cellOverflowLayoutModel.HeadingOverflowlayout.Column, SheetArea.Cells) * zoomFactor;
                    Size size = new Size(num8, layout.Height);
                    HeadingOverflowCell.Measure(size);
                }
                if (cellOverflowLayoutModel.TrailingOverflowlayout != null)
                {
                    TrailingOverflowCell.CellOverflowLayout = cellOverflowLayoutModel.TrailingOverflowlayout;
                    double num9 = worksheet.GetActualColumnWidth(cellOverflowLayoutModel.TrailingOverflowlayout.Column, SheetArea.Cells) * zoomFactor;
                    Size size2 = new Size(num9, layout.Height);
                    TrailingOverflowCell.Measure(size2);
                }
            }
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Row == -1 || finalSize.Width == 0 || finalSize.Height == 0)
            {
                foreach (UIElement elem in Children)
                {
                    elem.Arrange(_rcEmpty);
                }
                return finalSize;
            }

            ColumnLayoutModel colLayoutModel = GetColumnLayoutModel();
            RowLayout layout = OwnPanel.GetRowLayoutModel().FindRow(Row);
            foreach (ColumnLayout colLayout in colLayoutModel)
            {
                CellItem cell;
                if (colLayout.Width <= 0.0 || (cell = GetCell(colLayout.Column)) == null)
                    continue;

                double left = colLayout.X;
                double top = layout.Y;
                double w = colLayout.Width;
                double h = layout.Height;
                int zIndex = _normalCellZIndexBase + colLayout.Column;

                CellLayout cellLayout = cell.CellLayout;
                if (cellLayout != null)
                {
                    left = cellLayout.X;
                    top = cellLayout.Y;
                    w = cellLayout.Width;
                    h = cellLayout.Height;
                    zIndex = _spanCellZIndexBase + colLayout.Column;
                }

                // Canvas.SetZIndex 对所有继承自 Panel 的面板都有效，z值大的在上层，z值相同时按 Children 的索引，索引大的在上层
                // 但uno中 Canvas.SetZIndex 无效，只按 Children 的索引确定层次！
                if (cell.CellOverflowLayout != null)
                    zIndex = _flowCellZIndexBase + colLayout.Column;
                zIndex = zIndex % 0x7ffe;
                Canvas.SetZIndex(cell, zIndex);

                cell.Arrange(new Rect(left - Location.X, top - Location.Y, w, h));
            }

            if (_recycledCells.Count > 0)
            {
                foreach (var cell in _recycledCells)
                {
                    cell.Arrange(_rcEmpty);
                }
            }

            CellOverflowLayoutModel cellOverflowLayoutModel = OwnPanel.GetCellOverflowLayoutModel(Row);
            if (cellOverflowLayoutModel == null)
                return finalSize;

            Worksheet worksheet = OwnPanel.Excel.ActiveSheet;
            float zoomFactor = OwnPanel.Excel.ZoomFactor;
            if (cellOverflowLayoutModel.HeadingOverflowlayout != null)
            {
                double num7 = Location.X;
                double num8 = layout.Y;
                for (int j = cellOverflowLayoutModel.HeadingOverflowlayout.Column; j < colLayoutModel[0].Column; j++)
                {
                    double actualColumnWidth = worksheet.GetActualColumnWidth(j, SheetArea.Cells);
                    num7 -= actualColumnWidth * zoomFactor;
                }
                double num12 = worksheet.GetActualColumnWidth(cellOverflowLayoutModel.HeadingOverflowlayout.Column, SheetArea.Cells) * zoomFactor;
                Size size = new Size(num12, layout.Height);
                Rect rect = new Rect(PointToClient(new Point(num7, num8)), size);
                int num13 = _flowCellZIndexBase + cellOverflowLayoutModel.HeadingOverflowlayout.Column;
                num13 = num13 % 0x7ffe;
                Canvas.SetZIndex(HeadingOverflowCell, num13);
                HeadingOverflowCell.Arrange(rect);
            }
            if (cellOverflowLayoutModel.TrailingOverflowlayout == null)
                return finalSize;

            ColumnLayout layout4 = colLayoutModel[colLayoutModel.Count - 1];
            if (layout4 == null)
                return finalSize;

            double x = layout4.X;
            double y = layout.Y;
            for (int i = layout4.Column; i < cellOverflowLayoutModel.TrailingOverflowlayout.Column; i++)
            {
                x += worksheet.GetActualColumnWidth(i, SheetArea.Cells) * zoomFactor;
            }
            double width = worksheet.GetActualColumnWidth(cellOverflowLayoutModel.TrailingOverflowlayout.Column, SheetArea.Cells) * zoomFactor;
            Size size2 = new Size(width, layout.Height);
            Rect rect2 = new Rect(PointToClient(new Point(x, y)), size2);
            int num18 = _flowCellZIndexBase + cellOverflowLayoutModel.TrailingOverflowlayout.Column;
            num18 = num18 % 0x7ffe;
            Canvas.SetZIndex(TrailingOverflowCell, num18);
            TrailingOverflowCell.Arrange(rect2);
            return finalSize;
        }
        #endregion

        void PushRecycledCell(CellItem cell)
        {
            _recycledCells.Add(cell);
            Cells.Remove(cell.Column);
            cell.Column = -1;
            cell.CleanUpBeforeDiscard();
        }

        CellRange ClipCellRange(CellRange source)
        {
            RowLayoutModel rowLayoutModel = OwnPanel.GetRowLayoutModel();
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            ICellsSupport dataContext = OwnPanel.GetDataContext();
            int row = Math.Max(rowLayoutModel[0].Row, source.Row);
            int num2 = Math.Min(rowLayoutModel[rowLayoutModel.Count - 1].Row, (source.Row + source.RowCount) - 1);
            int column = Math.Max(columnLayoutModel[0].Column, source.Column);
            int num4 = Math.Min(columnLayoutModel[columnLayoutModel.Count - 1].Column, (source.Column + source.ColumnCount) - 1);
            for (int i = row; i <= num2; i++)
            {
                if (dataContext.Rows[i].ActualHeight > 0.0)
                {
                    row = i;
                    break;
                }
            }
            for (int j = column; j <= num4; j++)
            {
                if (dataContext.Columns[j].ActualWidth > 0.0)
                {
                    column = j;
                    break;
                }
            }
            return new CellRange(row, column, (num2 - row) + 1, (num4 - column) + 1);
        }

        Point PointToClient(Point point)
        {
            return new Point(point.X - Location.X, point.Y - Location.Y);
        }

        CellItem HeadingOverflowCell
        {
            get { return _headingOverflowCell; }
            set
            {
                if (_headingOverflowCell != value)
                {
                    if (_headingOverflowCell != null)
                        Children.Remove(_headingOverflowCell);

                    _headingOverflowCell = value;
                    if (value != null)
                        Children.Add(value);
                }
            }
        }

        CellItem TrailingOverflowCell
        {
            get { return _trailingOverflowCell; }
            set
            {
                if (_trailingOverflowCell != value)
                {
                    if (_trailingOverflowCell != null)
                        Children.Remove(_trailingOverflowCell);

                    _trailingOverflowCell = value;
                    if (value != null)
                        Children.Add(value);
                }
            }
        }

    }
}

