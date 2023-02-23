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
    public partial class ManyToManyForm : Form
    {
        ManyToManyParams _params;
        string _path;

        public ManyToManyForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
            AtSvc.BindSvcUrl(_svcUrl);
            AddTooltip();
        }

        async void _btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                _params = new ManyToManyParams
                {
                    NameSpace = Kit.GetText(_ns, "命名空间不可为空！"),
                };
            }
            catch
            {
                return;
            }

            if (_dgParent.Rows.Count == 0 || _dgChild.Rows.Count == 0 || _dgMid.Rows.Count == 0)
            {
                MessageBox.Show("未选择表与实体的映射关系！");
                return;
            }

            if (_dgChild.Rows.Count != _dgMid.Rows.Count)
            {
                MessageBox.Show("关联实体和中间实体的个数不相同！");
                return;
            }

            _params.MainTbl = _dgParent.Rows[0].Cells[0].Value.ToString();
            _params.MainEntity = _dgParent.Rows[0].Cells[1].Value.ToString();
            _params.MainRoot = _dgParent.Rows[0].Cells[2].Value.ToString();

            _dgChild.Sort(_dgChild.Columns[3], System.ComponentModel.ListSortDirection.Ascending);
            for (int i = 0; i < _dgChild.Rows.Count; i++)
            {
                var item = _dgChild.Rows[i];
                var ri = new RelatedInfo();
                ri.Tbl = item.Cells[0].Value.ToString();
                ri.Entity = item.Cells[1].Value.ToString();
                ri.Root = item.Cells[2].Value.ToString();
                _params.Related.Add(ri);
            }

            _dgMid.Sort(_dgMid.Columns[4], System.ComponentModel.ListSortDirection.Ascending);
            for (int i = 0; i < _dgMid.Rows.Count; i++)
            {
                var item = _dgMid.Rows[i];
                if (item.Cells[2].Value == null || item.Cells[3].Value == null)
                {
                    MessageBox.Show("中间实体的主实体外键、关联实体外键不可为空！");
                    return;
                }

                var ri = _params.Related[i];
                ri.RelatedTbl = item.Cells[0].Value.ToString();
                ri.RelatedEntity = item.Cells[1].Value.ToString();
                ri.MainRelatedID = item.Cells[2].Value.ToString();
                ri.RelatedID = item.Cells[3].Value.ToString();
            }

            _path = Kit.GetFolderPath();
            WriteWin();
            await WriteMainList();
            await WriteMainForm();
            await WriteMainQuery();

            foreach (var item in _params.Related)
            {
                await WriteRelatedItem(item);
            }
            Close();
        }

        void WriteWin()
        {
            var dt = _params.GetWinParams();
            var file = $"{_params.MainRoot}Win.xaml";
            Kit.WritePrjFile(Path.Combine(_path, file), "Dt.ManyToMany.Res.MainWin.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, file + ".cs"), "Dt.ManyToMany.Res.MainWin.xaml.cs", dt);
        }

        async Task WriteMainList()
        {
            var dt = await _params.GetMainListParams();
            var file = $"{_params.MainRoot}List.xaml";
            Kit.WritePrjFile(Path.Combine(_path, file), "Dt.ManyToMany.Res.MainList.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, file + ".cs"), "Dt.ManyToMany.Res.MainList.xaml.cs", dt);
        }

        async Task WriteMainForm()
        {
            var dt = await _params.GetMainFormParams();
            var file = $"{_params.MainRoot}Form.xaml";
            Kit.WritePrjFile(Path.Combine(_path, file), "Dt.ManyToMany.Res.MainForm.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, file + ".cs"), "Dt.ManyToMany.Res.MainForm.xaml.cs", dt);
        }

        async Task WriteMainQuery()
        {
            var dt = await _params.GetMainQueryParams();
            var file = $"{_params.MainRoot}Query.xaml";
            Kit.WritePrjFile(Path.Combine(_path, file), "Dt.ManyToMany.Res.MainQuery.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, file + ".cs"), "Dt.ManyToMany.Res.MainQuery.xaml.cs", dt);
        }

        async Task WriteRelatedItem(RelatedInfo p_ci)
        {
            var dt = await _params.GetRelatedParams(p_ci);
            var file = $"{_params.MainRoot}{p_ci.Root}List.xaml";
            Kit.WritePrjFile(Path.Combine(_path, file), "Dt.ManyToMany.Res.RelatedList.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, file + ".cs"), "Dt.ManyToMany.Res.RelatedList.xaml.cs", dt);

            file = $"{p_ci.Root}4{_params.MainRoot}Dlg.xaml";
            Kit.WritePrjFile(Path.Combine(_path, file), "Dt.ManyToMany.Res.SelectDlg.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, file + ".cs"), "Dt.ManyToMany.Res.SelectDlg.xaml.cs", dt);
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
                    _dgChild.Rows[i].Cells[3].Value = (index++).ToString();
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

        private void _btnMid_Click(object sender, EventArgs e)
        {
            var dlg = new SelectTbls(null);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _dgMid.Rows.Clear();
                int index = 1;
                foreach (var item in dlg.GetSelection())
                {
                    int i = _dgMid.Rows.Add();
                    _dgMid.Rows[i].Cells[0].Value = item;
                    _dgMid.Rows[i].Cells[1].Value = Kit.GetClsName(item) + "X";
                    _dgMid.Rows[i].Cells[4].Value = (index++).ToString();
                }
            }
        }

        private void AddTooltip()
        {
            var tip = new ToolTip();
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
        }
    }
}
