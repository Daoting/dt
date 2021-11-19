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
    /// A class implements interface <see cref="T:Dt.Xls.IExcelCustomFilter" />
    /// </summary>
    public class ExcelCustomFilter : IExcelCustomFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelCustomFilter" /> class.
        /// </summary>
        public ExcelCustomFilter()
        {
            this.Operator = ExcelFilterOperator.Equal;
        }

        /// <summary>
        /// Operator used in the filter comparison.
        /// </summary>
        /// <value></value>
        public ExcelFilterOperator Operator { get; set; }

        /// <summary>
        /// Top or bottom value used in the filter criteria.
        /// </summary>
        /// <value></value>
        public object Value { get; set; }

        /// <summary>
        /// The type of the value
        /// </summary>
        /// <value></value>
        public byte ValueType { get; set; }
    }
}

