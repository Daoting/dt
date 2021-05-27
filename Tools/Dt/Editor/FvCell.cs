using System.Text;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class FvCell : UserControl
    {
        public FvCell()
        {
            InitializeComponent();
        }

        public void GetText(StringBuilder p_sb)
        {
            var txt = _id.Text.Trim();
            if (txt != "")
                p_sb.Append($" ID=\"{txt}\"");

            txt = _title.Text.Trim();
            if (txt != "")
                p_sb.Append($" Title=\"{txt}\"");

            txt = _titleWidth.Text.Trim();
            if (txt != "140")
                p_sb.Append($" TitleWidth=\"{txt}\"");

            if (!_showTitle.Checked)
                p_sb.Append(" ShowTitle=\"False\"");

            if (_isHorStretch.Checked)
                p_sb.Append(" IsHorStretch=\"True\"");

            if (_isVerticalTitle.Checked)
                p_sb.Append(" IsVerticalTitle=\"True\"");

            if (_showStar.Checked)
                p_sb.Append(" ShowStar=\"True\"");

            if (_autoCookie.Checked)
                p_sb.Append(" AutoCookie=\"True\"");

            if (_isReadOnly.Checked)
                p_sb.Append(" IsReadOnly=\"True\"");

            txt = _rowSpan.Text.Trim();
            if (txt != "1")
                p_sb.Append($" RowSpan=\"{txt}\"");

            txt = _placeholder.Text.Trim();
            if (txt != "")
                p_sb.Append($" Placeholder=\"{txt}\"");
        }

        public void Reset()
        {
            _id.Text = "";
            _title.Text = "";
            _titleWidth.Text = "140";
            _showTitle.Checked = true;
            _isHorStretch.Checked = false;
            _isVerticalTitle.Checked = false;
            _showStar.Checked = false;
            _autoCookie.Checked = false;
            _isReadOnly.Checked = false;
            _rowSpan.Text = "1";
            _placeholder.Text = "";
        }
    }
}
