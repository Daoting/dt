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
    /// A class implements interface <see cref="T:Dt.Xls.IExcelTop10Filter" />
    /// </summary>
    public class ExcelTop10 : IExcelTop10Filter, IExcelFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelTop10" /> class.
        /// </summary>
        public ExcelTop10()
        {
            this.Top = true;
        }

        /// <summary>
        /// The actual cell value in the range which is used to perform the comparison for this filter
        /// </summary>
        /// <value></value>
        public double FilterValue { get; set; }

        /// <summary>
        /// Flag indicating whether or not to filter by percent value of the column.
        /// </summary>
        /// <value></value>
        public bool Percent { get; set; }

        /// <summary>
        /// Flag indicating whether or not to filter by top order
        /// </summary>
        /// <value></value>
        public bool Top { get; set; }

        /// <summary>
        /// Top or bottom value to use as the filter criteria.
        /// </summary>
        /// <value></value>
        public double Value { get; set; }
    }
}

