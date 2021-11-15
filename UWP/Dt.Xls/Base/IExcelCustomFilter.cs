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
    /// Specifies an operator and a value.
    /// </summary>
    /// <remarks>
    /// There can be at most two customFilters specified, and in
    /// that case the parent element specifies whether the two conditions are joined by 'and' or 'or'. For any cells
    /// whose values do not meet the specified criteria, the corresponding rows shall be hidden from view when the
    /// filter is applied.
    /// </remarks>
    public interface IExcelCustomFilter
    {
        /// <summary>
        /// Operator used in the filter comparison.
        /// </summary>
        ExcelFilterOperator Operator { get; set; }

        /// <summary>
        /// Top or bottom value used in the filter criteria.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// The type of the value
        /// </summary>
        byte ValueType { get; set; }
    }
}

