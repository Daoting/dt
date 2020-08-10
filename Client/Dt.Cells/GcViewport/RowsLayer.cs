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
    /// <summary>
    /// 按行布局的面板
    /// </summary>
    internal partial class RowsLayer : Panel
    {
        CellsPanel _owner;
        HashSet<RowItem> _cachedChildren;
        int _normalZIndexBase;
        Dictionary<int, RowItem> _rows;
        int _spanRowZIndexBase;

        public RowsLayer(CellsPanel p_owner)
        {
            _owner = p_owner;
            _normalZIndexBase = 0x2710;
            _spanRowZIndexBase = 0x4e20;
            _rows = new Dictionary<int, RowItem>();
            _cachedChildren = new HashSet<RowItem>();
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Background = new SolidColorBrush(Colors.White);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_owner.SupportCellOverflow && _owner.Sheet.Excel.CanCellOverflow)
            {
                int viewportLeftColumn = _owner.Sheet.GetViewportLeftColumn(_owner.ColumnViewportIndex);
                _owner.CellOverflowLayoutBuildEngine.ViewportLeftColumn = viewportLeftColumn;
                int viewportRightColumn = _owner.Sheet.GetViewportRightColumn(_owner.ColumnViewportIndex);
                _owner.CellOverflowLayoutBuildEngine.ViewportRightColumn = viewportRightColumn;
            }

            double x = _owner.Location.X;
            double y = _owner.Location.Y;
            RowLayoutModel rowLayoutModel = _owner.GetRowLayoutModel();
            Dictionary<int, RowItem> rows = _rows;
            _rows = new Dictionary<int, RowItem>();
            foreach (var presenter in rows.Values)
            {
                if ((rowLayoutModel.FindRow(presenter.Row) == null) && !TryRecycleRow(presenter))
                {
                    presenter.CleanUpBeforeDiscard();
                    if (_cachedChildren.Remove(presenter))
                    {
                        Children.Remove(presenter);
                        rows.Remove(presenter.Row);
                    }
                }
            }

            double num5 = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height < 0.0)
                    continue;

                RowItem element = null;
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
                    num7 = num7 % 0x7ffe;
                    Canvas.SetZIndex(element, num7);
                    _rows.Add(row, element);
                    element.Location = new Point(x, y);
                    element.Measure(new Size(availableSize.Width, layout.Height));
                    y += layout.Height;
                    num5 = Math.Max(num5, element.DesiredSize.Width);
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

            foreach (RowItem presenter3 in _owner.RecycledRows)
            {
                if (_cachedChildren.Remove(presenter3))
                {
                    Children.Remove(presenter3);
                    foreach (CellPresenterBase base2 in presenter3.Children)
                    {
                        base2.RemoveInvalidDataPresenter();
                    }
                }
            }
            rows.Clear();
            return new Size(num5 + _owner.Location.X, y);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            RowLayoutModel rowLayoutModel = _owner.GetRowLayoutModel();
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
                        RowItem presenter = _rows[layout.Row];
                        presenter.Arrange(new Rect(new Point(0.0, y), new Size(width, height)));
                        if (rowWidth == 0.0)
                        {
                            rowWidth = presenter.RowWidth;
                        }
                    }
                    y += height;
                }
            }
            rowWidth = Math.Min(_owner.GetViewportSize().Width, rowWidth);
            Size size = new Size(rowWidth, y);
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = new Rect(new Point(0.0, 0.0), size);
            base.Clip = geometry;
            return size;
        }

        RowItem GetNewRowWithRecyclingSupport(int rowIndex)
        {
            RowItem recycledRow = GetRecycledRow();
            if (recycledRow == null)
            {
                recycledRow = _owner.GenerateNewRow();
            }
            recycledRow.Row = rowIndex;
            recycledRow.OwningPresenter = _owner;
            return recycledRow;
        }

        RowItem GetRecycledRow()
        {
            RowItem presenter = null;
            while ((_owner.RecycledRows.Count > 0) && (presenter == null))
            {
                RowItem row = _owner.RecycledRows[0];
                if (row != null)
                {
                    _owner.RecycledRows.Remove(row);
                    if (row.IsRecyclable)
                    {
                        presenter = row;
                    }
                }
            }
            return presenter;
        }

        internal RowItem GetRow(int row)
        {
            RowItem presenter = null;
            _rows.TryGetValue(row, out presenter);
            return presenter;
        }

        bool TryRecycleRow(RowItem objRow)
        {
            if (objRow.IsRecyclable)
            {
                _owner.RecycledRows.Add(objRow);
                objRow.CellsDirty = true;
                return true;
            }
            return false;
        }

        internal IEnumerable<RowItem> Rows
        {
            get { return _rows.Values; }
        }
    }
}

