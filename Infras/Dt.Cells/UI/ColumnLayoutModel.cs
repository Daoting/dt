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
    internal class ColumnLayoutModel : CollectionBase<ColumnLayout>
    {
        public ColumnLayoutModel() : base("Column")
        {
        }

        public ColumnLayout FindColumn(int column)
        {
            return base.Find(column);
        }

        public ColumnLayout FindNearX(double x)
        {
            ColumnLayout layout = null;
            if (base.Count > 0)
            {
                layout = FindX(x);
                if (layout == null)
                {
                    layout = (x < Enumerable.ElementAt<ColumnLayout>((IEnumerable<ColumnLayout>) this, 0).X) ? Enumerable.ElementAt<ColumnLayout>((IEnumerable<ColumnLayout>) this, 0) : Enumerable.ElementAt<ColumnLayout>((IEnumerable<ColumnLayout>) this, base.Count - 1);
                }
            }
            return layout;
        }

        public ColumnLayout FindX(double x)
        {
            return Enumerable.FirstOrDefault<ColumnLayout>(this, delegate (ColumnLayout columnLayout) {
                return columnLayout.ContainsX(x);
            });
        }
    }
}

