#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System.Reflection;
#endregion

namespace Demo.UI
{
    static class CellDemoKit
    {
        public static void OnChanged(Fv p_fv, ICell p_tgtCell)
        {
            FvCell cell = p_fv[p_tgtCell.ID];
            if (cell != null)
                Kit.Msg($"{cell.Title}：{(p_tgtCell.Val != null ? p_tgtCell.Val : "空")}");
            else
                Kit.Msg($"{p_tgtCell.ID}：{(p_tgtCell.Val != null ? p_tgtCell.Val : "空")}");
        }
    }
}
