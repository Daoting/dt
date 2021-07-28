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
    public partial class LvContextMenu : Win
    {
        public LvContextMenu()
        {
            InitializeComponent();
            _lv.View = Resources["ListView"];
            _lv.Data = SampleData.CreatePersonsTbl(100);
        }

        void OnGridView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TableView"], ViewMode.Table);
        }

        void OnListView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["ListView"], ViewMode.List);
        }

        void OnFormList(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TableView"], ViewMode.List);
        }

        void OnTileView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TileView"], ViewMode.Tile);
        }

        void OnBtnEvent(object sender, RoutedEventArgs e)
        {
            var menu = Ex.GetMenu(_lv);
            if (menu != null)
            {
                menu.TriggerEvent = TriggerEvent.Custom;
            }
        }

        void OnRightHolding(object sender, RoutedEventArgs e)
        {
            var menu = Ex.GetMenu(_lv);
            if (menu != null)
            {
                menu.Placement = MenuPosition.Default;
                // 因TriggerEvent默认值RightTapped，不触发变化事件，先设置Custom
                menu.TriggerEvent = TriggerEvent.Custom;
                menu.TriggerEvent = TriggerEvent.RightTapped;
            }
        }

        void OnLeftTap(object sender, RoutedEventArgs e)
        {
            var menu = Ex.GetMenu(_lv);
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
            Ex.SetMenu(_lv, menu);
        }

        void OnNoMenu(object sender, RoutedEventArgs e)
        {
            Ex.SetMenu(_lv, null);
        }
    }
}