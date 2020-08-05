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
    /// Represents an individual <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpread" /> row.
    /// </summary>
    internal partial class RowPresenter : Panel
    {
        CellPresenterBase _headingOverflowCell;
        GcViewport _owningPresenter;
        List<CellPresenterBase> _recycledCells;
        int _row;
        double _rowWidth;
        CellPresenterBase _trailingOverflowCell;
        const int _FlowCellZIndexBase = 0x7530;
        const int _NormalCellZIndexBase = 0x2710;
        const int _SpanCellZIndexBase = 0x4e20;

        public RowPresenter(GcViewport viewport)
        {
            _row = -1;
            Cells = new Dictionary<int, CellPresenterBase>();
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Background = new SolidColorBrush(Colors.Transparent);
            _owningPresenter = viewport;
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            RowLayoutModel rowLayoutModel = OwningPresenter.GetRowLayoutModel();
            CellOverflowLayoutModel cellOverflowLayoutModel = OwningPresenter.GetCellOverflowLayoutModel(Row);
            RowLayout layout = rowLayoutModel.FindRow(Row);
            if (layout != null)
            {
                foreach (ColumnLayout layout2 in columnLayoutModel)
                {
                    if (layout2.Width > 0.0)
                    {
                        CellPresenterBase cell = GetCell(layout2.Column);
                        if (cell != null)
                        {
                            CellLayout cellLayout = cell.CellLayout;
                            double num = layout2.X;
                            double num2 = layout.Y;
                            double num3 = layout2.Width;
                            double height = layout.Height;
                            int num5 = 0x2710 + layout2.Column;
                            if (cellLayout != null)
                            {
                                num = cellLayout.X;
                                num2 = cellLayout.Y;
                                num3 = cellLayout.Width;
                                height = cellLayout.Height;
                                num5 = 0x4e20 + layout2.Column;
                            }
                            if (cell.CellOverflowLayout != null)
                            {
                                num5 = 0x7530 + layout2.Column;
                            }
                            num5 = num5 % 0x7ffe;
                            Canvas.SetZIndex(cell, num5);
                            cell.Arrange(new Windows.Foundation.Rect(PointToClient(new Windows.Foundation.Point(num, num2)), new Windows.Foundation.Size(num3, height)));
                        }
                    }
                }
                if (cellOverflowLayoutModel == null)
                {
                    return finalSize;
                }
                Worksheet worksheet = OwningPresenter.Sheet.Worksheet;
                float zoomFactor = OwningPresenter.Sheet.ZoomFactor;
                if (cellOverflowLayoutModel.HeadingOverflowlayout != null)
                {
                    double num7 = Location.X;
                    double num8 = layout.Y;
                    for (int j = cellOverflowLayoutModel.HeadingOverflowlayout.Column; j < columnLayoutModel[0].Column; j++)
                    {
                        double actualColumnWidth = worksheet.GetActualColumnWidth(j, OwningPresenter.SheetArea);
                        num7 -= actualColumnWidth * zoomFactor;
                    }
                    double num12 = worksheet.GetActualColumnWidth(cellOverflowLayoutModel.HeadingOverflowlayout.Column, OwningPresenter.SheetArea) * zoomFactor;
                    Windows.Foundation.Size size = new Windows.Foundation.Size(num12, layout.Height);
                    Windows.Foundation.Rect rect = new Windows.Foundation.Rect(PointToClient(new Windows.Foundation.Point(num7, num8)), size);
                    int num13 = 0x7530 + cellOverflowLayoutModel.HeadingOverflowlayout.Column;
                    num13 = num13 % 0x7ffe;
                    Canvas.SetZIndex(HeadingOverflowCell, num13);
                    HeadingOverflowCell.Arrange(rect);
                }
                if (cellOverflowLayoutModel.TrailingOverflowlayout == null)
                {
                    return finalSize;
                }
                double x = Location.X;
                double y = layout.Y;
                ColumnLayout layout4 = columnLayoutModel[columnLayoutModel.Count - 1];
                if (layout4 == null)
                {
                    return finalSize;
                }
                x = layout4.X;
                for (int i = layout4.Column; i < cellOverflowLayoutModel.TrailingOverflowlayout.Column; i++)
                {
                    x += worksheet.GetActualColumnWidth(i, OwningPresenter.SheetArea) * zoomFactor;
                }
                double width = worksheet.GetActualColumnWidth(cellOverflowLayoutModel.TrailingOverflowlayout.Column, OwningPresenter.SheetArea) * zoomFactor;
                Windows.Foundation.Size size2 = new Windows.Foundation.Size(width, layout.Height);
                Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(PointToClient(new Windows.Foundation.Point(x, y)), size2);
                int num18 = 0x7530 + cellOverflowLayoutModel.TrailingOverflowlayout.Column;
                num18 = num18 % 0x7ffe;
                Canvas.SetZIndex(TrailingOverflowCell, num18);
                TrailingOverflowCell.Arrange(rect2);
            }
            return finalSize;
        }

        internal void CleanUpBeforeDiscard()
        {
            if (Cells != null)
            {
                using (Dictionary<int, CellPresenterBase>.ValueCollection.Enumerator enumerator = Cells.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.CleanUpBeforeDiscard();
                    }
                }
            }
        }

        internal CellRange ClipCellRange(CellRange source)
        {
            RowLayoutModel rowLayoutModel = OwningPresenter.GetRowLayoutModel();
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            ICellsSupport dataContext = OwningPresenter.GetDataContext();
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

        protected virtual CellPresenterBase GenerateNewCell()
        {
            return new CellPresenter();
        }

        public virtual CellPresenterBase GetCell(int column)
        {
            CellPresenterBase base2 = null;
            Cells.TryGetValue(column, out base2);
            return base2;
        }

        protected virtual SheetSpanModelBase GetCellSpanModel()
        {
            return OwningPresenter.Sheet.Worksheet.SpanModel;
        }

        public virtual ColumnLayoutModel GetColumnLayoutModel()
        {
            return OwningPresenter.Sheet.GetViewportColumnLayoutModel(OwningPresenter.ColumnViewportIndex);
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            if (OwningPresenter == null)
            {
                return base.MeasureOverride(availableSize);
            }
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                return Windows.Foundation.Size.Empty;
            }
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            RowLayout layout = OwningPresenter.GetRowLayoutModel().FindRow(Row);
            CellOverflowLayoutModel cellOverflowLayoutModel = OwningPresenter.GetCellOverflowLayoutModel(Row);
            double height = layout.Height;
            _rowWidth = 0.0;
            foreach (ColumnLayout layout2 in columnLayoutModel)
            {
                double num2 = layout2.Width;
                double num3 = layout.Height;
                double num4 = num2;
                double num5 = num3;
                CellPresenterBase cell = GetCell(layout2.Column);
                if (cell != null)
                {
                    CellLayout cellLayout = cell.CellLayout;
                    if (cellLayout != null)
                    {
                        num4 = cellLayout.Width;
                        num5 = cellLayout.Height;
                    }
                    CellOverflowLayout cellOverflowLayout = null;
                    bool flag = false;
                    if ((cellLayout == null) && (cellOverflowLayoutModel != null))
                    {
                        cellOverflowLayout = cellOverflowLayoutModel.GetCellOverflowLayout(layout2.Column);
                        if ((cellOverflowLayout != null) && (cellOverflowLayout.Column == layout2.Column))
                        {
                            cell.CellOverflowLayout = cellOverflowLayout;
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        cell.CellOverflowLayout = null;
                    }
                    cell.Measure(new Windows.Foundation.Size(num4, num5));
                }
                _rowWidth += num2;
            }
            CellsDirty = false;
            double width = Math.Min(RowWidth, OwningPresenter.GetViewportSize().Width);
            if (cellOverflowLayoutModel != null)
            {
                Worksheet worksheet = OwningPresenter.Sheet.Worksheet;
                float zoomFactor = OwningPresenter.Sheet.ZoomFactor;
                if (cellOverflowLayoutModel.HeadingOverflowlayout != null)
                {
                    HeadingOverflowCell.CellOverflowLayout = cellOverflowLayoutModel.HeadingOverflowlayout;
                    if ((OwningPresenter != null) && OwningPresenter.IsCurrentEditingCell(HeadingOverflowCell.BindingCell.Row.Index, HeadingOverflowCell.BindingCell.Column.Index))
                    {
                        HeadingOverflowCell.HideForEditing();
                    }
                    double num8 = worksheet.GetActualColumnWidth(cellOverflowLayoutModel.HeadingOverflowlayout.Column, OwningPresenter.SheetArea) * zoomFactor;
                    Windows.Foundation.Size size = new Windows.Foundation.Size(num8, layout.Height);
                    HeadingOverflowCell.Measure(size);
                }
                if (cellOverflowLayoutModel.TrailingOverflowlayout != null)
                {
                    TrailingOverflowCell.CellOverflowLayout = cellOverflowLayoutModel.TrailingOverflowlayout;
                    double num9 = worksheet.GetActualColumnWidth(cellOverflowLayoutModel.TrailingOverflowlayout.Column, OwningPresenter.SheetArea) * zoomFactor;
                    Windows.Foundation.Size size2 = new Windows.Foundation.Size(num9, layout.Height);
                    TrailingOverflowCell.Measure(size2);
                }
            }
            return new Windows.Foundation.Size(width, height);
        }

        public Windows.Foundation.Point PointToClient(Windows.Foundation.Point point)
        {
            return new Windows.Foundation.Point(point.X - Location.X, point.Y - Location.Y);
        }

        internal void UpdateDisplayedCells(bool forceUpdate = false)
        {
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            List<CellPresenterBase> list = new List<CellPresenterBase>();
            List<CellPresenterBase> list2 = new List<CellPresenterBase>();
            foreach (KeyValuePair<int, CellPresenterBase> pair in Cells)
            {
                if (!pair.Value.IsRecylable)
                {
                    list2.Add(pair.Value);
                }
                else
                {
                    CellPresenterBase cell = GetCell(pair.Key);
                    ColumnLayout layout = columnLayoutModel.FindColumn(cell.Column);
                    if ((layout == null) || (layout.Width == 0.0))
                    {
                        list.Add(cell);
                    }
                }
            }
            foreach (CellPresenterBase base3 in list2)
            {
                Cells.Remove(base3.Column);
                base.Children.Remove(base3);
                base3.CleanUpBeforeDiscard();
            }
            CellLayoutModel cellLayoutModel = null;
            SpanGraph cachedSpanGraph = OwningPresenter.CachedSpanGraph;
            ContainsSpanCell = false;
            for (int i = 0; i < columnLayoutModel.Count; i++)
            {
                ColumnLayout layout2 = columnLayoutModel[i];
                if (layout2.Width > 0.0)
                {
                    int column = layout2.Column;
                    CellPresenterBase base4 = null;
                    base4 = GetCell(column);
                    CellLayout layout3 = null;
                    byte state = cachedSpanGraph.GetState(Row, layout2.Column);
                    bool flag = false;
                    if (state > 0)
                    {
                        if (cellLayoutModel == null)
                        {
                            cellLayoutModel = OwningPresenter.GetCellLayoutModel();
                        }
                        if (cellLayoutModel != null)
                        {
                            layout3 = cellLayoutModel.FindCell(Row, layout2.Column);
                        }
                    }
                    if (((layout3 != null) && (layout3.Width > 0.0)) && (layout3.Height > 0.0))
                    {
                        CellRange range = ClipCellRange(layout3.GetCellRange());
                        flag = (Row != range.Row) || (range.Column != layout2.Column);
                        if (layout3.ColumnCount > 1)
                        {
                            int num4 = (layout3.Column + layout3.ColumnCount) - 1;
                            for (int j = i + 1; j < columnLayoutModel.Count; j++)
                            {
                                int num6 = columnLayoutModel[j].Column;
                                if (num6 > num4)
                                {
                                    break;
                                }
                                i = j;
                                CellPresenterBase base5 = GetCell(num6);
                                if ((base5 != null) && Cells.Remove(num6))
                                {
                                    base.Children.Remove(base5);
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        if (base4 != null)
                        {
                            Cells.Remove(base4.Column);
                            base.Children.Remove(base4);
                        }
                    }
                    else
                    {
                        bool flag2 = false;
                        if ((base4 == null) && !flag)
                        {
                            if ((list.Count > 0) || (RecycledCells.Count > 0))
                            {
                                if (list.Count > 0)
                                {
                                    int num7 = list.Count - 1;
                                    base4 = list[num7];
                                    list.RemoveAt(num7);
                                }
                                else
                                {
                                    int num8 = RecycledCells.Count - 1;
                                    base4 = RecycledCells[num8];
                                    RecycledCells.RemoveAt(num8);
                                }
                                Cells.Remove(base4.Column);
                                base4.Column = layout2.Column;
                                base4.InvalidateMeasure();
                            }
                            else
                            {
                                base4 = GenerateNewCell();
                                base4.Column = layout2.Column;
                            }
                            if (base4.Parent != this)
                            {
                                base.Children.Add(base4);
                            }
                            Cells.Add(base4.Column, base4);
                            flag2 = true;
                        }
                        bool flag3 = false;
                        if (((_owningPresenter.SheetArea == SheetArea.ColumnHeader) && (_owningPresenter.Sheet.ResizeZeroIndicator == ResizeZeroIndicator.Enhanced)) && (_owningPresenter.Sheet.Worksheet.GetActualColumnWidth(layout2.Column, SheetArea.Cells) == 0.0))
                        {
                            if (base4.ShowContent)
                            {
                                base4.ShowContent = false;
                                flag3 = true;
                            }
                        }
                        else if (((_owningPresenter.SheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader)) && (_owningPresenter.Sheet.ResizeZeroIndicator == ResizeZeroIndicator.Enhanced)) && (_owningPresenter.Sheet.Worksheet.GetActualRowHeight(Row, SheetArea.Cells) == 0.0))
                        {
                            if (base4.ShowContent)
                            {
                                base4.ShowContent = false;
                                flag3 = true;
                            }
                        }
                        else if (((OwningPresenter.SheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader)) || (OwningPresenter.SheetArea == SheetArea.ColumnHeader)) && !base4.ShowContent)
                        {
                            base4.ShowContent = true;
                            flag3 = true;
                        }
                        base4.CellLayout = layout3;
                        if (layout3 != null)
                        {
                            ContainsSpanCell = true;
                        }
                        base4.OwningRow = this;
                        base4.UpdateBindingCell();
                        if ((CellsDirty || forceUpdate) || (flag2 || flag3))
                        {
                            if (base4.ShowContent)
                            {
                                base4.SetContentVisible(true);
                            }
                            base4.Reset();
                            base4.InvalidateMeasure();
                            base4.InvalidateArrange();
                            base.InvalidateMeasure();
                        }
                        if (base4.TryUpdateVisualTree())
                        {
                            base4.InvalidateMeasure();
                        }
                    }
                }
            }
            CellsDirty = false;
            foreach (CellPresenterBase base6 in list)
            {
                Cells.Remove(base6.Column);
                base.Children.Remove(base6);
            }
            if (OwningPresenter.SupportCellOverflow)
            {
                CellOverflowLayoutModel cellOverflowLayoutModel = OwningPresenter.GetCellOverflowLayoutModel(Row);
                if ((cellOverflowLayoutModel != null) && !cellOverflowLayoutModel.IsEmpty)
                {
                    if (cellOverflowLayoutModel.HeadingOverflowlayout != null)
                    {
                        CellPresenterBase headingOverflowCell = HeadingOverflowCell;
                        if (headingOverflowCell == null)
                        {
                            headingOverflowCell = GenerateNewCell();
                            headingOverflowCell.OwningRow = this;
                        }
                        HeadingOverflowCell = headingOverflowCell;
                        int num9 = cellOverflowLayoutModel.HeadingOverflowlayout.Column;
                        if (headingOverflowCell.Column != num9)
                        {
                            headingOverflowCell.Column = num9;
                            headingOverflowCell.Reset();
                        }
                    }
                    else
                    {
                        HeadingOverflowCell = null;
                    }
                    if (cellOverflowLayoutModel.TrailingOverflowlayout != null)
                    {
                        CellPresenterBase trailingOverflowCell = TrailingOverflowCell;
                        if (trailingOverflowCell == null)
                        {
                            trailingOverflowCell = GenerateNewCell();
                            trailingOverflowCell.OwningRow = this;
                        }
                        TrailingOverflowCell = trailingOverflowCell;
                        int num10 = cellOverflowLayoutModel.TrailingOverflowlayout.Column;
                        if (trailingOverflowCell.Column != num10)
                        {
                            trailingOverflowCell.Column = num10;
                            trailingOverflowCell.Reset();
                        }
                    }
                    else
                    {
                        TrailingOverflowCell = null;
                    }
                    base.InvalidateMeasure();
                }
                else
                {
                    HeadingOverflowCell = null;
                    TrailingOverflowCell = null;
                }
            }
        }

        internal bool AllCellsAreRecyclable
        {
            get
            {
                foreach (CellPresenterBase base2 in Cells.Values)
                {
                    if ((base2 != null) && !base2.IsRecylable)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        internal Dictionary<int, CellPresenterBase> Cells { get; private set; }

        internal bool CellsDirty { get; set; }

        internal bool ContainsSpanCell { get; set; }

        internal CellPresenterBase HeadingOverflowCell
        {
            get { return  _headingOverflowCell; }
            set
            {
                if (_headingOverflowCell != value)
                {
                    if (_headingOverflowCell != null)
                    {
                        base.Children.Remove(_headingOverflowCell);
                    }
                    _headingOverflowCell = value;
                    if (value != null)
                    {
                        base.Children.Add(value);
                    }
                    base.InvalidateMeasure();
                }
            }
        }

        public bool IsRecyclable
        {
            get { return  AllCellsAreRecyclable; }
        }

        public Windows.Foundation.Point Location { get; set; }

        public virtual GcViewport OwningPresenter
        {
            get { return  _owningPresenter; }
            set { _owningPresenter = value; }
        }

        protected virtual List<CellPresenterBase> RecycledCells
        {
            get
            {
                if (_recycledCells == null)
                {
                    _recycledCells = new List<CellPresenterBase>();
                }
                return _recycledCells;
            }
        }

        public int Row
        {
            get { return  _row; }
            set
            {
                if (_row != value)
                {
                    _row = value;
                    base.InvalidateMeasure();
                }
            }
        }

        internal double RowWidth
        {
            get { return  _rowWidth; }
        }

        internal CellPresenterBase TrailingOverflowCell
        {
            get { return  _trailingOverflowCell; }
            set
            {
                if (_trailingOverflowCell != value)
                {
                    if (_trailingOverflowCell != null)
                    {
                        base.Children.Remove(_trailingOverflowCell);
                    }
                    _trailingOverflowCell = value;
                    if (value != null)
                    {
                        base.Children.Add(value);
                    }
                    base.InvalidateMeasure();
                }
            }
        }
    }
}

