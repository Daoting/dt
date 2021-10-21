#region 文件描述
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
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.App.Workflow
{
    public partial class HistoryTasks : Win
    {
        public HistoryTasks()
        {
            InitializeComponent();
            LoadSearchData();
            _lv.CellEx = typeof(HistoryTaskView);
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
            Row row = e.Row;
            var status = (WfiPrcStatus)row.Int("status");
            if (status != WfiPrcStatus.活动)
            {
                Kit.Warn($"该任务已{status}，无法追回");
                return;
            }
            if (row.Int("reCount") > 0)
            {
                Kit.Warn("含回退，无法追回");
                return;
            }

            var tbl = await AtCm.Query("流程-后续活动工作项", new { atvdid = row.Long("atvdid"), prciid = row.Long("prciid") });
            if (tbl.Count == 0)
            {
                Kit.Warn("无后续活动，无法追回");
                return;
            }

            HashSet<long> ls = new HashSet<long>();
            foreach (var r in tbl)
            {
                var itemState = (WfiItemStatus)r.Int("Status");
                if (itemState == WfiItemStatus.同步)
                {
                    Kit.Warn("后续活动包含同步，无法追回");
                    return;
                }

                if (itemState != WfiItemStatus.活动
                    || r.Bool("IsAccept"))
                {
                    Kit.Warn("已签收无法追回！");
                    return;
                }
                ls.Add(r.Long("atviid"));
            }

            // 更新当前实例状态为活动
            DateTime time = Kit.Now;
            WfiAtvObj curAtvi = await AtCm.GetByID<WfiAtvObj>(row.Long("atviid"));
            curAtvi.Status = WfiAtvStatus.活动;
            curAtvi.InstCount += 1;
            curAtvi.Mtime = time;

            // 根据当前工作项创建新工作项并更改指派方式
            var curItem = await AtCm.GetByID<WfiItemObj>(row.Long("itemid"));
            var newItem = new WfiItemObj(
                ID: await AtCm.NewID(),
                AtviID: curItem.AtviID,
                Status: WfiItemStatus.活动,
                AssignKind: WfiItemAssignKind.追回,
                Sender: curItem.Sender,
                Stime: curItem.Stime,
                IsAccept: false,
                RoleID: curItem.RoleID,
                UserID: curItem.UserID,
                Note: curItem.Note,
                Dispidx: await AtCm.NewSeq("sq_wfi_item"),
                Ctime: time,
                Mtime: time);

            // 删除已发送的后续活动实例，关联删除工作项及迁移实例
            Table<WfiAtvObj> nextAtvs = new Table<WfiAtvObj>();
            nextAtvs.StartRecordDelRows();
            foreach (var id in ls)
            {
                nextAtvs.DeletedRows.Add(new WfiAtvObj(id));
            }

            // 一个事务批量保存
            List<object> data = new List<object>();
            data.Add(nextAtvs);
            data.Add(curAtvi);
            data.Add(newItem);
            bool suc = await AtCm.BatchSave(data, false);
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

    public class HistoryTaskView
    {
        public static Grid title(ViewItem p_item)
        {
            Grid grid = new Grid
            {
                ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Auto },
                            new ColumnDefinition { Width = GridLength.Auto },
                        },
                Children =
                        {
                            new TextBlock { Text = p_item.Row.Str("formname"), Margin= new Thickness(0,0,4,0), VerticalAlignment = VerticalAlignment.Center },
                        }
            };

            var rc = new Rectangle();
            int status = p_item.Row.Int("status");
            if (status == 0)
                rc.Fill = Res.中绿;
            else if (status == 1)
                rc.Fill = Res.深灰2;
            else
                rc.Fill = Res.BlackBrush;
            Grid.SetColumn(rc, 1);
            grid.Children.Add(rc);

            var tb = new TextBlock { Text = p_item.Row.Str("atvname"), Margin = new Thickness(4, 2, 4, 2), Foreground = Res.WhiteBrush };
            Grid.SetColumn(tb, 1);
            grid.Children.Add(tb);
            return grid;
        }
    }
}