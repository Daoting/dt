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
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Defines properties of excel print page settings
    /// </summary>
    public interface IExcelPrintPageSetting
    {
        /// <summary>
        /// Gets or sets the advanced head footer setting.
        /// </summary>
        /// <value>The advanced head footer setting.</value>
        IExtendedHeadFooterSetting AdvancedHeadFooterSetting { get; set; }

        /// <summary>
        /// Gets or sets the print policy if the cell value is error
        /// </summary>
        /// <value>The print style if the cell value is error</value>
        ExcelCellErrorPrintStyle CellErrorPrintStyle { get; set; }

        /// <summary>
        /// Gets or sets the column break lines.
        /// </summary>
        /// <value>The column break lines.</value>
        List<int> ColumnBreakLines { get; set; }

        /// <summary>
        /// Gets or sets the print node.
        /// </summary>
        /// <value>The print node.</value>
        ExcelPrintNotesStyle CommentsStyle { get; set; }

        /// <summary>
        /// Gets or sets the copies.
        /// </summary>
        /// <value>The copies.</value>
        ushort Copies { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.ExcelPrintPageSetting" /> is draft.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if draft; otherwise, <see langword="false" />.
        /// </value>
        bool Draft { get; set; }

        /// <summary>
        /// Gets or sets the first page number.
        /// </summary>
        /// <value>The first page number.</value>
        short FirstPageNumber { get; set; }

        /// <summary>
        /// Gets or sets the footer.
        /// </summary>
        /// <value>The footer.</value>
        string Footer { get; set; }

        /// <summary>
        /// Gets or sets the height of the footer.
        /// </summary>
        /// <value>The height of the footer.</value>
        double FooterHeight { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        string Header { get; set; }

        /// <summary>
        /// Gets or sets the height of the header.
        /// </summary>
        /// <value>The height of the header.</value>
        double HeaderHeight { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        ExcelPrintOrientation Orientation { get; set; }

        /// <summary>
        /// Gets or sets the page order.
        /// </summary>
        /// <value>The page order.</value>
        ExcelPrintPageOrder PageOrder { get; set; }

        /// <summary>
        /// Gets or sets the size of the paper.
        /// </summary>
        /// <value>The size of the paper.</value>
        int PaperSizeIndex { get; set; }

        /// <summary>
        /// Gets or sets the row break lines.
        /// </summary>
        /// <value>The row break lines.</value>
        List<int> RowBreakLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show color
        /// </summary>
        /// <value>
        /// <see langword="true" /> if show color; otherwise, <see langword="false" />.
        /// </value>
        bool ShowColor { get; set; }

        /// <summary>
        /// Gets or sets the height of the smart print pages.
        /// </summary>
        /// <value>The height of the smart print pages.</value>
        int SmartPrintPagesHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the smart print pages.
        /// </summary>
        /// <value>The width of the smart print pages.</value>
        int SmartPrintPagesWidth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use custom starting page].
        /// </summary>
        /// <value>
        /// <see langword="true" /> if [use custom starting page]; otherwise, <see langword="false" />.
        /// </value>
        bool UseCustomStartingPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use smart print].
        /// </summary>
        /// <value>
        /// <see langword="true" /> if [use smart print]; otherwise, <see langword="false" />.
        /// </value>
        bool UseSmartPrint { get; set; }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>The zoom factor.</value>
        float ZoomFactor { get; set; }
    }
}

