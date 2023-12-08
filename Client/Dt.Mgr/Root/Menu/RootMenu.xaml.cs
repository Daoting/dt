#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;
#endregion

namespace Dt.Mgr.Home
{
    /// <summary>
    /// 根级菜单项页面(带分组)
    /// </summary>
    public sealed partial class RootMenu : Tab
    {
        FavMenu _favMenu;

        public RootMenu()
        {
            InitializeComponent();
            _lv.Data = MenuDs.RootPageMenus;
            Menu.Loaded += MenuLoaded;
        }

        void MenuLoaded(object sender, RoutedEventArgs e)
        {
            Menu.Loaded -= MenuLoaded;
            if (Tag is not MenuHome)
                Menu[1].Visibility = Visibility.Collapsed;
        }

        void OnItemClick(ItemClickArgs e)
        {
            if (!Kit.IsPhoneUI)
            {
                if (Tag is MenuHome home)
                    home.OwnDlg?.Close();
                else
                    OwnDlg?.Close();
            }

            Kit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                    Forward(new GroupMenu(menu));
                else
                    MenuDs.OpenMenu(menu);
            });
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

        void OnFav()
        {
            if (Tag is MenuHome home)
            {
                if (_favMenu == null)
                    _favMenu = new FavMenu();
                home.Forward(_favMenu);
            }
        }

        void OnMenuOpening(object sender, AsyncCancelArgs e)
        {
            DoMenuOpening(sender, e);
        }

        void OnFav(Mi e)
        {
            DoFavMenu(e);
        }

        internal static async void DoMenuOpening(object sender, AsyncCancelArgs e)
        {
            Menu menu = (Menu)sender;
            var om = menu.TargetData.To<OmMenu>();
            if (om.IsGroup)
            {
                e.Cancel = true;
                return;
            }

            using (e.Wait())
            {
                int cnt = await MenuFavX.GetCount($"where userid={Kit.UserID} and MenuID={om.ID}");
                menu[0].ID = cnt > 0 ? "取消收藏" : "收藏";
            }
        }

        internal static async void DoFavMenu(Mi e)
        {
            var om = e.Data.To<OmMenu>();
            if (e.ID == "收藏")
            {
                int idx = await AtLob.GetScalar<int>("select max(dispidx) from menufav");
                int cnt = await AtLob.Exec($"insert into menufav (userid, menuid, dispidx) values ({Kit.UserID}, {om.ID}, {idx + 1})");
                if (cnt > 0)
                {
                    Kit.Msg($"[{om.Name}] 收藏成功！");
                    await MenuDs.LoadFavMenus();
                }
            }
            else
            {
                var cnt = await AtLob.Exec($"delete from menufav where userid={Kit.UserID} and MenuID={om.ID}");
                if (cnt > 0)
                {
                    Kit.Msg($"[{om.Name}] 已取消收藏！");
                    await MenuDs.LoadFavMenus();
                }
            }
        }
    }
}
