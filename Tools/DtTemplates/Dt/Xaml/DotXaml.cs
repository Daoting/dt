using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Dt.Editor
{
    public partial class DotXaml : Form
    {
        public DotXaml()
        {
            InitializeComponent();
            _cbUI.SelectedIndex = 0;
            _font.SelectedIndex = 0;
            _svcUrl.Text = AtSvc.SvcUrl;
            AddTooltip();
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            var id = _id.Text.Trim();
            if (id == "")
            {
                MessageBox.Show("ID不可为空！");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"<a:Dot ID=\"{id}\"");

            if (_cbUI.SelectedIndex != 0)
                sb.Append($" UI=\"{_cbUI.SelectedItem}\"");

            var format = _format.Text.Trim();
            if (format != "")
                sb.Append($" Format=\"{format}\"");

            if (_font.SelectedIndex != 0)
                sb.Append($" Font=\"{_font.SelectedItem}\"");

            sb.Append(" />");

            Kit.Paste(sb.ToString());
            _id.Text = "";
        }

        async void button1_Click(object sender, EventArgs e)
        {
            string tbl = null;
            if (_cbTbls.SelectedItem != null)
                tbl = _cbTbls.SelectedItem.ToString();
            if (string.IsNullOrEmpty(tbl))
            {
                MessageBox.Show("批量添加需要选择表！");
                return;
            }

            var xaml = await AtSvc.GetLvItemTemplate(tbl);
            Kit.Paste(xaml);
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
