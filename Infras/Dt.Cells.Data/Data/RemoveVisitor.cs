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
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal class RemoveVisitor : OperatorExpressionVisistor
    {
        int _count;
        int _index;

        public RemoveVisitor(int index, int count, bool row, bool isFullBand, ICalcSource currentCalcSource = null) : base(row, isFullBand, currentCalcSource)
        {
            this._index = index;
            this._count = count;
        }

        public override void GetCellOffset(int oldIndex, out int newIndex)
        {
            if ((oldIndex >= this._index) && (oldIndex < (this._index + this._count)))
            {
                newIndex = -2147483648;
            }
            else if (oldIndex >= (this._index + this._count))
            {
                newIndex = oldIndex - this._count;
            }
            else
            {
                newIndex = oldIndex;
            }
        }

        public override void GetRangeOffset(int oldStart, int oldEnd, out int newStart, out int newEnd)
        {
            if ((oldStart >= this._index) && (oldEnd < (this._index + this._count)))
            {
                newStart = -2147483648;
                newEnd = -2147483648;
            }
            else if (oldStart >= (this._index + this._count))
            {
                newStart = oldStart - this._count;
                newEnd = oldEnd - this._count;
            }
            else if (oldEnd < this._index)
            {
                newStart = oldStart;
                newEnd = oldEnd;
            }
            else if (oldStart >= this._index)
            {
                newStart = oldStart;
                newEnd = ((oldEnd - this._count) + oldStart) - this._index;
            }
            else if (oldEnd < (this._index + this._count))
            {
                newStart = oldStart;
                newEnd = this._index - 1;
            }
            else if ((oldStart < this._index) && (oldEnd >= (this._index + this._count)))
            {
                newStart = oldStart;
                newEnd = oldEnd - this._count;
            }
            else
            {
                newStart = oldStart;
                newEnd = oldEnd;
            }
        }
    }
}

