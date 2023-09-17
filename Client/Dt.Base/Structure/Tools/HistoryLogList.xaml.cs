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
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Base.Tools
{
    public sealed partial class HistoryLogList : Tab
    {
        const int _pageSize = 5000;
        FileInfo _fi;
        List<long> _linesPos = new List<long>();
        int _totalNum = 0;
        Table _tbl = null;

        public HistoryLogList()
        {
            InitializeComponent();
        }

        #region 搜索
        public void OnSearch(QueryClause p_clause)
        {
            NaviTo(this);
            if (_tbl == null)
            {
                Kit.Warn("未加载任何日志文件！\r\n请先点击[选择日志]或[打开其它]加载日志文件。");
                return;
            }

            string tmp;
            var r = p_clause.Fv.Row;
            var query = _tbl.AsQueryable();

            #region 包含
            if (r.Bool("eninclude") && (tmp = r.Str("include")) != "")
            {
                query = QueryContains(query, tmp, "msg");
            }
            #endregion

            #region 排除
            if (r.Bool("enexclude") && (tmp = r.Str("exclude")) != "")
            {
                var arrOr = tmp.Split('|');
                IQueryable<Row> tmpQuery = null;
                for (int i = 0; i < arrOr.Length; i++)
                {
                    var str = arrOr[i].Trim();
                    if (str == "")
                        continue;

                    var arrAnd = str.Split('&');
                    if (arrAnd.Length > 1)
                    {
                        for (int j = 0; j < arrAnd.Length; j++)
                        {
                            var strAnd = arrAnd[j].Trim();
                            if (strAnd == "")
                                continue;

                            query = from it in query
                                    where !it.Str("msg").Contains(strAnd, StringComparison.OrdinalIgnoreCase)
                                    select it;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            tmpQuery = from it in query
                                       where !it.Str("msg").Contains(str, StringComparison.OrdinalIgnoreCase)
                                       select it;
                        }
                        else
                        {
                            tmpQuery = tmpQuery.Union
                            (
                            from it in query
                            where !it.Str("msg").Contains(str, StringComparison.OrdinalIgnoreCase)
                            select it
                            );
                        }
                    }
                }
                if (tmpQuery != null)
                    query = tmpQuery;
            }
            #endregion

            #region 行号范围
            if (r.Bool("enline"))
            {
                int start = r.Int("startno") - 1;
                int end = r.Int("endno") - 1;
                query = from it in query
                        where it.Index >= start && it.Index <= end
                        select it;
            }
            #endregion

            #region 日志来源
            if (r.Bool("ensrc") && (tmp = r.Str("src")) != "")
            {
                query = QueryContains(query, tmp, "src");
            }
            #endregion

            #region 客户端IP
            if (r.Bool("enip") && (tmp = r.Str("ip")) != "")
            {
                query = QueryContains(query, tmp, "ip");
            }
            #endregion

            #region 用户ID
            if (r.Bool("enuser") && (tmp = r.Str("user")) != "")
            {
                query = QueryContains(query, tmp, "user");
            }
            #endregion

            #region 级别
            if (r.Str("level") != "全部")
            {
                var level = r.Str("level");
                // INF时缺省为空
                level = level == "INF" ? "" : level;
                query = from it in query
                        where it.Str("level") == level
                        select it;
            }
            #endregion

            var rows = new Nl<Row>(query);
            _lv.Data = rows;
            _tbTotalRow.Text = $"当前共 {rows.Count} 行";
        }

        IQueryable<Row> QueryContains(IQueryable<Row> p_query, string p_input, string p_colName)
        {
            var arrAnd = p_input.Split('&');
            for (int i = 0; i < arrAnd.Length; i++)
            {
                var str = arrAnd[i].Trim();
                if (str == "")
                    continue;

                var arrOr = str.Split('|');
                if (arrOr.Length > 1)
                {
                    IQueryable<Row> tmpQuery = null;
                    for (int j = 0; j < arrOr.Length; j++)
                    {
                        var strOr = arrOr[j].Trim();
                        if (strOr == "")
                            continue;

                        if (j == 0)
                        {
                            tmpQuery = from it in p_query
                                       where it.Str(p_colName).Contains(strOr, StringComparison.OrdinalIgnoreCase)
                                       select it;
                        }
                        else
                        {
                            tmpQuery = tmpQuery.Union
                            (
                            from it in p_query
                            where it.Str(p_colName).Contains(strOr, StringComparison.OrdinalIgnoreCase)
                            select it
                            );
                        }
                    }
                    p_query = tmpQuery;
                }
                else
                {
                    p_query = from it in p_query
                              where it.Str(p_colName).Contains(str, StringComparison.OrdinalIgnoreCase)
                              select it;
                }
            }
            return p_query;
        }
        #endregion

        #region 详细
        void OnOutputClick(object sender, ItemClickArgs e)
        {
            var r = e.Row;
            var lev = r.Str("level");
            lev = lev == "" ? "INF" : lev;
            var d = new TraceLogData
            {
                Info = $"[{r.Str("time")} {lev}] {r.Str("src")} {r.Str("ip")} {r.Str("user")}",
                Msg = r.Str("msg"),
            };
            _win.Form.Update(d);
            NaviTo(_win.Form);
        }
        #endregion

        #region 打开
        async void OnOpenFile(object sender, Mi e)
        {
            var picker = Kit.GetFileOpenPicker();
            picker.FileTypeFilter.Add(".log");
            picker.FileTypeFilter.Add(".txt");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
                LoadFile(new FileInfo(file.Path));
        }

        async void OnSelectOther(object sender, Mi e)
        {
            var dlg = new HisLogFileOtherDlg();
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 500;
                dlg.Height = 600;
            }
            if (await dlg.ShowAsync())
            {
                LoadFile(dlg.FileInfo);
            }
        }

        async void OnSelectSelf(object sender, Mi e)
        {
            var dlg = new HisLogFileDlg();
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 400;
                dlg.Height = 400;
            }
            if (await dlg.ShowAsync())
            {
                LoadFile(dlg.FileInfo);
            }
        }

        protected override void OnFirstLoaded()
        {
#if WIN
            if (_win.IncludeOtherApp)
                Menu[0].Visibility = Visibility.Visible;
#endif
        }
        #endregion

        #region 换页
        void OnFirstPage(object sender, RoutedEventArgs e)
        {
            LoadPage(1);
        }

        void OnPrePage(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(_tbPageNum.Text.Trim(), out int index))
            {
                LoadPage(index - 1);
            }
        }

        void OnNumKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter
                && int.TryParse(_tbPageNum.Text.Trim(), out int index))
            {
                LoadPage(index);
            }
        }

        void OnNextPage(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(_tbPageNum.Text.Trim(), out int index))
            {
                LoadPage(index + 1);
            }
        }

        void OnLastPage(object sender, RoutedEventArgs e)
        {
            LoadPage(_totalNum);
        }
        #endregion

        #region 加载
        void LoadFile(FileInfo p_info)
        {
            _fi = p_info;
            InitRowsInfo();
        }

        void InitRowsInfo()
        {
            long pos = 0;
            string json;
            _tbl = CreateTbl();
            _linesPos.Clear();

            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            // 1G文件，820360行，耗时2.7秒，内存占用增加2M
            using (FileStream fs = new FileStream(_fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                while (sr.Peek() >= 0)
                {
                    _linesPos.Add(pos);
                    json = sr.ReadLine();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = Encoding.UTF8.GetBytes(json);
                        pos += data.Length;
                        if (_linesPos.Count <= _pageSize)
                        {
                            AddRow(data);
                        }
                    }
                    // \r\n 两字符
                    pos += 2;
                }
                _linesPos.Add(pos);
            }
            int cnt = _linesPos.Count - 1;
            //watch.Stop();
            //Kit.Msg($"{cnt.ToString("n0")}行，耗时：{watch.ElapsedMilliseconds}毫秒", 0);

            _lv.Data = _tbl;
            _totalNum = (int)Math.Ceiling((double)cnt / _pageSize);
            _tbPageNum.Text = "1";
            _tbTotalPage.Text = $"/ {_totalNum.ToString("n0")} 页";
            _tbTotalRow.Text = $"当前共 {_tbl.Count} 行";
        }

        void LoadPage(int p_pageNo)
        {
            if (p_pageNo < 1 || p_pageNo > _totalNum)
            {
                Kit.Warn("页号超出范围！");
                return;
            }

            string json;
            _tbl = CreateTbl();

            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            // 1G文件，820360行，耗时2.7秒，内存占用增加2M
            using (FileStream fs = new FileStream(_fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // 前进到页开始位置
                fs.Seek(_linesPos[(p_pageNo - 1) * _pageSize], SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (sr.Peek() >= 0)
                    {
                        json = sr.ReadLine();
                        if (!string.IsNullOrEmpty(json))
                        {
                            if (_tbl.Count < _pageSize)
                            {
                                AddRow(Encoding.UTF8.GetBytes(json));
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            //watch.Stop();
            //Kit.Msg($"耗时：{watch.ElapsedMilliseconds}毫秒", 0);

            _lv.Data = _tbl;
            _tbPageNum.Text = p_pageNo.ToString();
            _tbTotalRow.Text = $"当前共 {_tbl.Count} 行";
        }

        Table CreateTbl()
        {
            return new Table
            {
                { "time" },
                { "level" },
                // 合并消息和异常内容
                { "msg" },
                { "ip" },
                { "src" },
                { "user" },
            };
        }

        void AddRow(byte[] p_json)
        {
            Utf8JsonReader reader = new Utf8JsonReader(p_json);
            // {
            reader.Read();

            // 都是键值对
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var row = _tbl.AddRow();
                while (reader.Read())
                {
                    // 结束
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        // 键名
                        var id = reader.GetString();
                        // 值
                        reader.Read();
                        // 只取有用的列
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            if (id == "@t")
                            {
                                var time = reader.GetDateTime();
                                row.InitVal("time", time.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else if (id == "@l")
                            {
                                var lev = reader.GetString();
                                if (lev == "Debug")
                                {
                                    lev = "DBG";
                                }
                                else if (lev == "Warning")
                                {
                                    lev = "WRN";
                                }
                                else if (lev == "Error")
                                {
                                    lev = "ERR";
                                }
                                else if (lev == "Fatal")
                                {
                                    lev = "FAL";
                                }
                                else
                                {
                                    lev = "INF";
                                }
                                row.InitVal("level", lev);
                            }
                            else if (id == "@mt" || id == "@x")
                            {
                                // @x 异常  @mt 消息模板
                                var con = reader.GetString();
                                if (!string.IsNullOrEmpty(con))
                                {
                                    con = con.Trim();
                                    var old = row.Str("msg");
                                    if (old != "")
                                    {
                                        row.InitVal("msg", old + "\r\n" + con);
                                    }
                                    else
                                    {
                                        row.InitVal("msg", con);
                                    }
                                }
                            }
                            else if (row.Contains(id))
                            {
                                row.InitVal(id, reader.GetString());
                            }
                        }
                    }
                }
            }
        }
        #endregion

        HistoryLogWin _win => OwnWin as HistoryLogWin;
    }

    [LvCall]
    public class HisLogStyle
    {
        public static void FormatItem(Env e)
        {
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = new GridLength(60) },
                },
            };

            var tbInfo = new TextBlock();
            grid.Children.Add(tbInfo);

            var tbIndex = new TextBlock { Foreground = Res.深灰1, FontSize = Res.小字, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
            grid.Children.Add(tbIndex);

            var tbMsg = new TextBlock { Style = Res.LvTextBlock, Margin = new Thickness(0, 6, 0, 6), };
            Grid.SetRow(tbMsg, 1);
            grid.Children.Add(tbMsg);
            e.UI = grid;

            e.Set += c =>
            {
                var r = c.Row;

                var lev = r.Str("level");
                lev = lev == "" ? "INF" : lev;
                if (lev == "DBG")
                {
                    tbInfo.Foreground = Res.深灰1;
                }
                else if (lev == "WRN")
                {
                    tbInfo.Foreground = Res.深黄;
                }
                else if (lev == "ERR")
                {
                    tbInfo.Foreground = Res.RedBrush;
                }
                else if (lev == "FAL")
                {
                    tbInfo.Foreground = Res.亮红;
                }
                else
                {
                    tbInfo.Foreground = Res.BlackBrush;
                }

                tbInfo.Text = $"[{r.Str("time")} {lev}] {r.Str("src")} {r.Str("ip")} {r.Str("user")}";
                tbMsg.Text = r.Str("msg");
                tbIndex.Text = (r.Index + 1).ToString();
            };
        }
    }
}
