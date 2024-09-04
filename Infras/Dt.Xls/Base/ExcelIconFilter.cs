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
    /// A class implements interface <see cref="T:Dt.Xls.IExcelIconFilter" />
    /// </summary>
    public class ExcelIconFilter : IExcelIconFilter, IExcelFilter
    {
        /// <summary>
        /// Zero-based index of an icon in an icon set
        /// </summary>
        /// <value></value>
        public uint IconId { get; set; }

        /// <summary>
        /// Specifies which icon set is used in the filter criteria.
        /// </summary>
        /// <value></value>
        public ExcelIconSetType IconSet { get; set; }

        /// <summary>
        /// Flag indicate whether show icon or not
        /// </summary>
        /// <value></value>
        public bool NoIcon { get; set; }
    }
}

