using System.Text;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CMask : UserControl, ICellControl
    {
        public CMask()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CMask");
            _header.GetText(sb);

            if (_maskType.SelectedIndex != 0)
                sb.Append($" MaskType=\"{_maskType.SelectedItem}\"");

            var txt = _mask.Text.Trim();
            if (txt != "-1")
                sb.Append($" Mask=\"{txt}\"");

            if (_autoComplete.SelectedIndex != 0)
                sb.Append($" AutoComplete=\"{_autoComplete.SelectedItem}\"");
            
            if (_showPlaceHolder.Checked)
                sb.Append(" ShowPlaceHolder=\"True\"");

            if (!_saveLiteral.Checked)
                sb.Append(" SaveLiteral=\"False\"");

            if (!_useAsDisplayFormat.Checked)
                sb.Append(" UseAsDisplayFormat=\"False\"");

            if (_allowNullInput.Checked)
                sb.Append(" AllowNullInput=\"True\"");

            if (!_ignoreBlank.Checked)
                sb.Append(" IgnoreBlank=\"False\"");

            _footer.GetText(sb);
            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
            _maskType.SelectedIndex = 0;
            _mask.Text = "";
            _autoComplete.SelectedIndex = 0;
            _showPlaceHolder.Checked = false;
            _saveLiteral.Checked = true;
            _useAsDisplayFormat.Checked = true;
            _allowNullInput.Checked = false;
            _ignoreBlank.Checked = true;
        }
    }
}
