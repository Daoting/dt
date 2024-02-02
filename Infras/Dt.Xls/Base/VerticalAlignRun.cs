#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Specifies the type of vertical alignment applied to the contents of the current run.
    /// </summary>
    public enum VerticalAlignRun : byte
    {
        /// <summary>
        /// Specifies that the text in the parent run shall be
        /// located at the baseline and presented in the same size
        /// as surrounding text.
        /// </summary>
        BaseLine = 0,
        /// <summary>
        /// Specifies that this text should be subscript.
        /// </summary>
        /// <remarks>
        /// This setting shall raise the text in this run above the
        /// baseline and change it to a smaller size, if a smaller
        /// size is available.
        /// </remarks>
        Subscript = 2,
        /// <summary>
        /// Specifies that this text should be subscript.
        /// </summary>
        /// <remarks>
        /// This setting shall raise the text in this run above the
        /// baseline and change it to a smaller size, if a smaller
        /// size is available.
        /// </remarks>
        Superscript = 1
    }
}

