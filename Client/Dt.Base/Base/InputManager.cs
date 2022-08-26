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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;
using System;
using Microsoft.UI.Xaml.Controls;
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
#if !WIN
            // WinUI中已移除 SystemNavigationManager，删除PhoneUI模式下窗口左上角的后退按钮
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
#else
            // 全局快捷键
            var accelerator = new KeyboardAccelerator()
            {
                Modifiers = VirtualKeyModifiers.Menu,
                Key = VirtualKey.Left
            };
            accelerator.Invoked += (s, e) =>
            {
                // Alt + ← 系统日志
                e.Handled = true;
                SysTrace.ShowBox();
            };
            // 因总有浮动的快捷键提示，放在提示信息层，少烦人！
            UITree.RootGrid.Children[UITree.RootGrid.Children.Count - 1].KeyboardAccelerators.Add(accelerator);
#endif
        }

        /// <summary>
        /// 按下后退按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = UITree.ExistDlg || UITree.RootFrame.CanGoBack;
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
            if (UITree.ExistDlg)
            {
                Dlg dlg = UITree.GetTopDlg() as Dlg;
                // 隐藏标题栏的不自动关闭，控制用户操作对话框内容关闭！
                if (dlg != null && !dlg.HideTitleBar)
                    dlg.Close();
                return;
            }

            var frame = UITree.RootFrame;
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

        #region 辅助键按下情况
        /// <summary>
        /// 获取当前是否按下Ctrl
        /// </summary>
        public static bool IsCtrlPressed
        {
            get { return (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down; }
        }

        /// <summary>
        /// 获取当前是否按下Alt
        /// </summary>
        public static bool IsMenuPressed
        {
            get { return (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down; }
        }

        /// <summary>
        /// 获取当前是否按下Shift
        /// </summary>
        public static bool IsShiftPressed
        {
            get { return (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down; }
        }

        /// <summary>
        /// 获取当前是否按下Windows
        /// </summary>
        public static bool IsWinPressed
        {
            get
            {
                return (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.LeftWindows) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down
                    || (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.RightWindows) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
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

