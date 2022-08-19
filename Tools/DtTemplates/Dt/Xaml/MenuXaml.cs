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
    public partial class MenuXaml : Form
    {
        public MenuXaml()
        {
            InitializeComponent();
            _cbMenu.SelectedIndex = 0;
        }


        void button1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            if (_cbMenu.SelectedIndex == 0)
                sb.Append("<a:Tab.Menu>\r\n<a:Menu>");
            else
                sb.Append("<a:Ex.Menu>\r\n<a:Menu>");

            if (_add.Checked)
                sb.Append("\r\n<a:Mi ID=\"增加\" Icon=\"加号\" ShowInPhone=\"Icon\" />");

            if (_save.Checked)
                sb.Append("\r\n<a:Mi ID=\"保存\" Icon=\"保存\" ShowInPhone=\"Icon\" />");

            if (_del.Checked)
                sb.Append("\r\n<a:Mi ID=\"删除\" Icon=\"删除\" />");

            if (_search.Checked)
                sb.Append("\r\n<a:Mi ID=\"搜索\" Icon=\"搜索\" ShowInPhone=\"Icon\" />");

            if (_edit.Checked)
                sb.Append("\r\n<a:Mi ID=\"编辑\" Icon=\"修改\" />");

            if (_setting.Checked)
                sb.Append("\r\n<a:Mi ID=\"设置\" Icon=\"设置\" />");

            if (_cbMenu.SelectedIndex == 0)
                sb.Append("\r\n</a:Menu>\r\n</a:Tab.Menu>");
            else
                sb.Append("\r\n</a:Menu>\r\n</a:Ex.Menu>");
            Kit.Paste(sb.ToString());
            Close();
        }
    }
}
