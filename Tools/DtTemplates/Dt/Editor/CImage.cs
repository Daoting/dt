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
    public partial class CImage : UserControl, ICellControl
    {
        public CImage()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CImage");
            _header.GetText(sb);

            var txt = _imageHeight.Text.Trim();
            if (txt != "82")
                sb.Append($" ImageHeight=\"{txt}\"");

            if (_imageStretch.SelectedIndex != 2)
                sb.Append($" ImageStretch=\"{_imageStretch.SelectedItem}\"");
            
            txt = _fixedVolume.Text.Trim();
            if (txt != "")
                sb.Append($" FixedVolume=\"{txt}\"");

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
            _imageHeight.Text = "82";
            _imageStretch.SelectedIndex = 2;
            _fixedVolume.Text = "";
            _showDefaultMenu.Checked = true;
            _enableClick.Checked = true;
        }
    }
}
