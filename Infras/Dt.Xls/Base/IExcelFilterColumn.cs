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
    /// Represents a column in the <see cref="T:Dt.Xls.IExcelAutoFilter" /> range and specifies filter information that
    /// has been applied to this column
    /// </summary>
    public interface IExcelFilterColumn
    {
        /// <summary>
        /// Zero-based index indicating the AutoFilter column to which this filter information applies. It's a relative value (relative to the filter range).
        /// </summary>
        uint AutoFilterColumnId { get; set; }

        /// <summary>
        /// Specifies the color to filter by and whether to use the cell's fill or font color in the filter criteria
        /// </summary>
        IExcelColorFilter ColorFilter { get; set; }

        /// <summary>
        /// Specifies the color to filter by and whether to use the cell's fill or font color in the filter criteria
        /// </summary>
        IExcelCustomFilters CustomFilters { get; set; }

        /// <summary>
        /// Specifies dynamic filter criteria
        /// </summary>
        IExcelDynamicFilter DynamicFilter { get; set; }

        /// <summary>
        /// Specifies a filter criteria value.
        /// </summary>
        IExcelFilters Filters { get; set; }

        /// <summary>
        /// Flag indicating whether the AutoFilter button for this column is hidden.
        /// </summary>
        bool HiddenButton { get; set; }

        /// <summary>
        /// Specifies the icon set and particular icon within that set to filter by
        /// </summary>
        IExcelIconFilter IconFilter { get; set; }

        /// <summary>
        /// Flag indicating whether the filter button is visible
        /// </summary>
        bool ShowButton { get; set; }

        /// <summary>
        /// Specifies the top N (percent or number of items) to filter by.
        /// </summary>
        IExcelTop10Filter Top10 { get; set; }
    }
}

