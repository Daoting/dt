#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;
#endregion

namespace Dt.Mgr.Home
{
    public partial class FavMenu : Tab
    {
        RootMenu _allMenu;

        public FavMenu()
        {
            InitializeComponent();
            _lv.Data = MenuDs.FavMenus;
            _lv.Loaded += (s, e) => RefreshMenuTips();
            Menu.Loaded += MenuLoaded;
        }

        void MenuLoaded(object sender, RoutedEventArgs e)
        {
            Menu.Loaded -= MenuLoaded;
            if (Tag is not MenuHome)
                Menu[1].Visibility = Visibility.Collapsed;
        }

        void RefreshMenuTips()
        {
            if (MenuDs.FavMenus.Count == 0)
                return;

            // 只有收藏的菜单项的显示提示信息
            // 原来采用每个服务批量获取的方式，现改为每个视图独自提供方式，互不影响！
            foreach (var mi in MenuDs.FavMenus)
            {
                MethodInfo method;
                Type tp = Kit.GetViewTypeByAlias(mi.ViewName);
                if (tp != null
                    && (method = tp.GetMethod("AttachMenuTip", BindingFlags.Public | BindingFlags.Static)) != null)
                {
                    // 视图类型中包含静态以下方法，方法原型：
                    // public static void AttachMenuTip(OmMenu om)
                    var pars = method.GetParameters();
                    if (pars.Length == 1 && pars[0].ParameterType == typeof(OmMenu))
                    {
                        try
                        {
                            method.Invoke(null, new object[1] { mi });
                        }
                        catch { }
                    }
                }
            }
        }

        void OnSearch()
        {
            if (Tag is MenuHome home)
            {
                home.Forward(new SearchMenu());
            }
            else
            {
                Forward(new SearchMenu());
            }
        }

        void OnAllMenu()
        {
            if (Tag is MenuHome home)
            {
                if (_allMenu == null)
                    _allMenu = new RootMenu();
                home.Forward(_allMenu);
            }
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (Tag is MenuHome home)
                home.OwnDlg?.Close();
            else
                OwnDlg?.Close();

            Kit.RunAsync(() => MenuDs.OpenMenu((OmMenu)e.Data));
        }

        async void OnTop(object sender, Mi e)
        {
            var om = e.Data.To<OmMenu>();
            var cnt = await AtLob.Exec($"update menufav set dispidx=(select min(dispidx)-1 from menufav) where userid={Kit.UserID} and MenuID={om.ID}");
            if (cnt > 0)
            {
                await MenuDs.LoadFavMenus();
            }
        }

        async void OnBottom(object sender, Mi e)
        {
            var om = e.Data.To<OmMenu>();
            var cnt = await AtLob.Exec($"update menufav set dispidx=(select max(dispidx)+1 from menufav) where userid={Kit.UserID} and MenuID={om.ID}");
            if (cnt > 0)
            {
                await MenuDs.LoadFavMenus();
            }
        }

        async void OnDelFav(object sender, Mi e)
        {
            var om = e.Data.To<OmMenu>();
            var cnt = await AtLob.Exec($"delete from menufav where userid={Kit.UserID} and MenuID={om.ID}");
            if (cnt > 0)
            {
                Kit.Msg($"[{om.Name}] 已取消收藏！");
                await MenuDs.LoadFavMenus();
            }
        }

        void OnMenuOpening(object sender, AsyncCancelEventArgs e)
        {
            Menu menu = (Menu)sender;
            var om = menu.TargetData.To<OmMenu>();
            menu[0].Visibility = MenuDs.FavMenus[0] == om ? Visibility.Collapsed : Visibility.Visible;
            menu[1].Visibility = MenuDs.FavMenus[MenuDs.FavMenus.Count - 1] == om ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}