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
    /// Represents a skipped cell.
    /// </summary>
    public class CellSkipper
    {
        /// <summary>
        /// the handler of skip
        /// </summary>
        SkipHandler skipHandler;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.Data.CellSkipper" /> class.
        /// </summary>
        /// <param name="skipHandler">Handler of the skipped row.</param>
        public CellSkipper(SkipHandler skipHandler)
        {
            this.skipHandler = skipHandler;
        }

        /// <summary>
        /// Returns a value that indicates whether to skip the cell.
        /// </summary>
        /// <param name="worksheet">The row of the sheet.</param>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="column">The column index of the cell.</param>
        /// <returns>
        /// Returns a value that indicates whether the cell was skipped.
        /// </returns>
        public virtual bool Skip(Worksheet worksheet, int row, int column)
        {
            return ((this.skipHandler != null) && this.skipHandler(worksheet, row, column));
        }

        /// <summary>
        /// The skip handler.
        /// </summary>
        public delegate bool SkipHandler(Worksheet worksheet, int row, int column);
    }
}

