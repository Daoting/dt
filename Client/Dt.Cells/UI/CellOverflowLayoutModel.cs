#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    internal class CellOverflowLayoutModel : CollectionBase<CellOverflowLayout>
    {
        public static readonly CellOverflowLayoutModel Empty = new CellOverflowLayoutModel();

        public CellOverflowLayoutModel() : base("Column")
        {
        }

        public bool Contains(int columnIndex)
        {
            for (int i = base.Count - 1; i >= 0; i--)
            {
                CellOverflowLayout layout = base[i];
                if (layout.Contains(columnIndex))
                {
                    return true;
                }
            }
            return (((this.HeadingOverflowlayout != null) && this.HeadingOverflowlayout.Contains(columnIndex)) || ((this.TrailingOverflowlayout != null) && this.TrailingOverflowlayout.Contains(columnIndex)));
        }

        public CellOverflowLayout GetCellOverflowLayout(int columnIndex)
        {
            if ((this.HeadingOverflowlayout != null) && this.HeadingOverflowlayout.Contains(columnIndex))
            {
                return this.HeadingOverflowlayout;
            }
            if ((this.TrailingOverflowlayout != null) && this.TrailingOverflowlayout.Contains(columnIndex))
            {
                return this.TrailingOverflowlayout;
            }
            return Enumerable.FirstOrDefault<CellOverflowLayout>(this, delegate (CellOverflowLayout c) {
                return c.Contains(columnIndex);
            });
        }

        public CellOverflowLayout HeadingOverflowlayout { get; set; }

        public bool IsEmpty
        {
            get { return  (((base.Count == 0) && (this.HeadingOverflowlayout == null)) && (this.TrailingOverflowlayout == null)); }
        }

        public CellOverflowLayout TrailingOverflowlayout { get; set; }
    }
}

