#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Specifies how the chart element is placed on the chart
    /// </summary>
    internal class ManualLayout
    {
        /// <summary>
        /// Specifies the height (if height mode is Factor) or bottom (if height mode is edge) of the chart
        /// element as a fraction of the height of the chart.
        /// </summary>
        internal double Height { get; set; }

        /// <summary>
        /// Specifies how to interpret the height element for this layout
        /// </summary>
        internal LayoutMode HeightMode { get; set; }

        /// <summary>
        /// Specifies the x location(left) of the chart element as a fraction of the width of the chart
        /// If Left mode is Factor, then the position is relative to the default position for the chart element
        /// </summary>
        internal double Left { get; set; }

        /// <summary>
        /// Specifies how to interpret the left element for this layout
        /// </summary>
        internal LayoutMode LeftMode { get; set; }

        /// <summary>
        /// Specifies the layout target
        /// </summary>
        internal LayoutTarget Target { get; set; }

        /// <summary>
        /// Specifies the top of the chart element as a fraction of the height of the chart. If Top mode is factor
        /// the the position is relative to the default position for the chart element
        /// </summary>
        internal double Top { get; set; }

        /// <summary>
        /// Specifies how to interpret the top element for this layout
        /// </summary>
        internal LayoutMode TopMode { get; set; }

        /// <summary>
        /// Specifies the width (if width mode is Factor) or right (if width mode is edge) of the chart
        /// element as a fraction of the width of the chart.
        /// </summary>
        internal double Width { get; set; }

        /// <summary>
        /// Specifies how to interpret the width element for this layout
        /// </summary>
        internal LayoutMode WidthMode { get; set; }
    }
}

