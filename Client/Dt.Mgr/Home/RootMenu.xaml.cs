#region 文件描述
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
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Mgr.Home
{
    /// <summary>
    /// 根级菜单项页面(带分组)
    /// </summary>
    public sealed partial class RootMenu : Mv
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

            if (MenuKit.FavMenus.Count > MenuKit.FixedMenusCount)
                _miReset.Visibility = Visibility.Visible;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Kit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                    Forward(new GroupMenu(menu));
                else
                    MenuKit.OpenMenu(menu);
            });
        }

        void OnSearch(object sender, Mi e)
        {
            Forward(new SearchMenu());
        }

        async void OnReset(object sender, Mi e)
        {
            if (MenuKit.FavMenus.Count > MenuKit.FixedMenusCount)
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

                    var strs = mi.SvcName.Split(':');
                    if (strs.Length == 2)
                    {
                        int num = await Kit.Rpc<int>(strs[0], strs[1], mi.ID, Kit.UserID);
                        mi.SetWarningNum(num);
                    }
                    
                }
            });
        }
    }
}
