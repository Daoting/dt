#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-06-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;
using Windows.Storage.Pickers;
using Windows.System;
#endregion

namespace Dt.Base.Tools
{
    public sealed partial class HistoryLogQuery : Tab
    {
        public HistoryLogQuery()
        {
            InitializeComponent();
            _fv.Query += OnQuery;
        }

        protected override void OnFirstLoaded()
        {
            var row = new Row
            {
                { "eninclude", true },
                { "include", "" },
                { "enexclude", false },
                { "exclude", "" },
                { "enline", false },
                { "startno", 0 },
                { "endno", 5000 },
                { "ensrc", false },
                { "src", "" },
                { "enip", false },
                { "ip", "" },
                { "enuser", false },
                { "user", "" },
                { "level", "全部" },
            };
            _fv.Data = row;
        }

        void OnQuery(QueryClause e)
        {
            _win.List.OnSearch(e);
        }

        void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                _fv.OnQuery();
            }
        }

        HistoryLogWin _win => OwnWin as HistoryLogWin;

    }
}
