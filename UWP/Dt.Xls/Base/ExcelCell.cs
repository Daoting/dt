#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Utils;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent a cell in the <see cref="T:Dt.Xls.IExcelWorksheet" />.
    /// </summary>
    public class ExcelCell : IExcelCell, IRange
    {
        private int _column;
        private IExtendedFormat _excelFormat;
        private int _formatId;
        private IExcelHyperLink _hyperLink;
        private IExcelWorksheet _owner;
        private int _row;
        private object _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelCell" /> class.
        /// </summary>
        /// <param name="workSheet">The owner <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        public ExcelCell(IExcelWorksheet workSheet)
        {
            this._formatId = -1;
            if (workSheet == null)
            {
                throw new ArgumentNullException("workSheet");
            }
            this._owner = workSheet;
        }

        internal ExcelCell(IExcelWorksheet workSheet, int row, int column)
        {
            this._formatId = -1;
            this._owner = workSheet;
            this._row = row;
            this._column = column;
        }

        private void InitExcelCellDefaultFormat()
        {
            this._excelFormat = ExtendedFormat.Default;
            if (this._owner != null)
            {
                if (this._owner.ExtendedFormats == null)
                {
                    this._owner.ExtendedFormats = new Dictionary<int, IExtendedFormat>();
                }
                this._formatId = this._owner.ExtendedFormats.Count;
                this._owner.ExtendedFormats.Add(this._owner.ExtendedFormats.Count, this._excelFormat);
            }
        }

        /// <summary>
        /// Set the format id of the cell. The cell will use the id to locate the <see cref="T:Dt.Xls.IExtendedFormat" /> from
        /// its parent <see cref="T:Dt.Xls.IExcelWorksheet" /> instance
        /// </summary>
        /// <param name="id">The zero-based index of the format id of the cell.</param>
        /// <remarks> If the passed value is -1, it means use the default format setting.</remarks>
        public void SetFormatId(int id)
        {
            if (id == -1)
            {
                this._formatId = id;
            }
            else
            {
                if (((this._owner == null) || (this._owner.ExtendedFormats == null)) || !this._owner.ExtendedFormats.ContainsKey(id))
                {
                    throw new ArgumentOutOfRangeException("id");
                }
                this._formatId = id;
            }
        }

        /// <summary>
        /// Gets or sets the cell formula.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExcelFormula" /> instance used to represents cell formula.
        /// </value>
        public IExcelFormula CellFormula { get; set; }

        /// <summary>
        /// Gets or sets the type of the cell.
        /// </summary>
        /// <value>The type of the cell.</value>
        public Dt.Xls.CellType CellType { get; set; }

        /// <summary>
        /// Gets the zero-based index of the start column of the range.
        /// </summary>
        /// <value>The start column index.</value>
        public int Column
        {
            get { return  this._column; }
            internal set { this._column = value; }
        }

        /// <summary>
        /// Gets the column span of the range.
        /// </summary>
        /// <value>The column span.</value>
        public int ColumnSpan
        {
            get
            {
                IRange spanCell = this._owner.GetSpanCell(this.Row, this.Column);
                if (spanCell == null)
                {
                    return 1;
                }
                return spanCell.ColumnSpan;
            }
        }

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExtendedFormat" /> associate with cell.
        /// </summary>
        /// <value>
        /// The <see cref="T:Dt.Xls.ExtendedFormat" /> associate with cell.
        /// </value>
        public IExtendedFormat Format
        {
            get
            {
                if (this._excelFormat == null)
                {
                    if (((this._owner != null) && (this._owner.ExtendedFormats != null)) && this._owner.ExtendedFormats.ContainsKey(this._formatId))
                    {
                        this._excelFormat = this._owner.ExtendedFormats[this._formatId].Clone();
                        this._formatId = this._owner.ExtendedFormats.Count;
                        this._owner.ExtendedFormats.Add(this._formatId, this._excelFormat);
                    }
                    else
                    {
                        this.InitExcelCellDefaultFormat();
                    }
                }
                return this._excelFormat;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Format");
                }
                this._excelFormat = value;
                if (this._owner != null)
                {
                    this._owner.ExtendedFormats = this._owner.ExtendedFormats ?? new Dictionary<int, IExtendedFormat>();
                    foreach (KeyValuePair<int, IExtendedFormat> pair in this._owner.ExtendedFormats)
                    {
                        if (pair.Value.Equals(this._excelFormat))
                        {
                            this._formatId = pair.Key;
                            break;
                        }
                    }
                    this._formatId = this._owner.ExtendedFormats.Count;
                    this._owner.ExtendedFormats.Add(this._formatId, this._excelFormat);
                }
            }
        }

        /// <summary>
        /// Gets the format id of the <see cref="T:Dt.Xls.IExcelCell" />.
        /// </summary>
        /// <value>The format id.</value>
        /// <remarks> The instance of <see cref="T:Dt.Xls.IExcelCell" /> will use this id to locate the
        /// <see cref="T:Dt.Xls.IExtendedFormat" /> from the underlying <see cref="T:Dt.Xls.IExcelWorkbook" /> instance</remarks>
        public int FormatId
        {
            get { return  this._formatId; }
            private set { this._formatId = value; }
        }

        /// <summary>
        /// Gets or sets the A1 reference style formula.
        /// </summary>
        /// <value>The A1 reference style formula.</value>
        public string Formula
        {
            get
            {
                if (this.CellFormula != null)
                {
                    return this.CellFormula.Formula;
                }
                return null;
            }
            set
            {
                if (this.CellFormula == null)
                {
                    this.CellFormula = new ExcelFormula();
                    this.CellFormula.Formula = value;
                }
                else
                {
                    this.CellFormula.Formula = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the A1 reference style array formula.
        /// </summary>
        /// <value>The A1 reference style array formula.</value>
        public string FormulaArray
        {
            get
            {
                if ((this.CellFormula != null) && this.CellFormula.IsArrayFormula)
                {
                    return this.CellFormula.Formula;
                }
                return null;
            }
            set
            {
                if (this.CellFormula == null)
                {
                    this.CellFormula = new ExcelFormula();
                    this.CellFormula.IsArrayFormula = true;
                    this.CellFormula.Formula = value;
                }
                else
                {
                    this.CellFormula.IsArrayFormula = true;
                    this.CellFormula.Formula = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the R1C1 reference style array formula.
        /// </summary>
        /// <value>the R1C1 reference style array formula.</value>
        public string FormulaArrayR1C1
        {
            get
            {
                if ((this.CellFormula != null) && this.CellFormula.IsArrayFormula)
                {
                    return this.CellFormula.FormulaR1C1;
                }
                return null;
            }
            set
            {
                if (this.CellFormula == null)
                {
                    this.CellFormula = new ExcelFormula();
                    this.CellFormula.IsArrayFormula = true;
                    this.CellFormula.FormulaR1C1 = value;
                }
                else
                {
                    this.CellFormula.IsArrayFormula = true;
                    this.CellFormula.FormulaR1C1 = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the R1C1 reference style formula.
        /// </summary>
        /// <value>The R1C1 reference style formula.</value>
        public string FormulaR1C1
        {
            get
            {
                if (this.CellFormula != null)
                {
                    return this.CellFormula.FormulaR1C1;
                }
                return null;
            }
            set
            {
                if (this.CellFormula == null)
                {
                    this.CellFormula = new ExcelFormula();
                    this.CellFormula.FormulaR1C1 = value;
                }
                else
                {
                    this.CellFormula.FormulaR1C1 = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the hyperlink.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExcelHyperLink" /> instance used to represents the hyperlink.
        /// </value>
        public IExcelHyperLink Hyperlink
        {
            get { return  this._hyperLink; }
            set { this._hyperLink = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the formula is array formula.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this formula is array formula; otherwise, <see langword="false" />.
        /// </value>
        public bool IsArrayFormula
        {
            get { return  ((this.CellFormula != null) && this.CellFormula.IsArrayFormula); }
            set
            {
                if (string.IsNullOrWhiteSpace(this.Formula) && string.IsNullOrWhiteSpace(this.FormulaR1C1))
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("arrayFormulaStingNullError"));
                }
                this.CellFormula.IsArrayFormula = value;
                if (this.CellFormula.ArrayFormulaRange == null)
                {
                    ExcelCellRange range = new ExcelCellRange {
                        Row = this.Row,
                        Column = this.Column,
                        RowSpan = 1,
                        ColumnSpan = 1
                    };
                    this.CellFormula.ArrayFormulaRange = range;
                }
            }
        }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>The note.</value>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the note style.
        /// </summary>
        /// <value>The note style.</value>
        public ExcelNoteStyle NoteStyle { get; set; }

        /// <summary>
        /// Gets the zero-based index of the start row of the range.
        /// </summary>
        /// <value>The start row index.</value>
        public int Row
        {
            get { return  this._row; }
            internal set { this._row = value; }
        }

        /// <summary>
        /// Gets the row span of the range.
        /// </summary>
        /// <value>The row span.</value>
        public int RowSpan
        {
            get
            {
                IRange spanCell = this._owner.GetSpanCell(this.Row, this.Column);
                if (spanCell == null)
                {
                    return 1;
                }
                return spanCell.RowSpan;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return  this._value; }
            set
            {
                if (value is DateTime)
                {
                    DateTime @this = (DateTime) value;
                    this._value = (double) @this.ToOADate();
                    this.Format.NumberFormatIndex = 0x16;
                }
                else if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    this._value = (double) span.TotalDays;
                    this.Format.NumberFormatIndex = 0x15;
                }
                else
                {
                    this._value = value;
                }
            }
        }
    }
}

