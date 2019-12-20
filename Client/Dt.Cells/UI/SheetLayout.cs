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
        protected double[] _viewportHeight = new double[1];
        protected double[] _viewportWidth = new double[1];
        protected double[] _viewportX = new double[1];
        protected double[] _viewportY = new double[1];

        public double GetViewportHeight(int row)
        {
            if (row == -1)
            {
                return this.FrozenHeight;
            }
            if ((row >= 0) && (row < this._viewportHeight.Length))
            {
                return this._viewportHeight[row];
            }
            if (row == this._viewportHeight.Length)
            {
                return this.FrozenTrailingHeight;
            }
            return 0.0;
        }

        public double GetViewportWidth(int column)
        {
            if (column == -1)
            {
                return this.FrozenWidth;
            }
            if ((column >= 0) && (column < this._viewportWidth.Length))
            {
                return this._viewportWidth[column];
            }
            if (column == this._viewportWidth.Length)
            {
                return this.FrozenTrailingWidth;
            }
            return 0.0;
        }

        public double GetViewportX(int column)
        {
            if (column == -1)
            {
                return this.FrozenX;
            }
            if ((column >= 0) && (column < this._viewportX.Length))
            {
                return this._viewportX[column];
            }
            if ((column == this._viewportX.Length) && (this._viewportX.Length > 0))
            {
                return (this._viewportX[this._viewportX.Length - 1] + this._viewportWidth[this._viewportX.Length - 1]);
            }
            return 0.0;
        }

        public double GetViewportY(int row)
        {
            if (row == -1)
            {
                return this.FrozenY;
            }
            if ((row >= 0) && (row < this._viewportY.Length))
            {
                return this._viewportY[row];
            }
            if ((row == this._viewportY.Length) && (this._viewportY.Length > 0))
            {
                return (this._viewportY[this._viewportY.Length - 1] + this._viewportHeight[this._viewportY.Length - 1]);
            }
            return 0.0;
        }

        public void SetViewportHeight(int row, double value)
        {
            if ((row < 0) && (row >= this._viewportHeight.Length))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            this._viewportHeight[row] = value;
        }

        public void SetViewportWidth(int column, double value)
        {
            if ((column < 0) || (column >= this._viewportWidth.Length))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            this._viewportWidth[column] = value;
        }

        public void SetViewportX(int column, double value)
        {
            if ((column < 0) || (column >= this._viewportX.Length))
            {
                throw new ArgumentOutOfRangeException("column");
            }
            this._viewportX[column] = value;
        }

        public void SetViewportY(int row, double value)
        {
            if ((row < 0) || (row >= this._viewportY.Length))
            {
                throw new ArgumentOutOfRangeException("row");
            }
            this._viewportY[row] = value;
        }

        public double FrozenHeight { get; set; }

        public double FrozenTrailingHeight { get; set; }

        public double FrozenTrailingWidth { get; set; }

        public double FrozenTrailingX
        {
            get { return  (this._viewportX[this._viewportX.Length - 1] + this._viewportWidth[this._viewportX.Length - 1]); }
        }

        public double FrozenTrailingY
        {
            get { return  (this._viewportY[this._viewportY.Length - 1] + this._viewportHeight[this._viewportY.Length - 1]); }
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
    }
}

