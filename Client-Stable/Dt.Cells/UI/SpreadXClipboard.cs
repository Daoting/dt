#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    internal static class SpreadXClipboard
    {
        public static FloatingObject[] FloatingObjects { get; internal set; }

        /// <summary>
        /// Indicates whether the action is a cut.
        /// </summary>
        public static bool IsCutting { get; internal set; }

        /// <summary>
        /// Gets the copy or cut range.
        /// </summary>
        public static CellRange Range { get; internal set; }

        /// <summary>
        /// Gets the copy or cut worksheet.
        /// </summary>
        public static Dt.Cells.Data.Worksheet Worksheet { get; internal set; }
    }
}

