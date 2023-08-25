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
            OpenWin(typeof(SystemMainWin), "系统");
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
                dlg.Width = 756;
            dlg.Closed += (s, e) => win.ClearData();
            dlg.Show();
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
        static void CopyToClipboard(string p_text, bool p_showText = false)
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
