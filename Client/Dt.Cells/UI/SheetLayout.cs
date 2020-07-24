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
    }
}

