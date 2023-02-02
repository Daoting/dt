using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Dt
{
    public partial class InsertClassForm : Form
    {
        string _tempFile;
        ClsType _clsType;

        public InsertClassForm(ClsType p_clsType)
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
            _clsType = p_clsType;

            switch (p_clsType)
            {
                case ClsType.DomainEx:
                    Text = "添加领域服务类";
                    if (Kit.IsClientPrj())
                    {
                        _info.Text = "命名规则：以Ds为后缀，建议以【模块名+Ds】命名";
                        _tempFile = "DomainSvc.cs";
                        _cls.Text = _ns.Text.Substring(_ns.Text.LastIndexOf(".") + 1) + "Ds";
                    }
                    else
                    {
                        _info.Text = "命名规则：以Ds为后缀";
                        _tempFile = "ServerDomainSvc.cs";
                        _cls.Text = "MyDs";
                    }
                    break;

                case ClsType.LvCall:
                    Text = "添加Lv的Call类";
                    _info.Text = "命名规则：以UI为后缀";
                    _tempFile = "LvCallTemp.cs";
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
                    Text = "添加数据访问类";
                    _info.Text = "命名规则：以At为前缀，Access To";
                    _tempFile = "AgentTemp.cs";
                    _cls.Text = "AtMySvc";
                    break;

                case ClsType.Event:
                    Text = "添加事件及处理类";
                    _info.Text = "内部自动为事件类添加 Event 后缀，为处理类添加 Handler 为后缀";
                    _tempFile = "EventTemp.cs";
                    _cls.Text = "InsertAbc";
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
            
            var path = _clsType == ClsType.Event ? Path.Combine(Kit.GetFolderPath(), $"{cls}Event.cs") : Path.Combine(Kit.GetFolderPath(), $"{cls}.cs");
            Kit.WritePrjFile(path, "Dt.InsertClass." + _tempFile, dt);
            Kit.OpenFile(path);

            Close();
        }
    }
}
