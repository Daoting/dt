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
        private int _columnViewportCount;
        private double[] _horizontalScrollBarWidth;
        private double[] _horizontalScrollBarX;
        private double[] _horizontalSplitBarWidth;
        private double[] _horizontalSplitBarX;
        private double[] _horizontalSplitBoxWidth;
        private double[] _horizontalSplitBoxX;
        private int _rowViewportCount;
        private double[] _verticalScrollBarHeight;
        private double[] _verticalScrollBarY;
        private double[] _verticalSplitBarHeight;
        private double[] _verticalSplitBarY;
        private double[] _verticalSplitBoxHeight;
        private double[] _verticalSplitBoxY;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="rowViewportCount">The row viewport count.</param>
        /// <param name="columnViewportCount">The column viewport count.</param>
        public SpreadLayout(int rowViewportCount, int columnViewportCount)
        {
            this._rowViewportCount = rowViewportCount;
            this._columnViewportCount = columnViewportCount;
            base._viewportX = new double[this.ColumnPaneCount];
            base._viewportWidth = new double[this.ColumnPaneCount];
            this._horizontalScrollBarX = new double[this.ColumnPaneCount];
            this._horizontalScrollBarWidth = new double[this.ColumnPaneCount];
            this._horizontalSplitBoxX = new double[this.ColumnPaneCount];
            this._horizontalSplitBoxWidth = new double[this.ColumnPaneCount];
            this._horizontalSplitBarX = new double[Math.Max(this.ColumnPaneCount - 1, 0)];
            this._horizontalSplitBarWidth = new double[Math.Max(this.ColumnPaneCount - 1, 0)];
            base._viewportY = new double[this.RowPaneCount];
            base._viewportHeight = new double[this.RowPaneCount];
            this._verticalScrollBarY = new double[this.RowPaneCount];
            this._verticalScrollBarHeight = new double[this.RowPaneCount];
            this._verticalSplitBoxY = new double[this.RowPaneCount];
            this._verticalSplitBoxHeight = new double[this.RowPaneCount];
            this._verticalSplitBarY = new double[Math.Max(this.RowPaneCount - 1, 0)];
            this._verticalSplitBarHeight = new double[Math.Max(this.RowPaneCount - 1, 0)];
        }

        public double GetHorizontalScrollBarWidth(int column)
        {
            if ((column < 0) || (column >= this.ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            return this._horizontalScrollBarWidth[column];
        }

        public double GetHorizontalScrollBarX(int column)
        {
            if ((column < 0) || (column >= this.ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            return this._horizontalScrollBarX[column];
        }

        public double GetHorizontalSplitBarWidth(int columnViewportIndex)
        {
            if ((columnViewportIndex < 0) || (columnViewportIndex >= (this.ColumnPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            return this._horizontalSplitBarWidth[columnViewportIndex];
        }

        public double GetHorizontalSplitBarX(int columnViewportIndex)
        {
            if ((columnViewportIndex < 0) || (columnViewportIndex >= (this.ColumnPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            return this._horizontalSplitBarX[columnViewportIndex];
        }

        public double GetHorizontalSplitBoxWidth(int column)
        {
            if ((column < 0) || (column >= this.ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            return this._horizontalSplitBoxWidth[column];
        }

        public double GetHorizontalSplitBoxX(int column)
        {
            if ((column < 0) || (column >= this.ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            return this._horizontalSplitBoxX[column];
        }

        public double GetVerticalScrollBarHeight(int row)
        {
            if ((row < 0) || (row >= this.RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            return this._verticalScrollBarHeight[row];
        }

        public double GetVerticalScrollBarY(int row)
        {
            if ((row < 0) || (row >= this.RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            return this._verticalScrollBarY[row];
        }

        public double GetVerticalSplitBarHeight(int rowViewportIndex)
        {
            if ((rowViewportIndex < 0) || (rowViewportIndex >= (this.RowPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            return this._verticalSplitBarHeight[rowViewportIndex];
        }

        public double GetVerticalSplitBarY(int rowViewportIndex)
        {
            if ((rowViewportIndex < 0) || (rowViewportIndex >= (this.RowPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            return this._verticalSplitBarY[rowViewportIndex];
        }

        public double GetVerticalSplitBoxHeight(int row)
        {
            if ((row < 0) || (row >= this.RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            return this._verticalSplitBoxHeight[row];
        }

        public double GetVerticalSplitBoxY(int row)
        {
            if ((row < 0) || (row >= this.RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            return this._verticalSplitBoxY[row];
        }

        public void SetHorizontalScrollBarWidth(int column, double value)
        {
            if ((column < 0) || (column >= this.ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            this._horizontalScrollBarWidth[column] = value;
        }

        public void SetHorizontalScrollBarX(int column, double value)
        {
            if ((column < 0) || (column >= this.ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            this._horizontalScrollBarX[column] = value;
        }

        public void SetHorizontalSplitBarWidth(int columnViewportIndex, double value)
        {
            if ((columnViewportIndex < 0) || (columnViewportIndex >= (this.ColumnPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            this._horizontalSplitBarWidth[columnViewportIndex] = value;
        }

        public void SetHorizontalSplitBarX(int columnViewportIndex, double value)
        {
            if ((columnViewportIndex < 0) || (columnViewportIndex >= (this.ColumnPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("columnViewportIndex");
            }
            this._horizontalSplitBarX[columnViewportIndex] = value;
        }

        public void SetHorizontalSplitBoxWidth(int column, double value)
        {
            if ((column < 0) || (column >= this.ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            this._horizontalSplitBoxWidth[column] = value;
        }

        public void SetHorizontalSplitBoxX(int column, double value)
        {
            if ((column < 0) || (column >= this.ColumnPaneCount))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            this._horizontalSplitBoxX[column] = value;
        }

        public void SetVerticalScrollBarHeight(int row, double value)
        {
            if ((row < 0) || (row >= this.RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            this._verticalScrollBarHeight[row] = value;
        }

        public void SetVerticalScrollBarY(int row, double value)
        {
            if ((row < 0) || (row >= this.RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            this._verticalScrollBarY[row] = value;
        }

        public void SetVerticalSplitBarHeight(int rowViewportIndex, double value)
        {
            if ((rowViewportIndex < 0) || (rowViewportIndex >= (this.RowPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            this._verticalSplitBarHeight[rowViewportIndex] = value;
        }

        public void SetVerticalSplitBarY(int rowViewportIndex, double value)
        {
            if ((rowViewportIndex < 0) || (rowViewportIndex >= (this.RowPaneCount - 1)))
            {
                throw new ArgumentOutOfRangeException("rowViewportIndex");
            }
            this._verticalSplitBarY[rowViewportIndex] = value;
        }

        public void SetVerticalSplitBoxHeight(int row, double value)
        {
            if ((row < 0) || (row >= this.RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            this._verticalSplitBoxHeight[row] = value;
        }

        public void SetVerticalSplitBoxY(int row, double value)
        {
            if ((row < 0) || (row >= this.RowPaneCount))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            this._verticalSplitBoxY[row] = value;
        }

        /// <summary>
        /// Gets the column pane count.
        /// </summary>
        public int ColumnPaneCount
        {
            get { return  this._columnViewportCount; }
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
            get { return  this._rowViewportCount; }
        }

        public double TabSplitBoxWidth { get; set; }

        public double TabSplitBoxX { get; set; }

        public double TabStripHeight { get; set; }

        public double TabStripWidth { get; set; }

        public double TabStripX { get; set; }

        public double TabStripY { get; set; }
    }
}

