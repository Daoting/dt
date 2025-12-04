#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Tools;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Serilog.Extensions.ElapsedTime;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Windows.System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 启动入口
    /// </summary>
    public partial class DefaultStub : Stub
    {
        /// <summary>
        /// 启动入口
        /// App.OnLaunched -> DefaultStub.OnLaunched -> DefaultStub.Launch
        /// </summary>
        /// <param name="p_launchArgs"></param>
        /// <param name="p_shareInfo"></param>
        /// <returns></returns>
        [UnconditionalSuppressMessage("AOT", "IL3050")]
        async Task Launch(string p_launchArgs = null, ShareInfo p_shareInfo = null)
        {
            if (!string.IsNullOrEmpty(p_launchArgs))
            {
                try
                {
                    // 带参数启动
                    DefUICallback._autoStartOnce = JsonSerializer.Deserialize<AutoStartInfo>(p_launchArgs);
                }
                catch { }
            }

            // app已启动过
            if (_isInited)
            {
                // 带参数启动
                if (DefUICallback._autoStartOnce != null)
                    ShowAutoStartOnce();
                Kit.MainWin.Activate();

                RecvShare(p_shareInfo);
                return;
            }
            
            try
            {
                // 附加全局按键事件
                InitInput();
                await Kit.OnLaunched();
                await InitConfig();
                _isInited = true;
            }
            catch (Exception ex)
            {
                OnInitFailed(ex);
                return;
            }

            try
            {
                // 由外部控制启动过程
                await OnStartup();

                // 接收分享
                RecvShare(p_shareInfo);

                // 注册后台任务
                BgJob.Register();

#if WIN && !DEBUG
                if (At.Framework == AccessType.Service)
                {
                    // win应用自动更新，间隔3秒再检查更新
                    await Task.Delay(3000).ContinueWith((s) => WinPkgUpdate.CheckUpdate());
                }
#endif
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        void RecvShare(ShareInfo p_info)
        {
            if (p_info != null)
            {
                var svc = Kit.GetService<IReceiveShare>();
                if (svc != null)
                    svc.OnReceive(p_info);
            }
        }

        void ShowAutoStartOnce()
        {
            Win win = AutoStartKit.CreateAutoStartWin(DefUICallback._autoStartOnce);
            if (win != null)
            {
                if (Kit.IsPhoneUI)
                    win.NaviToHome();
                else
                    Desktop.Inst.ShowNewWin(win);
            }
            DefUICallback._autoStartOnce = null;
        }

        /// <summary>
        /// 附加后退键、快捷键事件
        /// </summary>
        void InitInput()
        {
#if ANDROID || IOS
            // WinUI中已移除 SystemNavigationManager，删除PhoneUI模式下窗口左上角的后退按钮
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += InputKit.OnBackRequested;
#elif WIN
            // 1. 除win外，其他平台暂不支持 UIElement.KeyboardAccelerators：https://github.com/unoplatform/uno/issues/13341
            // 2. WinUI的 Window.CoreWindow 始终为null
            // 因此全局快捷键只能分开处理

            var accelerator = new KeyboardAccelerator()
            {
                Modifiers = VirtualKeyModifiers.Control,
                Key = VirtualKey.Enter
            };
            accelerator.Invoked += (s, e) =>
            {
                // Ctrl + 回车 系统
                e.Handled = true;
                SysTrace.ShowSysBox();
            };
            // 因总有浮动的快捷键提示，放在提示信息层，少烦人！
            var accs = UITree.RootGrid.Children[UITree.RootGrid.Children.Count - 1].KeyboardAccelerators;
            accs.Add(accelerator);

            accelerator = new KeyboardAccelerator()
            {
                Modifiers = VirtualKeyModifiers.Control,
                Key = VirtualKey.Left
            };
            accelerator.Invoked += (s, e) =>
            {
                // Ctrl + ← 动态日志
                e.Handled = true;
                SysTrace.ShowLogBox();
            };
            accs.Add(accelerator);

            accelerator = new KeyboardAccelerator()
            {
                Modifiers = VirtualKeyModifiers.Control,
                Key = VirtualKey.Right
            };
            accelerator.Invoked += (s, e) =>
            {
                // Ctrl + → 打开LocalState路径
                e.Handled = true;
                SysTrace.OpenLocalPath();
            };
            accs.Add(accelerator);
            
#else
            // linux wasm wpf的全局快捷键，Window.CoreWindow 始终为null
            //UITree.MainWin.CoreWindow.KeyDown += OnGlobalKeyDown;
            UITree.ContentBorder.AddHandler(Border.KeyDownEvent, (KeyEventHandler)OnGlobalKeyDown, true);
#endif
        }

        void OnGlobalKeyDown(object sender, KeyRoutedEventArgs e)
        {
            // 全局快捷键辅助键 Ctrl
            if (!InputKit.IsCtrlPressed)
                return;

            switch (e.Key)
            {
                case VirtualKey.Enter:
                    // Ctrl + 回车 系统
                    e.Handled = true;
                    SysTrace.ShowSysBox();
                    break;

                case VirtualKey.Left:
                    // Ctrl + ← 动态日志
                    e.Handled = true;
                    SysTrace.ShowLogBox();
                    break;

                case VirtualKey.Right:
                    // Ctrl + → 打开LocalState路径
                    e.Handled = true;
                    SysTrace.OpenLocalPath();
                    break;
            }
        }

        /// <summary>
        /// 留给 LobStub 初始化用
        /// </summary>
        /// <returns></returns>
        protected virtual Task InitConfig()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化失败，如：服务器连接失败
        /// </summary>
        /// <param name="p_ex"></param>
        protected virtual void OnInitFailed(Exception p_ex)
        {
            ShowError(p_ex.Message);
        }
        
        #region 静态方法
        /// <summary>
        /// 启动过程中显示错误信息，此时未加载任何UI
        /// </summary>
        /// <param name="p_error"></param>
        public static void ShowError(string p_error)
        {
            var dlg = new Dlg { IsPinned = true, Resizeable = false, HideTitleBar = true, ShowVeil = false, Background = Kit.ThemeBrush };
            if (!Kit.IsPhoneUI)
            {
                dlg.WinPlacement = DlgPlacement.CenterScreen;
                dlg.MinWidth = 300;
                dlg.MaxWidth = Kit.ViewWidth / 4;
                dlg.BorderThickness = new Thickness(0);
            }
            var pnl = new StackPanel { Margin = new Thickness(40), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            pnl.Children.Add(new TextBlock { Text = "\uE037", FontFamily = Res.IconFont, Foreground = Res.WhiteBrush, FontSize = 40, Margin = new Thickness(0, 0, 0, 10), HorizontalAlignment = HorizontalAlignment.Center });
            pnl.Children.Add(new TextBlock { Text = p_error, Foreground = Res.WhiteBrush, FontSize = 20, TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center });
            dlg.Content = pnl;
            dlg.Show();
        }

        public static void Once()
        {
            DefUICallback._notifyList.ItemsChanged += DefUICallback.OnNotifyItemsChanged;
        }
        #endregion

        bool _isInited;
    }
}