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
    /// 系统
    /// </summary>
    public sealed partial class SystemMainWin : Win
    {
        public SystemMainWin()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("实时日志", typeof(RealtimeLogWin), Icons.到今日) { Desc = "查看当前客户端正在输出的日志" },
                new Nav("历史日志", typeof(HistoryLogWin), Icons.选日) { Desc = "查看客户端历史日志" },
                new Nav("弹出实时日志", null, Icons.到今日) { Desc = "保持实时日志始终在最上层显示", Callback = (w, n) => SysTrace.ShowRealtimeLogDlg() },
                new Nav("服务日志", null, Icons.服务器) { Desc = "查看服务端日志", Callback = (w, n) => Kit.OpenUrl(Kit.GetSvcUrl("cm") + "/.output") },

                new Nav("本地库", typeof(LocalDbWin), Icons.数据库) { Desc = "管理 LocalState\\.data 目录下的 sqlite 库" },
                new Nav("本地文件", typeof(LocalFileWin), Icons.文件) { Desc = "管理 LocalState 的所有文件" },
                new Nav("更新缓存文件", typeof(RefreshSqliteWin), Icons.刷新) { Desc = "刷新服务端指定的 sqlite 缓存文件" },

                new Nav("视图类型", typeof(LocalDbWin), Icons.划卡) { Desc = "所有可作为独立视图显示的名称与类型列表" },
                new Nav("流程表单类型", typeof(LocalDbWin), Icons.排列) { Desc = "所有流程表单类型" },

                new Nav("切换服务", null, Icons.服务器) { Desc = "切换服务地址", Callback = (w, n) => SysTrace.ToggleSvcUrl() },
                new Nav("关于", null, Icons.证书) { Desc = "App V2.3.0\r\nDt  V4.2.1", Callback = (w, n) => Kit.Msg(n.Desc) },
            };
        }
    }
}
