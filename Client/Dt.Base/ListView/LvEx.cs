#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-31 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Dt.Base.Docking;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Lv扩展方法
    /// </summary>
    public static class LvEx
    {
        /// <summary>
        /// 增加多选、单选、全选菜单
        /// </summary>
        /// <param name="p_lv"></param>
        /// <param name="p_menu"></param>
        public static void AddMultiSelMenu(this Lv p_lv, Menu p_menu)
        {
            if (p_lv == null || p_menu == null)
                Throw.Msg("附加多选菜单项时参数不可为空！");

            Mi mi = new Mi { ID = "全选", Icon = Icons.正确, Visibility = Visibility.Collapsed };
            mi.Call += () => p_lv.SelectAll();
            p_menu.Items.Add(mi);

            mi = new Mi { ID = "选择", Icon = Icons.全选 };
            mi.Call += () =>
            {
                p_lv.SelectionMode = SelectionMode.Multiple;
                p_menu.Show("全选", "取消");
                p_menu.Hide("选择");
            };
            p_menu.Items.Add(mi);

            mi = new Mi { ID = "取消", Icon = Icons.全选, Visibility = Visibility.Collapsed };
            mi.Call += () =>
            {
                p_lv.SelectionMode = SelectionMode.Single;
                p_menu.Hide("全选", "取消");
                p_menu.Show("选择");
            };
            p_menu.Items.Add(mi);
        }
    }
}
