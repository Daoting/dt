#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    internal class TriCoord
    {
        private int col;
        private int row;
        private int sheet;

        public TriCoord()
        {
        }

        public TriCoord(int sheet, int row, int col)
        {
            this.sheet = sheet;
            this.col = col;
            this.row = row;
        }

        public override bool Equals(object obj)
        {
            TriCoord coord = obj as TriCoord;
            if (coord == null)
            {
                return false;
            }
            return (((coord.Sheet == this.Sheet) && (coord.Row == this.Row)) && (coord.Col == this.Col));
        }

        public override int GetHashCode()
        {
            return ((((int) this.Sheet).GetHashCode() ^ ((int) this.Row).GetHashCode()) ^ ((int) this.Col).GetHashCode());
        }

        public int Col
        {
            get { return  this.col; }
            set { this.col = value; }
        }

        public int Row
        {
            get { return  this.row; }
            set { this.row = value; }
        }

        public int Sheet
        {
            get { return  this.sheet; }
            set { this.sheet = value; }
        }
    }
}

