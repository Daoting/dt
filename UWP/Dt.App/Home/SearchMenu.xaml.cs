#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-11-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Home
{
    /// <summary>
    /// 搜索菜单项
    /// </summary>
    public sealed partial class SearchMenu : Mv
    {
        public SearchMenu()
        {
            InitializeComponent();
            LoadTopBar();
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

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="p_filter"></param>
        void OnSearch(object sender, string p_filter)
        {
            if (string.IsNullOrEmpty(p_filter))
                _lv.Data = null;
            else
                _lv.Data = MenuKit.LoadMenusByName(p_filter.ToLower());
        }

        void LoadTopBar()
        {
            var sb = new Base.SearchBox
            {
                Placeholder = "请输入拼音简码或包含的文字...",
                IsRealtime = true
            };
            sb.Search += OnSearch;

            if (Kit.IsPhoneUI)
            {
                // 隐藏标题栏
                HideTitleBar = true;

                Grid grid = new Grid
                {
                    Background = Res.主蓝,
                    Height = 50,
                    Margin = new Thickness(0, 0, 0, 10),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                };
                var btn = new Button
                {
                    Content = "\uE010",
                    Style = Res.字符按钮,
                    Foreground = Res.WhiteBrush,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Width = 50,
                };
                btn.Click += (s, e) => Backward();
                grid.Children.Add(btn);

                sb.BorderThickness = new Thickness(0);
                sb.Margin = new Thickness(0, 5, 10, 5);
                Grid.SetColumn(sb, 1);
                grid.Children.Add(sb);

                _grid.Children.Add(grid);
            }
            else
            {
                sb.BorderThickness = new Thickness(0, 0, 0, 1);
                _grid.Children.Add(sb);
            }
        }
    }
}
