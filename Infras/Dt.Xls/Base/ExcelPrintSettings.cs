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
    /// Represent the print settings for the entire workbook. the print setting contains
    /// page margin, page settings and print option settings.
    /// </summary>
    public class ExcelPrintSettings : IExcelPrintSettings
    {
        /// <summary>
        /// Initialize a new instance of <see cref="T:Dt.Xls.ExcelPrintSettings" />.
        /// </summary>
        public ExcelPrintSettings()
        {
            ExcelPrintPageMargin margin = new ExcelPrintPageMargin {
                Left = 0.7,
                Right = 0.7,
                Top = 0.75,
                Bottom = 0.75,
                Header = 0.3,
                Footer = 0.3
            };
            this.Margin = margin;
            this.Options = new ExcelPrintOptions();
            this.PageSetting = new ExcelPrintPageSetting();
        }

        /// <summary>
        /// Get or set the margin.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExcelPrintPageMargin" /> used to represents page margin settings
        /// </value>
        public IExcelPrintPageMargin Margin { get; set; }

        /// <summary>
        /// Get or set the options.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExcelPrintOptions" /> used to represents print options
        /// </value>
        public IExcelPrintOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the page setting.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExcelPrintPageSetting" /> used to represents the print page settings.
        /// </value>
        public IExcelPrintPageSetting PageSetting { get; set; }
    }
}

