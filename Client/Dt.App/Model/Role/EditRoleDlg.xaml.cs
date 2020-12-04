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
                _fv.Data = await AtCm.First<Role>("角色-编辑", new { id = p_id });
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
            if (await AtCm.Save(_fv.Data.To<Role>()))
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
