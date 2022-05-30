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
using System.Collections.Generic;
#endregion

namespace Dt.Cells.UI
{
    internal class FilterButtonInfoModel : List<FilterButtonInfo>
    {
        public FilterButtonInfo Find(int row, int column, SheetArea sheetArea)
        {
            for (int i = 0; i < base.Count; i++)
            {
                FilterButtonInfo info = base[i];
                if (((info != null) && (info.Row == row)) && ((info.Column == column) && (info.SheetArea == sheetArea)))
                {
                    return info;
                }
            }
            return null;
        }
    }
}

