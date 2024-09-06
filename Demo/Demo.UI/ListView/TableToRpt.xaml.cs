#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-03-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using Windows.Graphics.Printing;
#endregion

namespace Demo.UI
{
    public partial class TableToRpt : Win
    {
        TblRptInfo _info;
        Table _tbl;

        public TableToRpt()
        {
            InitializeComponent();
            _info = new TblRptInfo { RepeatRowHeaderCols = 1 };
            _fv.Data = _info;
            _tbl = SampleData.CreatePersonsTbl(100);
        }

        void ShowReportDlg(object sender, RoutedEventArgs e)
        {
            _tbl.ShowReport(_info, false, (bool)_cbSetting.IsChecked, (bool)_cbPdf.IsChecked);
        }

        void ShowReportWin(object sender, RoutedEventArgs e)
        {
            _tbl.ShowReport(_info, true, (bool)_cbSetting.IsChecked, (bool)_cbPdf.IsChecked);
        }

        async void SaveExcel(object sender, RoutedEventArgs e)
        {
            await _tbl.ExportExcel(_info, null, (bool)_cbSetting.IsChecked);
        }

        async void SaveExcelStream(object sender, RoutedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            await _tbl.ExportExcel(_info, ms, (bool)_cbSetting.IsChecked);
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
            _ = _tbl.ExportPdf(_info, null, (bool)_cbSetting.IsChecked);
        }

        void PrintExcel(object sender, RoutedEventArgs e)
        {
            _tbl.Print(_info, (bool)_cbSetting.IsChecked);
        }

        async void PrintPdf(object sender, RoutedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            await _tbl.ExportPdf(_info, ms, (bool)_cbSetting.IsChecked);
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
    }
}