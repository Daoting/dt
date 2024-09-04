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
    /// Defines a generalized collection of properties that a value or class implements to represent properties of excel cell
    /// </summary>
    public interface IExcelCell : IRange
    {
        /// <summary>
        /// Sets the format id which used to locate the correspond  <see cref="T:Dt.Xls.IExtendedFormat" /> from its parent <see cref="T:Dt.Xls.IExcelWorksheet" /> instance.
        /// </summary>
        /// <param name="id">the zero base index of the id used to locate the <see cref="T:Dt.Xls.IExtendedFormat" /></param>
        void SetFormatId(int id);

        /// <summary>
        /// Gets or sets the cell formula.
        /// </summary>
        /// <value> An <see cref="T:Dt.Xls.IExcelFormula" /> instance used to represents cell formula.</value>
        IExcelFormula CellFormula { get; set; }

        /// <summary>
        /// Gets or sets the type of the cell.
        /// </summary>
        /// <value>The type of the cell.</value>
        Dt.Xls.CellType CellType { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        IExtendedFormat Format { get; set; }

        /// <summary>
        /// Gets the format id of the <see cref="T:Dt.Xls.IExcelCell" />.
        /// </summary>
        /// <value>The format id.</value>
        /// <remarks> The instance of <see cref="T:Dt.Xls.IExcelCell" /> will use this id to locate the 
        /// <see cref="T:Dt.Xls.IExtendedFormat" /> from the underlying <see cref="T:Dt.Xls.IExcelWorkbook" /> instance</remarks>
        int FormatId { get; }

        /// <summary>
        /// Gets or sets the A1 reference style formula.
        /// </summary>
        /// <value>The A1 reference style formula.</value>
        string Formula { get; set; }

        /// <summary>
        /// Gets or sets the A1 reference style array formula.
        /// </summary>
        /// <value>The A1 reference style array formula.</value>
        string FormulaArray { get; set; }

        /// <summary>
        /// Gets or sets the R1C1 reference style array formula.
        /// </summary>
        /// <value>the R1C1 reference style array formula.</value>
        string FormulaArrayR1C1 { get; set; }

        /// <summary>
        /// Gets or sets the R1C1 reference style formula.
        /// </summary>
        /// <value>The R1C1 reference style formula.</value>
        string FormulaR1C1 { get; set; }

        /// <summary>
        /// Gets or sets the hyperlink.
        /// </summary>
        /// <value> An <see cref="T:Dt.Xls.IExcelHyperLink" /> instance used to represents the hyperlink.</value>
        IExcelHyperLink Hyperlink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the formula is array formula.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this formula is array formula; otherwise, <see langword="false" />.
        /// </value>
        bool IsArrayFormula { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>The note.</value>
        string Note { get; set; }

        /// <summary>
        /// Gets or sets the note style.
        /// </summary>
        /// <value>The note style.</value>
        ExcelNoteStyle NoteStyle { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; set; }
    }
}

