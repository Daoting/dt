#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents the client area of the window.
    /// </summary>
    public interface IExcelRect
    {
        /// <summary>
        /// Height of the window.
        /// </summary>
        /// <value>The height of the window.</value>
        double Height { get; set; }

        /// <summary>
        /// The horizontal position of the window. The value is relative to the logical left edge of the client area of the window.
        /// </summary>
        /// <value>The horizontal position of the window</value>
        double Left { get; set; }

        /// <summary>
        /// The vertical position of the window. The value is relative to the top edge of the client area of the window.
        /// </summary>
        /// <value>The vertical position of the window</value>
        double Top { get; set; }

        /// <summary>
        /// Width of the window.
        /// </summary>
        /// <value>The width of the window</value>
        double Width { get; set; }
    }
}

