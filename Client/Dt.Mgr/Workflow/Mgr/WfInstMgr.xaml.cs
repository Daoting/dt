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
        }

        async void LoadSearchData()
        {
            _fv["prcdname"].To<CList>().Data = await AtCm.Query("select id,name from cm_wfd_prc order by dispidx");
            Row row = new Row();
            row.AddCell<string>("prcd_id");
            row.AddCell<string>("prcdname");
            row.AddCell<int>("status", 3);
            row.AddCell("statusname", "全部");
            row.AddCell<string>("title");
            row.AddCell<DateTime>("start");
            row.AddCell<DateTime>("end");
            _fv.Data = row;
        }

        async void OnSearch(object sender, Mi e)
        {
            var row = _fv.Row;
            if (row.Str("prcd_id") == "")
                Kit.Warn("未选择流程模板！");
            else
                _lv.Data = await WfiPrcX.Query("cm_流程_查找实例", new
                {
                    p_prcdid = row.Str("prcd_id"),
                    p_start = row.Date("start"),
                    p_end = row.Date("end"),
                    p_status = row.Int("status"),
                    p_title = row.Str("title"),
                } );
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
                _lvItem.Data = await WfiItemX.Query("cm_流程_活动实例的工作项", new { p_atviid = e.Row.ID });
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