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
    internal class SpreadLayout : SheetLayout
    {
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

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="rowViewportCount">The row viewport count.</param>
        /// <param name="columnViewportCount">The column viewport count.</param>
        public SpreadLayout(int rowViewportCount, int columnViewportCount)
        {
            _rowViewportCount = rowViewportCount;
            _columnViewportCount = columnViewportCount;
            base._viewportX = new double[ColumnPaneCount];
            base._viewportWidth = new double[ColumnPaneCount];
            _horizontalScrollBarX = new double[ColumnPaneCount];
            _horizontalScrollBarWidth = new double[ColumnPaneCount];
            _horizontalSplitBoxX = new double[ColumnPaneCount];
            _horizontalSplitBoxWidth = new double[ColumnPaneCount];
            _horizontalSplitBarX = new double[Math.Max(ColumnPaneCount - 1, 0)];
            _horizontalSplitBarWidth = new double[Math.Max(ColumnPaneCount - 1, 0)];
            base._viewportY = new double[RowPaneCount];
            base._viewportHeight = new double[RowPaneCount];
            _verticalScrollBarY = new double[RowPaneCount];
            _verticalScrollBarHeight = new double[RowPaneCount];
            _verticalSplitBoxY = new double[RowPaneCount];
            _verticalSplitBoxHeight = new double[RowPaneCount];
            _verticalSplitBarY = new double[Math.Max(RowPaneCount - 1, 0)];
            _verticalSplitBarHeight = new double[Math.Max(RowPaneCount - 1, 0)];
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

        /// <summary>
        /// Gets the column pane count.
        /// </summary>
        public int ColumnPaneCount
        {
            get { return  _columnViewportCount; }
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
            get { return  _rowViewportCount; }
        }

        public double TabSplitBoxWidth { get; set; }

        public double TabSplitBoxX { get; set; }

        public double TabStripHeight { get; set; }

        public double TabStripWidth { get; set; }

        public double TabStripX { get; set; }

        public double TabStripY { get; set; }
    }
}

