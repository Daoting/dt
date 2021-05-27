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
    public partial class CBar : UserControl, ICellControl
    {
        public CBar()
        {
            InitializeComponent();

            ToolTip tip = new ToolTip();
            tip.SetToolTip(_rowSpan, "默认1行，-1时自动行高");
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CBar");
            var txt = _title.Text.Trim();
            if (txt != "")
                sb.Append($" Title=\"{txt}\"");

            txt = _rowSpan.Text.Trim();
            if (txt != "" && txt != "1")
                sb.Append($" RowSpan=\"{txt}\"");

            if (!_isHorStretch.Checked)
                sb.Append(" IsHorStretch=\"False\"");

            sb.AppendLine(" />");
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _title.Text = "";
            _rowSpan.Text = "1";
            _isHorStretch.Checked = true;
        }
    }
}
