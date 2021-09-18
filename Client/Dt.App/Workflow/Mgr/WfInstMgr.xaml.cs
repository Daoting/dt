#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
#endregion

namespace Dt.App.Workflow
{
    public partial class WfInstMgr : Win
    {
        public WfInstMgr()
        {
            InitializeComponent();
            LoadSearchData();
        }

        async void LoadSearchData()
        {
            _fv["prcdname"].To<CList>().Data = await AtCm.Query("流程-所有流程模板名称");
            Row row = new Row();
            row.AddCell<string>("prcdid");
            row.AddCell<string>("prcdname");
            row.AddCell<long>("status", 3);
            row.AddCell("statusname", "全部");
            row.AddCell<string>("title");
            row.AddCell<DateTime>("start");
            row.AddCell<DateTime>("end");
            _fv.Data = row;
        }

        async void OnSearch(object sender, Mi e)
        {
            var row = _fv.Row;
            if (row.Str("prcdid") == "")
                Kit.Warn("未选择流程模板！");
            else
                _lv.Data = await AtCm.Query<WfiPrcObj>("流程-查找实例", row.ToDict());
        }

        void OnMonthClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DateTime time = Kit.Now;
            _fv.Row["start"] = new DateTime(time.Year, time.Month, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month), 23, 59, 59);
        }

        void OnQuarterClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DateTime time = Kit.Now;
            int qMonth = (time.Month - 1) / 3 * 3 + 1;
            _fv.Row["start"] = new DateTime(time.Year, qMonth, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, qMonth + 2, DateTime.DaysInMonth(time.Year, qMonth + 2), 23, 59, 59);
        }

        void OnYearClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DateTime time = Kit.Now;
            _fv.Row["start"] = new DateTime(time.Year, 1, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, 12, 31, 23, 59, 59);
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _lvAtv.Data = await AtCm.Query<WfiAtvObj>("流程-流程实例的活动实例", new { prciID = e.Row.ID });
        }

        async void OnAtvClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _lvItem.Data = await AtCm.Query<WfiItemObj>("流程-活动实例的工作项", new { atviID = e.Row.ID });
        }

        void OnShowInst(object sender, Mi e)
        {
            ShowFormWin(e.Row);
        }

        void OnItemDoubleClick(object sender, object e)
        {
            ShowFormWin((Row)e);
        }

        void ShowFormWin(Row p_row)
        {
            var info = new WfFormInfo(p_row.Long("id"), WfFormUsage.Manage);
            AtWf.OpenFormWin(info);
        }
    }
}