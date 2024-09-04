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
    /// Specifies dynamic filter criteria
    /// </summary>
    /// <remarks>
    /// These criteria are considered dynamic because they can change,
    /// either with the data itself (e.g., "above average") or with the current system date (e.g., show values for "today").
    /// For any cells whose values do not meet the specified criteria, the corresponding rows shall be hidden from view
    /// when the filter is applied.
    /// </remarks>
    public interface IExcelDynamicFilter : IExcelFilter
    {
        /// <summary>
        /// A maximum value for dynamic filter
        /// </summary>
        object MaxValue { get; set; }

        /// <summary>
        /// Dynamic filter type
        /// </summary>
        ExcelDynamicFilterType Type { get; set; }

        /// <summary>
        /// A minimum value for dynamic filter
        /// </summary>
        object Value { get; set; }
    }
}

