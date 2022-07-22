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
using System.Globalization;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal class TableCalcEvaluatorContext : CalcEvaluatorContext
    {
        const int REF_COLUMN = 0;
        SheetTable table;

        public TableCalcEvaluatorContext(SheetTable table, ICalcSource source, bool arrayFormulaMode = false, int baseRowIndex = 0, int baseColumnIndex = 0, int baseRowCount = 1, int baseColumnCount = 1) : base(source, arrayFormulaMode, baseRowIndex, baseColumnIndex, baseRowCount, baseColumnCount)
        {
            this.table = table;
        }

        public override object GetReference(CalcIdentity id)
        {
            int num;
            object reference = base.GetReference(id);
            CalcStructReferenceIndentity indentity = id as CalcStructReferenceIndentity;
            if (((indentity != null) && (reference is SpreadCalcReference)) && this.TryParseColumnIndex(indentity.ToString(), out num))
            {
                return ((SpreadCalcReference) reference).Clone(this.table.DataRange.Row, this.table.DataRange.Column + num, this.table.DataRange.RowCount, 1);
            }
            return reference;
        }

        public override bool IsIntersected(CalcIdentity srcId, CalcIdentity destId)
        {
            if (!object.ReferenceEquals(srcId, destId))
            {
                int num;
                int num2;
                CalcStructReferenceIndentity indentity = srcId as CalcStructReferenceIndentity;
                if ((indentity != null) && this.TryParseColumnIndex(indentity.ToString(), out num))
                {
                    srcId = new CalcRangeIdentity(this.table.DataRange.Row, this.table.DataRange.Column + num, this.table.DataRange.RowCount, 1);
                }
                CalcStructReferenceIndentity indentity2 = destId as CalcStructReferenceIndentity;
                if ((indentity2 != null) && this.TryParseColumnIndex(indentity2.ToString(), out num2))
                {
                    destId = new CalcRangeIdentity(this.table.DataRange.Row, this.table.DataRange.Column + num2, this.table.DataRange.RowCount, 1);
                }
            }
            return base.IsIntersected(srcId, destId);
        }

        bool TryParseColumnIndex(string token, out int val)
        {
            val = 0;
            int num = CultureInfo.InvariantCulture.CompareInfo.IndexOf(token, "[Column", (CompareOptions) CompareOptions.IgnoreCase);
            int num2 = token.LastIndexOf("]");
            return (((num == 0) && (num2 > 7)) && int.TryParse(token.Substring(7, num2 - 7), out val));
        }
    }
}

