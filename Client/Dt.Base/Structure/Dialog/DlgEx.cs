#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 对话框扩展方法
    /// </summary>
    public static class DlgEx
    {
        /// <summary>
        /// 显示提示消息
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_content"></param>
        public static void Msg(this FrameworkElement p_target, string p_content)
        {
            ShowMessage(p_target, p_content, Res.BlackBrush);
        }

        /// <summary>
        /// 显示警告信息
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_content"></param>
        public static void Warn(this FrameworkElement p_target, string p_content)
        {
            ShowMessage(p_target, p_content, Res.RedBrush);
        }

        /// <summary>
        /// 显示等待对话框
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_msg"></param>
        /// <param name="p_showVeil"></param>
        /// <returns></returns>
        public static Dlg Busy(this FrameworkElement p_target, string p_msg, bool p_showVeil = true)
        {
            Dlg dlg = new Dlg
            {
                WinPlacement = DlgPlacement.TargetOverlap,
                PhonePlacement = DlgPlacement.TargetOverlap,
                PlacementTarget = p_target,
                HideTitleBar = true,
                IsPinned = true,
                Background = null,
                BorderThickness = new Thickness(0),
                Resizeable = false,
                AutoAdjustPosition = false,
            };

            Grid grid = new Grid();
            if (p_showVeil)
                grid.Background = Res.深暗遮罩;

            var sp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            sp.Children.Add(new ProgressRing { Height = 60, Width = 60, IsActive = true, HorizontalAlignment = HorizontalAlignment.Center });
            sp.Children.Add(new Border
            {
                Background = Res.主蓝,
                Margin = new Thickness(0, 10, 0, 0),
                Padding = new Thickness(10),
                Child = new TextBlock
                {
                    Text = string.IsNullOrEmpty(p_msg) ? "请稍等..." : p_msg,
                    Foreground = Res.WhiteBrush,
                    TextWrapping = TextWrapping.Wrap,
                },
            });
            grid.Children.Add(sp);
            
            dlg.Content = grid;
            dlg.Show();
            return dlg;
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_msg"></param>
        /// <param name="p_brush"></param>
        public static void ShowMessage(FrameworkElement p_target, string p_msg, SolidColorBrush p_brush)
        {
            Dlg dlg = new Dlg
            {
                WinPlacement = DlgPlacement.TargetOuterTop,
                PhonePlacement = DlgPlacement.TargetOuterTop,
                PlacementTarget = p_target,
                HideTitleBar = true,
                Background = null,
                Foreground = Res.WhiteBrush,
                BorderThickness = new Thickness(0),
                Resizeable = false,
                AutoAdjustPosition = false,
            };

            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                }
            };

            // 消息内容
            grid.Children.Add(new Border
            {
                Background = p_brush,
                BorderThickness = new Thickness(0),
                MinHeight = 40,
                MinWidth = 100,
                MaxWidth = 300,
                Child = new TextBlock { Text = p_msg, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(10) },
            });

            // 三角
            Polygon poly = new Polygon
            {
                Points = new PointCollection { new Point(0, 0), new Point(10, 10), new Point(20, 0) },
                Fill = p_brush,
                Width = 18,
                Height = 14,
                HorizontalAlignment = HorizontalAlignment.Left,
                Stretch = Stretch.Fill,
                Margin = new Thickness(18, -2, 0, 0),
            };
            Grid.SetRow(poly, 1);
            grid.Children.Add(poly);

            dlg.Content = grid;
            dlg.Show();
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="p_dlg">对话框</param>
        /// <param name="p_winPlacement">windows模式的显示位置</param>
        /// <param name="p_phonePlacement">phone模式的显示位置</param>
        /// <param name="p_target">采用相对位置显示时的目标元素</param>
        /// <param name="p_hideTitleBar">否隐藏标题栏</param>
        /// <param name="p_isPinned">是否固定对话框</param>
        public static void ShowAt(
            this Dlg p_dlg,
            DlgPlacement p_winPlacement,
            DlgPlacement p_phonePlacement,
            FrameworkElement p_target = null,
            bool p_hideTitleBar = false,
            bool p_isPinned = false)
        {
            if (p_dlg != null)
            {
                p_dlg.WinPlacement = p_winPlacement;
                p_dlg.PhonePlacement = p_phonePlacement;
                if (p_target != null)
                    p_dlg.PlacementTarget = p_target;
                if (p_hideTitleBar)
                    p_dlg.HideTitleBar = true;
                if (p_isPinned)
                    p_dlg.IsPinned = true;
                p_dlg.Show();
            }
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="p_dlg">对话框</param>
        /// <param name="p_winPlacement">windows模式的显示位置</param>
        /// <param name="p_phonePlacement">phone模式的显示位置</param>
        /// <param name="p_target">采用相对位置显示时的目标元素</param>
        /// <param name="p_hideTitleBar">否隐藏标题栏</param>
        /// <param name="p_isPinned">是否固定对话框</param>
        /// <returns></returns>
        public static Task ShowAtAsync(
            this Dlg p_dlg,
            DlgPlacement p_winPlacement,
            DlgPlacement p_phonePlacement,
            FrameworkElement p_target = null,
            bool p_hideTitleBar = false,
            bool p_isPinned = false)
        {
            if (p_dlg != null)
            {
                p_dlg.WinPlacement = p_winPlacement;
                p_dlg.PhonePlacement = p_phonePlacement;
                if (p_target != null)
                    p_dlg.PlacementTarget = p_target;
                if (p_hideTitleBar)
                    p_dlg.HideTitleBar = true;
                if (p_isPinned)
                    p_dlg.IsPinned = true;
                return p_dlg.ShowAsync();
            }
            return Task.CompletedTask;
        }
    }
}
