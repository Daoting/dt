#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Tools;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation.Collections;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 提示信息相关
    /// </summary>
    partial class DefUICallback : IUICallback
    {
        /// <summary>
        /// 显示确认对话框
        /// </summary>
        /// <param name="p_content">消息内容</param>
        /// <param name="p_title">标题</param>
        /// <returns>true表确认</returns>
        public Task<bool> Confirm(string p_content, string p_title)
        {
            var dispatcher = UITree.MainWin.DispatcherQueue;
            if (dispatcher.HasThreadAccess)
            {
                return ConfirmInternal(p_content, p_title);
            }

            var taskSrc = new TaskCompletionSource<bool>();
            dispatcher.TryEnqueue(async () =>
            {
                var ok = await ConfirmInternal(p_content, p_title);
                taskSrc.TrySetResult(ok);
            });
            taskSrc.Task.Wait();
            return Task.FromResult(taskSrc.Task.Result);
        }

        Task<bool> ConfirmInternal(string p_content, string p_title)
        {
            var dlg = new Dlg { Title = p_title, IsPinned = true };
            if (Kit.IsPhoneUI)
            {
                dlg.PhonePlacement = DlgPlacement.CenterScreen;
                dlg.Width = Kit.ViewWidth - 40;
            }
            else
            {
                dlg.WinPlacement = DlgPlacement.CenterScreen;
                dlg.MinWidth = 300;
                dlg.MaxWidth = Kit.ViewWidth / 4;
                dlg.ShowVeil = true;
            }
            Grid grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            TextBlock tb = new TextBlock { Text = p_content, TextWrapping = TextWrapping.Wrap };
            grid.Children.Add(tb);

            StackPanel spBtn = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 20, 0, 0), HorizontalAlignment = HorizontalAlignment.Right };
            var btn = new Button { Content = "确认", Margin = new Thickness(0, 0, 20, 0) };
            btn.Click += (s, e) => dlg.Close(true);
            spBtn.Children.Add(btn);
            btn = new Button { Content = "取消" };
            btn.Click += (s, e) => dlg.Close(false);
            spBtn.Children.Add(btn);
            Grid.SetRow(spBtn, 1);
            grid.Children.Add(spBtn);
            dlg.Content = grid;
            return dlg.ShowAsync();
        }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="p_content">消息内容</param>
        /// <param name="p_title">标题</param>
        public void Error(string p_content, string p_title)
        {
            Kit.RunAsync(() =>
            {
                var dlg = new Dlg { Title = p_title, IsPinned = true };
                if (Kit.IsPhoneUI)
                {
                    dlg.PhonePlacement = DlgPlacement.CenterScreen;
                    dlg.Width = Kit.ViewWidth - 40;
                }
                else
                {
                    dlg.WinPlacement = DlgPlacement.CenterScreen;
                    dlg.MinWidth = 300;
                    dlg.MaxWidth = Kit.ViewWidth / 4;
                    dlg.ShowVeil = true;
                }
                Grid grid = new Grid { Margin = new Thickness(20), VerticalAlignment = VerticalAlignment.Center };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });
                grid.Children.Add(new TextBlock { Text = "\uE037", FontFamily = Res.IconFont, Foreground = Res.RedBrush, FontSize = 30, Margin = new Thickness(0, 0, 10, 0), });
                var tb = new TextBlock { Text = p_content, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(tb, 1);
                grid.Children.Add(tb);
                dlg.Content = grid;
                dlg.Show();
            });
        }

        /// <summary>
        /// 根据窗口/视图类型和参数激活旧窗口、打开新窗口 或 自定义启动(IView)
        /// </summary>
        /// <param name="p_type">窗口/视图类型</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">初始参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public object OpenWin(Type p_type, string p_title, Icons p_icon, object p_params)
        {
            Throw.IfNull(p_type, "待显示的窗口类型不可为空！");

            // 激活旧窗口，比较窗口类型和初始参数
            Win win;
            Desktop desktop = Desktop.Inst;
            if (!Kit.IsPhoneUI
                && desktop != null
                && (win = desktop.ActiveWin(p_type, p_params)) != null)
            {
                return win;
            }

            // 打开新窗口
            if (p_type.IsSubclassOf(typeof(Win)))
            {
                if (p_params == null)
                    win = (Win)Activator.CreateInstance(p_type);
                else
                    win = (Win)Activator.CreateInstance(p_type, p_params);

                if (string.IsNullOrEmpty(win.Title) && string.IsNullOrEmpty(p_title))
                    win.Title = "无标题";
                else if (!string.IsNullOrEmpty(p_title))
                    win.Title = p_title;

                if (p_icon != Icons.None)
                    win.Icon = p_icon;

                // 记录初始参数，设置自启动和win模式下识别窗口时使用
                if (p_params != null)
                    win.Params = p_params;

                if (Kit.IsPhoneUI)
                {
                    win.NaviToHome();
                }
                else if (desktop != null)
                {
                    desktop.ShowNewWin(win);
                }
                else
                {
                    // 无桌面时用对话框显示
                    var dlg = new Dlg { IsPinned = true };
#if WIN
                    // 空出主窗口的标题栏
                    dlg.Top = 50;
                    dlg.Height = Kit.ViewHeight - 50;
#endif
                    dlg.LoadWin(win);
                    dlg.Show();
                }
                return win;
            }

            // 处理自定义启动情况
            if (p_type.GetInterface("IView") == typeof(IView))
            {
                IView viewer = Activator.CreateInstance(p_type) as IView;
                viewer.Run(p_title, p_icon, p_params);
                return viewer;
            }

            Kit.Msg("打开窗口失败，窗口类型继承自Win或实现IView接口！");
            return null;
        }

        /// <summary>
        /// 显示系统日志窗口
        /// </summary>
        public void ShowLogBox()
        {
            SysTrace.ShowLogBox();
        }

        /// <summary>
        /// WinUI模式 和 PhoneUI模式切换
        /// </summary>
        public void OnUIModeChanged()
        {
            // WinUI中已移除 SystemNavigationManager
            //#if WIN
            //            // 显示/隐藏后退按钮
            //            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Kit.IsPhoneUI ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            //#endif

            // 重构根元素，将切换前的窗口设为自启动，符合习惯
            if (UITree.RootContent is Frame frame
                && frame.Content is PhonePage page)
            {
                Win win = null;
                if (page.Content is Tab tab)
                {
                    win = tab.OwnWin;
                }
                else if (page.Content is PhoneTabs tabs)
                {
                    win = tabs.OwnWin;
                }
                if (win != null && win.GetType() != _rootElementType)
                {
                    _autoStartOnce = AutoStartKit.GetAutoStartInfo(win);
                }
                LoadRootDesktop(CreateRootWin());
            }
            else if (UITree.RootContent is Desktop desktop)
            {
                desktop.Destroy();
                if (desktop.MainWin != desktop.HomeWin)
                {
                    _autoStartOnce = AutoStartKit.GetAutoStartInfo(desktop.MainWin);
                }
                LoadRootFrame(CreateRootWin());
            }
        }

        /// <summary>
        /// 主题颜色，logo图标、启动页背景色，在app项目.csprj中设置
        /// </summary>
        public Brush ThemeBrush => ((AppBase)Application.Current).ThemeBrush;

        /// <summary>
        /// 标题，在app项目.csprj中设置 ApplicationTitle
        /// </summary>
        public string Title => ((AppBase)Application.Current).Title;
    }
}