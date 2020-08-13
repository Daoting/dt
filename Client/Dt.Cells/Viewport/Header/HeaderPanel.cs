#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal abstract partial class HeaderPanel : Panel
    {
        const int _normalZIndexBase = 10000;
        const int _spanRowZIndexBase = 20000;
        HashSet<HeaderItem> _cachedChildren;
        Dictionary<int, HeaderItem> _rows;
        SpanGraph _cachedSpanGraph;
        readonly List<HeaderItem> _recycledRows;

        public HeaderPanel(SheetView p_sheet, SheetArea p_sheetArea)
        {
            Area = p_sheetArea;
            Sheet = p_sheet;
            _rows = new Dictionary<int, HeaderItem>();
            _cachedChildren = new HashSet<HeaderItem>();
            _cachedSpanGraph = new SpanGraph();
            CellCache = new CellCachePool(GetDataContext);
            _recycledRows = new List<HeaderItem>();

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public SheetView Sheet { get; }

        public SheetArea Area { get; }

        public Point Location { get; internal set; }

        public int RowViewportIndex { get; internal set; }

        public int ColumnViewportIndex { get; internal set; }

        public CellCachePool CellCache { get; }

        public SpanGraph CachedSpanGraph
        {
            get { return _cachedSpanGraph; }
        }

        public Rect GetRangeBounds(CellRange range, SheetArea area)
        {
            SheetSpanModel spanModel = null;
            if (Sheet.ActiveSheet != null)
            {
                switch (area)
                {
                    case SheetArea.Cells:
                        spanModel = Sheet.ActiveSheet.SpanModel;
                        break;

                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                        spanModel = Sheet.ActiveSheet.RowHeaderSpanModel;
                        break;

                    case SheetArea.ColumnHeader:
                        spanModel = Sheet.ActiveSheet.ColumnHeaderSpanModel;
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
            RowLayoutModel rowLayoutModel = Sheet.GetRowLayoutModel(RowViewportIndex, area);
            ColumnLayoutModel columnLayoutModel = Sheet.GetColumnLayoutModel(ColumnViewportIndex, area);
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
            return (Area == SheetArea.ColumnHeader) ? Sheet.GetColumnHeaderRowLayoutModel() : Sheet.GetViewportRowLayoutModel(RowViewportIndex);
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
            return (Area == SheetArea.ColumnHeader) ? (ICellsSupport)Sheet.ActiveSheet.ColumnHeader : Sheet.ActiveSheet.RowHeader;
        }

        public Size GetViewportSize()
        {
            return GetViewportSize(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        public Size GetViewportSize(Size availableSize)
        {
            SheetLayout sheetLayout = Sheet.GetSheetLayout();
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
                Sheet.GetColumnHeaderCellLayoutModel(ColumnViewportIndex) : Sheet.GetRowHeaderCellLayoutModel(RowViewportIndex);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            BuildSpanGraph();

            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            Dictionary<int, HeaderItem> rows = _rows;
            _rows = new Dictionary<int, HeaderItem>();
            foreach (var rowItem in rows.Values)
            {
                if ((rowLayoutModel.FindRow(rowItem.Row) == null) && !TryRecycleRow(rowItem))
                {
                    if (_cachedChildren.Remove(rowItem))
                    {
                        Children.Remove(rowItem);
                        rows.Remove(rowItem.Row);
                    }
                }
            }

            double x = Location.X;
            double y = Location.Y;
            double maxWidth = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height < 0.0)
                    continue;

                HeaderItem element = null;
                int row = layout.Row;
                if (rows.TryGetValue(row, out element))
                {
                    rows.Remove(row);
                    if (layout.Height > 0.0)
                    {
                        element.UpdateDisplayedCells();
                    }
                }
                else
                {
                    element = GetNewRowWithRecyclingSupport(row);
                    if (layout.Height > 0.0)
                    {
                        if (!_cachedChildren.Contains(element))
                        {
                            Children.Add(element);
                            _cachedChildren.Add(element);
                            element.UpdateDisplayedCells();
                        }
                        else
                        {
                            element.UpdateDisplayedCells(true);
                        }
                    }
                }

                if (layout.Height > 0.0)
                {
                    int num7 = _normalZIndexBase + element.Row;
                    if (element.ContainsSpanCell)
                    {
                        num7 = _spanRowZIndexBase + element.Row;
                    }
                    num7 = num7 % 32776;
                    Canvas.SetZIndex(element, num7);
                    _rows.Add(row, element);
                    element.Location = new Point(x, y);
                    element.Measure(new Size(availableSize.Width, layout.Height));
                    y += layout.Height;
                    maxWidth = Math.Max(maxWidth, element.DesiredSize.Width);
                }
                else
                {
                    if (_cachedChildren.Remove(element))
                    {
                        Children.Remove(element);
                    }
                    TryRecycleRow(element);
                }
            }

            foreach (HeaderItem rowItem in _recycledRows)
            {
                if (_cachedChildren.Remove(rowItem))
                {
                    Children.Remove(rowItem);
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
                if (layout.Height < 0.0)
                    continue;

                double width = finalSize.Width;
                double height = layout.Height;
                if (_rows.ContainsKey(layout.Row))
                {
                    HeaderItem rowItem = _rows[layout.Row];
                    rowItem.Arrange(new Rect(new Point(0.0, y), new Size(width, height)));
                    if (rowWidth == 0.0)
                    {
                        rowWidth = rowItem.RowWidth;
                    }
                }
                y += height;
            }

            rowWidth = Math.Min(GetViewportSize().Width, rowWidth);
            Size size = new Size(rowWidth, y);
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = new Rect(new Point(0.0, 0.0), size);
            Clip = geometry;
            return size;
        }

        HeaderItem GetNewRowWithRecyclingSupport(int rowIndex)
        {
            HeaderItem recycledRow = GetRecycledRow();
            if (recycledRow == null)
            {
                recycledRow = (Area == SheetArea.ColumnHeader) ? (HeaderItem)new ColHeaderItem(this) : new RowHeaderItem(this);
            }
            recycledRow.Row = rowIndex;
            return recycledRow;
        }

        HeaderItem GetRecycledRow()
        {
            HeaderItem item = null;
            if (_recycledRows.Count > 0)
            {
                item = _recycledRows[0];
                _recycledRows.RemoveAt(0);
            }
            return item;
        }

        bool TryRecycleRow(HeaderItem objRow)
        {
            _recycledRows.Add(objRow);
            objRow.CellsDirty = true;
            return true;
        }

        void BuildSpanGraph()
        {
            _cachedSpanGraph.Reset();
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
                    rowStart = Sheet.GetViewportTopRow(RowViewportIndex);
                    rowEnd = Sheet.GetViewportBottomRow(RowViewportIndex);
                    columnStart = 0;
                    columnEnd = Sheet.ActiveSheet.RowHeader.ColumnCount - 1;
                    break;

                case SheetArea.ColumnHeader:
                    rowStart = 0;
                    rowEnd = Sheet.ActiveSheet.ColumnHeader.RowCount - 1;
                    columnStart = Sheet.GetViewportLeftColumn(ColumnViewportIndex);
                    columnEnd = Sheet.GetViewportRightColumn(ColumnViewportIndex);
                    break;
            }
            if ((rowStart <= rowEnd) && (columnStart <= columnEnd))
            {
                int num5 = -1;
                for (int i = rowStart - 1; i > -1; i--)
                {
                    if (Sheet.ActiveSheet.GetActualRowVisible(i, Area))
                    {
                        num5 = i;
                        break;
                    }
                }
                rowStart = num5;
                int count = GetDataContext().Rows.Count;
                for (int j = rowEnd + 1; j < count; j++)
                {
                    if (Sheet.ActiveSheet.GetActualRowVisible(j, Area))
                    {
                        rowEnd = j;
                        break;
                    }
                }
                int num9 = -1;
                for (int k = columnStart - 1; k > -1; k--)
                {
                    if (Sheet.ActiveSheet.GetActualColumnVisible(k, Area))
                    {
                        num9 = k;
                        break;
                    }
                }
                columnStart = num9;
                int num11 = GetDataContext().Columns.Count;
                for (int m = columnEnd + 1; m < num11; m++)
                {
                    if (Sheet.ActiveSheet.GetActualColumnVisible(m, Area))
                    {
                        columnEnd = m;
                        break;
                    }
                }
                _cachedSpanGraph.BuildGraph(columnStart, columnEnd, rowStart, rowEnd, GetSpanModel(), CellCache);
            }
        }

        SheetSpanModelBase GetSpanModel()
        {
            return (Area == SheetArea.ColumnHeader) ? Sheet.ActiveSheet.ColumnHeaderSpanModel : Sheet.ActiveSheet.RowHeaderSpanModel;
        }
    }
}

