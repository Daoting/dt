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

namespace Dt.Sample
{
    static class CellDemoKit
    {
        public static void OnCellClick(FvCell p_tgtCell, Fv p_fv)
        {
            if (p_fv.Data == p_tgtCell)
                return;

            var items = p_fv.Items;
            using (items.Defer())
            {
                items.Clear();
                FvCell cell = new CBool();
                cell.ID = "IsReadOnly";
                cell.Title = "只读";
                items.Add(cell);

                foreach (var info in p_tgtCell.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    // 可设置属性
                    var attr = (CellParamAttribute)info.GetCustomAttribute(typeof(CellParamAttribute), false);
                    if (attr == null)
                        continue;

                    cell = Fv.CreateCell(info.PropertyType, info.Name);
                    cell.Title = attr.Title;
                    items.Add(cell);
                }
            }
            p_fv.Data = p_tgtCell;
        }

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
