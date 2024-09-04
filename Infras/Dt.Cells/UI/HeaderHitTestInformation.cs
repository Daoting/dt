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
    /// Represents hit test information for the headers of the spreadsheet.
    /// </summary>
    public class HeaderHitTestInformation
    {
        /// <summary>
        /// Creates a new set of header hit test information.
        /// </summary>
        internal HeaderHitTestInformation()
        {
        }

        /// <summary>
        /// Gets the column location referred to in the header.
        /// </summary>
        public int Column { get; internal set; }

        /// <summary>
        /// Gets whether the location refers to a column resize area.
        /// </summary>
        public bool InColumnResize { get; internal set; }

        /// <summary>
        /// Gets whether the location refers to a row resize area.
        /// </summary>
        public bool InRowResize { get; internal set; }

        /// <summary>
        /// Gets the column being resized, if the InColumnResize property is true.
        /// </summary>
        public int ResizingColumn { get; internal set; }

        /// <summary>
        /// Gets the row being resized, if the InRowResize property is true.
        /// </summary>
        public int ResizingRow { get; internal set; }

        /// <summary>
        /// Gets the row location referred to in the header.
        /// </summary>
        public int Row { get; internal set; }
    }
}

