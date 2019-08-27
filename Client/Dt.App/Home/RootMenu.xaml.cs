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
using System;
using System.Collections.Generic;
using System.Linq;
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
    public sealed partial class RootMenu : UserControl, ITabContent
    {
        List<OmMenu> _fixedMenus;
        DateTime _dtLast;

        public RootMenu()
        {
            InitializeComponent();
            _fixedMenus = MenuKit.DefaultFixedMenus;
            MenuKit.LoadMenus(_fixedMenus);
            _lv.Data = MenuKit.RootPageMenus;
            _lv.Loaded += OnLoaded;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            AtKit.RunAsync(() =>
            {
                OmMenu menu = (OmMenu)e.Data;
                if (menu.IsGroup)
                    Tab.NaviTo(new GroupMenu(menu));
                else
                    MenuKit.OpenMenu(menu);
            });
        }

        void OnSearch(object sender, Mi e)
        {
            Tab.NaviTo(new SearchMenu());
        }

        void OnReset(object sender, Mi e)
        {
            if (MenuKit.FavMenus.Count > _fixedMenus.Count)
            {
                var cnt = AtLocal.Execute($"delete from menufav where userid='{AtUser.ID}'");
                if (cnt > 0)
                {
                    MenuKit.LoadMenus(_fixedMenus);
                    _lv.Data = MenuKit.RootPageMenus;
                    e.Visibility = Visibility.Collapsed;
                    AtKit.Msg("重置常用菜单成功！");
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
            //if ((DateTime.Now - _dtLast).TotalSeconds < 30)
            //    return;

            //_dtLast = DateTime.Now;
            //AtKit.RunAsync(async () =>
            //{
            //    // 只取收藏项的提示信息
            //    var items = new Dictionary<string, List<string>>();
            //    foreach (var mi in AtUser.FavMenus)
            //    {
            //        if (string.IsNullOrEmpty(mi.SrvName))
            //            continue;

            //        List<string> ls;
            //        if (!items.TryGetValue(mi.SrvName, out ls))
            //        {
            //            ls = new List<string>();
            //            items[mi.SrvName] = ls;
            //        }
            //        ls.Add(mi.ID);
            //    }
            //    if (items.Count == 0)
            //        return;

            //    foreach (var item in items)
            //    {
            //        Dict dt = await AtSrv.GetMenuTips(item.Key, item.Value);
            //        if (dt == null || dt.Count == 0)
            //            continue;

            //        foreach (var obj in dt)
            //        {
            //            OmMenu om = (from mi in AtUser.FavMenus
            //                         where mi.ID == obj.Key
            //                         select mi).FirstOrDefault();
            //            if (om == null)
            //                continue;

            //            int num;
            //            if (obj.Value == null || !int.TryParse(obj.Value.ToString(), out num))
            //                om.SetWarningNum(0);
            //            else
            //                om.SetWarningNum(num);
            //        }
            //    }
            //});
        }

        #region ITabContent
        public Tab Tab { get; set; }

        Menu _menu;
        public Menu Menu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = new Menu();
                    Mi mi = new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon };
                    mi.Click += OnSearch;
                    _menu.Items.Add(mi);
                    if (MenuKit.FavMenus.Count > _fixedMenus.Count)
                    {
                        mi = new Mi { ID = "重置常用", Icon = Icons.刷新, ShowInPhone = VisibleInPhone.Icon };
                        mi.Click += OnReset;
                        _menu.Items.Add(mi);
                    }
                }
                return _menu;
            }
        }

        public string Title
        {
            get { return "开始"; }
        }
        #endregion
    }

    class MenuIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Icons icon = Icons.None;
            if (value != null)
                Enum.TryParse<Icons>(value.ToString(), true, out icon);
            string str = AtRes.GetIconChar(icon);
            if (!string.IsNullOrEmpty(str))
                return new TextBlock { Text = str, FontFamily = AtRes.IconFont, FontSize = 30, TextAlignment = TextAlignment.Center };
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class MenuWarningConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value.ToString() == string.Empty)
                return null;

            return new Grid
            {
                Children =
                {
                    new Ellipse { Fill = AtRes.RedBrush, Width = 20, Height = 20},
                    new TextBlock {Text = value.ToString(), Foreground = AtRes.WhiteBrush, FontSize = 12, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
                }
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
