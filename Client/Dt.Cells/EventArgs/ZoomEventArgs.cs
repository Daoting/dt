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
    /// Represents the event data for the UserZooming event for the GcSpreadSheet component; occurs when the user zooms. 
    /// </summary>
    public class ZoomEventArgs : EventArgs
    {
        internal ZoomEventArgs(float oldZoomFactor, float newZoomFactor)
        {
            OldZoomFactor = oldZoomFactor;
            NewZoomFactor = newZoomFactor;
        }

        /// <summary>
        /// Gets the new zoom factor.
        /// </summary>
        /// <value>The new zoom factor.</value>
        public float NewZoomFactor { get; private set; }

        /// <summary>
        /// Gets the old zoom factor.
        /// </summary>
        /// <value>The old zoom factor.</value>
        public float OldZoomFactor { get; private set; }
    }
}

