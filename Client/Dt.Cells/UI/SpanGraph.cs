#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections;
#endregion

namespace Dt.Cells.UI
{
    internal class SpanGraph
    {
        private int _cachedBottomRow = -1;
        private int _cachedLeftColumn = -1;
        private int _cachedRightColumn = -1;
        private int _cachedTopRow = -1;
        private byte[,] _cahcedSpanGraph = new byte[0, 0];
        public const byte IsAnchorCell = 0x40;
        public const byte NoneSpan = 0;
        public const byte SpanBottom = 8;
        public const byte SpanCenter = 0x10;
        public const byte SpanHasBackground = 0x20;
        public const byte SpanLeft = 1;
        public const byte SpanRight = 4;
        public const byte SpanTop = 2;

        public void BuildGraph(int columnStart, int columnEnd, int rowStart, int rowEnd, SheetSpanModelBase spanModel, ICellSupport cellsPool)
        {
            this._cachedLeftColumn = columnStart;
            this._cachedRightColumn = columnEnd;
            this._cachedTopRow = rowStart;
            this._cachedBottomRow = rowEnd;
            int rowCount = (this._cachedBottomRow - this._cachedTopRow) + 1;
            int columnCount = (this._cachedRightColumn - this._cachedLeftColumn) + 1;
            if ((rowCount > 0) && (columnCount > 0))
            {
                this._cahcedSpanGraph = new byte[rowCount, columnCount];
                IEnumerator enumerator = spanModel.GetEnumerator(this._cachedTopRow, this._cachedLeftColumn, rowCount, columnCount);
                while (enumerator.MoveNext())
                {
                    CellRange current = (CellRange) enumerator.Current;
                    Cell cell = cellsPool.GetCell(current.Row, current.Column);
                    byte num3 = 0;
                    if ((cell != null) && (cell.ActualBackground != null))
                    {
                        num3 = 0x20;
                    }
                    for (int i = 0; i < current.RowCount; i++)
                    {
                        int num5 = i + current.Row;
                        if ((num5 <= this._cachedBottomRow) && (num5 >= this._cachedTopRow))
                        {
                            for (int j = 0; j < current.ColumnCount; j++)
                            {
                                int num7 = j + current.Column;
                                if ((num7 >= this._cachedLeftColumn) && (num7 <= this._cachedRightColumn))
                                {
                                    _cahcedSpanGraph[num5 - this._cachedTopRow, num7 - this._cachedLeftColumn] += num3;
                                    if ((j == 0) && (i == 0))
                                    {
                                        _cahcedSpanGraph[num5 - this._cachedTopRow, num7 - this._cachedLeftColumn]+= 0x40;
                                    }
                                    if (j == 0)
                                    {
                                        _cahcedSpanGraph[num5 - this._cachedTopRow, num7 - this._cachedLeftColumn]++;
                                    }
                                    if (j == (current.ColumnCount - 1))
                                    {
                                        _cahcedSpanGraph[num5 - this._cachedTopRow, num7 - this._cachedLeftColumn] += 4;
                                    }
                                    if (i == 0)
                                    {
                                        _cahcedSpanGraph[num5 - this._cachedTopRow, num7 - this._cachedLeftColumn] += 2;
                                    }
                                    if (i == (current.RowCount - 1))
                                    {
                                        _cahcedSpanGraph[num5 - this._cachedTopRow, num7 - this._cachedLeftColumn] += 8;
                                    }
                                    if (((j > 0) && (j < (current.ColumnCount - 1))) && ((i > 0) && (i < (current.RowCount - 1))))
                                    {
                                        _cahcedSpanGraph[num5 - this._cachedTopRow, num7 - this._cachedLeftColumn] += 0x10;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public byte GetState(int row, int column)
        {
            if (((row >= this._cachedTopRow) && (row <= this._cachedBottomRow)) && ((column >= this._cachedLeftColumn) && (column <= this._cachedRightColumn)))
            {
                return this._cahcedSpanGraph[row - this._cachedTopRow, column - this._cachedLeftColumn];
            }
            return 0;
        }

        public void Reset()
        {
            this._cachedLeftColumn = -1;
            this._cachedRightColumn = -1;
            this._cachedTopRow = -1;
            this._cachedBottomRow = -1;
            this._cahcedSpanGraph = new byte[0, 0];
        }
    }
}

