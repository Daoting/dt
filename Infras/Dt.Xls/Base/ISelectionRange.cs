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
    /// Represents a selection range
    /// </summary>
    public interface ISelectionRange : IRange
    {
        /// <summary>
        /// Gets or sets the type of the active pane.
        /// </summary>
        /// <value>The type of the active pane.</value>
        PaneType activePaneType { get; set; }
    }
}

