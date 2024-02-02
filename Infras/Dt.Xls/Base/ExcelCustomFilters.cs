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
    /// A class implements interface <see cref="T:Dt.Xls.IExcelCustomFilters" />
    /// </summary>
    public class ExcelCustomFilters : IExcelCustomFilters, IExcelFilter
    {
        /// <summary>
        /// Flags indicate these two condition are joined by 'and' or 'or'
        /// </summary>
        /// <value></value>
        public bool And { get; set; }

        /// <summary>
        /// The first condition
        /// </summary>
        /// <value></value>
        public IExcelCustomFilter Filter1 { get; set; }

        /// <summary>
        /// The second condition
        /// </summary>
        /// <value></value>
        public IExcelCustomFilter Filter2 { get; set; }
    }
}

