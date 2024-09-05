#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Report;
using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using Windows.UI;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 导出Table
    /// </summary>
    internal class TblExporter
    {
        static Color _titleBackground = Color.FromArgb(0xff, 0xE0, 0xE0, 0xE0);
        readonly TblRptInfo _info;
        readonly Table _data;
        Cols _cols;

        public TblExporter(Table p_data, TblRptInfo p_info)
        {
            _data = p_data;
            _info = p_info ?? new TblRptInfo();
            ((TblRptScript)_info.ScriptObj).Data = _data;
        }

        public void ShowReport(bool p_inNewWin, bool p_isPdf)
        {
            PrepareCols();
            BuildTemplate();

            if (p_inNewWin)
            {
                Rpt.Show(_info, p_isPdf);
            }
            else
            {
                _ = Rpt.ShowDlg(_info, p_isPdf);
            }
        }

        public async Task SaveExcel(Stream p_stream)
        {
            PrepareCols();
            BuildTemplate();
            var excel = await CreateExcel();
            await excel.SaveExcel(p_stream);
        }

        public async Task SavePdf(Stream p_stream)
        {
            PrepareCols();
            BuildTemplate();
            var excel = await CreateExcel();
            await excel.SavePdf(p_stream);
        }

        public async void Print()
        {
            PrepareCols();
            BuildTemplate();
            var excel = await CreateExcel();
            excel.Print(_info.Sheet.PrintInfo, -1, _info.Name);
        }

        void PrepareCols()
        {
            List<double> widths = new List<double>();
            if (_info.RowNO)
                widths.Add(40);

            _cols = new Cols();
            if (_data != null)
            {
                // 找出该列的最大宽度
                for (int i = 0; i < _data.Columns.Count; i++)
                {
                    var col = _data.Columns[i];
                    int width = 0;

                    // 标题宽度
                    foreach (var c in col.ID)
                    {
                        width += c > 255 ? 15 : 7;
                    }

                    foreach (var r in _data)
                    {
                        int cur = 0;
                        foreach (var c in r.Str(i))
                        {
                            cur += c > 255 ? 15 : 7;
                        }
                        if (cur > width)
                            width = cur;

                        // 不超过300
                        if (width > 300)
                        {
                            width = 300;
                            break;
                        }
                    }
                    widths.Add(width + 20);
                    _cols.Add(new Col { ID = col.ID, Title = col.ID });
                }
            }

            _info.Root.Cols = widths.ToArray();
            _info.Root.Body.Rows = new double[] { 30, 30 };
        }

        void BuildTemplate()
        {
            RptTblPartRow tblRow;
            var tbl = _info.Table;
            tbl.ColSpan = _cols.Count;
            int colStart = _info.RowNO ? 1 : 0;

            if (_info.ShowColHeader)
            {
                tbl.ColHeader = new RptTblColHeader(tbl);
                tblRow = new RptTblPartRow(tbl.ColHeader);
                tbl.ColHeader.Rows.Add(tblRow);

                if (_info.RowNO)
                {
                    var txt = new RptText(tblRow);
                    txt.Background = _titleBackground;
                    tblRow.Cells.Add(txt);
                }

                for (int i = 0; i < _cols.Count; i++)
                {
                    var col = (Col)_cols[i];
                    var txt = new RptText(tblRow);
                    txt.Col = i + colStart;
                    txt.Val = col.Title;
                    txt.Horalign = CellHorizontalAlignment.Center;
                    txt.Background = _titleBackground;
                    tblRow.Cells.Add(txt);
                }
            }

            tbl.Body = new RptTblRow(tbl);
            tblRow = new RptTblPartRow(tbl.Body);
            tbl.Body.Rows.Add(tblRow);

            if (_info.RowNO)
            {
                var txt = new RptText(tblRow);
                txt.Row = 1;
                txt.Val = ":Index(tbl)";
                txt.Horalign = CellHorizontalAlignment.Center;
                txt.Background = _titleBackground;
                txt.ParseVal();
                tblRow.Cells.Add(txt);
            }

            for (int i = 0; i < _cols.Count; i++)
            {
                var col = (Col)_cols[i];
                var txt = new RptText(tblRow);
                txt.Row = 1;
                txt.Col = i + colStart;
                txt.Val = $":Val(tbl,{col.ID})";
                txt.ParseVal();
                tblRow.Cells.Add(txt);
            }
        }

        async Task<Excel> CreateExcel()
        {
            await new RptRootInst(_info).Draw();

            var excel = new Excel
            {
                TabStripVisibility = Visibility.Collapsed,
                ShowDecoration = true,
            };
            using (excel.Defer())
            {
                excel.Sheets.Clear();
                var ws = _info.Sheet;
                ws.ColumnHeader.IsVisible = false;
                ws.RowHeader.IsVisible = false;
                ws.ShowGridLine = false;
                excel.Sheets.Add(ws);
            }
            return excel;
        }
    }
}
