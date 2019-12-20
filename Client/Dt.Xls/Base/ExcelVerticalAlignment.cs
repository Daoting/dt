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
    /// Represent the vertical alignment of the content within a <see cref="T:Dt.Xls.ExcelCell" />
    /// </summary>
    public enum ExcelVerticalAlignment : byte
    {
        /// <summary>
        /// The content is vertically at the bottom of a cell. 
        /// </summary>
        Bottom = 2,
        /// <summary>
        /// The content is vertically at the center of a cell. 
        /// </summary>
        Center = 1,
        /// <summary>
        /// The content is vertically distributed in a cell. 
        /// </summary>
        Distributed = 4,
        /// <summary>
        /// The content is vertically justified in a cell. 
        /// </summary>
        Justify = 3,
        /// <summary>
        /// The content is vertically at the top of a cell. 
        /// </summary>
        Top = 0
    }
}

