using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Dt
{
    public partial class ManyToManyForm : Form
    {
        public ManyToManyForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            string ns, entity, title;
            try
            {
                ns = Kit.GetText(_ns);
                entity = Kit.GetText(_clsa);
                title = Kit.GetText(_clsaTitle);
            }
            catch
            {
                _info.Text = "当前内容不可为空！";
                return;
            }

            var dt = new Dictionary<string, string>
                {
                    {"$rootnamespace$", ns },
                    {"$entityname$", entity },
                    {"$entitytitle$", title },
                    {"$time$", DateTime.Now.ToString("yyyy-MM-dd") },
                    {"$username$", Environment.UserName },
                };
            var path = Kit.GetFolderPath();

            Kit.WritePrjFile(Path.Combine(path, $"{entity}Win.xaml"), "Dt.SingleTbl.EntityWin.xaml", dt);
            Kit.WritePrjFile(Path.Combine(path, $"{entity}Win.xaml.cs"), "Dt.SingleTbl.EntityWin.xaml.cs", dt);

            Kit.WritePrjFile(Path.Combine(path, $"{entity}List.xaml"), "Dt.SingleTbl.EntityList.xaml", dt);
            Kit.WritePrjFile(Path.Combine(path, $"{entity}List.xaml.cs"), "Dt.SingleTbl.EntityList.xaml.cs", dt);

            Kit.WritePrjFile(Path.Combine(path, $"{entity}Form.xaml"), "Dt.SingleTbl.EntityForm.xaml", dt);
            Kit.WritePrjFile(Path.Combine(path, $"{entity}Form.xaml.cs"), "Dt.SingleTbl.EntityForm.xaml.cs", dt);

            Kit.WritePrjFile(Path.Combine(path, $"{entity}Obj.cs"), "Dt.SingleTbl.EntityObj.cs", dt);
            Close();
        }


    }
}
