using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Dt
{
    public partial class InsertEntityClsForm : Form
    {
        public InsertEntityClsForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
            AtSvc.BindSvcUrl(_svcUrl);
            AtSvc.BindSvcName(_cbSvcName);
            AddTooltip();
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            List<string> tbls = null;
            if (_dg.Rows.Count > 0)
            {
                tbls = new List<string>();
                for (int i = 0; i < _dg.Rows.Count; i++)
                {
                    tbls.Add(_dg.Rows[i].Cells[0].Value.ToString());
                }
            }

            var dlg = new SelectTbls(tbls);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (var item in dlg.GetSelection())
                {
                    int i = _dg.Rows.Add();
                    _dg.Rows[i].Cells[0].Value = item;
                    _dg.Rows[i].Cells[1].Value = Kit.GetClsName(item) + "X";
                }
            }
        }

        void btnClear_Click(object sender, EventArgs e)
        {
            if (_dg.Rows.Count > 0)
            {
                if (MessageBox.Show("确定要清空所有已选择的表吗？", "确认") == DialogResult.OK)
                    _dg.Rows.Clear();
            }
        }

        async void btnOK_Click(object sender, EventArgs e)
        {
            if (_dg.Rows.Count == 0)
            {
                Close();
                return;
            }

            string ns;
            try
            {
                ns = Kit.GetText(_ns);
            }
            catch
            {
                MessageBox.Show("命名空间不可为空！");
                return;
            }

            var dt = new Dictionary<string, string>
            {
                {"$rootnamespace$", ns },
                {"$time$", DateTime.Now.ToString("yyyy-MM-dd") },
                {"$username$", Environment.UserName },
            };

            int cntDesign = 0;
            int cntCustom = 0;
            int cntSql = 0;
            for (int i = 0; i < _dg.Rows.Count; i++)
            {
                var tbl = _dg.Rows[i].Cells[0].Value.ToString();
                var cls = _dg.Rows[i].Cells[1].Value.ToString();
                if (string.IsNullOrEmpty(cls))
                    continue;

                var path = Path.Combine(Kit.GetFolderPath(), $"{cls}.tbl.cs");
                var entity = await AtSvc.GetEntityClass(tbl, cls);
                dt["$entitybody$"] = entity;
                Kit.WritePrjFile(path, "Dt.InsertEntity.Entity.cs", dt, false);
                cntDesign++;

                // 空的含sql的文件，只生成一次
                path = Path.Combine(Kit.GetFolderPath(), $"{cls}.sql.cs");
                if (!File.Exists(path))
                {
                    dt["$entitybody$"] = $"    public partial class {cls}\r\n    {{\r\n\r\n    }}";
                    Kit.WritePrjFile(path, "Dt.InsertEntity.Entity.cs", dt, false);
                    cntSql++;
                }

                path = Path.Combine(Kit.GetFolderPath(), $"{cls}.cs");
                if (File.Exists(path))
                {
                    if (_rbDef.Checked)
                        continue;

                    if (MessageBox.Show($"{Path.GetFileName(path)}已存在，覆盖后不可恢复，确认要覆盖此文件吗？", "覆盖文件", MessageBoxButtons.OKCancel)
                        != DialogResult.OK)
                    {
                        continue;
                    }
                }
                entity = await AtSvc.GetEntityClassEx(tbl, cls);
                dt["$entitybody$"] = entity;
                Kit.WritePrjFile(path, "Dt.InsertEntity.Entity.cs", dt, false);
                cntCustom++;
            }

            Kit.Output($"共生成：.tbl.cs文件 {cntDesign} 个，.sql.cs文件 {cntSql} 个，.cs文件 {cntCustom} 个");
            Close();
        }

        void _dg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_dg.Columns[e.ColumnIndex].Name == "Del" && e.RowIndex >= 0)
            {
                _dg.Rows.RemoveAt(e.RowIndex);
            }
        }

        void AddTooltip()
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(linkLabel3,
@"请确保当前服务正在运行，通过服务：
1. 获取所有表目录
2. 根据表结构生成实体类及实体类扩展的代码");

            tip.SetToolTip(linkLabel1, Kit.SvcNameTip);
        }
    }
}
