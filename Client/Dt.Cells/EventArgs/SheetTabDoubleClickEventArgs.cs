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
    /// Represents the event data for the SheetTabDoubleClick event for the GcSpreadSheet component; occurs when the user double-clicks on a sheet name tab.
    /// </summary>
    public class SheetTabDoubleClickEventArgs : EventArgs
    {
        internal SheetTabDoubleClickEventArgs(int sheetTabIndex)
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

