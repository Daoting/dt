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
    /// Represents the extended header footer setting for print.
    /// </summary>
    public class ExtendedHeadFooterSetting : IExtendedHeadFooterSetting
    {
        private bool _headerFooterAlignWithPageMargin = true;
        private bool _headerFooterDifferentFirstPage;
        private bool _headerFooterDifferentOddEvenPages;
        private bool _headerFooterScalesWithDocument = true;

        /// <summary>
        /// Gets or sets the footer even page.
        /// </summary>
        /// <value>The footer even page.</value>
        public string FooterEvenPage { get; set; }

        /// <summary>
        /// Gets or sets the footer first page.
        /// </summary>
        /// <value>The footer first page.</value>
        public string FooterFirstPage { get; set; }

        /// <summary>
        /// Gets or sets the footer odd page.
        /// </summary>
        /// <value>The footer odd page.</value>
        public string FooterOddPage { get; set; }

        /// <summary>
        /// Gets or sets the header even page.
        /// </summary>
        /// <value>The header even page.</value>
        public string HeaderEvenPage { get; set; }

        /// <summary>
        /// Gets or sets the header first page.
        /// </summary>
        /// <value>The header first page.</value>
        public string HeaderFirstPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether header footer align with page margin.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if header footer align with page margin; otherwise, <see langword="false" />.
        /// </value>
        public bool HeaderFooterAlignWithPageMargin
        {
            get { return  this._headerFooterAlignWithPageMargin; }
            set { this._headerFooterAlignWithPageMargin = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the header footer of the first page is different.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the header footer of the first page is different; otherwise, <see langword="false" />.
        /// </value>
        public bool HeaderFooterDifferentFirstPage
        {
            get { return  this._headerFooterDifferentFirstPage; }
            set { this._headerFooterDifferentFirstPage = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether odd even  header footer are different.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if odd even  header footer are different; otherwise, <see langword="false" />.
        /// </value>
        public bool HeaderFooterDifferentOddEvenPages
        {
            get { return  this._headerFooterDifferentOddEvenPages; }
            set { this._headerFooterDifferentOddEvenPages = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether header footer scales with document.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if header footer scales with document; otherwise, <see langword="false" />.
        /// </value>
        public bool HeaderFooterScalesWithDocument
        {
            get { return  this._headerFooterScalesWithDocument; }
            set { this._headerFooterScalesWithDocument = value; }
        }

        /// <summary>
        /// Gets or sets the header odd page.
        /// </summary>
        /// <value>The header odd page.</value>
        public string HeaderOddPage { get; set; }
    }
}

