using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Dt
{
    public partial class InsertSvcClassForm : Form
    {
        public InsertSvcClassForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
        }

        void InsertClass(string p_fileName)
        {
            string ns, cls;
            try
            {
                ns = Kit.GetText(_ns);
                cls = Kit.GetText(_cls);
            }
            catch
            {
                _lbl.Text = "命名空间和类名不可为空！";
                return;
            }

            var dt = new Dictionary<string, string>
                {
                    {"$rootnamespace$", ns },
                    {"$clsname$", cls },
                    {"$time$", DateTime.Now.ToString("yyyy-MM-dd") },
                    {"$username$", Environment.UserName },
                };
            var path = Path.Combine(Kit.GetFolderPath(), $"{cls}.cs");
            Kit.WritePrjFile(path, "Dt.SvcCls." + p_fileName, dt);
            Kit.OpenFile(path);

            Close();
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            InsertClass("ApiTemp.cs");
        }
    }
}
