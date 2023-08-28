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
#endregion

namespace Dt.Base.Tools
{
    public sealed partial class HistoryLogQuery : Tab
    {
        public HistoryLogQuery()
        {
            InitializeComponent();
            _grid.DataContext = _fv;
            _fv.Query += OnQuery;
        }

        protected override void OnFirstLoaded()
        {
            var row = new Row();
            row.AddCell<DateTime>("start");
            row.AddCell<DateTime>("end");
            row.AddCell("level", "全部");
            row.AddCell("content", "");

            _fv.Data = row;
        }

        void OnMonthClick(object sender, RoutedEventArgs e)
        {
            DateTime time = Kit.Now;
            _fv.Row["start"] = new DateTime(time.Year, time.Month, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month), 23, 59, 59);
        }

        void OnQuarterClick(object sender, RoutedEventArgs e)
        {
            DateTime time = Kit.Now;
            int qMonth = (time.Month - 1) / 3 * 3 + 1;
            _fv.Row["start"] = new DateTime(time.Year, qMonth, 1, 0, 0, 0);
            _fv.Row["end"] = new DateTime(time.Year, qMonth + 2, DateTime.DaysInMonth(time.Year, qMonth + 2), 23, 59, 59);
        }

        void OnDaysClick(object sender, RoutedEventArgs e)
        {
            DateTime time = Kit.Now;
            _fv.Row["start"] = time.AddDays(-3);
            _fv.Row["end"] = time;
        }

        void OnQuery(object sender, QueryClause e)
        {
            _win.List.OnSearch(e);
        }

        HistoryLogWin _win => OwnWin as HistoryLogWin;
    }
}
