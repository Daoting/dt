#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal abstract partial class HeaderPanel : Panel
    {
        const int _normalZIndexBase = 10000;
        const int _spanRowZIndexBase = 20000;
        static Rect _rcEmpty = new Rect();
        static Size _szEmpty = new Size();
        readonly Dictionary<int, HeaderItem> _rows;
        readonly List<HeaderItem> _recycledRows;

        public HeaderPanel(Excel p_excel, SheetArea p_sheetArea)
        {
            Area = p_sheetArea;
            Excel = p_excel;
            CachedSpanGraph = new SpanGraph();
            CellCache = new CellCachePool(GetDataContext());
            _rows = new Dictionary<int, HeaderItem>();
            _recycledRows = new List<HeaderItem>();

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public Excel Excel { get; }

        public SheetArea Area { get; }

        public Point Location { get; internal set; }

        public int RowViewportIndex { get; internal set; }

        public int ColumnViewportIndex { get; internal set; }

        public CellCachePool CellCache { get; }

        public SpanGraph CachedSpanGraph { get; }

        public Rect GetRangeBounds(CellRange range, SheetArea area)
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
            return new Rect(x - Location.X, y - Location.Y, width, height);
        }

        public void InvalidateRowsMeasureState(bool forceInvalidateRows)
        {
            InvalidateMeasure();
            if (forceInvalidateRows)
            {
                CellCache.ClearAll();
                foreach (UIElement elem in Children)
                {
                    elem.InvalidateMeasure();
                }
            }
        }

        public HeaderItem GetRow(int row)
        {
            if (_rows.TryGetValue(row, out var item))
                return item;
            return null;
        }

        public RowLayoutModel GetRowLayoutModel()
        {
            return (Area == SheetArea.ColumnHeader) ? Excel.GetColumnHeaderRowLayoutModel() : Excel.GetViewportRowLayoutModel(RowViewportIndex);
        }

        public HeaderCellItem GetViewportCell(int row, int column, bool containsSpan)
        {
            HeaderCellItem cell = null;
            HeaderItem presenter = GetRow(row);
            if (presenter != null)
            {
                cell = presenter.GetCell(column);
            }
            if (containsSpan && cell == null)
            {
                foreach (HeaderItem item in _rows.Values)
                {
                    if (item != null)
                    {
                        foreach (var ci in item.Cells.Values)
                        {
                            if (((ci != null) && (ci.CellLayout != null)) && ((ci.CellLayout.Row == row) && (ci.CellLayout.Column == column)))
                            {
                                return ci;
                            }
                        }
                    }
                }
            }
            return cell;
        }

        public ICellsSupport GetDataContext()
        {
            return (Area == SheetArea.ColumnHeader) ? (ICellsSupport)Excel.ActiveSheet.ColumnHeader : Excel.ActiveSheet.RowHeader;
        }

        public Size GetViewportSize()
        {
            return GetViewportSize(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        public Size GetViewportSize(Size availableSize)
        {
            SheetLayout sheetLayout = Excel.GetSheetLayout();
            if (Area == SheetArea.ColumnHeader)
            {
                double viewportWidth = sheetLayout.GetViewportWidth(ColumnViewportIndex);
                double headerHeight = sheetLayout.HeaderHeight;
                viewportWidth = Math.Min(viewportWidth, availableSize.Width);
                return new Size(viewportWidth, Math.Min(headerHeight, availableSize.Height));
            }

            double headerWidth = sheetLayout.HeaderWidth;
            double viewportHeight = sheetLayout.GetViewportHeight(RowViewportIndex);
            headerWidth = Math.Min(headerWidth, availableSize.Width);
            return new Size(headerWidth, Math.Min(viewportHeight, availableSize.Height));
        }

        public CellLayoutModel GetCellLayoutModel()
        {
            return (Area == SheetArea.ColumnHeader) ?
                Excel.GetColumnHeaderCellLayoutModel(ColumnViewportIndex) : Excel.GetRowHeaderCellLayoutModel(RowViewportIndex);
        }

        #region 测量布局
        protected override Size MeasureOverride(Size availableSize)
        {
            BuildSpanGraph();

            // 频繁增删Children子元素会出现卡顿现象！
            // Children = _rows + _recycledRows
            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            int less = rowLayoutModel.Count - Children.Count;
            if (less > 0)
            {
                for (int i = 0; i < less; i++)
                {
                    HeaderItem rowItem = new HeaderItem(this);
                    Children.Add(rowItem);
                    _recycledRows.Add(rowItem);
                }
            }

            // 先回收不可见行
            List<HeaderItem> rows = _rows.Values.ToList();
            foreach (var rowItem in rows)
            {
                RowLayout layout = rowLayoutModel.FindRow(rowItem.Row);
                if (layout == null || layout.Height <= 0.0)
                {
                    _recycledRows.Add(rowItem);
                    _rows.Remove(rowItem.Row);
                    rowItem.Row = -1;
                }
            }

            double x = Location.X;
            double y = Location.Y;
            double maxWidth = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height <= 0.0)
                    continue;

                bool updateAllCell = false;
                HeaderItem rowItem = null;
                if (!_rows.TryGetValue(layout.Row, out rowItem))
                {
                    // 重新利用回收的行
                    rowItem = _recycledRows[0];
                    _recycledRows.RemoveAt(0);
                    rowItem.Row = layout.Row;
                    _rows.Add(layout.Row, rowItem);
                    updateAllCell = true;
                }
                rowItem.UpdateChildren(updateAllCell);

                int z = rowItem.ContainsSpanCell ? _spanRowZIndexBase + rowItem.Row : _normalZIndexBase + rowItem.Row;
                z = z % 0x7ffe;
                Canvas.SetZIndex(rowItem, z);

                rowItem.Location = new Point(x, y);
                // 测量尺寸足够大，否则当单元格占多行时在uno上只绘一行！
                rowItem.Measure(availableSize);
                y += layout.Height;
                maxWidth = Math.Max(maxWidth, rowItem.DesiredSize.Width);
            }

            // 测量回收的行
            if (_recycledRows.Count > 0)
            {
                foreach (var rowItem in _recycledRows)
                {
                    rowItem.Measure(_szEmpty);
                }
            }
            return new Size(maxWidth + Location.X, y);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            double y = 0.0;
            double rowWidth = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height <= 0.0)
                    continue;

                if (_rows.TryGetValue(layout.Row, out var rowItem))
                {
                    // 一定按行的最大高度布局，否则当单元格占多行时在uno上只绘一行！
                    rowItem.Arrange(new Rect(0.0, y, finalSize.Width, rowItem.DesiredSize.Height));
                    if (rowWidth == 0.0)
                        rowWidth = rowItem.RowWidth;
                }
                y += layout.Height;
            }

            if (_recycledRows.Count > 0)
            {
                foreach (var rowItem in _recycledRows)
                {
                    rowItem.Arrange(_rcEmpty);
                }
            }

            rowWidth = Math.Min(GetViewportSize().Width, rowWidth);
            Size size = new Size(rowWidth, y);
            Clip = new RectangleGeometry { Rect = new Rect(new Point(), size) };
            return size;
        }
        #endregion

        void BuildSpanGraph()
        {
            CachedSpanGraph.Reset();
            SheetSpanModelBase spanModel = GetSpanModel();
            if (spanModel == null || spanModel.IsEmpty())
                return;

            int rowStart = -1;
            int rowEnd = -1;
            int columnStart = -1;
            int columnEnd = -1;
            switch (Area)
            {
                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    rowStart = Excel.GetViewportTopRow(RowViewportIndex);
                    rowEnd = Excel.GetViewportBottomRow(RowViewportIndex);
                    columnStart = 0;
                    columnEnd = Excel.ActiveSheet.RowHeader.ColumnCount - 1;
                    break;

                case SheetArea.ColumnHeader:
                    rowStart = 0;
                    rowEnd = Excel.ActiveSheet.ColumnHeader.RowCount - 1;
                    columnStart = Excel.GetViewportLeftColumn(ColumnViewportIndex);
                    columnEnd = Excel.GetViewportRightColumn(ColumnViewportIndex);
                    break;
            }
            if ((rowStart <= rowEnd) && (columnStart <= columnEnd))
            {
                int num5 = -1;
                for (int i = rowStart - 1; i > -1; i--)
                {
                    if (Excel.ActiveSheet.GetActualRowVisible(i, Area))
                    {
                        num5 = i;
                        break;
                    }
                }
                rowStart = num5;
                int count = GetDataContext().Rows.Count;
                for (int j = rowEnd + 1; j < count; j++)
                {
                    if (Excel.ActiveSheet.GetActualRowVisible(j, Area))
                    {
                        rowEnd = j;
                        break;
                    }
                }
                int num9 = -1;
                for (int k = columnStart - 1; k > -1; k--)
                {
                    if (Excel.ActiveSheet.GetActualColumnVisible(k, Area))
                    {
                        num9 = k;
                        break;
                    }
                }
                columnStart = num9;
                int num11 = GetDataContext().Columns.Count;
                for (int m = columnEnd + 1; m < num11; m++)
                {
                    if (Excel.ActiveSheet.GetActualColumnVisible(m, Area))
                    {
                        columnEnd = m;
                        break;
                    }
                }
                CachedSpanGraph.BuildGraph(columnStart, columnEnd, rowStart, rowEnd, GetSpanModel(), CellCache);
            }
        }

        SheetSpanModelBase GetSpanModel()
        {
            return (Area == SheetArea.ColumnHeader) ? Excel.ActiveSheet.ColumnHeaderSpanModel : Excel.ActiveSheet.RowHeaderSpanModel;
        }
    }
}

