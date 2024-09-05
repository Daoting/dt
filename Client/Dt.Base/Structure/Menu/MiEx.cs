#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.MenuView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 常用菜单项
    /// </summary>
    public partial class Mi
    {
        /// <summary>
        /// 新菜单项
        /// </summary>
        /// <param name="click">Click事件处理</param>
        /// <param name="call">Call事件处理</param>
        /// <param name="enable">IsEnabled值</param>
        /// <param name="visible">Visibility</param>
        /// <returns></returns>
        public static Mi 增加(Action<Mi> click = null, Action call = null, bool enable = true, bool visible = true)
        {
            return NewMi("增加", Icons.加号, click, call, enable, visible);
        }

        /// <summary>
        /// 新菜单项
        /// </summary>
        /// <param name="click">Click事件处理</param>
        /// <param name="call">Call事件处理</param>
        /// <param name="enable">IsEnabled值</param>
        /// <param name="visible">Visibility</param>
        /// <returns></returns>
        public static Mi 保存(Action<Mi> click = null, Action call = null, bool enable = true, bool visible = true)
        {
            return NewMi("保存", Icons.保存, click, call, enable, visible);
        }

        /// <summary>
        /// 新菜单项
        /// </summary>
        /// <param name="click">Click事件处理</param>
        /// <param name="call">Call事件处理</param>
        /// <param name="enable">IsEnabled值</param>
        /// <param name="visible">Visibility</param>
        /// <returns></returns>
        public static Mi 删除(Action<Mi> click = null, Action call = null, bool enable = true, bool visible = true)
        {
            return NewMi("删除", Icons.删除, click, call, enable, visible);
        }

        /// <summary>
        /// 新菜单项
        /// </summary>
        /// <param name="click">Click事件处理</param>
        /// <param name="call">Call事件处理</param>
        /// <param name="enable">IsEnabled值</param>
        /// <param name="visible">Visibility</param>
        /// <returns></returns>
        public static Mi 编辑(Action<Mi> click = null, Action call = null, bool enable = true, bool visible = true)
        {
            return NewMi("编辑", Icons.修改, click, call, enable, visible);
        }

        /// <summary>
        /// 新菜单项
        /// </summary>
        /// <param name="click">Click事件处理</param>
        /// <param name="call">Call事件处理</param>
        /// <param name="enable">IsEnabled值</param>
        /// <param name="visible">Visibility</param>
        /// <returns></returns>
        public static Mi 撤消(Action<Mi> click = null, Action call = null, bool enable = true, bool visible = true)
        {
            return NewMi("撤消", Icons.撤消, click, call, enable, visible);
        }

        /// <summary>
        /// 新菜单项
        /// </summary>
        /// <param name="click">Click事件处理</param>
        /// <param name="call">Call事件处理</param>
        /// <param name="enable">IsEnabled值</param>
        /// <param name="visible">Visibility</param>
        /// <returns></returns>
        public static Mi 添加(Action<Mi> click = null, Action call = null, bool enable = true, bool visible = true)
        {
            return NewMi("添加", Icons.加号, click, call, enable, visible);
        }

        /// <summary>
        /// 新菜单项
        /// </summary>
        /// <param name="click">Click事件处理</param>
        /// <param name="call">Call事件处理</param>
        /// <param name="enable">IsEnabled值</param>
        /// <param name="visible">Visibility</param>
        /// <returns></returns>
        public static Mi 确定(Action<Mi> click = null, Action call = null, bool enable = true, bool visible = true)
        {
            return NewMi("确定", Icons.正确, click, call, enable, visible);
        }

        /// <summary>
        /// 新菜单项
        /// </summary>
        /// <param name="click">Click事件处理</param>
        /// <param name="call">Call事件处理</param>
        /// <param name="enable">IsEnabled值</param>
        /// <param name="visible">Visibility</param>
        /// <returns></returns>
        public static Mi 停用(Action<Mi> click = null, Action call = null, bool enable = true, bool visible = true)
        {
            return NewMi("停用", Icons.禁, click, call, enable, visible);
        }
        
        static Mi NewMi(string p_id, Icons p_icon, Action<Mi> p_click, Action p_call, bool p_enabled, bool p_visible)
        {
            var mi = new Mi
            {
                ID = p_id,
                Icon = p_icon,
                ShowInPhone = VisibleInPhone.Icon,
            };

            if (p_click != null)
                mi.Click += p_click;
            else if (p_call != null)
                mi.Call += p_call;

            if (!p_enabled)
                mi.IsEnabled = false;
            if (!p_visible)
                mi.Visibility = Visibility.Collapsed;
            return mi;
        }
    }
}
