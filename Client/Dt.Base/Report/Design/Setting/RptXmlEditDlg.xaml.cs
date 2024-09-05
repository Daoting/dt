#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-03-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;
using System.Text;
#endregion

namespace Dt.Base.Report
{
    public partial class RptXmlEditDlg : Dlg
    {
        RptDesignInfo _info;

        public RptXmlEditDlg()
        {
            InitializeComponent();
            IsPinned = true;
            if (!Kit.IsPhoneUI)
            {
                MaxWidth = Kit.ViewWidth - 200;
                MaxHeight = Kit.ViewHeight - 200;
            }
        }

        public void ShowDlg(RptDesignInfo p_info)
        {
            _info = p_info;
            _tb.Text = Rpt.SerializeTemplate(_info.Root);
            Show();
        }

        async void OnApply()
        {
            await _info.ImportTemplate(_tb.Text);
        }
    }
}