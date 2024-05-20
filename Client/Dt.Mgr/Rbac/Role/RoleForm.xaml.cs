#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class RoleForm : FvDlg
    {
        public RoleForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public RoleX Data
        {
            get { return _fv.Data.To<RoleX>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            Data = await RoleX.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await RoleX.GetByID(p_id);
        }
        
        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is RoleWin win)
            {
                _ = win.MainList.Refresh(p_id);
            }
        }

        protected override void UpdateRelated(long p_id)
        {
            if (OwnWin is RoleWin win)
            {
                win.UserList.Update(p_id);
                win.MenuList.Update(p_id);
                win.PermissionList.Update(p_id);
                win.GroupList.Update(p_id);
            }
        }
    }
}