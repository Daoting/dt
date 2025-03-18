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

namespace Dt.Base.FormView
{
    public partial class FvXamlEditDlg : Dlg
    {
        FvDesign _design;

        public FvXamlEditDlg()
        {
            InitializeComponent();
            IsPinned = true;
            if (!Kit.IsPhoneUI)
            {
                Width = 600;
                Height = 500;
            }
        }

        public void ShowDlg(FvDesign p_design)
        {
            _design = p_design;
            _tb.Text = _design.GetXaml();
            Show();
        }

        void OnApply()
        {
            _design.Jz(_tb.Text);
            Close();
        }
    }
}