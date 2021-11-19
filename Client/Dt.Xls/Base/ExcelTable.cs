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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// An implementation of <see cref="T:Dt.Xls.IExcelTable" />
    /// </summary>
    public class ExcelTable : IExcelTable
    {
        private List<IExcelTableColumn> _columns;

        /// <summary>
        /// Gets or sets the auto filter information about the table.
        /// </summary>
        /// <value></value>
        public IExcelAutoFilter AutoFilter { get; set; }

        /// <summary>
        /// An element representing the collection of all table columns for this table.
        /// </summary>
        /// <value></value>
        public List<IExcelTableColumn> Columns
        {
            get
            {
                if (this._columns == null)
                {
                    this._columns = new List<IExcelTableColumn>();
                }
                return this._columns;
            }
            set { this._columns = value; }
        }

        /// <summary>
        /// A string representing the name of the table. This is the name that shall be used in formula references, and displayed
        /// in the UI to the end user.
        /// </summary>
        /// <value></value>
        public string DisplayName { get; set; }

        /// <summary>
        /// A non zero integer representing the unique identifier for this table. Each table in the workbook shall have a unique id.
        /// </summary>
        /// <value></value>
        public int Id { get; set; }

        /// <summary>
        /// A string representing the name of the table that is used to reference the table programmatically through the spreadsheet
        /// application object model.
        /// </summary>
        /// <value></value>
        /// <remarks>By default this should be the same as the table's displayName.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// The range on the relevant sheet that the table.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// The reference shall include the totals row if it is shown.</remarks>
        public IRange Range { get; set; }

        /// <summary>
        /// A flag indicate whether show header row or not.
        /// </summary>
        /// <value></value>
        public bool ShowHeaderRow { get; set; }

        /// <summary>
        /// A Boolean indicating whether the totals row has ever been shown in the past for this table.
        /// </summary>
        /// <value></value>
        public bool ShowTotalsRow { get; set; }

        /// <summary>
        /// Describes which style is used to display this table, and specifies which portions of the table have the style applied.
        /// </summary>
        /// <value></value>
        public IExcelTableStyleInfo TableStyleInfo { get; set; }
    }
}

