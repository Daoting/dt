#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent the print page settings for the entire workbook.
    /// </summary>
    public class ExcelPrintPageSetting : IExcelPrintPageSetting
    {
        private IExtendedHeadFooterSetting _advancedHeaderFooterSetting;
        private short _firstPageNumber = 1;
        private double _footerHeight;
        private double _headerHeight;
        private bool _showColor = true;
        private float _zoomFactor = 1f;

        /// <summary>
        /// Gets or sets the advanced head footer setting.
        /// </summary>
        /// <value>The advanced head footer setting.</value>
        public IExtendedHeadFooterSetting AdvancedHeadFooterSetting
        {
            get
            {
                if (this._advancedHeaderFooterSetting == null)
                {
                    this._advancedHeaderFooterSetting = new ExtendedHeadFooterSetting();
                }
                return this._advancedHeaderFooterSetting;
            }
            set { this._advancedHeaderFooterSetting = value; }
        }

        /// <summary>
        /// Gets or sets the print policy if the cell value is error
        /// </summary>
        /// <value>The print style if the cell value is error</value>
        public ExcelCellErrorPrintStyle CellErrorPrintStyle { get; set; }

        /// <summary>
        /// Gets or sets the column break lines.
        /// </summary>
        /// <value>The column break lines.</value>
        public List<int> ColumnBreakLines { get; set; }

        /// <summary>
        /// Gets or sets the print node.
        /// </summary>
        /// <value>The print node.</value>
        public ExcelPrintNotesStyle CommentsStyle { get; set; }

        /// <summary>
        /// Gets or sets the copies.
        /// </summary>
        /// <value>The copies.</value>
        public ushort Copies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.ExcelPrintPageSetting" /> is draft.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if draft; otherwise, <see langword="false" />.
        /// </value>
        public bool Draft { get; set; }

        /// <summary>
        /// Gets or sets the first page number.
        /// </summary>
        /// <value>The first page number.</value>
        public short FirstPageNumber
        {
            get { return  this._firstPageNumber; }
            set { this._firstPageNumber = value; }
        }

        /// <summary>
        /// Gets or sets the footer.
        /// </summary>
        /// <value>The footer.</value>
        public string Footer { get; set; }

        /// <summary>
        /// Gets or sets the height of the footer.
        /// </summary>
        /// <value>The height of the footer.</value>
        public double FooterHeight
        {
            get { return  this._footerHeight; }
            set { this._footerHeight = value * 96.0; }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the height of the header.
        /// </summary>
        /// <value>The height of the header.</value>
        public double HeaderHeight
        {
            get { return  this._headerHeight; }
            set { this._headerHeight = value * 96.0; }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public ExcelPrintOrientation Orientation { get; set; }

        /// <summary>
        /// Gets or sets the page order.
        /// </summary>
        /// <value>The page order.</value>
        public ExcelPrintPageOrder PageOrder { get; set; }

        /// <summary>
        /// Gets or sets the size of the paper.
        /// </summary>
        /// <value>The size of the paper.</value>
        public int PaperSizeIndex { get; set; }

        /// <summary>
        /// Gets or sets the row break lines.
        /// </summary>
        /// <value>The row break lines.</value>
        public List<int> RowBreakLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show color
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show color; otherwise, <see langword="false" />.
        /// </value>
        public bool ShowColor
        {
            get { return  this._showColor; }
            set { this._showColor = value; }
        }

        /// <summary>
        /// Gets or sets the height of the smart print pages.
        /// </summary>
        /// <value>The height of the smart print pages.</value>
        public int SmartPrintPagesHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the smart print pages.
        /// </summary>
        /// <value>The width of the smart print pages.</value>
        public int SmartPrintPagesWidth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use custom starting page].
        /// </summary>
        /// <value>
        /// <see langword="true" /> if [use custom starting page]; otherwise, <see langword="false" />.
        /// </value>
        public bool UseCustomStartingPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use smart print].
        /// </summary>
        /// <value>
        /// <see langword="true" /> if [use smart print]; otherwise, <see langword="false" />.
        /// </value>
        public bool UseSmartPrint { get; set; }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>The zoom factor.</value>
        public float ZoomFactor
        {
            get { return  this._zoomFactor; }
            set { this._zoomFactor = value; }
        }
    }
}

