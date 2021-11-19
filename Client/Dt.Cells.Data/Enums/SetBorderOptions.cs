#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Specifies the options for the border setting.
    /// </summary>
    [Flags]
    public enum SetBorderOptions
    {
        /// <summary>
        /// Specifies all directions for the border.
        /// </summary>
        All = 0x3f,
        /// <summary>
        /// Specifies the bottom border.
        /// </summary>
        Bottom = 8,
        /// <summary>
        /// Specifies the horizontal border.
        /// </summary>
        InnerHorizontal = 0x20,
        /// <summary>
        /// Specifies the vertical border.
        /// </summary>
        InnerVertical = 0x10,
        /// <summary>
        /// Specifies the inner horizontal and vertical borders.
        /// </summary>
        Inside = 0x30,
        /// <summary>
        /// Specifies the right left border.
        /// </summary>
        Left = 2,
        /// <summary>
        /// Specifies the left, top, right, and bottom borders.
        /// </summary>
        OutLine = 15,
        /// <summary>
        /// Specifies the right border.
        /// </summary>
        Right = 4,
        /// <summary>
        /// Specifies the top border.
        /// </summary>
        Top = 1
    }
}

