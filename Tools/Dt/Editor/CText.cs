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
    public partial class CText : UserControl, ICellControl
    {
        public CText()
        {
            InitializeComponent();
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CText");
            _header.GetText(sb);

            if (_acceptsReturn.Checked)
                sb.Append(" AcceptsReturn=\"True\"");

            if (!_updateTimely.Checked)
                sb.Append(" UpdateTimely=\"False\"");

            _footer.GetText(sb);
            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
            _acceptsReturn.Checked = false;
            _updateTimely.Checked = true;
        }
    }
}
