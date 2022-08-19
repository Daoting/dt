using Dt.Editor;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt
{
    public partial class CmdForm : Form
    {
        ICmdForm _content;

        public CmdForm(Type p_type)
        {
            InitializeComponent();

			_content = Activator.CreateInstance(p_type) as ICmdForm;

			var uc = _content as UserControl;
			uc.AutoScroll = true;
			uc.Location = new Point((ClientSize.Width - uc.ClientSize.Width) / 2, 0);
			Controls.Add(uc);
		}

        async void _btnOK_Click(object sender, EventArgs e)
        {
            string txt = await _content.GetText();
			if (!string.IsNullOrEmpty(txt))
            {
				Kit.Paste(txt);
            }
			DialogResult = DialogResult.OK;
			Close();
		}

        void _btnCancel_Click(object sender, EventArgs e)
        {
			DialogResult = DialogResult.Cancel;
			Close();
		}
    }
}
