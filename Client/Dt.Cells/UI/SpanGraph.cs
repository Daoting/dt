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
        int _cachedBottomRow = -1;
        int _cachedLeftColumn = -1;
        int _cachedRightColumn = -1;
        int _cachedTopRow = -1;
        byte[,] _cahcedSpanGraph = new byte[0, 0];
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
            _cachedLeftColumn = columnStart;
            _cachedRightColumn = columnEnd;
            _cachedTopRow = rowStart;
            _cachedBottomRow = rowEnd;
            int rowCount = (_cachedBottomRow - _cachedTopRow) + 1;
            int columnCount = (_cachedRightColumn - _cachedLeftColumn) + 1;
            if ((rowCount > 0) && (columnCount > 0))
            {
                _cahcedSpanGraph = new byte[rowCount, columnCount];
                IEnumerator enumerator = spanModel.GetEnumerator(_cachedTopRow, _cachedLeftColumn, rowCount, columnCount);
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
                        if ((num5 <= _cachedBottomRow) && (num5 >= _cachedTopRow))
                        {
                            for (int j = 0; j < current.ColumnCount; j++)
                            {
                                int num7 = j + current.Column;
                                if ((num7 >= _cachedLeftColumn) && (num7 <= _cachedRightColumn))
                                {
                                    _cahcedSpanGraph[num5 - _cachedTopRow, num7 - _cachedLeftColumn] += num3;
                                    if ((j == 0) && (i == 0))
                                    {
                                        _cahcedSpanGraph[num5 - _cachedTopRow, num7 - _cachedLeftColumn]+= 0x40;
                                    }
                                    if (j == 0)
                                    {
                                        _cahcedSpanGraph[num5 - _cachedTopRow, num7 - _cachedLeftColumn]++;
                                    }
                                    if (j == (current.ColumnCount - 1))
                                    {
                                        _cahcedSpanGraph[num5 - _cachedTopRow, num7 - _cachedLeftColumn] += 4;
                                    }
                                    if (i == 0)
                                    {
                                        _cahcedSpanGraph[num5 - _cachedTopRow, num7 - _cachedLeftColumn] += 2;
                                    }
                                    if (i == (current.RowCount - 1))
                                    {
                                        _cahcedSpanGraph[num5 - _cachedTopRow, num7 - _cachedLeftColumn] += 8;
                                    }
                                    if (((j > 0) && (j < (current.ColumnCount - 1))) && ((i > 0) && (i < (current.RowCount - 1))))
                                    {
                                        _cahcedSpanGraph[num5 - _cachedTopRow, num7 - _cachedLeftColumn] += 0x10;
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
            if (((row >= _cachedTopRow) && (row <= _cachedBottomRow)) && ((column >= _cachedLeftColumn) && (column <= _cachedRightColumn)))
            {
                return _cahcedSpanGraph[row - _cachedTopRow, column - _cachedLeftColumn];
            }
            return 0;
        }

        public void Reset()
        {
            _cachedLeftColumn = -1;
            _cachedRightColumn = -1;
            _cachedTopRow = -1;
            _cachedBottomRow = -1;
            _cahcedSpanGraph = new byte[0, 0];
        }
    }
}

