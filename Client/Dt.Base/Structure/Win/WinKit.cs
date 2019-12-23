#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Tools;
using Dt.Core;
using System.Text.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// UI交互代理类
    /// </summary>
    internal static class WinKit
    {
        static MenuFlyout _menu;

        /// <summary>
        /// Phone模式附加标题右键菜单
        /// </summary>
        /// <param name="p_elem">标题元素</param>
        /// <param name="p_win">所属窗口</param>
        public static void OnPhoneTitleTapped(FrameworkElement p_elem, IWin p_win)
        {
            if (p_elem == null || p_win == null)
                return;

            p_elem.DoubleTapped += (s, e) =>
            {
                if (_menu == null)
                {
                    _menu = new MenuFlyout();
                    _menu.Placement = FlyoutPlacementMode.Bottom;
                    var item = new MenuFlyoutItem { Text = "取消自启动" };
                    item.Command = new BaseCommand((p_params) => LaunchManager.DelAutoStart());
                    _menu.Items.Add(item);
                    _menu.Items.Add(new MenuFlyoutItem { Text = "设置自启动" });
                    item = new MenuFlyoutItem { Text = "系统监视" };
                    item.Command = new BaseCommand((p_params) => SysTrace.ShowBox());
                    _menu.Items.Add(item);
                }

                var autoStart = AtLocal.GetAutoStart();
                if (autoStart != null
                    && autoStart.WinType == p_win.GetType().AssemblyQualifiedName
                    && (p_win.Params == null || autoStart.Params == JsonSerializer.Serialize(p_win.Params, JsonOptions.UnsafeSerializer)))
                {
                    _menu.Items[0].Visibility = Visibility.Visible;
                    _menu.Items[1].Visibility = Visibility.Collapsed;
                }
                else
                {
                    _menu.Items[0].Visibility = Visibility.Collapsed;
                    _menu.Items[1].Visibility = Visibility.Visible;
                    ((MenuFlyoutItem)_menu.Items[1]).Command = new BaseCommand((p_params) => LaunchManager.SetAutoStart(p_win));
                }
                _menu.ShowAt(p_elem);
            };
        }
    }
}
