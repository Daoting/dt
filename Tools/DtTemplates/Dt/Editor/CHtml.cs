using System.Text;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CHtml : UserControl, ICellControl
    {
        public CHtml()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CHtml");
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
