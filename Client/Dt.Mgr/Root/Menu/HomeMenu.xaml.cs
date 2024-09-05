#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Reflection;
#endregion

namespace Dt.Mgr.Home
{
    /// <summary>
    /// 所有菜单项首页
    /// </summary>
    public sealed partial class HomeMenu : Tab
    {
        public HomeMenu()
        {
            InitializeComponent();

            // 收藏在PhoneUI模式不可用
            if (!Kit.IsPhoneUI)
            {
                var menu = new Menu { new Mi("收藏", Icons.收藏, OnFav) };
                menu.Opening += OnMenuOpening;
                _lv.SetMenu(menu);
            }
        }
        
        /// <summary>
        /// 初始化菜单，可设置固定菜单项
        /// </summary>
        /// <param name="p_fixedMenus">固定菜单项</param>
        public void InitMenu(IList<OmMenu> p_fixedMenus)
        {
            MenuDs.FixedMenus = p_fixedMenus;
            if (!Kit.IsLogon)
                LoginDs.LoginSuc += LoadMenus;
            else
                LoadMenus();
        }

        async void LoadMenus()
        {
            await MenuDs.InitMenus();
            _lv.Data = MenuDs.RootPageMenus;
        }
        
        void OnItemClick(ItemClickArgs e)
        {
            Kit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                {
                    Forward(new GroupMenu(menu));
                }
                else
                {
                    MenuDs.OpenMenu(menu);
                }
            });
        }

        void OnSearch()
        {
            Forward(new SearchMenu());
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

    [LvCall]
    public class MenuUI
    {
        public static void Icon(Env e)
        {
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
            };
            e.UI = tb;

            e.Set += c =>
            {
                var val = c.CellVal;
                string txt = null;
                if (c.Data is OmMenu m && m.IsGroup)
                {
                    txt = Res.GetIconChar(Icons.文件夹);
                    tb.Foreground = Res.BlackBrush;
                }
                else if (val != null)
                {
                    if (val is int || val is byte)
                        txt = Res.GetIconChar((Icons)val);
                    else
                        txt = Res.ParseIconChar(val.ToString());
                }
                tb.Text = string.IsNullOrEmpty(txt) ? "" : txt;
                c.Dot.ToggleVisible(string.IsNullOrEmpty(txt));
            };
        }
    }
}
