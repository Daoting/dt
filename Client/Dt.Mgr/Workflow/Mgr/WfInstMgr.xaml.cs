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

namespace Dt.Mgr.Workflow
{
    public partial class WfInstMgr : Win
    {
        public WfInstMgr()
        {
            InitializeComponent();
            LoadSearchData();
            _fv.Query += OnSearch;
        }

        async void LoadSearchData()
        {
            _fv["prcdname"].To<CList>().Data = await At.Query("select id,name from cm_wfd_prc order by dispidx");
            Row row = new Row();
            row.Add<string>("prcd_id");
            row.Add<string>("prcdname");
            row.Add<int>("status", 3);
            row.Add("statusname", "全部");
            row.Add<string>("title");
            row.Add<DateTime>("start");
            row.Add<DateTime>("end");
            _fv.Data = row;
        }

        async void OnSearch(object sender, QueryClause e)
        {
            var row = _fv.Row;
            if (row.Str("prcd_id") == "")
                Kit.Warn("未选择流程模板！");
            else
                _lv.Data = await WfiPrcX.Search(row.Long("prcd_id"), row.Date("start"), row.Date("end"), row.Int("status"), row.Str("title"));
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

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _lvAtv.Data = await WfiAtvX.Query($"select atvi.id,atvd.name,status,inst_count from cm_wfi_atv atvi,cm_wfd_atv atvd where atvi.atvd_id=atvd.id and atvi.prci_id={e.Row.ID} order by atvi.ctime");
        }

        async void OnAtvClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _lvItem.Data = await WfiItemX.GetItemsOfAtvi(e.Row.ID);
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
            AtWf.OpenFormWin(p_prciID: p_row.ID);
        }
    }
}