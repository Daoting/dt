using System.Text;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CNum : UserControl, ICellControl
    {
        public CNum()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CNum");
            _header.GetText(sb);

            var txt = _decimals.Text.Trim();
            if (txt != "-1")
                sb.Append($" Decimals=\"{txt}\"");

            txt = _maximum.Text.Trim();
            if (txt != "")
                sb.Append($" Maximum=\"{txt}\"");

            txt = _minimum.Text.Trim();
            if (txt != "")
                sb.Append($" Minimum=\"{txt}\"");

            txt = _customUnit.Text.Trim();
            if (txt != "")
                sb.Append($" CustomUnit=\"{txt}\"");

            txt = _nullValue.Text.Trim();
            if (txt != "")
                sb.Append($" NullValue=\"{txt}\"");

            txt = _largeChange.Text.Trim();
            if (txt != "10")
                sb.Append($" LargeChange=\"{txt}\"");

            txt = _smallChange.Text.Trim();
            if (txt != "1")
                sb.Append($" SmallChange=\"{txt}\"");

            if (_valueFormat.SelectedIndex != 0)
                sb.Append($" ValueFormat=\"{_valueFormat.SelectedItem}\"");

            if (_isInteger.Checked)
                sb.Append(" IsInteger=\"True\"");

            if (_updateTimely.Checked)
                sb.Append(" UpdateTimely=\"True\"");

            if (!_autoReverse.Checked)
                sb.Append(" AutoReverse=\"False\"");

            _footer.GetText(sb);
            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
            _decimals.Text = "-1";
            _maximum.Text = "";
            _minimum.Text = "";
            _customUnit.Text = "";
            _nullValue.Text = "";
            _largeChange.Text = "10";
            _smallChange.Text = "1";
            _valueFormat.SelectedIndex = 0;
            _isInteger.Checked = false;
            _updateTimely.Checked = false;
            _autoReverse.Checked = true;
        }
    }
}
