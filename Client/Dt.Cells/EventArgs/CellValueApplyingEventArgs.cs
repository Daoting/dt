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
    public class CellValueApplyingEventArgs : CellEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CellValueApplyingEventArgs" /> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        public CellValueApplyingEventArgs(int row, int column, object value) : base(row, column)
        {
            CellValue = value;
        }

        /// <summary>
        /// Gets or sets the cell value.
        /// </summary>
        /// <value>
        /// The cell value.
        /// </value>
        public object CellValue { get; set; }
    }
}

