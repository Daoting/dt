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
    /// Represents the event arguments for the TouchStripOpening event.
    /// </summary>
    public class TouchToolbarOpeningEventArgs : EventArgs
    {
        internal TouchToolbarOpeningEventArgs(int x, int y, TouchToolbarShowingArea area)
        {
            X = x;
            Y = y;
            Area = area;
        }

        /// <summary>
        /// Specifies the location of the touch strip pop up.
        /// </summary>
        /// <value>
        /// One of the <see cref="T:TouchToolbarShowingArea" /> values.
        /// </value>
        public TouchToolbarShowingArea Area { get; private set; }

        /// <summary>
        /// Gets or sets the horizontal tapped position.
        /// </summary>
        /// <value>
        /// An integer that indicates the horizontal tapped position.
        /// </value>
        public int X { get; private set; }

        /// <summary>
        /// Gets or sets the vertical tapped position.
        /// </summary>
        /// <value>
        /// An integer that indicates the vertical tapped position.
        /// </value>
        public int Y { get; private set; }
    }
}

