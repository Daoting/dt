#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class SpreadCalcReference : CalcReference
    {
        int column;
        int columnCount;
        ICalcSource context;
        int row;
        int rowCount;
        int sheetColumnCount;
        int sheetRowCount;
        SpreadCalcReference standaloneCalcRef;
        ISubtotalSupport subtotalSupport;

        internal SpreadCalcReference(ISubtotalSupport subtotalSupport)
        {
            this.subtotalSupport = subtotalSupport;
        }

        public SpreadCalcReference(SpreadCalcReference standaloneCalcRef, ICalcSource context, int row, int column, int rowCount, int columnCount, int sheetRowCount, int sheetColumnCount)
        {
            this.standaloneCalcRef = standaloneCalcRef;
            this.context = context;
            this.row = row;
            this.column = column;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.sheetRowCount = sheetRowCount;
            this.sheetColumnCount = sheetColumnCount;
            standaloneCalcRef.context = context;
            standaloneCalcRef.row = 0;
            standaloneCalcRef.column = 0;
            standaloneCalcRef.rowCount = sheetRowCount;
            standaloneCalcRef.columnCount = sheetColumnCount;
            standaloneCalcRef.sheetRowCount = sheetRowCount;
            standaloneCalcRef.sheetColumnCount = sheetColumnCount;
        }

        public SpreadCalcReference Clone(int row, int column, int rowCount, int columnCount)
        {
            return new SpreadCalcReference(this.standaloneCalcRef, this.context, row, column, rowCount, columnCount, this.sheetRowCount, this.sheetColumnCount);
        }

        public override bool Equals(object obj)
        {
            SpreadCalcReference objA = obj as SpreadCalcReference;
            if (object.ReferenceEquals(objA, null))
            {
                return false;
            }
            return this.context.Equals(objA.context);
        }

        public override int GetColumn(int range)
        {
            return this.column;
        }

        public override int GetColumnCount(int range)
        {
            return this.columnCount;
        }

        public ICalcSource GetContext()
        {
            return this.context;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override int GetRow(int range)
        {
            return this.row;
        }

        public override int GetRowCount(int range)
        {
            return this.rowCount;
        }

        public override CalcReference GetSource()
        {
            return this.standaloneCalcRef;
        }

        public override object GetValue(int range, int rowOffset, int columnOffset)
        {
            int rowIndex = this.row + rowOffset;
            int columnIndex = this.column + columnOffset;
            if (((rowIndex > -1) && (rowIndex < this.sheetRowCount)) && ((columnIndex > -1) && (columnIndex < this.sheetColumnCount)))
            {
                CalcCellIdentity id = new CalcCellIdentity(rowIndex, columnIndex);
                return this.context.GetValue(id);
            }
            return null;
        }

        public override bool IsSubtotal(int range, int rowOffset, int columnOffset)
        {
            int num = this.row + rowOffset;
            int num2 = this.column + columnOffset;
            if (((num > -1) && (num < this.sheetRowCount)) && ((num2 > -1) && (num2 < this.sheetColumnCount)))
            {
                if ((this.standaloneCalcRef != null) && (this.standaloneCalcRef.subtotalSupport != null))
                {
                    return this.standaloneCalcRef.IsSubtotal(range, num, num2);
                }
                if (this.subtotalSupport != null)
                {
                    return this.subtotalSupport.IsSubtotal(this, range, num, num2);
                }
            }
            return false;
        }

        public override int RangeCount
        {
            get { return  1; }
        }
    }
}

