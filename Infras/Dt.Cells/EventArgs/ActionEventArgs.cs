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
    /// Provides data for spread action related events.
    /// </summary>
    public class ActionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ActionEventArgs" /> class.
        /// </summary>
        public ActionEventArgs()
        {
            Handled = false;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the action is performed successfully. The default is <c>false</c>.
        /// </summary>
        public bool Handled { get; set; }
    }
}

