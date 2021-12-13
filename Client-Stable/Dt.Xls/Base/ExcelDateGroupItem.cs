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
    /// A class implements interface <see cref="T:Dt.Xls.IExcelDateGroupItem" />
    /// </summary>
    public class ExcelDateGroupItem : IExcelDateGroupItem
    {
        /// <summary>
        /// Grouping level.
        /// </summary>
        /// <value></value>
        public ExcelDateTimeGrouping DateTimeGrouping { get; set; }

        /// <summary>
        /// Day
        /// </summary>
        /// <value></value>
        public ushort Day { get; set; }

        /// <summary>
        /// hour
        /// </summary>
        /// <value></value>
        public ushort Hour { get; set; }

        /// <summary>
        /// Minute
        /// </summary>
        /// <value></value>
        public ushort Minute { get; set; }

        /// <summary>
        /// Month
        /// </summary>
        /// <value></value>
        public ushort Month { get; set; }

        /// <summary>
        /// Second
        /// </summary>
        /// <value></value>
        public ushort Second { get; set; }

        /// <summary>
        /// Year
        /// </summary>
        /// <value></value>
        public ushort Year { get; set; }
    }
}

