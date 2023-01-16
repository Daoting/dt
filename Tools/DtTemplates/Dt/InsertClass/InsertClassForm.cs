using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Dt
{
    public partial class InsertClassForm : Form
    {
        string _tempFile;

        public InsertClassForm(ClsType p_clsType)
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();

            switch (p_clsType)
            {
                case ClsType.DomainEx:
                    Text = "添加业务扩展类";
                    _info.Text = "命名规则：以Ex为后缀，建议以【模块名+Ex】 命名";
                    _tempFile = "DomainEx.cs";
                    _cls.Text = _ns.Text.Substring(_ns.Text.LastIndexOf(".") + 1) + "Ex";
                    break;

                case ClsType.LvCall:
                    Text = "添加Lv的Call类";
                    _info.Text = "命名规则：以UI为后缀";
                    _tempFile = "CellUITemp.cs";
                    _cls.Text = "MyCallUI";
                    break;

                case ClsType.FvCall:
                    Text = "添加Fv的Call类";
                    _info.Text = "命名规则：以UI为后缀";
                    _tempFile = "FvCallTemp.cs";
                    _cls.Text = "MyCallUI";
                    break;

                case ClsType.CListEx:
                    Text = "添加CList的Ex类";
                    _info.Text = "";
                    _tempFile = "CListExTemp.cs";
                    _cls.Text = "MyCListEx";
                    break;

                case ClsType.Agent:
                    Text = "添加Agent类";
                    _info.Text = "命名规则：以At为前缀";
                    _tempFile = "AgentTemp.cs";
                    _cls.Text = "AtMyCls";
                    break;

                case ClsType.Api:
                    Text = "添加Api类";
                    _info.Text = "命名规则：以Api为后缀";
                    _tempFile = "ApiTemp.cs";
                    _cls.Text = "MyClsApi";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ns, cls;
            try
            {
                ns = Kit.GetText(_ns);
                cls = Kit.GetText(_cls);
            }
            catch
            {
                MessageBox.Show("命名空间和类名不可为空！");
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
            Kit.WritePrjFile(path, "Dt.InsertClass." + _tempFile, dt);
            Kit.OpenFile(path);

            Close();
        }
    }
}
