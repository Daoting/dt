#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the data for RowsSwapped event during sorting.
    /// </summary>
    public class RowsSwappedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.RowsSwappedEventArgs" /> class.
        /// </summary>
        /// <param name="row1">The first row that is swapped during sorting.</param>
        /// <param name="row2">The second row that is swapped during sorting.</param>
        public RowsSwappedEventArgs(int row1, int row2)
        {
            this.Row1 = row1;
            this.Row2 = row2;
        }

        /// <summary>
        /// Gets the first row that is swapped during sorting.
        /// </summary>
        /// <value>
        /// The row1.
        /// </value>
        public int Row1 { get; private set; }

        /// <summary>
        /// Gets the second row that is swapped during sorting.
        /// </summary>
        /// <value>
        /// The row2.
        /// </value>
        public int Row2 { get; private set; }
    }
}

