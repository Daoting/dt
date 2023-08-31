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
                var arrAnd = tmp.Split('&');
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
                                tmpQuery = from it in query
                                           where it.Str("msg").Contains(strOr)
                                           select it;
                            }
                            else
                            {
                                tmpQuery = tmpQuery.Union
                                (
                                from it in query
                                where it.Str("msg").Contains(strOr)
                                select it
                                );
                            }
                        }
                        query = tmpQuery;
                    }
                    else
                    {
                        query = from it in query
                                where it.Str("msg").Contains(str)
                                select it;
                    }
                }
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
                                    where !it.Str("msg").Contains(strAnd)
                                    select it;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            tmpQuery = from it in query
                                       where !it.Str("msg").Contains(str)
                                       select it;
                        }
                        else
                        {
                            tmpQuery = tmpQuery.Union
                            (
                            from it in query
                            where !it.Str("msg").Contains(str)
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
                int start = r.Int("startno");
                int end = r.Int("endno");
                query = from it in query
                        where it.Index >= start && it.Index <= end
                        select it;
            }
            #endregion

            if (r.Str("level") != "全部")
            {
                var level = r.Str("level");
                query = from it in query
                        where it.Str("@l") == level
                        select it;
            }

            var rows = new Nl<Row>(query);
            _lv.Data = rows;
            _tbTotalRow.Text = $"当前共 {rows.Count} 行";
        }

        void OnOutputClick(object sender, ItemClickArgs e)
        {
            //var r = e.Row;
            //var d = new TraceLogData
            //{
            //    TimeLevel = r.Str("Time") + " — " + r.Str("Level"),
            //    Source = r.Str("Source"),
            //    Detial = r.Str("Detial")
            //};
            //_win.Form.Update(d);
        }

        async void OnOpenFile(object sender, Mi e)
        {
            var picker = Kit.GetFileOpenPicker();
            picker.FileTypeFilter.Add(".log");
            picker.FileTypeFilter.Add(".txt");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
                LoadFile(new FileInfo(file.Path));
        }

        async void OnSelectFile(object sender, Mi e)
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

        void LoadFile(FileInfo p_info)
        {
            _fi = p_info;
            _tbInfo.Text = $"✔ 当前日志文件\r\n{p_info.Name}　　{Kit.GetFileSizeDesc((ulong)p_info.Length)}";
            InitRowsInfo();
            _tbInfo.Text += $"　　{_linesPos.Count.ToString("n0")} 行";
        }

        void InitRowsInfo()
        {
            long pos = 0;
            string json;
            _tbl = CreateTbl();
            _linesPos.Clear();

            Stopwatch watch = new Stopwatch();
            watch.Start();

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
                        pos += Encoding.UTF8.GetBytes(json).Length;
                        if (_linesPos.Count < _pageSize)
                        {
                            AddRow(json);
                        }
                    }
                    // \r\n 两字符
                    pos += 2;
                }
                _linesPos.Add(pos);
            }
            int cnt = _linesPos.Count - 1;
            watch.Stop();
            Kit.Msg($"{cnt.ToString("n0")}行，耗时：{watch.ElapsedMilliseconds}毫秒", 0);

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

            Stopwatch watch = new Stopwatch();
            watch.Start();

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
                                AddRow(json);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            watch.Stop();
            Kit.Msg($"耗时：{watch.ElapsedMilliseconds}毫秒", 0);

            _lv.Data = _tbl;
            _tbPageNum.Text = p_pageNo.ToString();
            _tbTotalRow.Text = $"当前共 {_tbl.Count} 行";
        }

        Table CreateTbl()
        {
            return new Table
            {
                { "@t" },
                { "@l" },
                { "ip" },
                { "src" },
                { "user" },
                // 合并消息和异常内容
                { "msg" },

                { "tbinfo", typeof(TextBlock) },
                { "tbmsg", typeof(TextBlock) },
            };
        }

        void AddRow(string p_json)
        {
            Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(p_json));
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
                            if (row.Contains(id))
                            {
                                row.InitVal(id, reader.GetString());
                            }
                            else if (id == "@mt" || id == "@x")
                            {
                                // @x 异常  @mt 消息模板
                                var con = reader.GetString();
                                if (!string.IsNullOrEmpty(con))
                                {
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
                        }
                    }
                }

                //var tb = new TextBlock();
                //Run r = new Run { Text = $"[{row.Str("@t")} ", Foreground = Res.深灰1 };
                //tb.Inlines.Add(r);

                //var lev = row.Str("@l");
                //if (lev == "Information")
                //{
                //    r = new Run { Text = $"INF" };
                //}
                //else if(lev == "Warning")
                //{
                //    r = new Run { Text = $"WAN", Foreground = Res.深黄 };
                //}
                //else if (lev == "Error")
                //{
                //    r = new Run { Text = $"ERR", Foreground = Res.RedBrush };
                //}
                //else if (lev == "Fatal")
                //{
                //    r = new Run { Text = $"FAL", Foreground = Res.亮红 };
                //}
                //else
                //{
                //    r = new Run { Text = $"DBG" };
                //}
                //tb.Inlines.Add(r);

                //r = new Run { Text = $"] {row.Str("ip")} {row.Str("src")} {row.Str("user")}", Foreground = Res.深灰1 };
                //tb.Inlines.Add(r);

                //row["tbinfo"] = tb;

                //tb = new TextBlock { TextTrimming = TextTrimming.CharacterEllipsis, TextWrapping = TextWrapping.Wrap };
                //var msg = row.Str("msg");
                //if (msg.Length > 80)
                //    msg = msg.Substring(0, 78) + "...";
                //tb.Text = msg;
                //row["tbmsg"] = tb;
            }
        }

        HistoryLogWin _win => OwnWin as HistoryLogWin;
    }
}
