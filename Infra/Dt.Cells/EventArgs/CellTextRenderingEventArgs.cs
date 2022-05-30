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
    /// 
    /// </summary>
    public class CellTextRenderingEventArgs : CellEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CellTextRenderingEventArgs" /> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="text">The text.</param>
        public CellTextRenderingEventArgs(int row, int column, string text) : base(row, column)
        {
            CellText = text;
        }

        /// <summary>
        /// Gets or sets the cell text.
        /// </summary>
        /// <value>
        /// The cell text.
        /// </value>
        public string CellText { get; set; }
    }
}

