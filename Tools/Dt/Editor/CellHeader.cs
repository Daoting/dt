using System.Text;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CellHeader : UserControl
    {
        public CellHeader()
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
        }

        public void Reset()
        {
            _id.Text = "";
            _title.Text = "";
            _id.Focus();
        }
    }
}
