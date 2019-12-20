#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    internal class CellOverflowLayout
    {
        public CellOverflowLayout(int column, double width)
        {
            this.Column = column;
            this.BackgroundWidth = width;
            this.StartingColumn = -1;
            this.EndingColumn = -1;
        }

        public bool Contains(int columnIndex)
        {
            int startingColumn = this.StartingColumn;
            if (startingColumn == -1)
            {
                startingColumn = this.Column;
            }
            int endingColumn = this.EndingColumn;
            if (endingColumn == -1)
            {
                endingColumn = this.Column;
            }
            return ((columnIndex >= startingColumn) && (columnIndex <= endingColumn));
        }

        public override bool Equals(object obj)
        {
            CellOverflowLayout layout = obj as CellOverflowLayout;
            if (layout == null)
            {
                return base.Equals(obj);
            }
            return (((((layout.BackgroundWidth == this.BackgroundWidth) && (layout.Column == this.Column)) && ((layout.ContentWidth == this.ContentWidth) && (layout.EndingColumn == this.EndingColumn))) && ((layout.LeftBackgroundWidth == this.LeftBackgroundWidth) && (layout.RightBackgroundWidth == this.RightBackgroundWidth))) && (layout.StartingColumn == this.StartingColumn));
        }

        public override int GetHashCode()
        {
            return ((((((((double) this.BackgroundWidth).GetHashCode() | ((int) this.Column).GetHashCode()) | ((double) this.ContentWidth).GetHashCode()) | ((int) this.EndingColumn).GetHashCode()) | ((double) this.LeftBackgroundWidth).GetHashCode()) | ((double) this.RightBackgroundWidth).GetHashCode()) | ((int) this.StartingColumn).GetHashCode());
        }

        public double BackgroundWidth { get; set; }

        public int Column { get; set; }

        public double ContentWidth { get; set; }

        public int EndingColumn { get; set; }

        public double LeftBackgroundWidth { get; set; }

        public double RightBackgroundWidth { get; set; }

        public int StartingColumn { get; set; }
    }
}

