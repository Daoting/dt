#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-08-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Tools
{
    public partial class ToggleSvcUrlDlg : Dlg
    {
        public ToggleSvcUrlDlg()
        {
            InitializeComponent();
        }

        public static void ShowDlg()
        {
            var dlg = new ToggleSvcUrlDlg
            {
                IsPinned = true,
            };

            dlg.Show();
        }
    }
}