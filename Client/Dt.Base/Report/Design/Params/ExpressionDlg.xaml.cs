#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public partial class ExpressionDlg : Dlg
    {
        public ExpressionDlg()
        {
            InitializeComponent();
            _lv.Data = ValueExpression.Data;
            if (!Kit.IsPhoneUI)
            {
                Width = 300;
                Height = 600;
            }
        }

        public static async Task<string> ShowDlg()
        {
            var dlg = new ExpressionDlg();
            if (await dlg.ShowAsync() && dlg._lv.SelectedItem is Row row)
                return row.Str("name");
            return null;
        }

        void OnCopy()
        {
            Close(true);
        }

        void OnItemDoubleClick(object obj)
        {
            Close(true);
        }
    }
}