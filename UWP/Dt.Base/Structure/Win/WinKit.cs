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
#endregion

namespace Dt.Base
{
    /// <summary>
    /// UI交互代理类
    /// </summary>
    internal static class WinKit
    {
        static Menu _menu;
        static Win _currentWin;

        /// <summary>
        /// Phone模式附加标题右键菜单
        /// </summary>
        /// <param name="p_elem">标题元素</param>
        /// <param name="p_win">所属窗口</param>
        public static void OnPhoneTitleTapped(FrameworkElement p_elem, Win p_win)
        {
            if (p_elem == null || p_win == null)
                return;

            p_elem.RightTapped += (s, e) =>
            {
                if (_menu == null)
                {
                    _menu = new Menu { IsContextMenu = true, Placement = MenuPosition.BottomLeft };
                    var item = new Mi { ID = "取消自启动" };
                    item.Click += (o, a) => Startup.DelAutoStart();
                    _menu.Items.Add(item);

                    item = new Mi { ID = "设置自启动" };
                    item.Click += SetAutoStart;
                    _menu.Items.Add(item);

                    item = new Mi { ID = "系统监视" };
                    item.Click += (o, a) => SysTrace.ShowBox();
                    _menu.Items.Add(item);
                }

                var autoStart = AtState.GetAutoStart();
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
                    _currentWin = p_win;
                }
                _ = _menu.OpenContextMenu(default, p_elem);
            };
        }

        static void SetAutoStart(object sender, Mi e)
        {
            Startup.SetAutoStart(_currentWin);
            _currentWin = null;
        }
    }
}
