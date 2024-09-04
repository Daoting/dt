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
    internal class ReorderVisitor : OperatorExpressionVisistor
    {
        int _count;
        int _from;
        int _to;

        public ReorderVisitor(int from, int to, int count, bool row, bool isFullBand, ICalcSource currentCalcSource = null) : base(row, isFullBand, currentCalcSource)
        {
            this._from = from;
            this._to = to;
            this._count = count;
        }

        internal static void Adjust(int rangeStart, int rangeCount, int moveRangeStart, int moveRangeCount, int moveOffset, out int newStart, out int newCount)
        {
            if (moveOffset == 0)
            {
                newStart = rangeStart;
                newCount = rangeCount;
            }
            else if (moveOffset < 0)
            {
                int baseline = Reverse(ref rangeStart, ref rangeCount, ref moveRangeStart, ref moveRangeCount, ref moveOffset);
                AdjustDown(rangeStart, rangeCount, moveRangeStart, moveRangeCount, moveOffset, out newStart, out newCount);
                ReverseBack(baseline, ref newStart, ref newCount);
            }
            else
            {
                AdjustDown(rangeStart, rangeCount, moveRangeStart, moveRangeCount, moveOffset, out newStart, out newCount);
            }
        }

        static void AdjustDown(int rangeStart, int rangeCount, int moveRangeStart, int moveRangeCount, int moveOffset, out int newStart, out int newCount)
        {
            switch (GetAdjustMode(rangeStart, rangeCount, moveRangeStart, moveRangeCount, moveOffset))
            {
                case 0:
                    newStart = rangeStart;
                    newCount = rangeCount;
                    return;

                case 1:
                    newStart = rangeStart;
                    newCount = rangeCount - moveRangeCount;
                    return;

                case 2:
                    newStart = rangeStart;
                    newCount = rangeCount + moveOffset;
                    return;

                case 3:
                    newStart = rangeStart + moveOffset;
                    newCount = rangeCount;
                    return;

                case 4:
                    newStart = rangeStart + moveOffset;
                    newCount = rangeCount - moveOffset;
                    return;

                case 5:
                    newStart = moveRangeStart;
                    newCount = ((rangeStart + rangeCount) - moveRangeStart) - moveRangeCount;
                    return;

                case 6:
                    newStart = rangeStart - moveRangeCount;
                    newCount = rangeCount + moveRangeCount;
                    return;

                case 7:
                    newStart = rangeStart - moveRangeCount;
                    newCount = rangeCount;
                    return;
            }
            throw new InvalidOperationException();
        }

        internal static int GetAdjustMode(int rangeStart, int rangeCount, int moveRangeStart, int moveRangeCount, int moveOffset)
        {
            if (moveOffset == 0)
            {
                return 0;
            }
            if (moveOffset > 0)
            {
                return GetMoveDownAdjustMode(rangeStart, rangeCount, moveRangeStart, moveRangeCount, moveOffset);
            }
            return GetMoveUpAdjustMode(rangeStart, rangeCount, moveRangeStart, moveRangeCount, moveOffset);
        }

        public override void GetCellOffset(int oldIndex, out int newIndex)
        {
            int num;
            Adjust(oldIndex, 1, this._from, this._count, this._to - this._from, out newIndex, out num);
        }

        static int GetMoveDownAdjustMode(int rangeStart, int rangeCount, int moveRangeStart, int moveRangeCount, int moveOffset)
        {
            if ((rangeStart > moveRangeStart) || (rangeCount <= (((moveRangeStart + moveRangeCount) + moveOffset) - rangeStart)))
            {
                if (((rangeStart <= moveRangeStart) && (rangeCount <= (((moveRangeStart + moveRangeCount) + moveOffset) - rangeStart))) && (rangeCount > ((moveRangeStart + moveRangeCount) - rangeStart)))
                {
                    return 1;
                }
                if (((rangeStart < moveRangeStart) && (rangeCount <= ((moveRangeStart + moveRangeCount) - rangeStart))) && (rangeCount > (moveRangeStart - rangeStart)))
                {
                    return 2;
                }
                if ((rangeStart <= moveRangeStart) && (rangeCount <= (moveRangeStart - rangeStart)))
                {
                    return 0;
                }
                if ((rangeStart >= moveRangeStart) && (rangeCount <= ((moveRangeStart + moveRangeCount) - rangeStart)))
                {
                    return 3;
                }
                if (((rangeStart >= moveRangeStart) && (rangeStart < (moveRangeStart + moveRangeCount))) && (rangeCount > (((moveRangeStart + moveRangeCount) + moveOffset) - rangeStart)))
                {
                    return 4;
                }
                if (((rangeStart >= moveRangeStart) && (rangeStart < (moveRangeStart + moveRangeCount))) && ((rangeCount > ((moveRangeStart + moveRangeCount) - rangeStart)) && (rangeCount <= (((moveRangeStart + moveRangeCount) + moveOffset) - rangeStart))))
                {
                    return 5;
                }
                if (((rangeStart >= (moveRangeStart + moveRangeCount)) && (rangeStart < ((moveRangeStart + moveRangeCount) + moveOffset))) && (rangeCount > (((moveRangeStart + moveRangeCount) + moveOffset) - rangeStart)))
                {
                    return 6;
                }
                if ((rangeStart >= (moveRangeStart + moveRangeCount)) && (rangeCount <= (((moveRangeStart + moveRangeCount) + moveOffset) - rangeStart)))
                {
                    return 7;
                }
                if (rangeStart < ((moveRangeStart + moveRangeCount) + moveOffset))
                {
                    throw new InvalidOperationException();
                }
            }
            return 0;
        }

        static int GetMoveUpAdjustMode(int rangeStart, int rangeCount, int moveRangeStart, int moveRangeCount, int moveOffset)
        {
            Reverse(ref rangeStart, ref rangeCount, ref moveRangeStart, ref moveRangeCount, ref moveOffset);
            return GetMoveDownAdjustMode(rangeStart, rangeCount, moveRangeStart, moveRangeCount, moveOffset);
        }

        public override void GetRangeOffset(int oldStart, int oldEnd, out int newStart, out int newEnd)
        {
            int num;
            Adjust(oldStart, (oldEnd - oldStart) + 1, this._from, this._count, this._to - this._from, out newStart, out num);
            newEnd = (num + newStart) - 1;
        }

        static int Reverse(ref int rangeStart, ref int rangeCount, ref int moveRangeStart, ref int moveRangeCount, ref int moveOffset)
        {
            int num = rangeStart + rangeCount;
            int num2 = moveRangeStart + moveRangeCount;
            int num3 = Math.Max(num2, num) + 1;
            rangeStart = num3 - num;
            moveRangeStart = num3 - num2;
            moveOffset = -moveOffset;
            return num3;
        }

        static void ReverseBack(int baseline, ref int rangeStart, ref int rangeCount)
        {
            int num = baseline - rangeStart;
            rangeStart = num - rangeCount;
        }
    }
}

