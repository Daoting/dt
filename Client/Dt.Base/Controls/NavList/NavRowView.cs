#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 根据数据源自动调整显示内容
    /// </summary>
    class NavRowView : IRowView
    {
        NavList _owner;

        public NavRowView(NavList p_owner)
        {
            _owner = p_owner;
        }

        public UIElement Create(LvItem p_item)
        {
            // NavList仅支持Nav类型的数据源
            if (p_item.Data is Nav nav)
            {
                if (_owner.ViewMode == NavViewMode.List)
                    return CreateListRow(nav);
                return CreateTileRow(nav);
            }
            return null;
        }

        UIElement CreateListRow(Nav p_nav)
        {
            Grid grid = new Grid { Padding = new Thickness(10), MinHeight = 60 };

            if (!string.IsNullOrEmpty(p_nav.Title) && !string.IsNullOrEmpty(p_nav.Desc))
            {
                var sp = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                sp.Children.Add(CreateTitle(p_nav));
                sp.Children.Add(CreateDesc(p_nav));
                grid.Children.Add(sp);
            }
            else if (!string.IsNullOrEmpty(p_nav.Title))
            {
                grid.Children.Add(CreateTitle(p_nav));
            }
            else if (!string.IsNullOrEmpty(p_nav.Desc))
            {
                grid.Children.Add(CreateDesc(p_nav));
            }

            if (p_nav.Icon != Icons.None)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                Grid.SetColumn(grid.Children[0] as FrameworkElement, 1);

                grid.Children.Add(new TextBlock
                {
                    Text = Res.GetIconChar(p_nav.Icon),
                    FontFamily = Res.IconFont,
                    FontSize = 30,
                    Margin = new Thickness(0, 0, 10, 0),
                    Foreground = Res.主蓝,
                    VerticalAlignment = VerticalAlignment.Center,
                });
            }

            if (!string.IsNullOrEmpty(p_nav.Warning))
            {
                var g = CreateWarning(p_nav);
                grid.Children.Add(g);
                if (p_nav.Icon != Icons.None)
                    Grid.SetColumn(g, 1);
            }
            return grid;
        }

        UIElement CreateTileRow(Nav p_nav)
        {
            Grid grid = new Grid { Padding = new Thickness(12), MinHeight = 80 };

            var sp = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            if (p_nav.Icon != Icons.None)
            {
                sp.Children.Add(new TextBlock
                {
                    Text = Res.GetIconChar(p_nav.Icon),
                    FontFamily = Res.IconFont,
                    FontSize = 30,
                    Margin = new Thickness(0, 0, 0, 4),
                    Foreground = Res.主蓝,
                    HorizontalAlignment = HorizontalAlignment.Center,
                });
            }

            if (!string.IsNullOrEmpty(p_nav.Title))
            {
                var title = CreateTitle(p_nav);
                title.HorizontalAlignment = HorizontalAlignment.Center;
                sp.Children.Add(title);
            }

            if (!string.IsNullOrEmpty(p_nav.Desc))
            {
                var desc = CreateDesc(p_nav);
                desc.HorizontalAlignment = HorizontalAlignment.Center;
                sp.Children.Add(desc);
            }
            grid.Children.Add(sp);

            if (!string.IsNullOrEmpty(p_nav.Warning))
            {
                var g = CreateWarning(p_nav);
                grid.Children.Add(g);
            }
            return grid;
        }

        TextBlock CreateTitle(Nav p_nav)
        {
            return new TextBlock
            {
                Text = p_nav.Title,
                FontSize = 16d,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextWrapping = TextWrapping.Wrap,
            };
        }

        TextBlock CreateDesc(Nav p_nav)
        {
            return new TextBlock
            {
                Text = p_nav.Desc,
                FontSize = Res.小字,
                Foreground = Res.深灰2,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextWrapping = TextWrapping.Wrap,
            };
        }

        Grid CreateWarning(Nav p_nav)
        {
            var g = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Children = { new Ellipse { Fill = Res.RedBrush, Width = 23, Height = 23 } }
            };
            var tb = new TextBlock
            {
                Text = p_nav.Warning.Length > 2 ? "┅" : p_nav.Warning,
                Foreground = Res.WhiteBrush,
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            g.Children.Add(tb);
            return g;
        }
    }
}