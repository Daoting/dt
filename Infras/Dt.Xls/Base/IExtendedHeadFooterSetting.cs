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
    /// Defines advanced excel head foot print settings
    /// </summary>
    public interface IExtendedHeadFooterSetting
    {
        /// <summary>
        /// Gets or sets the footer even page.
        /// </summary>
        /// <value>The footer even page.</value>
        string FooterEvenPage { get; set; }

        /// <summary>
        /// Gets or sets the footer first page.
        /// </summary>
        /// <value>The footer first page.</value>
        string FooterFirstPage { get; set; }

        /// <summary>
        /// Gets or sets the footer odd page.
        /// </summary>
        /// <value>The footer odd page.</value>
        string FooterOddPage { get; set; }

        /// <summary>
        /// Gets or sets the header even page.
        /// </summary>
        /// <value>The header even page.</value>
        string HeaderEvenPage { get; set; }

        /// <summary>
        /// Gets or sets the header first page.
        /// </summary>
        /// <value>The header first page.</value>
        string HeaderFirstPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether header footer align with page margin.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if header footer align with page margin; otherwise, <see langword="false" />.
        /// </value>
        bool HeaderFooterAlignWithPageMargin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the header footer of the first page is different.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the header footer of the first page is different; otherwise, <see langword="false" />.
        /// </value>
        bool HeaderFooterDifferentFirstPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether odd even  header footer are different.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if odd even  header footer are different; otherwise, <see langword="false" />.
        /// </value>
        bool HeaderFooterDifferentOddEvenPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether header footer scales with document.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if header footer scales with document; otherwise, <see langword="false" />.
        /// </value>
        bool HeaderFooterScalesWithDocument { get; set; }

        /// <summary>
        /// Gets or sets the header odd page.
        /// </summary>
        /// <value>The header odd page.</value>
        string HeaderOddPage { get; set; }
    }
}

