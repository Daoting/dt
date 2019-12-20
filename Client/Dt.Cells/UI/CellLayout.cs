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
            this.Row = row;
            this.Column = column;
            this.RowCount = rowCount;
            this.ColumnCount = columnCount;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool ContainsCell(int row, int column)
        {
            return ((((row >= this.Row) && (row < (this.Row + this.RowCount))) && (column >= this.Column)) && (column < (this.Column + this.ColumnCount)));
        }

        public bool ContainsPoint(double x, double y)
        {
            return ((((x >= this.X) && (x < (this.X + this.Width))) && (y >= this.Y)) && (y < (this.Y + this.Height)));
        }

        public CellRange GetCellRange()
        {
            return new CellRange(this.Row, this.Column, this.RowCount, this.ColumnCount);
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

