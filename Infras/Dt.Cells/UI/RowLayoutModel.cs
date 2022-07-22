#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Cells.UI
{
    internal class RowLayoutModel : CollectionBase<RowLayout>
    {
        public RowLayoutModel() : base("Row")
        {
        }

        public RowLayout FindNearY(double y)
        {
            RowLayout layout = null;
            if (base.Count > 0)
            {
                layout = FindY(y);
                if (layout == null)
                {
                    layout = (y < Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>) this, 0).Y) ? Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>) this, 0) : Enumerable.ElementAt<RowLayout>((IEnumerable<RowLayout>) this, base.Count - 1);
                }
            }
            return layout;
        }

        public RowLayout FindRow(int row)
        {
            return base.Find(row);
        }

        public RowLayout FindY(double y)
        {
            return Enumerable.FirstOrDefault<RowLayout>(this, delegate (RowLayout rowLayout) {
                return rowLayout.ContainsY(y);
            });
        }
    }
}

