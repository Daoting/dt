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
    /// A class implement interface <see cref="T:Dt.Xls.IExcelDynamicFilter" />
    /// </summary>
    public class ExcelDynamicFilter : IExcelDynamicFilter, IExcelFilter
    {
        /// <summary>
        /// A maximum value for dynamic filter
        /// </summary>
        /// <value></value>
        public object MaxValue { get; set; }

        /// <summary>
        /// Dynamic filter type
        /// </summary>
        /// <value></value>
        public ExcelDynamicFilterType Type { get; set; }

        /// <summary>
        /// A minimum value for dynamic filter
        /// </summary>
        /// <value></value>
        public object Value { get; set; }
    }
}

