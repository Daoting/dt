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
    /// Represents a row's layout information.
    /// </summary>
    internal class RowLayout
    {
        /// <summary>
        /// Initializes a new instance of the RowLayout class.
        /// </summary>
        /// <param name="row">
        /// The index of the row whose information is represented.
        /// </param>
        /// <param name="y">
        /// The Y coordinate of the top side of the row.
        /// </param>
        /// <param name="height">
        /// The height of the row.
        /// </param>
        public RowLayout(int row, double y, double height)
        {
            Row = row;
            Y = y;
            Height = height;
        }

        /// <summary>
        /// Indicates whether the row represented by the <see cref="T:RowLayout" /> object 
        /// contains the specified Y coordinate.
        /// </summary>
        /// <param name="y">
        /// The specified Y coordinate.
        /// </param>
        /// <returns>
        /// Returns true if the row contains the specified Y coordinate; otherwise, returns false.
        /// </returns>
        public bool ContainsY(double y)
        {
            return ((y >= Y) && (y < (Y + Height)));
        }

        /// <summary>
        /// Indicates whether the row represented by the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.RowLayout" /> object 
        /// intersects with the specified <see cref="T:System.Windows.Rect" /> object.
        /// </summary>
        /// <param name="rect">
        /// The specified <see cref="T:Rect" /> object.
        /// </param>
        /// <returns>
        /// Returns true if the row intersects with the <see cref="T:System.Windows.Rect" /> object; otherwise, returns false.
        /// </returns>
        public bool IntersectsWith(Rect rect)
        {
            return (((Height > 0.0) && ((rect.Y + rect.Height) > Y)) && ((Y + Height) > rect.Y));
        }

        /// <summary>
        /// Gets the row height.
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// Gets the index of the row whose layout information is represented.
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// Gets the Y coordinate of the top side of the row.
        /// </summary>
        public double Y { get; private set; }
    }
}

