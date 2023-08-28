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
using System.Text.Json;
#endregion

namespace Dt.Base.Tools
{
    public sealed partial class HistoryLogList : Tab
    {
        string _sql;
        Dict _sqlParams;

        public HistoryLogList()
        {
            InitializeComponent();
        }

        #region 搜索
        public void OnSearch(QueryClause p_clause)
        {
            Row row = p_clause.Fv.Row;

            _sqlParams = new Dict();
            _sql = "select * from logs where 1=1 ";
            if (row.Date("start") != default)
            {
                _sql += " and timestamp>=@start";
                _sqlParams["start"] = row.Date("start");
            }
            if (row.Date("end") != default)
            {
                _sql += " and timestamp<=@end";
                _sqlParams["end"] = row.Date("end");
            }
            if (row.Str("level") != "全部")
            {
                _sql += " and level=@level";
                _sqlParams["level"] = row.Str("level");
            }
            if (row.Str("content") != "")
            {
                var con = row.Str("content");
                var flag = p_clause.Fv["content"].QueryFlag;
                if (flag == CompFlag.Contains)
                    _sql += $" and (RenderedMessage like '%{con}%' or Exception like '%{con}%')";
            }
            _sql += " order by id desc";

            _lv.PageData = new PageData { NextPage = OnNextPage };

            NaviTo(this);
        }

        async void OnNextPage(PageData p_pd)
        {
            var tbl = new Table
            {
                { "Time" },
                { "LevelAndSource" },
                { "Level" },
                { "Source" },
                { "Message" },
                { "Title" },
                { "Detial" },
            };

            var ls = await AtDtlog.Each(_sql + $" limit {p_pd.PageNo * p_pd.PageSize},{p_pd.PageSize}", _sqlParams);
            foreach (var r in ls)
            {
                var row = tbl.AddRow();

                string tmp;
                var date = r.Date("Timestamp");
                TimeSpan ts = DateTime.Now.Date - date.Date;
                switch (ts.Days)
                {
                    case 0:
                        tmp = date.ToString("HH:mm:ss");
                        break;
                    case 1:
                        tmp = "昨天";
                        break;
                    case -1:
                        tmp = "明天";
                        break;
                    default:
                        tmp = date.ToString("yyyy-MM-dd HH:mm");
                        break;
                }
                row.InitVal("Time", tmp);

                row.InitVal("Level", r.Str("Level"));

                // Source Title
                try
                {
                    var elem = JsonSerializer.Deserialize<JsonElement>(r.Str("Properties"));
                    if (elem.ValueKind == JsonValueKind.Object)
                    {
                        if (elem.TryGetProperty("SourceContext", out var prop))
                        {
                            row.InitVal("Source", prop.ToString());
                        }
                        if (elem.TryGetProperty("Title", out prop))
                        {
                            row.InitVal("Title", prop.ToString());
                        }
                    }
                }
                catch { }

                // LevelAndSource
                if (row.Str("Source") != "")
                {
                    // 含日志来源，不显示命名空间，后缀为级别
                    tmp = row.Str("Source");
                    int index = tmp.LastIndexOf('.');
                    if (index > -1)
                        tmp = tmp.Substring(index + 1);
                    tmp = $"{tmp} — {row.Str("Level")}";
                }
                else
                {
                    tmp = row.Str("Level");
                }
                row.InitVal("LevelAndSource", tmp);

                // Detial
                var detail = $"{r.Str("RenderedMessage")}\r\n{r.Str("Exception")}";
                row.InitVal("Detial", detail);

                // Message
                if (row.Str("Title") != "")
                {
                    // 内置标题属性
                    tmp = row.Str("Title");
                }
                else
                {
                    if (detail.Length > 100)
                        tmp = detail.Substring(0, 100) + "...";
                    else
                        tmp = detail;
                }
                row.InitVal("Message", tmp);
            }

            p_pd.LoadPageData(tbl);
        }
        #endregion

        void OnOutputClick(object sender, ItemClickArgs e)
        {
            var r = e.Row;
            var d = new TraceLogData
            {
                TimeLevel = r.Str("Time") + " — " + r.Str("Level"),
                Source = r.Str("Source"),
                Detial = r.Str("Detial")
            };
            _win.Form.Update(d);
        }

        HistoryLogWin _win => OwnWin as HistoryLogWin;
    }

    [LvCall]
    public class HisLogCellUI
    {
        public static void TitleBrush(Env e)
        {
            e.Set += c =>
            {
                var r = c.Row;
                if (r.Str("Level") == "Error" || r.Str("Level") == "Fatal")
                {
                    e.Dot.Foreground = Res.WhiteBrush;
                    e.Dot.Background = Res.RedBrush;
                }
                else if (r.Str("Level") == "Warning")
                {
                    e.Dot.Foreground = Res.WhiteBrush;
                    e.Dot.Background = Res.深黄;
                }
                else
                {
                    e.Dot.Foreground = Res.深灰1;
                    e.Dot.Background = Res.WhiteBrush;
                }
            };
        }
    }
}
