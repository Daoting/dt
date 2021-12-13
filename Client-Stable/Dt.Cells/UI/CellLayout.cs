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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 
    /// </summary>
    internal class CellLayout
    {
        public CellLayout(int row, int column, int rowCount, int columnCount, double x, double y, double width, double height)
        {
            Row = row;
            Column = column;
            RowCount = rowCount;
            ColumnCount = columnCount;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool ContainsCell(int row, int column)
        {
            return ((((row >= Row) && (row < (Row + RowCount))) && (column >= Column)) && (column < (Column + ColumnCount)));
        }

        public bool ContainsPoint(double x, double y)
        {
            return ((((x >= X) && (x < (X + Width))) && (y >= Y)) && (y < (Y + Height)));
        }

        public CellRange GetCellRange()
        {
            return new CellRange(Row, Column, RowCount, ColumnCount);
        }

        public int Column { get; set; }

        public int ColumnCount { get; set; }

        public double Height { get; set; }

        public int Row { get; set; }

        public int RowCount { get; set; }

        public double Width { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}

