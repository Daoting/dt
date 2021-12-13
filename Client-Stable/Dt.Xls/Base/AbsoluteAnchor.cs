#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents an absolute anchor
    /// </summary>
    public class AbsoluteAnchor : IAnchor
    {
        /// <summary>
        /// Specifies a height of the absolute anchor
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Specifies a width of the absolute anchor
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Specifies a coordinate on the x-axis.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Specifies a coordinate on the y-axis.
        /// </summary>
        public double Y { get; set; }
    }
}

