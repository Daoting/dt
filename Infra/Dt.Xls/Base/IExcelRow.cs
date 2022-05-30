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
    /// Defines a generalized collection of properties that a value or class implements to represent properties of excel row.
    /// </summary>
    public interface IExcelRow
    {
        /// <summary>
        /// Sets the format id which used to locate the correspond  <see cref="T:Dt.Xls.IExtendedFormat" /> from its parent <see cref="T:Dt.Xls.IExcelWorksheet" /> instance.
        /// </summary>
        /// <param name="id">the zero base index of the id used to locate the <see cref="T:Dt.Xls.IExtendedFormat" /></param>
        void SetFormatId(int id);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.IExcelRow" /> is collapsed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if collapsed; otherwise, <see langword="false" />.
        /// </value>
        bool Collapsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the height of this row is manually set.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's manually set; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>
        /// If the value is false, excel will invoke AutoFit row height on the current row.
        /// </remarks>
        bool CustomHeight { get; set; }

        /// <summary>
        /// Gets or sets the format of the row.
        /// </summary>
        /// <value> An <see cref="T:Dt.Xls.IExtendedFormat" /> instance represents the format setting of the row.</value>
        IExtendedFormat Format { get; set; }

        /// <summary>
        /// Gets the format id of the row.
        /// </summary>
        /// <value>The format id of the row.</value>
        int FormatId { get; }

        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        /// <value>The height of the two.</value>
        double Height { get; set; }

        /// <summary>
        /// Gets zero-based index of the row.
        /// </summary>
        /// <value>The index of the row</value>
        int Index { get; }

        /// <summary>
        /// Gets or sets the outline level of the row.
        /// </summary>
        /// <value>The outline level of the row</value>
        byte OutLineLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the row is page break row.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the row is page break row; otherwise, <see langword="false" />.
        /// </value>
        bool PageBreak { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.IExcelRow" /> is visible.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if visible; otherwise, <see langword="false" />.
        /// </value>
        bool Visible { get; set; }
    }
}

