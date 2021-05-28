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
    public partial class CFile : UserControl, ICellControl
    {
        public CFile()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CFile");
            _header.GetText(sb);

            var txt = _colCount.Text.Trim();
            if (txt != "1")
                sb.Append($" ColCount=\"{txt}\"");

            txt = _spacing.Text.Trim();
            if (txt != "0")
                sb.Append($" Spacing=\"{txt}\"");

            txt = _imageHeight.Text.Trim();
            if (txt != "82")
                sb.Append($" ImageHeight=\"{txt}\"");

            if (_imageStretch.SelectedIndex != 2)
                sb.Append($" ImageStretch=\"{_imageStretch.SelectedItem}\"");

            txt = _fixedVolume.Text.Trim();
            if (txt != "")
                sb.Append($" FixedVolume=\"{txt}\"");

            txt = _maxFileCount.Text.Trim();
            if (txt != "")
                sb.Append($" MaxFileCount=\"{txt}\"");

            if (!_showDefaultToolbar.Checked)
                sb.Append(" ShowDefaultToolbar=\"False\"");

            if (!_showDefaultMenu.Checked)
                sb.Append(" ShowDefaultMenu=\"False\"");

            if (!_enableClick.Checked)
                sb.Append(" EnableClick=\"False\"");

            _footer.GetText(sb);
            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
            _colCount.Text = "1";
            _spacing.Text = "0";
            _imageHeight.Text = "82";
            _imageStretch.SelectedIndex = 2;
            _fixedVolume.Text = "";
            _maxFileCount.Text = "";
            _showDefaultToolbar.Checked = true;
            _showDefaultMenu.Checked = true;
            _enableClick.Checked = true;
        }
    }
}
