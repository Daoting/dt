﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using Dt.Core.Rpc;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.App.Home
{
    /// <summary>
    /// 根级菜单项页面(带分组)
    /// </summary>
    public sealed partial class RootMenu : UserControl, INaviContent
    {
        DateTime _dtLast;

        public RootMenu()
        {
            InitializeComponent();

            if (Kit.IsLogon)
                LoadMenus();
            else
                Kit.LoginSuc += LoadMenus;
        }

        async void LoadMenus()
        {
            await MenuKit.LoadMenus();
            _lv.Data = MenuKit.RootPageMenus;
            _lv.Loaded += OnLoaded;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Kit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                    _host.NaviTo(new GroupMenu(menu));
                else
                    MenuKit.OpenMenu(menu);
            });
        }

        void OnSearch(object sender, Mi e)
        {
            _host.NaviTo(new SearchMenu());
        }

        async void OnReset(object sender, Mi e)
        {
            int cntFixed = Kit.Stub.FixedMenus == null ? 0 : Kit.Stub.FixedMenus.Count;
            if (MenuKit.FavMenus.Count > cntFixed)
            {
                var cnt = AtState.Exec($"delete from menufav where userid={Kit.UserID}");
                if (cnt > 0)
                {
                    await MenuKit.LoadMenus();
                    _lv.Data = MenuKit.RootPageMenus;
                    e.Visibility = Visibility.Collapsed;
                    Kit.Msg("重置常用菜单成功！");
                }
            }
        }

        /// <summary>
        /// 每次加载时刷新提示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 过滤太频繁的刷新
            if ((DateTime.Now - _dtLast).TotalSeconds < 30)
                return;

            _dtLast = DateTime.Now;
            Kit.RunAsync(async () =>
            {
                // 只取常用组菜单项的提示信息
                // 原来采用每个服务批量获取的方式，现改为简单方式，互不影响！
                foreach (var mi in MenuKit.FavMenus)
                {
                    if (string.IsNullOrEmpty(mi.SvcName))
                        continue;

                    int num = await new UnaryRpc(mi.SvcName, "Entry.GetMenuTip", mi.ID, Kit.UserID).Call<int>();
                    mi.SetWarningNum(num);
                }
            });
        }

        #region INaviContent
        INaviHost _host;

        void INaviContent.AddToHost(INaviHost p_host)
        {
            _host = p_host;
            _host.Title = "开始";

            var menu = new Menu();
            Mi mi = new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon };
            mi.Click += OnSearch;
            menu.Items.Add(mi);

            int cntFixed = Kit.Stub.FixedMenus == null ? 0 : Kit.Stub.FixedMenus.Count;
            if (MenuKit.FavMenus.Count > cntFixed)
            {
                mi = new Mi { ID = "重置常用", Icon = Icons.刷新, ShowInPhone = VisibleInPhone.Icon };
                mi.Click += OnReset;
                menu.Items.Add(mi);
            }
            _host.Menu = menu;
        }
        #endregion
    }
}
