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
    public partial class CDate : UserControl, ICellControl
    {
        public CDate()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CDate");
            _header.GetText(sb);

            if (_format.SelectedIndex != 0)
                sb.Append($" Format=\"{_format.Text}\"");

            if (_alwaysTouchPicker.Checked)
                sb.Append(" AlwaysTouchPicker=\"True\"");

            _footer.GetText(sb);
            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
            _format.SelectedIndex = 0;
            _alwaysTouchPicker.Checked = false;
        }
    }
}
