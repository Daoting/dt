#region 文件描述
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
    /// 系统日志输出面板
    /// </summary>
    public static class SysTrace
    {
        public static void ShowBox()
        {
            var nav = new NavList { ViewMode = NavViewMode.Tile, Title = "系统", To = NavTarget.NewWin };
            nav.Data = new Nl<Nav>
            {
                new Nav("实时日志", typeof(RealtimeLogWin), Icons.到今日) { Desc = "查看当前客户端正在输出的日志" },
                new Nav("历史日志", typeof(HistoryLogWin), Icons.选日) { Desc = "查看客户端历史日志" },
                new Nav("弹出实时日志", null, Icons.到今日) { Desc = "保持实时日志始终在最上层显示", Callback = (w, n) => SysTrace.ShowRealtimeLogDlg() },
                new Nav("服务日志", null, Icons.服务器) { Desc = "查看服务端日志", Callback = (w, n) => Kit.OpenUrl(Kit.GetSvcUrl("cm") + "/.output") },

                new Nav("本地库", typeof(LocalDbWin), Icons.数据库) { Desc = "管理 LocalState\\.data 目录下的 sqlite 库" },
                new Nav("本地文件", typeof(LocalFileWin), Icons.文件) { Desc = "管理 LocalState 的所有文件" },
                new Nav("更新缓存文件", typeof(RefreshSqliteWin), Icons.刷新) { Desc = "刷新服务端指定的 sqlite 缓存文件" },

                new Nav("数据库工具", typeof(LocalDbWin), Icons.数据库) { Desc = "" },

                new Nav("视图类型", typeof(LocalDbWin), Icons.划卡) { Desc = "所有可作为独立视图显示的名称与类型列表" },
                new Nav("流程表单类型", typeof(LocalDbWin), Icons.排列) { Desc = "所有流程表单类型" },

                new Nav("切换服务", null, Icons.服务器) { Desc = "切换服务地址", Callback = (w, n) => SysTrace.ToggleSvcUrl() },
                new Nav("关于", null, Icons.证书) { Desc = "App V2.3.0\r\nDt  V4.2.1", Callback = (w, n) => Kit.Msg(n.Desc) },
            };

            var dlg = new Dlg
            {
                HideTitleBar = true,
                WinPlacement = DlgPlacement.FromTop,
            };
            dlg.LoadTab(nav);

            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 600;
            }
            dlg.Show();
        }

        public static void ShowRealtimeLogDlg()
        {
            var win = new RealtimeLogWin();
            var dlg = new Dlg
            {
                Title = "实时日志",
                Content = win,
                IsPinned = true,
                ShowVeil = false,
                WinPlacement = DlgPlacement.FromRight
            };

            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 755;
                dlg.Height = Kit.ViewHeight / 2;
            }
            dlg.Closed += (s, e) => win.ClearData();
            dlg.Show();
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
            CopyToClipboard(name, true);
        }

        /// <summary>
        /// 将文本复制到剪贴板
        /// </summary>
        /// <param name="p_text"></param>
        /// <param name="p_showText">是否显示要复制的内容</param>
        public static void CopyToClipboard(string p_text, bool p_showText = false)
        {
            DataPackage data = new DataPackage();
            data.SetText(p_text);
            Clipboard.SetContent(data);
            if (p_showText)
                Kit.Msg("已复制到剪切板：\r\n" + p_text);
            else
                Kit.Msg("已复制到剪切板！");
        }

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
