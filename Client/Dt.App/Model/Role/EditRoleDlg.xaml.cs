#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    public sealed partial class EditRoleDlg : Dlg
    {
        const string _tblName = "cm_role";
        bool _needRefresh;

        public EditRoleDlg()
        {
            InitializeComponent();
        }

        public async Task<bool> Show(long p_id)
        {
            if (p_id > 0)
                _fv.Data = await AtCm.GetRow("角色-编辑", new { id = p_id });
            else
                CreateRole();
            await ShowAsync();
            return _needRefresh;
        }

        void CreateRole()
        {
            _fv.Data = Table.NewRow(_tblName);
        }

        async void OnSave(object sender, Mi e)
        {
            if (_fv.ExistNull("name"))
                return;

            Row row = _fv.Row;
            string name = row.Str("name");
            if ((row.IsAdded || row.Cells["name"].IsChanged)
                && await AtCm.GetScalar<int>("角色-名称重复", new { name = name }) > 0)
            {
                _fv["name"].Warn("角色名称重复！");
                return;
            }

            if (row.IsAdded)
            {
                row["id"] = await AtCm.NewID();
            }

            if (await AtCm.SaveRow(row, _tblName))
            {
                _needRefresh = true;
                AtKit.Msg("保存成功！");
                CreateRole();
                _fv.GotoFirstCell();
            }
            else
            {
                AtKit.Warn("保存失败！");
            }
        }

        void OnAdd(object sender, Mi e)
        {
            CreateRole();
        }

        protected override Task<bool> OnClosing()
        {
            if (_fv.Row.IsChanged)
                return AtKit.Confirm("数据未保存，要放弃修改吗？");
            return Task.FromResult(true);
        }
    }
}
