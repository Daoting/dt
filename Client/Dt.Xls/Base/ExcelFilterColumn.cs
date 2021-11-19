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
    /// A class implements interface <see cref="T:Dt.Xls.IExcelFilterColumn" />
    /// </summary>
    public class ExcelFilterColumn : IExcelFilterColumn
    {
        /// <summary>
        /// Zero-based index indicating the AutoFilter column to which this filter information applies.
        /// </summary>
        /// <value></value>
        public uint AutoFilterColumnId { get; set; }

        /// <summary>
        /// Specifies the color to filter by and whether to use the cell's fill or font color in the filter criteria
        /// </summary>
        /// <value></value>
        public IExcelColorFilter ColorFilter { get; set; }

        /// <summary>
        /// Specifies the color to filter by and whether to use the cell's fill or font color in the filter criteria
        /// </summary>
        /// <value></value>
        public IExcelCustomFilters CustomFilters { get; set; }

        /// <summary>
        /// Specifies dynamic filter criteria
        /// </summary>
        /// <value></value>
        public IExcelDynamicFilter DynamicFilter { get; set; }

        /// <summary>
        /// Specifies a filter criteria value.
        /// </summary>
        /// <value></value>
        public IExcelFilters Filters { get; set; }

        /// <summary>
        /// Flag indicating whether the AutoFilter button for this column is hidden.
        /// </summary>
        /// <value></value>
        public bool HiddenButton { get; set; }

        /// <summary>
        /// Specifies the icon set and particular icon within that set to filter by
        /// </summary>
        /// <value></value>
        public IExcelIconFilter IconFilter { get; set; }

        /// <summary>
        /// Flag indicating whether the filter button is visible
        /// </summary>
        /// <value></value>
        public bool ShowButton { get; set; }

        /// <summary>
        /// Specifies the top N (percent or number of items) to filter by.
        /// </summary>
        /// <value></value>
        public IExcelTop10Filter Top10 { get; set; }
    }
}

