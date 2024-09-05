#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Graphics.Printing;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class PageSettingDlg : Dlg
    {
        RptDesignInfo _info;

        public PageSettingDlg()
        {
            InitializeComponent();
            IsPinned = true;
        }

        public void ShowDlg(RptDesignInfo p_info)
        {
            _info = p_info;
            _fv.Data = _info.Root.PageSetting.Data;
            _cbHeader.IsChecked = _info.Root.Header.DefaultHeader;
            _cbFooter.IsChecked = _info.Root.Footer.DefaultFooter;
            InitPaperName();
            Show();
        }

        void InitPaperName()
        {
            Nl<string> ls = new Nl<string>();
            foreach (var item in PaperSize.Dict.Keys)
            {
                ls.Add(item.ToString());
            }
            ((CList)_fv["papername"]).Data = ls;
        }

        void OnPaperChanged(CList arg1, object arg2)
        {
            Size size = PaperSize.Dict[(PrintMediaSize)Enum.Parse(typeof(PrintMediaSize), (string)arg2)];
            if (!size.IsEmpty)
            {
                _info.Root.PageSetting.Height = Math.Round(size.Height / 0.96);
                _info.Root.PageSetting.Width = Math.Round(size.Width / 0.96);
            }
        }

        async void OnDefHeader(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var header = _info.Root.Header;
            if (cb.IsChecked == true)
            {
                if (header.Items.Count > 0)
                {
                    if (await Kit.Confirm("采用默认页眉将删除已有的页眉，确认继续吗？"))
                    {
                        header.DefaultHeader = true;
                    }
                    else
                    {
                        cb.IsChecked = false;
                    }
                }
                else
                {
                    header.DefaultHeader = true;
                }
            }
            else
            {
                header.DefaultHeader = false;
            }
        }

        async void OnDefFooter(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var footer = _info.Root.Footer;
            if (cb.IsChecked == true)
            {
                if (footer.Items.Count > 0)
                {
                    if (await Kit.Confirm("采用默认页脚将删除已有的页脚，确认继续吗？"))
                    {
                        footer.DefaultFooter = true;
                    }
                    else
                    {
                        cb.IsChecked = false;
                    }
                }
                else
                {
                    footer.DefaultFooter = true;
                }
            }
            else
            {
                footer.DefaultFooter = false;
            }
        }
    }
}
