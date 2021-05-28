using System.Text;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CPassword : UserControl, ICellControl
    {
        public CPassword()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CPassword");
            _header.GetText(sb);

            var txt = _holder.Text.Trim();
            if (txt != "●" && txt != "")
                sb.Append($" Holder=\"{txt}\"");

            txt = _maxLength.Text.Trim();
            if (txt != "")
                sb.Append($" MaxLength=\"{txt}\"");

            _footer.GetText(sb);
            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
            _holder.Text = "●";
            _maxLength.Text = "";
        }
    }
}
