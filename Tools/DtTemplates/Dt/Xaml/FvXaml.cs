using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class FvXaml : UserControl, ICmdForm
    {
        const string _tipItemHeight = "大于0时为固定行高，0时以第一行高度为准，NaN时自动调整高度(性能差)，默认0";

        public FvXaml()
        {
            InitializeComponent();

            AtSvc.BindSvcUrl(_svcUrl);
            AddTooltip();
        }

        async Task<string> ICmdForm.GetText()
        {
            StringBuilder sb = new StringBuilder();
            var txt = _name.Text.Trim() == "" ? "_fv" : _name.Text.Trim();
            sb.Append($"<a:Fv x:Name=\"{txt}\"");

            if (_cbReadonly.Checked)
                sb.Append($" IsReadOnly=\"True\"");
            sb.AppendLine(">");

            string tbl = null;
            if (_cbTbls.SelectedItem != null)
                tbl = _cbTbls.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(tbl))
            {
                // $namespace$ $rootnamespace$只能手动
                var xaml = await AtSvc.GetFvCells(tbl);
                if (!string.IsNullOrEmpty(xaml))
                    sb.AppendLine(xaml);
            }
            sb.Append("</a:Fv>");
            return sb.ToString();
        }

        async void _cbTbls_DropDown(object sender, EventArgs e)
        {
            var ls = await AtSvc.GetAllTables();
            if (_cbTbls.DataSource != ls)
                _cbTbls.DataSource = ls;
        }

        void AddTooltip()
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel4, Kit.AllTblsTip);
        }
    }
}
