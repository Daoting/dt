#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal partial class RowsPanel : Panel
    {
        HashSet<RowPresenter> _cachedChildren;
        internal int _normalZIndexBase;
        Dictionary<int, RowPresenter> _rows;
        internal int _spanRowZIndexBase;

        public RowsPanel()
        {
            _normalZIndexBase = 0x2710;
            _spanRowZIndexBase = 0x4e20;
            _rows = new Dictionary<int, RowPresenter>();
            _cachedChildren = new HashSet<RowPresenter>();
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Background = new SolidColorBrush(Colors.White);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (ParentViewport.SupportCellOverflow && ParentViewport.Sheet.CanCellOverflow)
            {
                int viewportLeftColumn = ParentViewport.Sheet.GetViewportLeftColumn(ParentViewport.ColumnViewportIndex);
                ParentViewport.CellOverflowLayoutBuildEngine.ViewportLeftColumn = viewportLeftColumn;
                int viewportRightColumn = ParentViewport.Sheet.GetViewportRightColumn(ParentViewport.ColumnViewportIndex);
                ParentViewport.CellOverflowLayoutBuildEngine.ViewportRightColumn = viewportRightColumn;
            }
            double x = ParentViewport.Location.X;
            double y = ParentViewport.Location.Y;
            RowLayoutModel rowLayoutModel = ParentViewport.GetRowLayoutModel();
            Dictionary<int, RowPresenter> rows = _rows;
            _rows = new Dictionary<int, RowPresenter>();
            foreach (RowPresenter presenter in Enumerable.ToArray<RowPresenter>((IEnumerable<RowPresenter>)rows.Values))
            {
                if ((rowLayoutModel.FindRow(presenter.Row) == null) && !TryRecycleRow(presenter))
                {
                    presenter.CleanUpBeforeDiscard();
                    if (_cachedChildren.Remove(presenter))
                    {
                        base.Children.Remove(presenter);
                        rows.Remove(presenter.Row);
                    }
                }
            }

            double num5 = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height >= 0.0)
                {
                    RowPresenter element = null;
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
                                base.Children.Add(element);
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
                        num7 = num7 % 0x7ffe;
                        Canvas.SetZIndex(element, num7);
                        _rows.Add(row, element);
                        element.Location = new Windows.Foundation.Point(x, y);
                        element.Measure(new Size(availableSize.Width, layout.Height));
                        y += layout.Height;
                        num5 = Math.Max(num5, element.DesiredSize.Width);
                    }
                    else
                    {
                        if (_cachedChildren.Remove(element))
                        {
                            base.Children.Remove(element);
                        }
                        TryRecycleRow(element);
                    }
                }
            }
            foreach (RowPresenter presenter3 in RecycledRows)
            {
                if (_cachedChildren.Remove(presenter3))
                {
                    base.Children.Remove(presenter3);
                    foreach (CellPresenterBase base2 in presenter3.Children)
                    {
                        base2.RemoveInvalidDataPresenter();
                    }
                }
            }
            rows.Clear();
            return new Size(num5 + ParentViewport.Location.X, y);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            RowLayoutModel rowLayoutModel = ParentViewport.GetRowLayoutModel();
            double y = 0.0;
            double rowWidth = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height >= 0.0)
                {
                    double width = finalSize.Width;
                    double height = layout.Height;
                    if (_rows.ContainsKey(layout.Row))
                    {
                        RowPresenter presenter = _rows[layout.Row];
                        presenter.Arrange(new Rect(new Windows.Foundation.Point(0.0, y), new Size(width, height)));
                        if (rowWidth == 0.0)
                        {
                            rowWidth = presenter.RowWidth;
                        }
                    }
                    y += height;
                }
            }
            rowWidth = Math.Min(ParentViewport.GetViewportSize().Width, rowWidth);
            Size size = new Size(rowWidth, y);
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = new Rect(new Windows.Foundation.Point(0.0, 0.0), size);
            base.Clip = geometry;
            return size;
        }

        RowPresenter GetNewRowWithRecyclingSupport(int rowIndex)
        {
            RowPresenter recycledRow = null;
            recycledRow = GetRecycledRow();
            if (recycledRow == null)
            {
                recycledRow = ParentViewport.GenerateNewRow();
            }
            recycledRow.Row = rowIndex;
            recycledRow.OwningPresenter = ParentViewport;
            return recycledRow;
        }

        internal RowPresenter GetRecycledRow()
        {
            RowPresenter presenter = null;
            while ((RecycledRows.Count > 0) && (presenter == null))
            {
                RowPresenter presenter2 = RecycledRows[0];
                if (presenter2 != null)
                {
                    RecycledRows.Remove(presenter2);
                    if (presenter2.IsRecyclable)
                    {
                        presenter = presenter2;
                    }
                }
            }
            return presenter;
        }

        internal RowPresenter GetRow(int row)
        {
            RowPresenter presenter = null;
            _rows.TryGetValue(row, out presenter);
            return presenter;
        }

        bool TryRecycleRow(RowPresenter objRow)
        {
            if (objRow.IsRecyclable)
            {
                RecycledRows.Add(objRow);
                objRow.CellsDirty = true;
                return true;
            }
            return false;
        }

        public GcViewport ParentViewport { get; set; }

        internal List<RowPresenter> RecycledRows
        {
            get { return ParentViewport.RecycledRows; }
        }

        internal List<RowPresenter> Rows
        {
            get { return new List<RowPresenter>((IEnumerable<RowPresenter>)_rows.Values); }
        }
    }
}

