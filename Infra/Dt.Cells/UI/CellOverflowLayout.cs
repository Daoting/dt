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
            Column = column;
            BackgroundWidth = width;
            StartingColumn = -1;
            EndingColumn = -1;
        }

        public bool Contains(int columnIndex)
        {
            int startingColumn = StartingColumn;
            if (startingColumn == -1)
            {
                startingColumn = Column;
            }
            int endingColumn = EndingColumn;
            if (endingColumn == -1)
            {
                endingColumn = Column;
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
            return (((((layout.BackgroundWidth == BackgroundWidth) && (layout.Column == Column)) && ((layout.ContentWidth == ContentWidth) && (layout.EndingColumn == EndingColumn))) && ((layout.LeftBackgroundWidth == LeftBackgroundWidth) && (layout.RightBackgroundWidth == RightBackgroundWidth))) && (layout.StartingColumn == StartingColumn));
        }

        public override int GetHashCode()
        {
            return ((((((((double) BackgroundWidth).GetHashCode() | ((int) Column).GetHashCode()) | ((double) ContentWidth).GetHashCode()) | ((int) EndingColumn).GetHashCode()) | ((double) LeftBackgroundWidth).GetHashCode()) | ((double) RightBackgroundWidth).GetHashCode()) | ((int) StartingColumn).GetHashCode());
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

