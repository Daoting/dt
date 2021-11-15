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
    /// Represents the data for ColumnSwapped event during sorting.
    /// </summary>
    public class ColumnsSwappedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ColumnsSwappedEventArgs" /> class.
        /// </summary>
        /// <param name="column1">The first column that is swapped during sorting.</param>
        /// <param name="column2">The second column that is swapped during sorting.</param>
        public ColumnsSwappedEventArgs(int column1, int column2)
        {
            this.Column1 = column1;
            this.Column2 = column2;
        }

        /// <summary>
        /// Gets the first column that is swapped during sorting.
        /// </summary>
        /// <value>
        /// The column1.
        /// </value>
        public int Column1 { get; private set; }

        /// <summary>
        /// Gets the second column that is swapped during sorting.
        /// </summary>
        /// <value>
        /// The column2.
        /// </value>
        public int Column2 { get; private set; }
    }
}

