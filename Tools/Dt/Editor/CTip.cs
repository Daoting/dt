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
    public partial class CTip : UserControl, ICellControl
    {
        public CTip()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CTip");
            _header.GetText(sb);

            if (_format.Text.Trim() != "")
                sb.Append($" Format=\"{_format.Text.Trim()}\"");

            _footer.GetText(sb);
            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
            _format.SelectedIndex = -1;
        }
    }
}
