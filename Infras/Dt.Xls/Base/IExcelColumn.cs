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
    /// Defines a generalized collection of properties that a value or class implements to represent properties of excel column
    /// </summary>
    public interface IExcelColumn
    {
        /// <summary>
        /// Sets the format id which used to locate the correspond  <see cref="T:Dt.Xls.IExtendedFormat" /> from its parent <see cref="T:Dt.Xls.IExcelWorksheet" /> instance.
        /// </summary>
        /// <param name="id">the zero base index of the id used to locate the <see cref="T:Dt.Xls.IExtendedFormat" /></param>
        void SetFormatId(int id);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.IExcelColumn" /> is collapsed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if collapsed; otherwise, <see langword="false" />.
        /// </value>
        bool Collapsed { get; set; }

        /// <summary>
        /// Gets or sets the format of the column
        /// </summary>
        /// <value> An <see cref="T:Dt.Xls.IExtendedFormat" /> instance represents the format setting of the column.</value>
        IExtendedFormat Format { get; set; }

        /// <summary>
        /// Gets the format id of the column.
        /// </summary>
        /// <value>The format id of the column.</value>
        int FormatId { get; }

        /// <summary>
        /// Gets zero based index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        int Index { get; }

        /// <summary>
        /// Gets or sets the outline level of the column.
        /// </summary>
        /// <value>The outline level of the column</value>
        byte OutLineLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is a page break column.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's a page break column; otherwise, <see langword="false" />.
        /// </value>
        bool PageBreak { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.IExcelColumn" /> is visible.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if visible; otherwise, <see langword="false" />.
        /// </value>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        /// <value>The width of the column</value>
        double Width { get; set; }
    }
}

