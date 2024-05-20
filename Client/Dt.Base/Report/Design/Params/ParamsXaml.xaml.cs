#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Toolkit.Sql;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base.Report
{
    public partial class ParamsXaml : Tab
    {
        ParamsDlg _dlg;

        public ParamsXaml(ParamsDlg p_dlg)
        {
            InitializeComponent();
            _dlg = p_dlg;
            _fv.Data = _dlg.Info.Root.Params;
            _tbXaml.SetBinding(TextBox.TextProperty, new Binding { Source = _dlg.Info.Root.Params.XamlRow, Path = new PropertyPath("Cells[xaml].Val"), Mode = BindingMode.TwoWay });
        }

        async void OnCreatePreview()
        {
            var fv = await _dlg.Info.Root.Params.CreateQueryForm(null);
            var dlg = new Dlg { Title = "查询面板", MinHeight = 300 };
            dlg.Content = fv;
            dlg.Show();
        }

        void OnAddCellXaml(Mi e)
        {
            AddCellXamlDlg.ShowDlg(_dlg, e.ID, txt => AppendXaml(txt));
        }

        void OnAddCListXaml()
        {
            AddCListXamlDlg.ShowDlg(_dlg, txt => AppendXaml(txt));
        }

        void OnAddCPickXaml()
        {
            AddCPickXamlDlg.ShowDlg(_dlg, txt => AppendXaml(txt));
        }
        
        void AppendXaml(string p_txt)
        {
            if (string.IsNullOrEmpty(p_txt))
                return;

            var txt = p_txt.Trim('\r', '\n');
            var prefix = _tbXaml.Text.Substring(0, _tbXaml.SelectionStart).TrimEnd('\r');
            if (prefix != "")
                prefix += "\r";

            string postfix = "";
            if (_tbXaml.Text.Length > _tbXaml.SelectionStart)
                postfix = "\r" + _tbXaml.Text.Substring(_tbXaml.SelectionStart).Trim('\r');
            _tbXaml.Text = prefix + txt + postfix;
        }
        
        async void OnAuto(object sender, RoutedEventArgs e)
        {
            if (_tbXaml.Text.Length > 0)
            {
                if (!await Kit.Confirm("自动生成Xaml会覆盖现有内容，要继续吗？"))
                    return;
            }
            _tbXaml.Text = _dlg.Info.Root.Params.CreateXamlByDefine();
        }

        void OnBar()
        {
            AppendXaml("<a:CBar Title=\"标题\" />");
        }

        void OnSqlFormatter(object sender, RoutedEventArgs e)
        {
            if (_tbXaml.SelectedText.Length > 7)
            {
                var sql = SqlFormatter.Format(_tbXaml.SelectedText);
                _tbXaml.Text = _tbXaml.Text.Substring(0, _tbXaml.SelectionStart).TrimEnd('\r') + "\r" + sql + "\r" + _tbXaml.Text.Substring(_tbXaml.SelectionStart + _tbXaml.SelectionLength).Trim('\r');
            }
        }
    }
}