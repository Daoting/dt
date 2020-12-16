﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
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
            row.AddCell<DateTime>("start");
            row.AddCell<DateTime>("end");
            row.AddCell<long>("status", 3);
            row.AddCell("statusname", "全部");
            row.AddCell<bool>("type");
            row.AddCell("userid", AtUser.ID);
            _fv.Data = row;
        }

        async void OnSearch(object sender, Mi e)
        {
            var row = _fv.Row;
            if (row.Bool("type"))
            {
                // 用户在一个流程实例中参与的所有任务
                _lv.Data = await AtCm.Query("流程-所有经办历史任务", row.ToDict());
            }
            else
            {
                // 用户只能看到一个流程实例的最后完成的任务
                _lv.Data = await AtCm.Query("流程-历史任务", row.ToDict());
            }
        }

        void OnMonthClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DateTime time = AtSys.Now;
            _fv.Row["start"] = new DateTime(time.Year, time.Month, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month), 23, 59, 59);
        }

        void OnQuarterClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DateTime time = AtSys.Now;
            int qMonth = (time.Month - 1) / 3 * 3 + 1;
            _fv.Row["start"] = new DateTime(time.Year, qMonth, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, qMonth + 2, DateTime.DaysInMonth(time.Year, qMonth + 2), 23, 59, 59);
        }

        void OnYearClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DateTime time = AtSys.Now;
            _fv.Row["start"] = new DateTime(time.Year, 1, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, 12, 31, 23, 59, 59);
        }

        void OnShowInst(object sender, Mi e)
        {
            AtWf.OpenFormWin(new WfFormInfo(e.Row.Long("prcdid"), e.Row.Long("itemid"), WfFormUsage.Read));
        }

        void OnRetrieve(object sender, Mi e)
        {

        }
    }
}