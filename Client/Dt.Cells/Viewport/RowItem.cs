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
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 单元格行面板
    /// </summary>
    internal partial class RowItem : Panel
    {
        CellItem _headingOverflowCell;
        int _row;
        double _rowWidth;
        CellItem _trailingOverflowCell;
        const int _FlowCellZIndexBase = 0x7530;
        const int _NormalCellZIndexBase = 0x2710;
        const int _SpanCellZIndexBase = 0x4e20;

        public RowItem(CellsPanel p_panel)
        {
            OwnPanel = p_panel;
            _row = -1;
            Cells = new Dictionary<int, CellItem>();
        }

        #region 测量布局
        //*** CellsPanel.Measure -> RowsLayer.Measure -> RowItem.UpdateVisual -> CellItem.UpdateVisual -> RowItem.Measure -> CellItem.Measure ***

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
                return Size.Empty;

            ColumnLayoutModel colLayoutModel = GetColumnLayoutModel();
            RowLayout layout = OwnPanel.GetRowLayoutModel().FindRow(Row);
            CellOverflowLayoutModel cellOverflowLayoutModel = OwnPanel.GetCellOverflowLayoutModel(Row);
            _rowWidth = 0.0;

            foreach (ColumnLayout colLayout in colLayoutModel)
            {
                _rowWidth += colLayout.Width;
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
            }

            double width = Math.Min(_rowWidth, OwnPanel.GetViewportSize().Width);
            if (cellOverflowLayoutModel != null)
            {
                Worksheet worksheet = OwnPanel.Sheet.ActiveSheet;
                float zoomFactor = OwnPanel.Sheet.ZoomFactor;
                if (cellOverflowLayoutModel.HeadingOverflowlayout != null)
                {
                    HeadingOverflowCell.CellOverflowLayout = cellOverflowLayoutModel.HeadingOverflowlayout;
                    if ((OwnPanel != null) && OwnPanel.IsCurrentEditingCell(HeadingOverflowCell.BindingCell.Row.Index, HeadingOverflowCell.BindingCell.Column.Index))
                    {
                        HeadingOverflowCell.HideForEditing();
                    }
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
            return new Size(width, layout.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ColumnLayoutModel colLayoutModel = GetColumnLayoutModel();
            RowLayout layout = OwnPanel.GetRowLayoutModel().FindRow(Row);
            foreach (ColumnLayout colLayout in colLayoutModel)
            {
                if (colLayout.Width <= 0.0)
                    continue;

                CellItem cell = GetCell(colLayout.Column);
                if (cell != null)
                {
                    double left = colLayout.X;
                    double top = layout.Y;
                    double w = colLayout.Width;
                    double h = layout.Height;
                    int num5 = 0x2710 + colLayout.Column;

                    CellLayout cellLayout = cell.CellLayout;
                    if (cellLayout != null)
                    {
                        left = cellLayout.X;
                        top = cellLayout.Y;
                        w = cellLayout.Width;
                        h = cellLayout.Height;
                        num5 = 0x4e20 + colLayout.Column;
                    }
                    if (cell.CellOverflowLayout != null)
                        num5 = 0x7530 + colLayout.Column;
                    num5 = num5 % 0x7ffe;
                    Canvas.SetZIndex(cell, num5);
                    cell.Arrange(new Rect(left - Location.X, top - Location.Y, w, h));
                }
            }

            CellOverflowLayoutModel cellOverflowLayoutModel = OwnPanel.GetCellOverflowLayoutModel(Row);
            if (cellOverflowLayoutModel == null)
                return finalSize;

            Worksheet worksheet = OwnPanel.Sheet.ActiveSheet;
            float zoomFactor = OwnPanel.Sheet.ZoomFactor;
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
                int num13 = 0x7530 + cellOverflowLayoutModel.HeadingOverflowlayout.Column;
                num13 = num13 % 0x7ffe;
                Canvas.SetZIndex(HeadingOverflowCell, num13);
                HeadingOverflowCell.Arrange(rect);
            }
            if (cellOverflowLayoutModel.TrailingOverflowlayout == null)
                return finalSize;

            double x = Location.X;
            double y = layout.Y;
            ColumnLayout layout4 = colLayoutModel[colLayoutModel.Count - 1];
            if (layout4 == null)
            {
                return finalSize;
            }
            x = layout4.X;
            for (int i = layout4.Column; i < cellOverflowLayoutModel.TrailingOverflowlayout.Column; i++)
            {
                x += worksheet.GetActualColumnWidth(i, SheetArea.Cells) * zoomFactor;
            }
            double width = worksheet.GetActualColumnWidth(cellOverflowLayoutModel.TrailingOverflowlayout.Column, SheetArea.Cells) * zoomFactor;
            Size size2 = new Size(width, layout.Height);
            Rect rect2 = new Rect(PointToClient(new Point(x, y)), size2);
            int num18 = 0x7530 + cellOverflowLayoutModel.TrailingOverflowlayout.Column;
            num18 = num18 % 0x7ffe;
            Canvas.SetZIndex(TrailingOverflowCell, num18);
            TrailingOverflowCell.Arrange(rect2);
            return finalSize;
        }
        #endregion

        public void CleanUpBeforeDiscard()
        {
            foreach (var cell in Cells.Values)
            {
                cell.CleanUpBeforeDiscard();
            }
        }

        void RemoveCell(CellItem p_cell)
        {
            Cells.Remove(p_cell.Column);
            Children.Remove(p_cell);
            p_cell.CleanUpBeforeDiscard();
        }


        void UpdateChildren()
        {
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            List<CellItem> lsRecy = new List<CellItem>();
            List<CellItem> lsUsing = new List<CellItem>();
            ContainsSpanCell = false;

            // Cells 和 Children 元素同步
            foreach (var cell in Cells.Values)
            {
                if (!cell.IsRecylable)
                {
                    lsUsing.Add(cell);
                }
                else
                {
                    ColumnLayout layout = columnLayoutModel.FindColumn(cell.Column);
                    if (layout == null || layout.Width <= 0.0)
                    {
                        // 没有使用
                        lsRecy.Add(cell);
                    }
                }
            }

            // 移除不可回收的
            foreach (CellItem cp in lsUsing)
            {
                RemoveCell(cp);
            }

            SpanGraph cachedSpanGraph = OwnPanel.CachedSpanGraph;
            for (int i = 0; i < columnLayoutModel.Count; i++)
            {
                ColumnLayout colLayout = columnLayoutModel[i];
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
                if (layout != null && layout.Width > 0.0 && layout.Height > 0.0)
                {
                    CellRange range = ClipCellRange(layout.GetCellRange());
                    bool rangeChanged = (Row != range.Row) || (range.Column != colLayout.Column);
                    if (rangeChanged && cell != null)
                    {
                        // 跨度不同移除
                        RemoveCell(cell);
                        cell = null;
                    }

                    if (layout.ColumnCount > 1)
                    {
                        // 移除跨度区域内的所有格
                        int num4 = (layout.Column + layout.ColumnCount) - 1;
                        for (int j = i + 1; j < columnLayoutModel.Count; j++)
                        {
                            int num6 = columnLayoutModel[j].Column;
                            if (num6 > num4)
                                break;

                            i = j;
                            CellItem ci = GetCell(num6);
                            if ((ci != null) && Cells.Remove(num6))
                            {
                                Children.Remove(ci);
                            }
                        }
                    }
                }

                if (cell == null)
                {
                    if (lsRecy.Count > 0)
                    {
                        // 优先使用Cells中可回收的格
                        cell = lsRecy[0];
                        lsRecy.RemoveAt(0);
                    }
                    else
                    {
                        cell = new CellItem(this);
                        Cells.Add(colLayout.Column, cell);
                        Children.Add(cell);
                    }
                }
                cell.CellLayout = layout;
                cell.Column = colLayout.Column;
                cell.UpdateChildren();
            }

            foreach (CellItem cell in lsRecy)
            {
                RemoveCell(cell);
            }

            if (OwnPanel.SupportCellOverflow)
            {
                CellOverflowLayoutModel cellOverflowLayoutModel = OwnPanel.GetCellOverflowLayoutModel(Row);
                if ((cellOverflowLayoutModel != null) && !cellOverflowLayoutModel.IsEmpty)
                {
                    if (cellOverflowLayoutModel.HeadingOverflowlayout != null)
                    {
                        if (HeadingOverflowCell == null)
                            HeadingOverflowCell = new CellItem(this);
                        HeadingOverflowCell.Column = cellOverflowLayoutModel.HeadingOverflowlayout.Column;
                    }
                    else
                    {
                        HeadingOverflowCell = null;
                    }

                    if (cellOverflowLayoutModel.TrailingOverflowlayout != null)
                    {
                        if (TrailingOverflowCell == null)
                            TrailingOverflowCell = new CellItem(this);
                        TrailingOverflowCell.Column = cellOverflowLayoutModel.TrailingOverflowlayout.Column;
                    }
                    else
                    {
                        TrailingOverflowCell = null;
                    }
                    InvalidateMeasure();
                }
                else
                {
                    HeadingOverflowCell = null;
                    TrailingOverflowCell = null;
                }
            }
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

        public CellItem GetCell(int column)
        {
            if (Cells.TryGetValue(column, out var cell))
                return cell;
            return null;
        }

        protected SheetSpanModelBase GetCellSpanModel()
        {
            return OwnPanel.Sheet.ActiveSheet.SpanModel;
        }

        public ColumnLayoutModel GetColumnLayoutModel()
        {
            return OwnPanel.Sheet.GetViewportColumnLayoutModel(OwnPanel.ColumnViewportIndex);
        }

        public Point PointToClient(Point point)
        {
            return new Point(point.X - Location.X, point.Y - Location.Y);
        }

        public Dictionary<int, CellItem> Cells { get; }

        public bool ContainsSpanCell { get; set; }

        public bool IsRecyclable
        {
            get
            {
                foreach (CellItem cell in Cells.Values)
                {
                    if (!cell.IsRecylable)
                        return false;
                }
                return true;
            }
        }

        public Point Location { get; set; }

        public CellsPanel OwnPanel { get; }

        public int Row
        {
            get { return _row; }
            set
            {
                if (_row != value)
                {
                    _row = value;

                    // -1表示被回收
                    if (_row > -1)
                        UpdateChildren();
                }
            }
        }

        public double RowWidth
        {
            get { return _rowWidth; }
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

