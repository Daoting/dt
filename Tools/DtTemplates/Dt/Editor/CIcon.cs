using System.Text;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CIcon : UserControl, ICellControl
    {
        public CIcon()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CIcon");
            _header.GetText(sb);
            _footer.GetText(sb);
            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
        }
    }
}
