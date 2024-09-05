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
using System.Text.RegularExpressions;
#endregion

namespace Dt.Base.Report
{
    public partial class AddCListXamlDlg : Dlg
    {
        const string _sql = "    <a:CList.Sql>\r        <a:Sql>\rselect * from tbl where id=@[列名] \r        </a:Sql>\r    </a:CList.Sql>";
        ParamsDlg _dlg;
        Action<string> _callback;
        List<string> _ids = new List<string>();

        public AddCListXamlDlg()
        {
            InitializeComponent();
            _fv.Data = new Row
            {
                { "param",  typeof(string) },
                { "SrcID", "" },
                { "TgtID", "" },
                { "IsEditable", false },
                { "Data", "" },
                { "View", "" },
            };
        }

        public static void ShowDlg(ParamsDlg p_dlg, Action<string> p_callback)
        {
            var dlg = new AddCListXamlDlg { _dlg = p_dlg, _callback = p_callback };
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 460;
                dlg.Height = 780;
            }
            dlg.InitData();
            dlg.Show();
        }

        void InitData()
        {
            _ids = new List<string>();
            var xaml = _dlg.Info.Root.Params.Xaml;
            _ids.Clear();
            if (xaml != "")
            {
                Regex reg = new Regex(@" ID=""([^""]+)");
                var matches = reg.Matches(xaml);
                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 1)
                        _ids.Add(match.Groups[1].Value);
                }
            }

            _params.Lv.ItemStyle = e =>
            {
                e.Foreground = _ids.Contains(e.Row.Str("name")) ? Res.RedBrush : Res.GreenBrush;
            };
            _params.Data = _dlg.Info.Root.Params.Data;

            foreach (var r in _dlg.Info.Root.Params.Data)
            {
                var id = r.Str("name");
                if (!_ids.Contains(id))
                {
                    _fv.Row["param"] = r.Str("name");
                    break;
                }
            }
        }

        async void OnCopy()
        {
            var r = _fv.Row;
            var id = r.Str("param");
            if (id == "")
            {
                Kit.Warn("请选择参数！");
                return;
            }

            if (_ids.Contains(id))
            {
                if (!await Kit.Confirm($"Xaml中已包含 [{id}] 单元格，是否继续添加？"))
                    return;
            }

            StringBuilder sb = new StringBuilder("<a:CList");

            if (Kit.IsChiness(id))
                sb.AppendFormat(" ID=\"{0}\"", id);
            else
                sb.AppendFormat(" ID=\"{0}\" Title=\"{0}\"", id);
            
            if (r.Str("SrcID") != "")
                sb.AppendFormat(" SrcID=\"{0}\"", r.Str("SrcID"));

            if (r.Str("TgtID") != "")
                sb.AppendFormat(" TgtID=\"{0}\"", r.Str("TgtID"));

            if (r.Bool("IsEditable"))
                sb.Append(" IsEditable=\"True\"");
            sb.AppendLine(">");
            
            var data = r.Str("data");
            if (data != "")
                sb.AppendLine(data);
            else
                sb.AppendLine(_sql);
            
            var view = r.Str("view");
            if (view != "")
                sb.AppendLine(view);
            sb.Append("</a:CList>");
            
            _callback(sb.ToString());
            Close();
        }

        void OnAddOption()
        {
            AddDataText("    <a:CList.Ex>Option#分组名</a:CList.Ex>");
        }

        void OnAddSql()
        {
            AddDataText(_sql);
        }
        
        void OnAddItems()
        {
            AddDataText("    <a:CList.Items>\r        <x:String>字符串</x:String>\r        <x:Int32>1</x:Int32>\r        <a:IDStr ID=\"0\" Str=\"名称\" />\r    </a:CList.Items>");
        }

        async void AddDataText(string p_str)
        {
            var r = _fv.Row;
            if (r.Str("data") != "")
            {
                if (!await Kit.Confirm("已有内容，是否覆盖？"))
                    return;
            }
            _fv.Row["data"] = p_str;
        }

        void OnAddCols()
        {
            AddViewText("    <a:Cols>\r        <a:Col ID=\"name\" Title=\"名称\" />\r    </a:Cols>");
        }

        void OnAddTemplate()
        {
            AddViewText("<DataTemplate>\r    <a:Dot ID=\"name\" Padding=\"10\" />\r</DataTemplate>");
        }

        async void AddViewText(string p_str)
        {
            var r = _fv.Row;
            if (r.Str("View") != "")
            {
                if (!await Kit.Confirm("已有内容，是否覆盖？"))
                    return;
            }
            _fv.Row["View"] = p_str;
        }
    }
}