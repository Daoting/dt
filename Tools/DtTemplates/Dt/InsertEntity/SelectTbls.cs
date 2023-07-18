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
    public partial class SelectTbls : Form
    {
        public SelectTbls(List<string> p_tbls)
        {
            InitializeComponent();
            LoadTbls(p_tbls);
            _cbCls.Checked = Kit.TblNameNoPrefix;
        }

        public IEnumerable<string> GetSelection()
        {
            return from string item in _cbList.CheckedItems
                   select item;
        }

        async void LoadTbls(List<string> p_tbls)
        {
            var ls = await AtSvc.GetAllTables();
            if (ls == null || ls.Count == 0)
            {
                MessageBox.Show("无法连接服务，请确认服务正在运行！");
                return;
            }

            if (p_tbls == null)
            {
                foreach (var tbl in ls)
                {
                    if (!string.IsNullOrEmpty(tbl))
                        _cbList.Items.Add(tbl);
                }
            }
            else
            {
                foreach (var tbl in ls)
                {
                    if (!string.IsNullOrEmpty(tbl)
                        && !p_tbls.Contains(tbl))
                    {
                        _cbList.Items.Add(tbl);
                    }
                }
            }
        }

        void _btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        void _cbCls_CheckedChanged(object sender, EventArgs e)
        {
            Kit.TblNameNoPrefix = _cbCls.Checked;
        }
    }
}
