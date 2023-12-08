#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Mgr.Workflow
{
    public partial class HistoryTasks : Win
    {
        public HistoryTasks()
        {
            InitializeComponent();
            LoadSearchData();
        }

        void LoadSearchData()
        {
            Row row = new Row();
            row.Add<DateTime>("start");
            row.Add<DateTime>("end");
            row.Add<int>("status", 3);
            row.Add("statusname", "全部");
            row.Add<bool>("type");
            row.Add("prcdname", "全部");
            row.Add("prcd_id", -1L);
            _fv.Data = row;
            _fv.Query += OnSearch;
        }

        async void OnSearch(QueryClause e)
        {
            var row = _fv.Row;
            _lv.Data = await WfdDs.GetMyHistoryPrcs(row.Bool("type"), row.Date("start"), row.Date("end"), row.Int("status"), row.Long("prcd_id"));
        }

        void OnMonthClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DateTime time = Kit.Now;
            _fv.Row["start"] = new DateTime(time.Year, time.Month, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month), 23, 59, 59);
        }

        void OnQuarterClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DateTime time = Kit.Now;
            int qMonth = (time.Month - 1) / 3 * 3 + 1;
            _fv.Row["start"] = new DateTime(time.Year, qMonth, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, qMonth + 2, DateTime.DaysInMonth(time.Year, qMonth + 2), 23, 59, 59);
        }

        void OnYearClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DateTime time = Kit.Now;
            _fv.Row["start"] = new DateTime(time.Year, 1, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, 12, 31, 23, 59, 59);
        }

        void OnShowInst(Mi e)
        {
            AtWf.OpenFormWin(p_itemID: e.Row.Long("item_id"));
        }

        void OnItemDoubleClick(object e)
        {
            Row row = (Row)e;
            AtWf.OpenFormWin(p_itemID: row.Long("item_id"));
        }

        async void OnRetrieve(Mi e)
        {
            bool suc = await WfiDs.Retrieve(e.Row);
            if (suc)
            {
                Kit.Msg("追回成功");
                OnSearch(null);
            }
            else
            {
                Kit.Warn("追回失败");
            }
        }

        void OnShowLog(Mi e)
        {
            AtWf.ShowLog(e.Row.Long("prci_id"), e.Row.Long("prcd_id"));
        }

        void OnOpenList(Mi e)
        {
            var prc = ((Row)e.Data).Str("prcname");
            var tp = Kit.GetTypeByAlias(typeof(WfListAttribute), prc);
            Throw.IfNull(tp, $"未指定 [{prc}] 的管理窗口类型，请在管理窗口类型上添加 [WfList(\"{prc}\")] 标签！");
            Kit.OpenWin(tp, prc);
        }

        async void OnLoadPrcd(CList arg1, AsyncArgs arg2)
        {
            using (arg2.Wait())
            {
                var prc = await At.Query("select id,name from cm_wfd_prc order by dispidx");
                prc.Insert(0, prc.NewRow(new { id = -1, name = "全部" }));
                arg1.Data = prc;
            }
        }
    }

    [LvCall]
    public class HistoryTaskUI
    {
        public static void FormatStatus(Env e)
        {
            var tbInfo = new TextBlock
            {
                FontFamily = Res.IconFont,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            e.UI = tbInfo;

            e.Set += c =>
            {
                var kind = (WfiPrcStatus)c.Int;
                switch (kind)
                {
                    case WfiPrcStatus.活动:
                        tbInfo.Foreground = Res.中绿;
                        tbInfo.Text = "\uE01E";
                        ToolTipService.SetToolTip(e.Dot, "流程进行中...");
                        break;

                    case WfiPrcStatus.结束:
                        tbInfo.Foreground = Res.BlackBrush;
                        tbInfo.Text = "\uE16C";
                        ToolTipService.SetToolTip(e.Dot, "已结束");
                        break;

                    case WfiPrcStatus.终止:
                        tbInfo.Foreground = Res.亮红;
                        tbInfo.Text = "\uE036";
                        ToolTipService.SetToolTip(e.Dot, "已被终止");
                        break;
                }
            };
        }
    }
}