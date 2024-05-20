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
    public sealed partial class GroupForm : FvDlg
    {
        public GroupForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public GroupX Data
        {
            get { return _fv.Data.To<GroupX>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            Data = await GroupX.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await GroupX.GetByID(p_id);
        }
        
        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is GroupWin win)
            {
                _ = win.MainList.Refresh(p_id);
            }
        }

        protected override void UpdateRelated(long p_id)
        {
            if (OwnWin is GroupWin win)
            {
                win.UserList.Update(p_id);
                win.RoleList.Update(p_id);
            }
        }
    }
}