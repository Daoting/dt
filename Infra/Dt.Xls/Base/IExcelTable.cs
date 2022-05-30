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
    /// Describes a excel table definition.
    /// </summary>
    public interface IExcelTable
    {
        /// <summary>
        /// Gets or sets the auto filter information about the table.
        /// </summary>
        IExcelAutoFilter AutoFilter { get; set; }

        /// <summary>
        /// An element representing the collection of all table columns for this table.
        /// </summary>
        List<IExcelTableColumn> Columns { get; set; }

        /// <summary>
        /// A string representing the name of the table. This is the name that shall be used in formula references, and displayed
        /// in the UI to the end user.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// A non zero integer representing the unique identifier for this table. Each table in the workbook shall have a unique id.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// A string representing the name of the table that is used to reference the table programmatically through the spreadsheet
        /// application object model.
        /// </summary>
        /// <remarks>By default this should be the same as the table's displayName.</remarks>
        string Name { get; set; }

        /// <summary>
        /// The range on the relevant sheet that the table.
        /// </summary>
        /// <remarks>
        /// The reference shall include the totals row if it is shown.</remarks>
        IRange Range { get; set; }

        /// <summary>
        /// A flag indicate whether show header row or not.
        /// </summary>
        bool ShowHeaderRow { get; set; }

        /// <summary>
        /// A Boolean indicating whether the totals row has ever been shown in the past for this table.
        /// </summary>
        bool ShowTotalsRow { get; set; }

        /// <summary>
        /// Describes which style is used to display this table, and specifies which portions of the table have the style applied.
        /// </summary>
        IExcelTableStyleInfo TableStyleInfo { get; set; }
    }
}

