#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public partial class ParamsForm : Dlg
    {
        ParamsDlg _dlg;

        public ParamsForm(ParamsDlg p_dlg)
        {
            InitializeComponent();
            _dlg = p_dlg;
            IsPinned = true;
            Width = 450;
            Height = 400;
        }

        public void Update(Row p_row)
        {
            _fv.Data = p_row;
        }

        public void Open(Row p_row)
        {
            _fv.Data = p_row;
            Show();
        }

        void OnAdd()
        {
            _fv.Data = _dlg.Info.Root.Params.Data.AddRow(new { name = "", type = "string" });
        }

        async void OnDel()
        {
            if (_fv.Data != null && await Kit.Confirm("确认要删除吗？"))
            {
                _dlg.Info.Root.Params.Data.Remove(_fv.Row);
                _fv.Data = null;
            }
        }

        async void OnExpression(object sender, RoutedEventArgs e)
        {
            if (_fv.Data is Row row)
            {
                var val = await ExpressionDlg.ShowDlg();
                if (!string.IsNullOrEmpty(val))
                    row["val"] = val;
            }
        }

        async void OnCustom(object sender, RoutedEventArgs e)
        {
            if (_fv.Data is Row row)
            {
                var val = await ValueCallsDlg.ShowDlg((Button)sender);
                if (!string.IsNullOrEmpty(val))
                    row["val"] = val;
            }
        }
    }
}