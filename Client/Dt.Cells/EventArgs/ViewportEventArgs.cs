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
    /// Represents the event data for the TopRowChanged or LeftColumnChanged event in the GcSpreadSheet component; occurs when the top row view or the left column view changes. 
    /// </summary>
    public class ViewportEventArgs : EventArgs
    {
        internal ViewportEventArgs(int oldIndex, int newIndex, int viewportIndex)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
            ViewportIndex = viewportIndex;
        }

        /// <summary>
        /// Gets the index of the new viewport index.
        /// </summary>
        /// <value>The index of the new viewport index.</value>
        public int NewIndex { get; private set; }

        /// <summary>
        /// Gets the index of the previous viewport index.
        /// </summary>
        /// <value>The index of the previous viewport index.</value>
        public int OldIndex { get; private set; }

        /// <summary>
        /// Gets the index of the viewport in which the change occurred. 
        /// </summary>
        /// <value>The index of the viewport in which the change occurred.</value>
        public int ViewportIndex { get; private set; }
    }
}

