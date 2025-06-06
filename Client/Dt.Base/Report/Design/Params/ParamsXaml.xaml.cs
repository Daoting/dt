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
            _tbXaml.Text = _dlg.Info.Root.Params.XamlRow.Str("xaml");
        }

        async void OnCreatePreview()
        {
            var fv = await _dlg.Info.Root.Params.CreateQueryForm(null);
            var dlg = new Dlg { Title = "查询面板", MinHeight = 300 };
            dlg.Content = fv;
            dlg.Show();
        }
        
        async void OnDesign(object sender, RoutedEventArgs e)
        {
            var info = new FvDesignInfo { Xaml = _dlg.Info.Root.Params.XamlRow.Str("xaml"), IsQueryFv = true };
            var cols = new List<EntityCol>();
            foreach (var r in _dlg.Info.Root.Params.Data)
            {
                Type tp;
                switch (r.Str("type").ToLower())
                {
                    case "bool":
                        tp = typeof(bool);
                        break;

                    case "double":
                        tp = typeof(double);
                        break;

                    case "int":
                        tp = typeof(int);
                        break;

                    case "date":
                        tp = typeof(DateTime);
                        break;

                    default:
                        tp = typeof(string);
                        break;
                }
                cols.Add(new EntityCol(r.Str("name"), tp));
            }
            info.Cols = cols;

            var xaml = await FvDesign.ShowDlg(info);
            if (!string.IsNullOrEmpty(xaml))
                _tbXaml.Text = xaml;
        }

        void OnXamlChanged(object sender, TextChangedEventArgs e)
        {
#if WIN || DESKTOP
            _dlg.Info.Root.Params.XamlRow["xaml"] = _tbXaml.Text.Trim().Replace('\r', '\n');
#else
            _dlg.Info.Root.Params.XamlRow["xaml"] = _tbXaml.Text.Trim();
#endif
        }
    }
}