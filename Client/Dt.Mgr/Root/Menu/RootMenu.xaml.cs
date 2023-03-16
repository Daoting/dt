#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using System.Reflection;
#endregion

namespace Dt.Mgr.Home
{
    /// <summary>
    /// 根级菜单项页面(带分组)
    /// </summary>
    public sealed partial class RootMenu : Tab
    {
        public RootMenu()
        {
            InitializeComponent();

            if (Kit.IsLogon)
                LoadMenus();
            else
                LoginDs.LoginSuc += LoadMenus;
        }

        async void LoadMenus()
        {
            await MenuDs.LoadMenus();
            _lv.Data = MenuDs.RootPageMenus;
            _miReset.Visibility = (MenuDs.FavMenus.Count > MenuDs.FixedMenusCount) ? Visibility.Visible : Visibility.Collapsed;

            // 只有【常用】组菜单项的显示提示信息
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

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Kit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                    Forward(new GroupMenu(menu));
                else
                    MenuDs.OpenMenu(menu);
            });
        }

        void OnSearch(object sender, Mi e)
        {
            Forward(new SearchMenu());
        }

        async void OnReset(object sender, Mi e)
        {
            if (MenuDs.FavMenus.Count > MenuDs.FixedMenusCount)
            {
                var cnt = await AtLob.Exec($"delete from menufav where userid={Kit.UserID}");
                if (cnt > 0)
                {
                    LoadMenus();
                    Kit.Msg("重置常用菜单成功！");
                }
            }
        }
    }
}
