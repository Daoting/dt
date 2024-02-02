#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Specifies the area in the sheet.
    /// </summary>
    [Flags]
    public enum SheetArea : byte
    {
        /// <summary>
        /// Indicates the cells area.
        /// </summary>
        Cells = 1,
        /// <summary>
        /// Indicates the column header area.
        /// </summary>
        ColumnHeader = 4,
        /// <summary>
        /// Indicates a corner area.
        /// </summary>
        CornerHeader = 0,
        /// <summary>
        /// Indicates the row header area.
        /// </summary>
        RowHeader = 2
    }
}

