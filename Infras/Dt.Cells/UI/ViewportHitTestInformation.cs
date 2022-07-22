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
    /// Represents hit test information for the viewports of the spreadsheet.
    /// </summary>
    public class ViewportHitTestInformation
    {
        /// <summary>
        /// Creates a new set of viewport hit test information.
        /// </summary>
        internal ViewportHitTestInformation()
        {
        }

        /// <summary>
        /// Gets the column location referred to in the viewport.
        /// </summary>
        public int Column { get; internal set; }

        /// <summary>
        /// Gets whether the location refers to a drag fill area.
        /// </summary>
        public bool InDragFillIndicator { get; internal set; }

        /// <summary>
        /// Gets whether the location refers to an editor area when the sheet is in edit mode.
        /// </summary>
        public bool InEditor { get; internal set; }

        /// <summary>
        /// Gets whether the location refers to a drag drop area.
        /// </summary>
        public bool InSelectionDrag { get; internal set; }

        /// <summary>
        /// Gets the row location referred to in the viewport.
        /// </summary>
        public int Row { get; internal set; }
    }
}

