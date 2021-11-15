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
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a column's layout information.
    /// </summary>
    internal class ColumnLayout
    {
        /// <summary>
        /// Initializes a new instance of the ColumnLayout class.
        /// </summary>
        /// <param name="column">
        /// The index of the column whose layout information is represented.
        /// </param>
        /// <param name="x">
        /// The X coordinate of the left side of the column.
        /// </param>
        /// <param name="width">
        /// The width of the column.
        /// </param>
        public ColumnLayout(int column, double x, double width)
        {
            Column = column;
            X = x;
            Width = width;
        }

        /// <summary>
        /// Indicates whether the column represented by the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.ColumnLayout" /> 
        /// contains the specified X coordinate.
        /// </summary>
        /// <param name="x">
        /// The specified X coordinate.
        /// </param>
        /// <returns>
        /// Returns true if the specified X coordinate is contained, otherwise, returns false.
        /// </returns>
        public bool ContainsX(double x)
        {
            return ((X <= x) && (x < (X + Width)));
        }

        /// <summary>
        /// Indicates whether the column represented by the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.ColumnLayout" /> 
        /// intersects with the specified <see cref="T:System.Windows.Rect" />.
        /// </summary>
        /// <param name="rect">
        /// The specified <see cref="T:System.Rect" />.
        /// </param>
        /// <returns>
        /// Returns true if the column represented by the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.ColumnLayout" /> 
        /// intersects with the specified rectangle, otherwise, returns false.
        /// </returns>
        public bool IntersectsWith(Rect rect)
        {
            return (((Width > 0.0) && ((rect.X + rect.Width) > X)) && (rect.X < (X + Width)));
        }

        /// <summary>
        /// Gets the index of the column whose layout information is represented.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the width of the column.
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// Gets the X coordinate of the left side of the column.
        /// </summary>
        public double X { get; private set; }
    }
}

