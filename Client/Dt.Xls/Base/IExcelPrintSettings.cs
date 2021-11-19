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
    /// Defines excel print settings
    /// </summary>
    public interface IExcelPrintSettings
    {
        /// <summary>
        /// Get or set the margin.
        /// </summary>
        /// <value>An <see cref="T:Dt.Xls.IExcelPrintPageMargin" /> used to represents page margin settings</value>
        IExcelPrintPageMargin Margin { get; set; }

        /// <summary>
        /// Get or set the options.
        /// </summary>
        /// <value>An <see cref="T:Dt.Xls.IExcelPrintOptions" /> used to represents print options</value>
        IExcelPrintOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the page setting.
        /// </summary>
        /// <value>An <see cref="T:Dt.Xls.IExcelPrintPageSetting" /> used to represents the print page settings.</value>
        IExcelPrintPageSetting PageSetting { get; set; }
    }
}

