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
    /// Represents the action delegate for input maps.
    /// </summary>
    /// <param name="sender">The object to do the action on.</param>
    /// <param name="e">The <see cref="T:ActionEventArgs" /> instance that contains the action event data.</param>
    public delegate void SpreadAction(object sender, ActionEventArgs e);
}

