#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using Windows.Graphics.Printing;
#endregion

namespace Dt.Base.ListView
{
    public partial class LvRptSettingDlg : Dlg
    {
        public LvRptSettingDlg()
        {
            InitializeComponent();
        }

        public Task ShowDlg(LvRptInfo p_info)
        {
            _fv.Data = p_info;
            return ShowAsync();
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

        void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}