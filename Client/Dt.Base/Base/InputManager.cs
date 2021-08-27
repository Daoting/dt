#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Tools;
using Dt.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 输入管理类
    /// </summary>
    internal static class InputManager
    {
        /// <summary>
        /// 附加后退键事件
        /// </summary>
        public static void Init()
        {
            var view = SystemNavigationManager.GetForCurrentView();
            view.BackRequested += OnBackRequested;

#if UWP
            if (Kit.IsPhoneUI)
                view.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            // 全局快捷键
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += AcceleratorKeyActivated;
#endif
        }

        /// <summary>
        /// 按下后退按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = SysVisual.ExistDlg || SysVisual.RootFrame.CanGoBack;
            GoBack();
        }

        /// <summary>
        /// 按下后退按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnBackClick(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        /// <summary>
        /// 执行页面后退
        /// </summary>
        /// <returns></returns>
        public static async void GoBack()
        {
            if (SysVisual.ExistDlg)
            {
                Dlg dlg = SysVisual.GetTopDlg() as Dlg;
                // 隐藏标题栏的不自动关闭，控制用户操作对话框内容关闭！
                if (dlg != null && !dlg.HideTitleBar)
                    dlg.Close();
                return;
            }

            var frame = SysVisual.RootFrame;
            if (frame.CanGoBack)
            {
                if (frame.Content is PhonePage page)
                {
                    // 因OnNavigatingFrom中的取消导航无法实现异步！在此处判断
                    if (await page.IsAllowBack())
                        frame.GoBack();
                }
                else
                {
                    frame.GoBack();
                }
            }
        }

        /// <summary>
        /// 处理全局快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if ((args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown || args.EventType == CoreAcceleratorKeyEventType.KeyDown)
                && (Window.Current.CoreWindow.GetKeyState(VirtualKey.Menu) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down
                && args.VirtualKey == VirtualKey.Right)
            {
                // Alt + → 系统监控
                args.Handled = true;
                SysTrace.ShowBox();
            }
        }

        #region 辅助键按下情况
        /// <summary>
        /// 获取当前是否按下Ctrl
        /// </summary>
        public static bool IsCtrlPressed
        {
            get { return (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down; }
        }

        /// <summary>
        /// 获取当前是否按下Alt
        /// </summary>
        public static bool IsMenuPressed
        {
            get { return (Window.Current.CoreWindow.GetKeyState(VirtualKey.Menu) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down; }
        }

        /// <summary>
        /// 获取当前是否按下Shift
        /// </summary>
        public static bool IsShiftPressed
        {
            get { return (Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down; }
        }

        /// <summary>
        /// 获取当前是否按下Windows
        /// </summary>
        public static bool IsWinPressed
        {
            get
            {
                return (Window.Current.CoreWindow.GetKeyState(VirtualKey.LeftWindows) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down
                    || (Window.Current.CoreWindow.GetKeyState(VirtualKey.RightWindows) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            }
        }

        /// <summary>
        /// 获取当前辅助键按下情况
        /// </summary>
        public static VirtualKeyModifiers ModifierKeys
        {
            get
            {
                return (IsMenuPressed ? VirtualKeyModifiers.Menu : VirtualKeyModifiers.None)
                    | (IsCtrlPressed ? VirtualKeyModifiers.Control : VirtualKeyModifiers.None)
                    | (IsShiftPressed ? VirtualKeyModifiers.Shift : VirtualKeyModifiers.None)
                    | (IsWinPressed ? VirtualKeyModifiers.Windows : VirtualKeyModifiers.None);
            }
        }
        #endregion
    }
}

