#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents a single table style definition that indicates how a spreadsheet application
    /// should format and display a table.
    /// </summary>
    public interface IExcelTableStyle
    {
        /// <summary>
        /// A flag indicate whether this table style should be shown as an available pivot table style.
        /// </summary>
        /// <remarks>
        /// Not mutually exclusive with table - both can be true.
        /// </remarks>
        bool IsPivotStyle { get; set; }

        /// <summary>
        /// A flag indicate whether this table style should be shown as an available table style.
        /// </summary>
        /// <remarks>
        /// Not mutually exclusive with pivot - both can be true.
        /// </remarks>
        bool IsTableStyle { get; set; }

        /// <summary>
        /// Name of this table style.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Table style elements defined for this table style.
        /// </summary>
        List<IExcelTableStyleElement> TableStyleElements { get; set; }
    }
}

