#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class TvContextMenu : Win
    {
        public TvContextMenu()
        {
            InitializeComponent();
            _tv.Data = TvData.GetTbl();
        }

        void OnBtnEvent(object sender, RoutedEventArgs e)
        {
            var menu = Ex.GetMenu(_tv);
            if (menu != null)
            {
                menu.TriggerEvent = TriggerEvent.Custom;
            }
        }

        void OnRightHolding(object sender, RoutedEventArgs e)
        {
            var menu = Ex.GetMenu(_tv);
            if (menu != null)
            {
                menu.Placement = MenuPosition.Default;
                menu.TriggerEvent = TriggerEvent.RightTapped;
            }
        }

        void OnLeftTap(object sender, RoutedEventArgs e)
        {
            var menu = Ex.GetMenu(_tv);
            if (menu != null)
            {
                menu.Placement = MenuPosition.Default;
                menu.TriggerEvent = TriggerEvent.LeftTapped;
            }
        }

        void OnChangeMenu(object sender, RoutedEventArgs e)
        {
            var menu = new Menu();
            string id = new Random().Next(1, 100).ToString();
            menu.Items.Add(new Mi { ID = "修改" + id, Icon = Icons.修改 });
            menu.Items.Add(new Mi { ID = "搜索" + id, Icon = Icons.搜索 });
            Ex.SetMenu(_tv, menu);
        }

        void OnNoMenu(object sender, RoutedEventArgs e)
        {
            Ex.SetMenu(_tv, null);
        }
    }
}