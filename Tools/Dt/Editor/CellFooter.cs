using System.Text;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CellFooter : UserControl
    {
        public CellFooter()
        {
            InitializeComponent();

            ToolTip tip = new ToolTip();
            tip.SetToolTip(_rowSpan, "默认1行，-1时自动行高");
        }

        public void GetText(StringBuilder p_sb)
        {
            var txt = _titleWidth.Text.Trim();
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
