#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using Dt.Base.Report;
using Dt.Cells.Data;
using Windows.UI;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 导出Lv，Cols定义的列样式
    /// </summary>
    internal class LvExporter
    {
        static Color _titleBackground = Color.FromArgb(0xff, 0xE0, 0xE0, 0xE0);
        readonly Lv _lv;
        readonly LvRptInfo _info;
        Table _data;
        Cols _cols;

        public LvExporter(Lv p_lv, LvRptInfo p_info)
        {
            _lv = p_lv;
            _info = p_info ?? new LvRptInfo();
            ((LvRptScript)_info.ScriptObj).SetExporter(this);
        }

        public LvRptInfo Info => _info;
        public Table Data => _data;
        public Cols Cols => _cols;
        public Lv Lv => _lv;

        /// <summary>
        /// 是否自定义行样式
        /// </summary>
        public bool CustomRowStyle => _lv.View is Cols && _lv.ItemStyle != null;

        public void ShowReport(bool p_inNewWin, bool p_isPdf)
        {
            PrepareData();
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
            PrepareData();
            PrepareCols();
            BuildTemplate();
            var excel = await CreateExcel();
            await excel.SaveExcel(p_stream);
        }

        public async Task SavePdf(Stream p_stream)
        {
            PrepareData();
            PrepareCols();
            BuildTemplate();
            var excel = await CreateExcel();
            await excel.SavePdf(p_stream);
        }

        public async void Print()
        {
            PrepareData();
            PrepareCols();
            BuildTemplate();
            var excel = await CreateExcel();
            excel.Print(_info.Sheet.PrintInfo, -1, _info.Name);
        }

        void PrepareData()
        {
            if (_lv.Data == null || _lv.Data.Count == 0)
                return;

            if (_lv.Data is Table tbl)
            {
                if (_info.OnlySelection)
                {
                    _data = tbl.Clone();
                    foreach (var row in _lv.SelectedRows)
                    {
                        _data.Add(row);
                    }
                }
                else
                {
                    _data = tbl;
                }
                return;
            }

            Type rowType = _lv.Data[0].GetType();
            var pis = rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (pis != null && pis.Length > 0)
            {
                // 将非Table数据整理成Table格式
                _data = new Table();
                foreach (var p in pis)
                {
                    _data.Add(p.Name, p.PropertyType);
                }

                IEnumerable items = _info.OnlySelection ? _lv.SelectedItems : _lv.Data;
                foreach (var item in items)
                {
                    var r = new Core.Row();
                    foreach (var p in pis)
                    {
                        new Core.Cell(r, p.Name, p.PropertyType, p.GetValue(item));
                    }
                    _data.Add(r);
                }
            }
        }

        void PrepareCols()
        {
            List<double> widths = new List<double>();
            if (_info.RowNO)
                widths.Add(40);

            if (_lv.View is Cols cols)
            {
                _cols = cols;
                foreach (var col in _cols.OfType<Col>())
                {
                    if (double.TryParse(col.Width, out var width))
                    {
                        widths.Add(width);
                    }
                    else
                    {
                        widths.Add(120);
                    }
                }
            }
            else
            {
                // Table数据源中的列作为Col
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

            // 是否自定义行样式
            bool customRowStyle = CustomRowStyle;
            for (int i = 0; i < _cols.Count; i++)
            {
                var col = (Col)_cols[i];
                var txt = new RptText(tblRow);
                txt.Row = 1;
                txt.Col = i + colStart;

                if (col.Call != null
                    || col.Format != null
                    || customRowStyle)
                {
                    txt.Val = "#script#";
                }
                else
                {
                    txt.Val = $":Val(tbl,{col.ID})";
                }
                txt.ParseVal();

                if (col.ExistLocalValue(Col.ForegroundProperty))
                    txt.Foreground = col.Foreground.Color;
                if (col.ExistLocalValue(Col.BackgroundProperty))
                    txt.Background = col.Background.Color;
                if (col.ExistLocalValue(Col.FontWeightProperty) && col.FontWeight.Weight >= 700)
                    txt.Bold = true;
                if (col.ExistLocalValue(Col.FontStyleProperty) && col.FontStyle == Windows.UI.Text.FontStyle.Italic)
                    txt.Italic = true;
                if (col.ExistLocalValue(Col.FontSizeProperty))
                    txt.FontSize = col.FontSize;

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
