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

namespace Dt.OnToMany
{
    public partial class SelectChildTbls : Form
    {

        public SelectChildTbls(string p_tbls)
        {
            InitializeComponent();
            LoadTbls(p_tbls);
        }

        public IEnumerable<string> GetSelection()
        {
            return from string item in _cbList.CheckedItems
                   select item;
        }

        async void LoadTbls(string p_tbls)
        {
            string[] selected = null;
            if (!string.IsNullOrEmpty(p_tbls))
                selected = p_tbls.Split(',');

            var ls = await AtSvc.GetAllTables();
            foreach (var tbl in ls)
            {
                if (string.IsNullOrEmpty(tbl))
                    continue;

                _cbList.Items.Add(tbl);
                if (selected != null && selected.Contains(tbl))
                {
                    _cbList.SetItemChecked(_cbList.Items.Count - 1, true);
                }
            }
        }

        async void _btnOK_Click(object sender, EventArgs e)
        {
            foreach (string item in _cbList.CheckedItems)
            {
                if (!await AtSvc.ExistParentID(item))
                {
                    MessageBox.Show($"表 [{item}] 中不包含 [ParentID] 字段");
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
