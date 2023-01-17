using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt
{
    public partial class SingleForm : Form
    {
        FileParams _params;
        string _path;

        public SingleForm()
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
                _params = new FileParams
                {
                    NameSpace = Kit.GetText(_ns, "命名空间不可为空！"),
                    ClsRoot = Kit.GetText(_clsRoot, "框架类的词根不可为空！"),
                };
            }
            catch
            {
                return;
            }

            if (_dg.Rows.Count == 0)
            {
                MessageBox.Show("未选择表与实体的映射关系！");
                return;
            }

            for (int i = 0; i < _dg.Rows.Count; i++)
            {
                _params.Tbls.Add(_dg.Rows[i].Cells[0].Value.ToString());
                _params.Entities.Add(_dg.Rows[i].Cells[1].Value.ToString());
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("正在生成单实体框架...")
                .AppendLine($"已选择{_dg.Rows.Count}个实体类型")
                .AppendLine($"框架类的词根：{_params.ClsRoot}");

            _path = Kit.GetFolderPath();
            WriteWin();
            await WriteForm();
            await WriteList();
            await WriteQuery();

            Close();
        }

        void WriteWin()
        {
            var dt = _params.Params;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Win.xaml"), "Dt.Single.Res.EntityWin.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Win.xaml.cs"), "Dt.Single.Res.EntityWin.xaml.cs", dt);
        }

        async Task WriteForm()
        {
            var dt = _params.Params;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Form.xaml.cs"), "Dt.Single.Res.EntityForm.xaml.cs", dt);

            var body = await AtSvc.GetFvCells(_params.Tbls);
            // 可能包含命名空间
            dt["$fvbody$"] = body.Replace("$namespace$", _params.NameSpace).Replace("$rootnamespace$", Kit.GetRootNamespace());

            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Form.xaml"), "Dt.Single.Res.EntityForm.xaml", dt);
            dt.Remove("$fvbody$");
        }

        async Task WriteList()
        {
            var dt = _params.Params;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}List.xaml.cs"), "Dt.Single.Res.EntityList.xaml.cs", dt);

            dt["$lvtemp$"] = await AtSvc.GetLvItemTemplate(_params.Tbls);
            dt["$lvcols$"] = await AtSvc.GetLvTableCols(_params.Tbls);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}List.xaml"), "Dt.Single.Res.EntityList.xaml", dt);
            dt.Remove("$lvbody$");
        }

        Task WriteQuery()
        {
            var dt = _params.Params;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Query.xaml"), "Dt.Single.Res.EntityQuery.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Query.xaml.cs"), "Dt.Single.Res.EntityQuery.xaml.cs", dt);
            return Task.CompletedTask;
        }

        private void btnAdd_Click(object sender, EventArgs e)
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
                    _dg.Rows[i].Cells[1].Value = Kit.GetClsName(item) + "Obj";
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (_dg.Rows.Count > 0)
            {
                if (MessageBox.Show("确定要清空已选择的表吗？", "确认") == DialogResult.OK)
                    _dg.Rows.Clear();
            }
        }

        private void AddTooltip()
        {
            var tip = new ToolTip();
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel2, Kit.RootNameTip);
        }
    }
}
