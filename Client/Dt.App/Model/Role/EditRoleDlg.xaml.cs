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
        bool _needRefresh;

        public EditRoleDlg()
        {
            InitializeComponent();
        }

        public async Task<bool> Show(long p_id)
        {
            if (p_id > 0)
                _fv.Data = await Repo.Get<Role>("角色-编辑", new { id = p_id });
            else
                CreateRole();
            await ShowAsync();
            return _needRefresh;
        }

        async void CreateRole()
        {
            _fv.Data = new Role(
                ID: await AtCm.NewID(),
                Name: "新角色");
        }

        async void OnSave(object sender, Mi e)
        {
            if (_fv.ExistNull("name"))
                return;

            var role = _fv.Data.To<Role>();
            if ((role.IsAdded || role.Cells["name"].IsChanged)
                && await AtCm.GetScalar<int>("角色-名称重复", new { name = role.Name }) > 0)
            {
                _fv["name"].Warn("角色名称重复！");
                return;
            }

            if (await Repo.Save(role))
            {
                _needRefresh = true;
                CreateRole();
                _fv.GotoFirstCell();
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
