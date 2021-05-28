using System.Text;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CColor : UserControl, ICellControl
    {
        public CColor()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CColor");
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
