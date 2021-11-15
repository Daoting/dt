#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class SheetCellRange : CellRange
    {
        Worksheet _sheet;

        public SheetCellRange(Worksheet sheet, CellRange range) : this(sheet, range.Row, range.Column, range.RowCount, range.ColumnCount)
        {
        }

        public SheetCellRange(Worksheet sheet, int row, int column, int rowCount, int columnCount) : base(row, column, rowCount, columnCount)
        {
            this._sheet = sheet;
        }

        public override object Clone()
        {
            return new SheetCellRange(this.Sheet, base.Row, base.Column, base.RowCount, base.ColumnCount);
        }

        public override bool Equals(object item)
        {
            return (base.Equals(item) && ((item as SheetCellRange).Sheet == this.Sheet));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Worksheet Sheet
        {
            get { return  this._sheet; }
        }
    }
}

