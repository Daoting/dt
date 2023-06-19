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
            AtSvc.BindSvcName(_cbSvcName);
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

            _dg.Sort(_dg.Columns[2], System.ComponentModel.ListSortDirection.Ascending);
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

            //dt["$blurclause$"] = await AtSvc.GetBlurClause(_params.Tbls);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Win.xaml.cs"), "Dt.Single.Res.EntityWin.xaml.cs", dt);
            //dt.Remove("$blurclause$");
        }

        async Task WriteForm()
        {
            var dt = _params.Params;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Form.xaml.cs"), "Dt.Single.Res.EntityForm.xaml.cs", dt);

            dt["$fvbody$"] = await AtSvc.GetFvCells(_params.Tbls);

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
            dt.Remove("$lvtemp$");
            dt.Remove("$lvcols$");
        }

        async Task WriteQuery()
        {
            var dt = _params.Params;
            dt["$queryxaml$"] = await AtSvc.GetQueryFvCells(_params.Tbls);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Query.xaml"), "Dt.Single.Res.EntityQuery.xaml", dt);
            dt.Remove("$queryxaml$");

            dt["$querydata$"] = await AtSvc.GetQueryFvData(_params.Tbls);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.ClsRoot}Query.xaml.cs"), "Dt.Single.Res.EntityQuery.xaml.cs", dt);
            dt.Remove("$querydata$");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var dlg = new SelectTbls(null);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _dg.Rows.Clear();
                int index = 1;
                foreach (var item in dlg.GetSelection())
                {
                    int i = _dg.Rows.Add();
                    _dg.Rows[i].Cells[0].Value = item;
                    _dg.Rows[i].Cells[1].Value = Kit.GetClsName(item) + "X";
                    _dg.Rows[i].Cells[2].Value = (index++).ToString();
                }
            }
        }

        private void AddTooltip()
        {
            var tip = new ToolTip();
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel2, Kit.RootNameTip);
            tip.SetToolTip(linkLabel1, Kit.SvcNameTip);
        }
    }
}
