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
    /// Specifies the top N (percent or number of items) to filter by.
    /// </summary>
    public interface IExcelTop10Filter : IExcelFilter
    {
        /// <summary>
        /// The actual cell value in the range which is used to perform the comparison for this filter
        /// </summary>
        double FilterValue { get; set; }

        /// <summary>
        /// Flag indicating whether or not to filter by percent value of the column.
        /// </summary>
        bool Percent { get; set; }

        /// <summary>
        /// Flag indicating whether or not to filter by top order
        /// </summary>
        bool Top { get; set; }

        /// <summary>
        /// Top or bottom value to use as the filter criteria.
        /// </summary>
        double Value { get; set; }
    }
}

