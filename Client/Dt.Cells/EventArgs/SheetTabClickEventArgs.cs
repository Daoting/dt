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
    /// Represents the event data for the SheetTabClick event for the GcSpreadSheet component; occurs when the user clicks the mouse button with the pointer on the sheet name tab. 
    /// </summary>
    public class SheetTabClickEventArgs : EventArgs
    {
        internal SheetTabClickEventArgs(int sheetTabIndex)
        {
            SheetTabIndex = sheetTabIndex;
        }

        /// <summary>
        /// Gets the index of the sheet tab that is clicked.
        /// </summary>
        /// <value>The index of the sheet tab that is clicked.</value>
        public int SheetTabIndex { get; private set; }
    }
}

