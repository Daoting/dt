#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
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
        DateTime _dtLast;

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
            _lv.Loaded += OnLoaded;

            if (MenuDs.FavMenus.Count > MenuDs.FixedMenusCount)
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
                    await MenuDs.LoadMenus();
                    _lv.Data = MenuDs.RootPageMenus;
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
            if ((DateTime.Now - _dtLast).TotalSeconds < 15)
                return;

            _dtLast = DateTime.Now;
            Kit.RunAsync(async () =>
            {
                // 只取常用组菜单项的提示信息
                // 原来采用每个服务批量获取的方式，现改为每个视图独自提供方式，互不影响！
                foreach (var mi in MenuDs.FavMenus)
                {
                    MethodInfo method;
                    Type tp = Kit.GetViewTypeByAlias(mi.ViewName);
                    if (tp != null
                        && (method = tp.GetMethod("GetMenuTip", BindingFlags.Public | BindingFlags.Static)) != null
                        && method.ReturnType == typeof(Task<int>))
                    {
                        // 视图类型中包含静态以下方法
                        // 方法原型： public static Task<int> GetMenuTip()
                        try
                        {
                            var task = (Task<int>)method.Invoke(null, new object[0]);
                            int num = await task;
                            mi.SetWarningNum(num);
                        }
                        catch { }
                    }
                }
            });
        }
    }
}
