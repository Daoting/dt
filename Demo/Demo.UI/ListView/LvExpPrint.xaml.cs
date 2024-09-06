#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-03-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics.Printing;
using Windows.Storage;
#endregion

namespace Demo.UI
{
    public partial class LvExpPrint : Win
    {
        LvRptInfo _info;

        public LvExpPrint()
        {
            InitializeComponent();
            _info = new LvRptInfo { RepeatRowHeaderCols = 1 };
            _fv.Data = _info;
            _lvFv.Data = _lv;
            OnClassicTbl(null, null);
        }

        void ShowReportDlg(object sender, RoutedEventArgs e)
        {
            _lv.ShowReport(_info, false, (bool)_cbSetting.IsChecked, (bool)_cbPdf.IsChecked);
        }

        void ShowReportWin(object sender, RoutedEventArgs e)
        {
            _lv.ShowReport(_info, true, (bool)_cbSetting.IsChecked, (bool)_cbPdf.IsChecked);
        }

        async void SaveExcel(object sender, RoutedEventArgs e)
        {
            await _lv.ExportExcel(_info, null, (bool)_cbSetting.IsChecked);
        }

        async void SaveExcelStream(object sender, RoutedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            await _lv.ExportExcel(_info, ms, (bool)_cbSetting.IsChecked);
            ms.Position = 0;

            Excel excel = new Excel();
            await excel.OpenExcel(ms);
            var dlg = new Dlg();
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 1000;
                dlg.Height = Kit.ViewHeight - 200;
            }
            dlg.Content = excel;
            dlg.Show();
        }

        void SavePdf(object sender, RoutedEventArgs e)
        {
            _ = _lv.ExportPdf(_info, null, (bool)_cbSetting.IsChecked);
        }

        void PrintExcel(object sender, RoutedEventArgs e)
        {
            _lv.Print(_info, (bool)_cbSetting.IsChecked);
        }

        async void PrintPdf(object sender, RoutedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            await _lv.ExportPdf(_info, ms, (bool)_cbSetting.IsChecked);
            ms.Position = 0;
            Kit.PrintPdf(ms, "新Pdf");
        }

        void OnPaperChanged(CList arg1, object arg2)
        {
            var size = PaperSize.Dict[(PrintMediaSize)Enum.Parse(typeof(PrintMediaSize), (string)arg2)];
            if (!size.IsEmpty)
            {
                _fv["PageHeight"].Val = Math.Round(size.Height / 0.96);
                _fv["PageWidth"].Val = Math.Round(size.Width / 0.96);
            }
        }

        void OnLoadPaperName(CList arg1, AsyncArgs arg2)
        {
            Nl<string> ls = new Nl<string>();
            foreach (var item in PaperSize.Dict.Keys)
            {
                ls.Add(item.ToString());
            }
            arg1.Data = ls;
        }

        void OnClassicTbl(object sender, RoutedEventArgs e)
        {
            using (_lv.Defer())
            {
                _lv.Data = null;
                _lv.ViewMode = ViewMode.Auto;
                _lv.ItemStyle = null;
                _lv.View = Resources["典型表格"];
                _lv.Data = CreateClassicData();
            }
        }

        void OnCustomTbl(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            DateTime birth = Kit.Now;
            INotifyList data = null;
            if (_cbDataSrc.IsChecked == true)
            {
                Nl<CustomItem> ls = new Nl<CustomItem>();
                for (int i = 0; i < 25; i++)
                {
                    ls.Add(new CustomItem
                    {
                        Style = (AppType)rand.Next(0, 6),
                        Shengao = (double)rand.Next(150, 190) / 100,
                        Warning = rand.Next(0, 20),
                    });
                }
                data = ls;
            }
            else
            {
                Table tbl = new Table
                {
                    { "style", typeof(AppType) },
                    { "shengao", typeof(double) },
                    { "warning", typeof(int) },
                };

                for (int i = 0; i < 25; i++)
                {
                    tbl.AddRow(new
                    {
                        style = (AppType)rand.Next(0, 6),
                        shengao = (double)rand.Next(150, 190) / 100,
                        warning = rand.Next(0, 20),
                    });
                }
                data = tbl;
            }

            using (_lv.Defer())
            {
                _lv.Data = null;
                _lv.ViewMode = ViewMode.Auto;
                _lv.ItemStyle = null;
                _lv.View = Resources["自定义单元格"];
                _lv.Data = data;
            }
        }

