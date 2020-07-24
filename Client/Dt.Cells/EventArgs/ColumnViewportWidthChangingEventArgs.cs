#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the event data for the ColumnViewportWidthChanging event for the GcSpreadSheet component, which occurs when the width of a viewport column is changing. 
    /// </summary>
    public class ColumnViewportWidthChangingEventArgs : CancelEventArgs
    {
        internal ColumnViewportWidthChangingEventArgs(int viewportIndex, double deltaViewportWidth)
        {
            ViewportIndex = viewportIndex;
            DeltaViewportWidth = deltaViewportWidth;
        }

        internal ColumnViewportWidthChangingEventArgs(int viewportIndex, double deltaViewportWidth, bool cancel) : this(viewportIndex, deltaViewportWidth)
        {
            base.Cancel = cancel;
        }

        /// <summary>
        /// Gets the changed value for the viewport column width.
        /// </summary>
        /// <value>The changed value of the viewport column width.</value>
        public double DeltaViewportWidth { get; private set; }

        /// <summary>
        /// Gets the index of the viewport column whose width is changing.
        /// </summary>
        /// <value>The index of the viewport column whose width is changing.</value>
        public int ViewportIndex { get; private set; }
    }
}

