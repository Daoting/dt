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
        #region 成员变量
        const int _normalZIndexBase = 10000;
        const int _spanRowZIndexBase = 20000;
        static Rect _rcEmpty = new Rect();
        static Size _szEmpty = new Size();
        readonly CellsPanel _owner;
        readonly Dictionary<int, RowItem> _rows;
        readonly List<RowItem> _recycledRows;
        #endregion

        #region 构造方法
        public RowsLayer(CellsPanel p_owner)
        {
            _owner = p_owner;
            _rows = new Dictionary<int, RowItem>();
            _recycledRows = new List<RowItem>();
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Background = BrushRes.WhiteBrush;
        }
        #endregion

        #region 属性
        public IEnumerable<RowItem> Rows
        {
            get { return _rows.Values; }
        }

        public RowItem GetRow(int row)
        {
            if (_rows.TryGetValue(row, out var item))
                return item;
            return null;
        }
        #endregion

        #region 测量布局
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_owner.Excel.CanCellOverflow)
            {
                int viewportLeftColumn = _owner.Excel.GetViewportLeftColumn(_owner.ColumnViewportIndex);
                _owner.CellOverflowLayoutBuildEngine.ViewportLeftColumn = viewportLeftColumn;
                int viewportRightColumn = _owner.Excel.GetViewportRightColumn(_owner.ColumnViewportIndex);
                _owner.CellOverflowLayoutBuildEngine.ViewportRightColumn = viewportRightColumn;
            }

            // 频繁增删Children子元素会出现卡顿现象！
            // Children = _rows + _recycledRows
            RowLayoutModel rowLayoutModel = _owner.GetRowLayoutModel();
            int less = rowLayoutModel.Count - Children.Count;
            if (less > 0)
            {
                for (int i = 0; i < less; i++)
                {
                    RowItem rowItem = new RowItem(_owner);
                    Children.Add(rowItem);
                    _recycledRows.Add(rowItem);
                }
            }

            // 先回收不可见行
            List<RowItem> rows = _rows.Values.ToList();
            foreach (var rowItem in rows)
            {
                RowLayout layout = rowLayoutModel.FindRow(rowItem.Row);
                if (layout == null || layout.Height <= 0.0)
                {
                    _recycledRows.Add(rowItem);
                    _rows.Remove(rowItem.Row);
                    rowItem.Row = -1;
                    rowItem.CleanUpBeforeDiscard();
                }
            }

            double y = _owner.Location.Y;
            double left = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height <= 0.0)
                    continue;

                bool updateAllCell = false;
                RowItem rowItem = null;
                if (!_rows.TryGetValue(layout.Row, out rowItem))
                {
                    // 重新利用回收的行
                    rowItem = _recycledRows[0];
                    _recycledRows.RemoveAt(0);
                    rowItem.Row = layout.Row;
                    _rows.Add(layout.Row, rowItem);
                    updateAllCell = true;
                }
                rowItem.Location = new Point(_owner.Location.X, y);
                rowItem.UpdateChildren(updateAllCell);

                int z = rowItem.ContainsSpanCell ? _spanRowZIndexBase + rowItem.Row : _normalZIndexBase + rowItem.Row;
                z = z % 0x7ffe;
                Canvas.SetZIndex(rowItem, z);

                // 测量尺寸足够大，否则当单元格占多行时在uno上只绘一行！
                rowItem.Measure(availableSize);
                y += layout.Height;
                left = Math.Max(left, rowItem.DesiredSize.Width);
            }

            // 测量回收的行
            if (_recycledRows.Count > 0)
            {
                foreach (var rowItem in _recycledRows)
                {
                    rowItem.Measure(_szEmpty);
                }
            }
            return new Size(left + _owner.Location.X, y);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            RowLayoutModel rowLayoutModel = _owner.GetRowLayoutModel();
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

            rowWidth = Math.Min(_owner.GetViewportSize().Width, rowWidth);
            Size size = new Size(rowWidth, y);
            Clip = new RectangleGeometry { Rect = new Rect(new Point(), size) };
            return size;
        }
        #endregion
    }
}

