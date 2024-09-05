#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-03-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Base.Report
{
    public partial class AddCellXamlDlg : Dlg
    {
        ParamsDlg _dlg;
        Action<string> _callback;
        string _cell;
        List<string> _ids;

        public AddCellXamlDlg()
        {
            InitializeComponent();
            _fv.Data = new Row { { "param", typeof(string) } };
        }

        public static void ShowDlg(ParamsDlg p_dlg, string p_cell, Action<string> p_callback)
        {
            var dlg = new AddCellXamlDlg { _dlg = p_dlg, _cell = p_cell, _callback = p_callback };
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 450;
                dlg.Height = 500;
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
                    _fv.Row[0] = r.Str("name");
                    break;
                }
            }

            LoadCellProp();
        }

        async void OnCopy()
        {
            var id = _fv.Row.Str(0);
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

            StringBuilder sb = new StringBuilder("<a:");
            sb.Append(_cell);

            if (Kit.IsChiness(id))
                sb.AppendFormat(" ID=\"{0}\"", id);
            else
                sb.AppendFormat(" ID=\"{0}\" Title=\"{0}\"", id);

            foreach (var row in _lvAttr.SelectedRows)
            {
                sb.Append(" ");
                sb.Append(row.Str("属性"));
                sb.Append("=\"");
                sb.Append(row.Str("值"));
                sb.Append("\"");
            }
            sb.Append(" />");
            _callback(sb.ToString());
            Close();
        }

        void LoadCellProp()
        {
            var tp = Type.GetType($"Dt.Base.{_cell},Dt.Base");
            if (tp == null)
                return;

            Table tbl = new Table { { "属性" }, { "说明" }, { "值" } };

            List<string> names = new List<string>();
            if (_cell == "CText")
            {
                tbl.AddRow(new { 属性 = "AcceptsReturn", 说明 = "允许多行", 值 = "true" });
                tbl.AddRow(new { 属性 = "MaxLength", 说明 = "最大字符数", 值 = "50" });
                names.Add("AcceptsReturn");
                names.Add("MaxLength");
            }
            else if (_cell == "CBool")
            {
                tbl.AddRow(new { 属性 = "ShowTitle", 说明 = "是否显示标题列", 值 = "false" });
                tbl.AddRow(new { 属性 = "IsSwitch", 说明 = "是否为开关", 值 = "true" });
                names.Add("ShowTitle");
                names.Add("IsSwitch");
            }
            else if (_cell == "CNum")
            {
                tbl.AddRow(new { 属性 = "IsInteger", 说明 = "是否为整数", 值 = "true" });
                tbl.AddRow(new { 属性 = "Decimals", 说明 = "小数位数", 值 = "2" });
                names.Add("IsInteger");
                names.Add("Decimals");
            }
            else if (_cell == "CDate")
            {
                tbl.AddRow(new { 属性 = "Format", 说明 = "格式串", 值 = "yyyy-MM-dd HH:mm:ss" });
                names.Add("Format");
            }

            tbl.AddRow(new { 属性 = "RowSpan", 说明 = "占用的行数，-1时自动行高", 值 = "2" });
            foreach (var info in tp.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                // 可设置属性
                var attr = (CellParamAttribute)info.GetCustomAttribute(typeof(CellParamAttribute), false);
                if (attr != null && !names.Contains(info.Name))
                    tbl.AddRow(new { 属性 = info.Name, 说明 = attr.Title });
            }
            tbl.AddRow(new { 属性 = "ShowTitle", 说明 = "是否显示标题列", 值 = "false" });
            tbl.AddRow(new { 属性 = "Placeholder", 说明 = "占位符文本", 值 = "提示信息" });
            tbl.AddRow(new { 属性 = "TitleWidth", 说明 = "列名的宽度", 值 = "200" });
            tbl.AddRow(new { 属性 = "IsVerticalTitle", 说明 = "是否垂直显示标题", 值 = "true" });
            tbl.AddRow(new { 属性 = "ColSpan", 说明 = "范围0~1，0水平填充，1占满整列", 值 = "0.5" });
            tbl.AddRow(new { 属性 = "IsReadOnly", 说明 = "是否只读", 值 = "true" });
            _lvAttr.Data = tbl;
        }
    }
}