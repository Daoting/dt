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
    /// Represent the horizontal alignment of the content within a <see cref="T:Dt.Xls.ExcelCell" />
    /// </summary>
    public enum ExcelHorizontalAlignment : byte
    {
        /// <summary>
        /// The content is horizontally at the center of a cell. 
        /// </summary>
        Center = 2,
        /// <summary>
        /// The content is horizontally aligned with center across selection.
        /// </summary>
        CenterContinuous = 6,
        /// <summary>
        /// The content is horizontally distributed in a cell. 
        /// </summary>
        Distributed = 7,
        /// <summary>
        /// the content is horizontally fill in a cell.
        /// </summary>
        Fill = 4,
        /// <summary>
        /// The content is general aligned in a cell.
        /// </summary>
        General = 0,
        /// <summary>
        /// The content is horizontally justified in a cell. 
        /// </summary>
        Justify = 5,
        /// <summary>
        /// The content is horizontally at the left of a cell. 
        /// </summary>
        Left = 1,
        /// <summary>
        /// The content is horizontally at the right of a cell. 
        /// </summary>
        Right = 3
    }
}

