#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Dt.Core.Sqlite;
#endregion

namespace Dt.Base.Tools
{
    /// <summary>
    /// 更新缓存文件
    /// </summary>
    public sealed partial class RefreshSqliteWin : Win
    {
        public RefreshSqliteWin()
        {
            InitializeComponent();
            LoadFileList();
        }

        async void LoadFileList()
        {
            var ls = await Kit.Rpc<List<string>>(
                "cm",
                "SysKernel.GetAllSqliteFile"
            );

            Nl<string> nl = new Nl<string>(ls);
            _lvFile.Data = nl;
        }

        void OnRefreshFile(Mi e)
        {
            UpdateSqliteFile(e.Data.ToString());
        }

        /// <summary>
        /// 更新sqlite文件
        /// </summary>
        /// <param name="p_fileName"></param>
        public static async void UpdateSqliteFile(string p_fileName)
        {
            if (await Kit.Confirm($"确认要更新服务端的{p_fileName}文件吗？\r\n更新后需要重启应用才能生效"))
            {
#if WIN
                ShowSvcLogDlg();

                await Kit.Rpc<string>(
                    "cm",
                    "SysKernel.UpdateSqliteFile",
                    p_fileName
                );
#else
                var msg = await Kit.Rpc<string>(
                    "cm",
                    "SysKernel.UpdateSqliteFile",
                    p_fileName
                );
                Kit.Msg(msg);
#endif
            }
        }

        public static void ShowSvcLogDlg()
        {
            Dlg dlg = new Dlg
            {
                Title = "服务日志",
                IsPinned = true,
                Content = new WebView2 { Source = new Uri($"{At.GetSvcUrl("cm")}/.output") }
            };

            if (!Kit.IsPhoneUI)
            {
                dlg.Height = Kit.ViewHeight - 200;
                dlg.Width = Math.Min(900, Kit.ViewWidth - 300);
            }
            dlg.Show();
        }
    }
}
