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
    /// Represents the event data for the RowViewportHeightChanged event for the GcSpreadSheet component, which occurs when the height of a viewport row has changed. 
    /// </summary>
    public class RowViewportHeightChangedEventArgs : EventArgs
    {
        internal RowViewportHeightChangedEventArgs(int viewportIndex, double deltaViewportHeight)
        {
            ViewportIndex = viewportIndex;
            DeltaViewportHeight = deltaViewportHeight;
        }

        /// <summary>
        /// Gets the changed value of the viewport row height.
        /// </summary>
        /// <value>The changed value of the viewport row height.</value>
        public double DeltaViewportHeight { get; private set; }

        /// <summary>
        /// Gets the index of the viewport row whose height has changed.
        /// </summary>
        /// <value>The index of the viewport row whose height has changed.</value>
        public int ViewportIndex { get; private set; }
    }
}

