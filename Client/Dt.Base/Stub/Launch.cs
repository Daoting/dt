#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Core;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Dt.Base.Tools;
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
        async Task Launch(string p_launchArgs = null, ShareInfo p_shareInfo = null)
        {
            // 创建可视树
            UITree.Init();

            if (!string.IsNullOrEmpty(p_launchArgs))
            {
                try
                {
                    // 带参数启动
                    _autoStartOnce = JsonSerializer.Deserialize<AutoStartInfo>(p_launchArgs);
                }
                catch { }
            }

            // app已启动过
            if (Kit.IsInited)
            {
                // 带参数启动
                if (_autoStartOnce != null)
                    ShowAutoStartOnce();
                Kit.MainWin.Activate();

                RecvShare(p_shareInfo);
                return;
            }

            try
            {
                // 系统初始化
                await Kit.Init();
                
                // 附加全局按键事件
                InitInput();

                await InitConfig();

                // 由外部控制启动过程
                await OnStartup();

                // 接收分享
                RecvShare(p_shareInfo);

                // 注册后台任务
                BgJob.Register();

#if WIN && !DEBUG
                if (Kit.IsUsingSvc)
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
        /// 启动过程中显示错误信息，此时未加载任何UI
        /// </summary>
        /// <param name="p_error"></param>
        static void ShowError(string p_error)
        {
            var dlg = new Dlg { IsPinned = true, Resizeable = false, HideTitleBar = true, ShowVeil = false, Background = Res.主蓝 };
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

        /// <summary>
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        void RecvShare(ShareInfo p_info)
        {
            if (p_info != null)
            {
                var svc = SvcProvider.GetService<IReceiveShare>();
                if (svc != null)
                    svc.OnReceive(p_info);
            }
        }

        void ShowAutoStartOnce()
        {
            Win win = AutoStartKit.CreateAutoStartWin(_autoStartOnce);
            if (win != null)
            {
                if (Kit.IsPhoneUI)
                    win.NaviToHome();
                else
                    Desktop.Inst.ShowNewWin(win);
            }
            _autoStartOnce = null;
        }

        /// <summary>
        /// 附加后退键、快捷键事件
        /// </summary>
        void InitInput()
        {
#if !WIN
            // WinUI中已移除 SystemNavigationManager，删除PhoneUI模式下窗口左上角的后退按钮
            SystemNavigationManager.GetForCurrentView().BackRequested += InputKit.OnBackRequested;
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
        /// 留给 LobStub 初始化用
        /// </summary>
        /// <returns></returns>
        protected virtual Task InitConfig()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stub新实例启动
        /// </summary>
        /// <returns></returns>
        protected sealed override async Task OnReboot()
        {
            await InitConfig();
            await OnStartup();
        }
    }
}