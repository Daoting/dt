﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
#endregion

namespace Dt.Base.Tools
{
    /// <summary>
    /// 系统面板
    /// </summary>
    public static class SysTrace
    {
        static Dlg _dlg;

        /// <summary>
        /// 显示系统面板
        /// </summary>
        public static void ShowSysBox()
        {
            var nav = new NavList { ViewMode = NavViewMode.Tile, Title = "系统", To = NavTarget.NewWin };
            nav.Data = new Nl<Nav>
            {
                new Nav("实时日志", Icons.到今日) { Desc = "查看当前客户端正在输出的日志", Callback = (s, n) =>
                {
                    ShowLogBox();
                    if (s is Dlg dlg)
                        dlg.Close();
                } },
                new Nav("历史日志", typeof(HistoryLogWin), Icons.选日) { Desc = "查看客户端历史日志" },
                new Nav("服务日志", Icons.服务器) { Desc = "查看服务端日志", Callback = (s, n) =>
                {
                    Kit.OpenUrl(Kit.GetSvcUrl("cm") + "/.output");
                    if (s is Dlg dlg)
                        dlg.Close();
                } },

                new Nav("本地库", typeof(LocalDbWin), Icons.数据库) { Desc = "管理 LocalState\\.data 目录下的 sqlite 库" },
                new Nav("本地文件", typeof(LocalFileWin), Icons.文件) { Desc = "管理 LocalState 的所有文件" },
                new Nav("更新缓存文件", typeof(RefreshSqliteWin), Icons.刷新) { Desc = "刷新服务端指定的 sqlite 缓存文件" },

                new Nav("数据库工具", typeof(RemoteDbWin), Icons.数据库) { Desc = "数据库初始化、备份等功能" },

                new Nav("类型别名", typeof(TypeAliasWin), Icons.划卡) { Desc = "所有为类型命名别名的名称与类型的列表" },

                new Nav("切换服务", Icons.服务器) { Desc = "切换服务地址", Callback = (s, n) =>
                {
                    ToggleSvcUrl();
                    if (s is Dlg dlg)
                        dlg.Close();
                } },

#if WIN
                new Nav("打开本地文件目录", Icons.文件夹) { Desc = "快捷键：Ctrl + →", Callback = (s, n) =>
                {
                    OpenLocalPath();
                    if (s is Dlg dlg)
                        dlg.Close();
                } },

                new Nav("复制窗口类型", Icons.复制) { Desc = "复制当前窗口类型", Callback = (s, n) =>
                {
                    CopyWinType();
                    if (s is Dlg dlg)
                        dlg.Close();
                } },

                new Nav("切换顶层显示", Icons.复制) { Desc = "快捷键：Ctrl + L", Callback = (s, n) =>
                {
                    ToggleAlwaysOnTop();
                    if (s is Dlg dlg)
                        dlg.Close();
                } },
#endif
                //new Nav("关于", null, Icons.证书) { Desc = "App V2.3.0\r\nDt  V4.2.1", Callback = (s, n) => Kit.Msg(n.Desc) },
            };

            if (Kit.IsPhoneUI)
            {
                PhonePage.Show(nav);
            }
            else
            {
                var dlg = new Dlg
                {
                    HideTitleBar = true,
                    WinPlacement = DlgPlacement.FromTop,
                    Width = 500,
                };
                dlg.LoadTab(nav);
                dlg.Show();
            }
        }

        /// <summary>
        /// 显示实时日志面板
        /// </summary>
        public static void ShowLogBox()
        {
            if (Kit.IsPhoneUI)
            {
                // phone模式，先关闭当前对话框
                Kit.OpenWin(typeof(RealtimeLogWin));
            }
            else
            {
                if (_dlg == null)
                {
                    var win = new RealtimeLogWin();
                    _dlg = new Dlg
                    {
                        Title = "实时日志",
                        IsPinned = true,
                        ShowVeil = false,
                        WinPlacement = DlgPlacement.FromRight,
                        Width = 755,
                        Height = Kit.ViewHeight / 2
                    };
                    _dlg.LoadWin(win);
                    _dlg.Closed += (s, e) => win.ClearData();
                }
                _dlg.Show();
            }
        }

        public static void ToggleSvcUrl()
        {
            ToggleSvcUrlDlg.ShowDlg();
        }

        public static void OpenLocalPath()
        {
            Kit.OpenUrl(ApplicationData.Current.LocalFolder.Path);
        }

        public static void CopyWinType()
        {
            string name;
            if (UITree.RootContent is Desktop)
            {
                name = Desktop.Inst.MainWin.GetType().FullName;
            }
            else if (UITree.RootContent is Frame frame)
            {
                if (frame.Content is PhonePage page)
                {
                    if (page.Content is Tab tab)
                        name = tab.OwnWin?.GetType().FullName;
                    else if (page.Content is PhoneTabs tabs)
                        name = tabs.OwnWin?.GetType().FullName;
                    else
                        name = page.Content.GetType().FullName;
                }
                else
                {
                    name = frame.Content.GetType().FullName;
                }
            }
            else
            {
                name = UITree.RootContent.GetType().FullName;
            }
            Kit.CopyToClipboard(name, true);
        }

#if WIN
        public static void ToggleAlwaysOnTop()
        {
            var pre = (Microsoft.UI.Windowing.OverlappedPresenter)Kit.MainWin.AppWindow.Presenter;
            pre.IsAlwaysOnTop = !pre.IsAlwaysOnTop;
        }
#endif

        static void OpenWin(Type p_type, string p_title)
        {
            if (UITree.RootContent is Desktop)
            {
                // win模式已登录
                Kit.OpenWin(p_type);
            }
            else if (Kit.IsPhoneUI)
            {
                // phone模式，先关闭当前对话框
                Kit.OpenWin(p_type);
            }
            else
            {
                // win模式未登录
                new Dlg
                {
                    Title = p_title,
                    Content = Activator.CreateInstance(p_type),
                    IsPinned = true,
                    WinPlacement = DlgPlacement.Maximized,
                }.Show();
            }
        }
    }
}
