using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CTree : UserControl, ICellControl
    {
        public CTree()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CTree");
            _header.GetText(sb);

            if (_selectionMode.SelectedIndex != 1)
                sb.Append($" SelectionMode=\"{_selectionMode.SelectedItem}\"");

            var txt = _srcID.Text.Trim();
            if (txt != "")
                sb.Append($" SrcID=\"{txt}\"");

            txt = _tgtID.Text.Trim();
            if (txt != "")
                sb.Append($" TgtID=\"{txt}\"");

            if (_refreshData.Checked)
                sb.Append(" RefreshData=\"True\"");

            _footer.GetText(sb);
            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
            _selectionMode.SelectedIndex = 1;
            _srcID.Text = "";
            _tgtID.Text = "";
            _refreshData.Checked = false;
        }
    }
}
