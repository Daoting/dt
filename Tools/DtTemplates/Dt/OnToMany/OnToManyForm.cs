using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt
{
    public partial class OnToManyForm : Form
    {
        OnToManyParams _params;
        string _path;

        public OnToManyForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
            AtSvc.BindSvcUrl(_svcUrl);
            AtSvc.BindSvcName(_cbSvcName);
            AddTooltip();
        }

        async void _btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                _params = new OnToManyParams
                {
                    NameSpace = Kit.GetText(_ns, "命名空间不可为空！"),
                };
            }
            catch
            {
                return;
            }

            if (_dgParent.Rows.Count == 0 || _dgChild.Rows.Count == 0)
            {
                MessageBox.Show("未选择表与实体的映射关系！");
                return;
            }

            _params.ParentTbl = _dgParent.Rows[0].Cells[0].Value.ToString();
            _params.ParentEntity = _dgParent.Rows[0].Cells[1].Value.ToString();
            _params.ParentRoot = _dgParent.Rows[0].Cells[2].Value.ToString();

            _dgChild.Sort(_dgChild.Columns[4], System.ComponentModel.ListSortDirection.Ascending);
            for (int i = 0; i < _dgChild.Rows.Count; i++)
            {
                var item = _dgChild.Rows[i];
                var child = new ChildInfo();
                child.Tbl = item.Cells[0].Value.ToString();
                child.Entity = item.Cells[1].Value.ToString();
                child.Root = item.Cells[2].Value.ToString();
                child.ParentID = item.Cells[3].Value.ToString();
                _params.Children.Add(child);
            }

            _path = Kit.GetFolderPath();
            WriteWin();
            await WriteParentList();
            await WriteParentForm();
            await WriteParentQuery();

            foreach (var item in _params.Children)
            {
                await WriteChild(item);
            }
            Close();
        }

        void WriteWin()
        {
            var dt = _params.ParentWinParams;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}Win.xaml"), "Dt.OnToMany.Res.ParentWin.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}Win.xaml.cs"), "Dt.OnToMany.Res.ParentWin.xaml.cs", dt);
        }

        async Task WriteParentList()
        {
            var dt = await _params.GetParentListParams();
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}List.xaml"), "Dt.OnToMany.Res.ParentList.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}List.xaml.cs"), "Dt.OnToMany.Res.ParentList.xaml.cs", dt);
        }

        async Task WriteParentForm()
        {
            var dt = await _params.GetParentFormParams();
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}Form.xaml"), "Dt.OnToMany.Res.ParentForm.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}Form.xaml.cs"), "Dt.OnToMany.Res.ParentForm.xaml.cs", dt);
        }

        async Task WriteParentQuery()
        {
            var dt = await _params.GetParentQueryParams();
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}Query.xaml"), "Dt.OnToMany.Res.ParentQuery.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}Query.xaml.cs"), "Dt.OnToMany.Res.ParentQuery.xaml.cs", dt);
        }

        async Task WriteChild(ChildInfo p_ci)
        {
            var dt = await _params.GetChildParams(p_ci);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}{p_ci.Root}List.xaml"), "Dt.OnToMany.Res.ChildList.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}{p_ci.Root}List.xaml.cs"), "Dt.OnToMany.Res.ChildList.xaml.cs", dt);

            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}{p_ci.Root}Form.xaml"), "Dt.OnToMany.Res.ChildForm.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ParentRoot}{p_ci.Root}Form.xaml.cs"), "Dt.OnToMany.Res.ChildForm.xaml.cs", dt);
        }

        private void btnChild_Click(object sender, EventArgs e)
        {
            var dlg = new SelectTbls(null);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _dgChild.Rows.Clear();
                int index = 1;
                foreach (var item in dlg.GetSelection())
                {
                    int i = _dgChild.Rows.Add();
                    _dgChild.Rows[i].Cells[0].Value = item;
                    _dgChild.Rows[i].Cells[1].Value = Kit.GetClsName(item) + "X";
                    _dgChild.Rows[i].Cells[2].Value = Kit.GetClsName(item);
                    _dgChild.Rows[i].Cells[3].Value = "ParentID";
                    _dgChild.Rows[i].Cells[4].Value = (index++).ToString();
                }
            }
        }

        private void _btnParent_Click(object sender, EventArgs e)
        {
            var dlg = new SelectTbls(null);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _dgParent.Rows.Clear();
                var item = dlg.GetSelection().FirstOrDefault();
                if (item != null)
                {
                    int i = _dgParent.Rows.Add();
                    _dgParent.Rows[i].Cells[0].Value = item;
                    _dgParent.Rows[i].Cells[1].Value = Kit.GetClsName(item) + "X";
                    _dgParent.Rows[i].Cells[2].Value = Kit.GetClsName(item);
                }
            }
        }

        private void AddTooltip()
        {
            var tip = new ToolTip();
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel1, Kit.SvcNameTip);
        }
    }
}
