#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal class ExcelCell : IExcelCell, IRange
    {
        public ExcelCell(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        public void SetFormatId(int id)
        {
            this.FormatId = id;
        }

        public IExcelFormula CellFormula { get; set; }

        public Dt.Xls.CellType CellType { get; set; }

        public int Column { get; internal set; }

        public int ColumnSpan
        {
            get { return  1; }
        }

        public IExtendedFormat Format { get; set; }

        public int FormatId { get; internal set; }

        public string Formula { get; set; }

        public string FormulaArray { get; set; }

        public string FormulaArrayR1C1 { get; set; }

        public string FormulaR1C1 { get; set; }

        public IExcelHyperLink Hyperlink { get; set; }

        public bool IsArrayFormula { get; set; }

        public string Note { get; set; }

        public ExcelNoteStyle NoteStyle { get; set; }

        public int Row { get; internal set; }

        public int RowSpan
        {
            get { return  1; }
        }

        public object Value { get; set; }
    }
}

