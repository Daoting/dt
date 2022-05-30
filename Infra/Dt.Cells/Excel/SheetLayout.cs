#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the layout information of a <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpread" /> control.
    /// </summary>
    internal class SheetLayout
    {
        double[] _viewportHeight = new double[1];
        double[] _viewportWidth = new double[1];
        double[] _viewportX = new double[1];
        double[] _viewportY = new double[1];
        int _columnViewportCount;
        double[] _horizontalScrollBarWidth;
        double[] _horizontalScrollBarX;
        double[] _horizontalSplitBarWidth;
        double[] _horizontalSplitBarX;
        double[] _horizontalSplitBoxWidth;
        double[] _horizontalSplitBoxX;
        int _rowViewportCount;
        double[] _verticalScrollBarHeight;
        double[] _verticalScrollBarY;
        double[] _verticalSplitBarHeight;
        double[] _verticalSplitBarY;
        double[] _verticalSplitBoxHeight;
        double[] _verticalSplitBoxY;

        public SheetLayout(int rowViewportCount, int columnViewportCount)
        {
            _rowViewportCount = rowViewportCount;
            _columnViewportCount = columnViewportCount;
            _viewportX = new double[ColumnPaneCount];
            _viewportWidth = new double[ColumnPaneCount];
            _horizontalScrollBarX = new double[ColumnPaneCount];
            _horizontalScrollBarWidth = new double[ColumnPaneCount];
            _horizontalSplitBoxX = new double[ColumnPaneCount];
            _horizontalSplitBoxWidth = new double[ColumnPaneCount];
            _horizontalSplitBarX = new double[Math.Max(ColumnPaneCount - 1, 0)];
            _horizontalSplitBarWidth = new double[Math.Max(ColumnPaneCount - 1, 0)];
            _viewportY = new double[RowPaneCount];
            _viewportHeight = new double[RowPaneCount];
            _verticalScrollBarY = new double[RowPaneCount];
            _verticalScrollBarHeight = new double[RowPaneCount];
            _verticalSplitBoxY = new double[RowPaneCount];
            _verticalSplitBoxHeight = new double[RowPaneCount];
            _verticalSplitBarY = new double[Math.Max(RowPaneCount - 1, 0)];
            _verticalSplitBarHeight = new double[Math.Max(RowPaneCount - 1, 0)];
        }

        public double GetViewportHeight(int row)
        {
            if (row == -1)
            {
                return FrozenHeight;
            }
            if ((row >= 0) && (row < _viewportHeight.Length))
            {
                return _viewportHeight[row];
            }
            if (row == _viewportHeight.Length)
            {
                return FrozenTrailingHeight;
            }
            return 0.0;
        }

        public double GetViewportWidth(int column)
        {
            if (column == -1)
            {
                return FrozenWidth;
            }
            if ((column >= 0) && (column < _viewportWidth.Length))
            {
                return _viewportWidth[column];
            }
            if (column == _viewportWidth.Length)
            {
                return FrozenTrailingWidth;
            }
            return 0.0;
        }

        public double GetViewportX(int column)
        {
            if (column == -1)
            {
                return FrozenX;
            }
            if ((column >= 0) && (column < _viewportX.Length))
            {
                return _viewportX[column];
            }
            if ((column == _viewportX.Length) && (_viewportX.Length > 0))
            {
                return (_viewportX[_viewportX.Length - 1] + _viewportWidth[_viewportX.Length - 1]);
            }
            return 0.0;
        }

        public double GetViewportY(int row)
        {
            if (row == -1)
            {
                return FrozenY;
            }
            if ((row >= 0) && (row < _viewportY.Length))
            {
                return _viewportY[row];
            }
            if ((row == _viewportY.Length) && (_viewportY.Length > 0))
            {
                return (_viewportY[_viewportY.Length - 1] + _viewportHeight[_viewportY.Length - 1]);
            }
            return 0.0;
        }

        public void SetViewportHeight(int row, double value)
        {
            if ((row < 0) && (row >= _viewportHeight.Length))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            _viewportHeight[row] = value;
        }

        public void SetViewportWidth(int column, double value)
        {
            if ((column < 0) || (column >= _viewportWidth.Length))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            _viewportWidth[column] = value;
        }

        public void SetViewportX(int column, double value)
        {
            if ((column < 0) || (column >= _viewportX.Length))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            _viewportX[column] = value;
        }

        public void SetViewportY(int row, double value)
        {
            if ((row < 0) || (row >= _viewportY.Length))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            _viewportY[row] = value;
        }

        public double GetHorizontalScrollBarWidth(int column)
        {
            if ((column < 0) || (column >= ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            return _horizontalScrollBarWidth[column];
        }

        public double GetHorizontalScrollBarX(int column)
        {
            if ((column < 0) || (column >= ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            return _horizontalScrollBarX[column];
        }

        public double GetHorizontalSplitBarWidth(int columnViewportIndex)
        {
            if ((columnViewportIndex < 0) || (columnViewportIndex >= (ColumnPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            return _horizontalSplitBarWidth[columnViewportIndex];
        }

        public double GetHorizontalSplitBarX(int columnViewportIndex)
        {
            if ((columnViewportIndex < 0) || (columnViewportIndex >= (ColumnPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            return _horizontalSplitBarX[columnViewportIndex];
        }

        public double GetHorizontalSplitBoxWidth(int column)
        {
            if ((column < 0) || (column >= ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            return _horizontalSplitBoxWidth[column];
        }

        public double GetHorizontalSplitBoxX(int column)
        {
            if ((column < 0) || (column >= ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            return _horizontalSplitBoxX[column];
        }

        public double GetVerticalScrollBarHeight(int row)
        {
            if ((row < 0) || (row >= RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            return _verticalScrollBarHeight[row];
        }

        public double GetVerticalScrollBarY(int row)
        {
            if ((row < 0) || (row >= RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            return _verticalScrollBarY[row];
        }

        public double GetVerticalSplitBarHeight(int rowViewportIndex)
        {
            if ((rowViewportIndex < 0) || (rowViewportIndex >= (RowPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            return _verticalSplitBarHeight[rowViewportIndex];
        }

        public double GetVerticalSplitBarY(int rowViewportIndex)
        {
            if ((rowViewportIndex < 0) || (rowViewportIndex >= (RowPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            return _verticalSplitBarY[rowViewportIndex];
        }

        public double GetVerticalSplitBoxHeight(int row)
        {
            if ((row < 0) || (row >= RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            return _verticalSplitBoxHeight[row];
        }

        public double GetVerticalSplitBoxY(int row)
        {
            if ((row < 0) || (row >= RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            return _verticalSplitBoxY[row];
        }

        public void SetHorizontalScrollBarWidth(int column, double value)
        {
            if ((column < 0) || (column >= ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            _horizontalScrollBarWidth[column] = value;
        }

        public void SetHorizontalScrollBarX(int column, double value)
        {
            if ((column < 0) || (column >= ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            _horizontalScrollBarX[column] = value;
        }

        public void SetHorizontalSplitBarWidth(int columnViewportIndex, double value)
        {
            if ((columnViewportIndex < 0) || (columnViewportIndex >= (ColumnPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            _horizontalSplitBarWidth[columnViewportIndex] = value;
        }

        public void SetHorizontalSplitBarX(int columnViewportIndex, double value)
        {
            if ((columnViewportIndex < 0) || (columnViewportIndex >= (ColumnPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            _horizontalSplitBarX[columnViewportIndex] = value;
        }

        public void SetHorizontalSplitBoxWidth(int column, double value)
        {
            if ((column < 0) || (column >= ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            _horizontalSplitBoxWidth[column] = value;
        }

        public void SetHorizontalSplitBoxX(int column, double value)
        {
            if ((column < 0) || (column >= ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            _horizontalSplitBoxX[column] = value;
        }

        public void SetVerticalScrollBarHeight(int row, double value)
        {
            if ((row < 0) || (row >= RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            _verticalScrollBarHeight[row] = value;
        }

        public void SetVerticalScrollBarY(int row, double value)
        {
            if ((row < 0) || (row >= RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            _verticalScrollBarY[row] = value;
        }

        public void SetVerticalSplitBarHeight(int rowViewportIndex, double value)
        {
            if ((rowViewportIndex < 0) || (rowViewportIndex >= (RowPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            _verticalSplitBarHeight[rowViewportIndex] = value;
        }

        public void SetVerticalSplitBarY(int rowViewportIndex, double value)
        {
            if ((rowViewportIndex < 0) || (rowViewportIndex >= (RowPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            _verticalSplitBarY[rowViewportIndex] = value;
        }

        public void SetVerticalSplitBoxHeight(int row, double value)
        {
            if ((row < 0) || (row >= RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            _verticalSplitBoxHeight[row] = value;
        }

        public void SetVerticalSplitBoxY(int row, double value)
        {
            if ((row < 0) || (row >= RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            _verticalSplitBoxY[row] = value;
        }

        public double FrozenHeight { get; set; }

        public double FrozenTrailingHeight { get; set; }

        public double FrozenTrailingWidth { get; set; }

        public double FrozenTrailingX
        {
            get { return  (_viewportX[_viewportX.Length - 1] + _viewportWidth[_viewportX.Length - 1]); }
        }

        public double FrozenTrailingY
        {
            get { return  (_viewportY[_viewportY.Length - 1] + _viewportHeight[_viewportY.Length - 1]); }
        }

        public double FrozenWidth { get; set; }

        public double FrozenX { get; set; }

        public double FrozenY { get; set; }

        public double HeaderHeight { get; set; }

        public double HeaderWidth { get; set; }

        public double HeaderX { get; set; }

        public double HeaderY { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        /// <summary>
        /// Gets the column pane count.
        /// </summary>
        public int ColumnPaneCount
        {
            get { return _columnViewportCount; }
        }

        public double OrnamentHeight { get; set; }

        public double OrnamentWidth { get; set; }

        public double OrnamentX { get; set; }

        public double OrnamentY { get; set; }

        /// <summary>
        /// Gets the row pane count.
        /// </summary>
        public int RowPaneCount
        {
            get { return _rowViewportCount; }
        }

        public double TabStripX { get; set; }

        public double TabStripY { get; set; }

        public double TabStripHeight { get; set; }

        public double TabStripWidth { get; set; }
    }
}

