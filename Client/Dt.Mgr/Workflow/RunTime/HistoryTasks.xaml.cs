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
            row.AddCell<DateTime>("start");
            row.AddCell<DateTime>("end");
            row.AddCell<long>("status", 3);
            row.AddCell("statusname", "全部");
            row.AddCell<bool>("type");
            row.AddCell("userid", Kit.UserID);
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

        void OnShowInst(object sender, Mi e)
        {
            AtWf.OpenFormWin(new WfFormInfo(e.Row.Long("prcdid"), e.Row.Long("itemid"), WfFormUsage.Read));
        }

        void OnItemDoubleClick(object sender, object e)
        {
            Row row = (Row)e;
            AtWf.OpenFormWin(new WfFormInfo(row.Long("prcdid"), row.Long("itemid"), WfFormUsage.Read));
        }

        async void OnRetrieve(object sender, Mi e)
        {
            bool suc = await WfiDs.Me.Retrieve(e.Row);
            if (suc)
            {
                Kit.Msg("追回成功");
                OnSearch(null, null);
            }
            else
            {
                Kit.Warn("追回失败");
            }
        }

        void OnShowLog(object sender, Mi e)
        {
            AtWf.ShowLog(e.Row.Long("prciid"), e.Row.Long("prcdid"));
        }
    }

    [LvCall]
    public class HistoryTaskUI
    {
        public static void FormatTile(Env e)
        {
            Grid grid = new Grid { ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Auto }, new ColumnDefinition { Width = GridLength.Auto }, } };

            var tbName = new TextBlock { Margin = new Thickness(0, 0, 4, 0), VerticalAlignment = VerticalAlignment.Center };
            grid.Children.Add(tbName);

            var rc = new Rectangle();
            Grid.SetColumn(rc, 1);
            grid.Children.Add(rc);

            var tbAtv = new TextBlock { Margin = new Thickness(4, 2, 4, 2), Foreground = Res.WhiteBrush };
            Grid.SetColumn(tbAtv, 1);
            grid.Children.Add(tbAtv);
            e.UI = grid;

            e.Set += c =>
            {
                tbName.Text = c.Row.Str("formname");

                int status = c.Row.Int("status");
                if (status == 0)
                    rc.Fill = Res.中绿;
                else if (status == 1)
                    rc.Fill = Res.深灰2;
                else
                    rc.Fill = Res.BlackBrush;

                tbAtv.Text = c.Row.Str("atvname");
            };
        }
    }
}