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
    /// An implementation of <see cref="T:Dt.Xls.IExcelTableStyleInfo" />
    /// </summary>
    public class ExcelTableStyleInfo : IExcelTableStyleInfo
    {
        /// <summary>
        /// Represents the name of the table style to use with the table
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// A boolean indicating whether column stripe formatting is applied.
        /// </summary>
        /// <value></value>
        public bool ShowColumnStripes { get; set; }

        /// <summary>
        /// A boolean indicating whether the first column in the table should have style applied.
        /// </summary>
        /// <value></value>
        public bool ShowFirstColumn { get; set; }

        /// <summary>
        /// A boolean indicating whether the last column in the table should have style applied.
        /// </summary>
        /// <value></value>
        public bool ShowLastColumn { get; set; }

        /// <summary>
        /// A boolean indicating whether row stripe formatting is applied.
        /// </summary>
        /// <value></value>
        public bool ShowRowStripes { get; set; }
    }
}

