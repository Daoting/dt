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
    /// Represents the event data for the ColumnViewportWidthChanged event for the GcSpreadSheet component, which occurs when the width of a viewport column has changed. 
    /// </summary>
    public class ColumnViewportWidthChangedEventArgs : EventArgs
    {
        internal ColumnViewportWidthChangedEventArgs(int viewportIndex, double deltaViewportWidth)
        {
            ViewportIndex = viewportIndex;
            DeltaViewportWidth = deltaViewportWidth;
        }

        /// <summary>
        /// Gets the changed value for the viewport column width.
        /// </summary>
        /// <value>The changed value of the viewport column width.</value>
        public double DeltaViewportWidth { get; private set; }

        /// <summary>
        /// Gets the index of the viewport column whose width has changed.
        /// </summary>
        /// <value>The index of the viewport column whose width has changed.</value>
        public int ViewportIndex { get; private set; }
    }
}

