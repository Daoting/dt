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
    /// 静态方法
    /// </summary>
    public partial class Menu
    {
        /// <summary>
        /// 创建新菜单并添加菜单项
        /// </summary>
        /// <param name="p_arr">菜单项</param>
        /// <returns></returns>
        public static Menu New(params Mi[] p_arr)
        {
            Menu menu = new Menu();
            menu.Items.AddRange(p_arr);
            return menu;
        }
    }
}
