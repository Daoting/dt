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
    /// 历史日志
    /// </summary>
    public sealed partial class HistoryLogWin : Win
    {
        public HistoryLogWin()
        {
            InitializeComponent();
        }

        public HistoryLogList List => _list;

        public TraceLogForm Form => _fm;

        public HistoryLogQuery Query => _query;
    }

    public class AtDtlog : AccessAgent<AtDtlog.Info>
    {
        public class Info : AgentInfo
        {
            public override AccessType Type => AccessType.Local;

            public override string Name => "dtlog";
        }
    }
}
