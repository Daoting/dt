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
    /// An interface used to represents a named cell range 
    /// </summary>
    public interface IName
    {
        /// <summary>
        /// Get or set the comment associated with this defined name
        /// </summary>
        string Comment { get; }

        /// <summary>
        /// Get or set the zero-base index of Worksheet that the defined name belongs
        /// </summary>
        /// <remarks>
        /// If the value is -1, it means it's  workbook defined name (global name)
        /// </remarks>
        int Index { get; }

        /// <summary>
        /// Gets or sets the property which determines whether this defined name is hidden to the user.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Get or set the name of the defined name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the formula which the defined name refers to.
        /// </summary>
        string RefersTo { get; }

        /// <summary>
        /// Gets or sets the formula which the defined name refers to in R1C1 notation.
        /// </summary>
        string RefersToR1C1 { get; }
    }
}

