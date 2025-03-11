#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using System.Reflection;
#endregion

namespace Dt.Base
{
    public static class FvDesignKit
    {
        /// <summary>
        /// 加载单元格属性面板
        /// </summary>
        /// <param name="p_tgtCell">目标单元格</param>
        /// <param name="p_fv">属性面板</param>
        public static void LoadCellProperties(object p_tgtCell, Fv p_fv)
        {
            if (p_tgtCell == null || p_fv == null)
                return;
            
            if (p_fv.Data == p_tgtCell)
                return;

            // 同类型直接切换数据源
            if (p_fv.Data != null && p_fv.Data.GetType() == p_tgtCell.GetType())
            {
                p_fv.Data = p_tgtCell;
                return;
            }
            
            var items = p_fv.Items;
            using (items.Defer())
            {
                items.Clear();

                if (p_tgtCell is FvCell fc)
                {
                    FvCell cell = new CTip();
                    cell.ID = "ID";
                    items.Add(cell);

                    cell = new CText();
                    cell.ID = "Title";
                    cell.Title = "标题";
                    items.Add(cell);

                    List<FvCell> bs = new List<FvCell>();
                    foreach (var info in p_tgtCell.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        // 可设置属性
                        var attr = (CellParamAttribute)info.GetCustomAttribute(typeof(CellParamAttribute), false);
                        if (attr == null)
                            continue;

                        cell = Fv.CreateCell(info.PropertyType, info.Name);
                        cell.Title = attr.Title;
                        if (info.PropertyType == typeof(bool))
                        {
                            cell.ShowTitle = false;
                            cell.ColSpan = 0.5;
                            bs.Add(cell);
                        }
                        else
                        {
                            items.Add(cell);
                        }
                    }

                    if (bs.Count > 0)
                    {
                        foreach (var item in bs)
                        {
                            items.Add(item);
                        }
                    }
                    
                    cell = new CText { TitleWidth = 240 };
                    cell.ID = "Call";
                    cell.Title = "取值赋值类名(Call)";
                    items.Add(cell);

                    cell = new CNum { IsInteger = true, TitleWidth = 240 };
                    cell.ID = "RowSpan";
                    cell.Title = "占用行数，-1自动行高";
                    items.Add(cell);

                    cell = new CNum { TitleWidth = 240 };
                    cell.ID = "ColSpan";
                    cell.Title = "列宽占比0~1，0填充，1整列";
                    items.Add(cell);

                    cell = new CNum { IsInteger = true, TitleWidth = 240 };
                    cell.ID = "TitleWidth";
                    cell.Title = "标题宽度，默认放6个中文字";
                    items.Add(cell);

                    cell = new CBool { ShowTitle = false, ColSpan = 0.5 };
                    cell.ID = "ShowTitle";
                    cell.Title = "显示标题";
                    items.Add(cell);

                    cell = new CBool { ShowTitle = false, ColSpan = 0.5 };
                    cell.ID = "IsReadOnly";
                    cell.Title = "只读";
                    items.Add(cell);

                    cell = new CBool { ShowTitle = false, ColSpan = 0.5 };
                    cell.ID = "IsVerticalTitle";
                    cell.Title = "垂直显示标题";
                    items.Add(cell);

                    cell = new CBool { ShowTitle = false, ColSpan = 0.5 };
                    cell.ID = "AutoCookie";
                    cell.Title = "自动加载历史值";
                    items.Add(cell);
                }
                else
                {
                    var cell = new CText();
                    cell.ID = "Title";
                    cell.Title = "标题";
                    items.Add(cell);
                }
            }
            p_fv.Data = p_tgtCell;
        }
    }
}