        void OnRowStyle(object sender, RoutedEventArgs e)
        {
            using (_lv.Defer())
            {
                _lv.Data = null;
                _lv.ViewMode = ViewMode.Auto;
                _lv.ItemStyle = e =>
                {
                    if (e.Data is Dt.Core.Row row)
                    {
                        e.Background = row.Date("chushengrq").Month == 9 ? Res.浅黄 : Res.WhiteBrush;
                        e.Foreground = row.Double("Shengao") > 1.75 ? Res.RedBrush : Res.BlackBrush;
                        e.FontWeight = row.Str("bumen") == "循环门诊" ? FontWeights.Bold : FontWeights.Normal;
                        e.FontStyle = row.Str("bumen") == "内分泌门诊" ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                    }
                    else if (e.Data is Person per)
                    {
                        e.Background = per.Chushengrq.Month == 9 ? Res.浅黄 : Res.WhiteBrush;
                        e.Foreground = per.Shengao > 1.75 ? Res.RedBrush : Res.BlackBrush;
                        e.FontWeight = per.Bumen == "循环门诊" ? FontWeights.Bold : FontWeights.Normal;
                        e.FontStyle = per.Bumen == "内分泌门诊" ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                    }
                };

                _lv.View = Resources["自定义行样式"];
                _lv.Data = (bool)_cbDataSrc.IsChecked ? SampleData.CreatePersonsList(100) : SampleData.CreatePersonsTbl(100);
            }
        }

        void OnListMode(object sender, RoutedEventArgs e)
        {
            using (_lv.Defer())
            {
                _lv.Data = null;
                _lv.ViewMode = ViewMode.List;
                _lv.ItemStyle = null;
                _lv.View = Resources["ListView"];
                _lv.Data = CreateClassicData();
            }
        }

        void OnTileMode(object sender, RoutedEventArgs e)
        {
            using (_lv.Defer())
            {
                _lv.Data = null;
                _lv.ViewMode = ViewMode.Tile;
                _lv.ItemStyle = null;
                _lv.View = Resources["ListView"];
                _lv.Data = CreateClassicData();
            }
        }

        void OnEmpty(object sender, RoutedEventArgs e)
        {
            _lv.Data = null;
        }
        
        INotifyList CreateClassicData()
        {
            Random rand = new Random();
            DateTime birth = Kit.Now;

            if (_cbDataSrc.IsChecked == true)
            {
                Nl<ClassicItem> ls = new Nl<ClassicItem>();
                for (int i = 0; i < 50; i++)
                {
                    ls.Add(new ClassicItem
                    {
                        Scale = rand.NextDouble(),
                        Date = birth.AddMonths(rand.Next(100)),
                        Icon = i,
                        CheckBox = (i % 2 == 0),
                        AutoDate = birth.AddHours(-rand.Next(100)),
                        红白 = "红底白字",
                    });
                }
                return ls;
            }

            Table tbl = new Table
            {
                { "scale", typeof(double) },
                { "date", typeof(DateTime) },
                { "Icon", typeof(int) },
                { "CheckBox", typeof(bool) },
                { "AutoDate", typeof(DateTime) },
                { "红白" },
            };


            for (int i = 0; i < 50; i++)
            {
                tbl.AddRow(new
                {
                    scale = rand.NextDouble(),
                    date = birth.AddMonths(rand.Next(100)),
                    Icon = i,
                    CheckBox = (i % 2 == 0),
                    AutoDate = birth.AddHours(-rand.Next(100)),
                    红白 = "红底白字",
                });
            }
            return tbl;
        }

        public class ClassicItem
        {
            public double Scale { get; set; }
            public DateTime Date { get; set; }
            public int Icon { get; set; }
            public bool CheckBox { get; set; }
            public DateTime AutoDate { get; set; }
            public string 红白 { get; set; }
        }

        public class CustomItem
        {
            public AppType Style { get; set; }
            public double Shengao { get; set; }
            public int Warning { get; set; }
        }
    }
}