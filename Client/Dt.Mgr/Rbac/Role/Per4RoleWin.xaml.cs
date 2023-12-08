#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class Per4RoleWin : Win
    {
        long _roleID;

        public Per4RoleWin(long p_releatedID)
        {
            InitializeComponent();
            _roleID = p_releatedID;
            LoadModule();
        }

        async void LoadModule()
        {
            _lvModule.Data = await PermissionModuleX.Query(null);
        }

        async void OnModuleItemClick(ItemClickArgs e)
        {
            if (e.IsChanged)
                _lvFunc.Data = await PermissionFuncX.Query("where module_id=" + e.Row.ID);
            NaviTo("功能列表");
        }

        async void OnFuncItemClick(ItemClickArgs e)
        {
            if (e.IsChanged)
                _lvPer.Data = await PermissionX.Query($"where not exists ( select per_id from cm_role_per b where a.id = b.per_id and role_id ={_roleID} ) and func_id={e.Row.ID}");
            NaviTo("权限列表");
        }

        public List<long> SelectedIDs => (from row in _lvPer.SelectedRows
                                          select row.ID).ToList();

        public bool IsOK { get; private set; }

        void OnOK()
        {
            IsOK = true;
            Close();
        }

        void OnSelectAll()
        {
            _lvPer.SelectAll();
        }
    }
}
