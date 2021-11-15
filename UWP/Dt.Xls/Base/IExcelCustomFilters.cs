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
    /// Specifies an operator and a value
    /// </summary>
    /// <remarks>
    /// There can be at most two customFilters specified, and in
    /// that case the parent element specifies whether the two conditions are joined by 'and' or 'or'. For any cells
    /// whose values do not meet the specified criteria, the corresponding rows shall be hidden from view when the
    /// filter is applied.
    /// </remarks>
    public interface IExcelCustomFilters : IExcelFilter
    {
        /// <summary>
        /// Flags indicate these two condition are joined by 'and' or 'or'
        /// </summary>
        bool And { get; set; }

        /// <summary>
        /// The first condition
        /// </summary>
        IExcelCustomFilter Filter1 { get; set; }

        /// <summary>
        /// The second condition
        /// </summary>
        IExcelCustomFilter Filter2 { get; set; }
    }
}

