using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Dt
{
    public partial class InsertMvForm : Form
    {
        public InsertMvForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
            _cb.SelectedIndex = 0;
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            string ns, cls, title;
            try
            {
                ns = Kit.GetText(_ns);
                cls = Kit.GetText(_cls);
                title = Kit.GetText(_title);
            }
            catch
            {
                _lbl.Text = "当前内容不可为空！";
                return;
            }

            var dt = new Dictionary<string, string>
                {
                    {"$rootnamespace$", ns },
                    {"$safeitemname$", cls },
                    {"$title$", title },
                    {"$time$", DateTime.Now.ToString("yyyy-MM-dd") },
                    {"$username$", Environment.UserName },
                };

            string res;
            switch (_cb.SelectedIndex)
            {
                case 0:
                    res = "Dt.Mv.BlankMv.xaml";
                    break;
                case 1:
                    res = "Dt.Mv.FormMv.xaml";
                    break;
                default:
                    res = "Dt.Mv.ListMv.xaml";
                    break;
            }

            var path = Path.Combine(Kit.GetFolderPath(), $"{cls}.xaml");
            Kit.WritePrjFile(path, res, dt);
            Kit.OpenFile(path);

            path = Path.Combine(Kit.GetFolderPath(), $"{cls}.xaml.cs");
            Kit.WritePrjFile(path, res + ".cs", dt);
            Kit.OpenFile(path);

            Close();
        }
    }
}
