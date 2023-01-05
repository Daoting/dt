using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Dt
{
    public partial class InsertClassForm : Form
    {
        public InsertClassForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
            AtSvc.BindSvcUrl(_svcUrl);
            AddTooltip();
        }

        void InsertClass(string p_fileName, Dictionary<string, string> p_extra = null)
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
            if (p_extra != null)
            {
                foreach (var item in p_extra)
                {
                    dt[item.Key] = item.Value;
                }
            }

            var path = Path.Combine(Kit.GetFolderPath(), $"{cls}.cs");
            Kit.WritePrjFile(path, "Dt.ClientCls." + p_fileName, dt);
            Kit.OpenFile(path);

            Close();
        }

        async void _btnEntity_Click(object sender, EventArgs e)
        {
            if (!_cls.Text.Trim().EndsWith("Obj"))
                _cls.Text = _cls.Text.Trim() + "Obj";

            var dt = new Dictionary<string, string> { { "$entitybody$", "" } };
            if (_cbTbls.SelectedItem != null)
            {
                var entity = await AtSvc.GetEntityClass(_cbTbls.SelectedItem.ToString(), _cls.Text);
                dt["$entitybody$"] = entity;
            }
            InsertClass("Entity.cs", dt);
        }

        private void _btnLv_Click(object sender, EventArgs e)
        {
            InsertClass("CellUITemp.cs");
        }

        private void _btnFv_Click(object sender, EventArgs e)
        {
            InsertClass("FvCallTemp.cs");
        }

        private void _btnCList_Click(object sender, EventArgs e)
        {
            InsertClass("CListExTemp.cs");
        }

        private void _btnAgent_Click(object sender, EventArgs e)
        {
            InsertClass("AgentTemp.cs");
        }

        async void _cbTbls_DropDown(object sender, EventArgs e)
        {
            var ls = await AtSvc.GetAllTables();
            if (_cbTbls.DataSource != ls)
                _cbTbls.DataSource = ls;
        }

        void AddTooltip()
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel4, Kit.AllTblsTip);
        }
    }
}
