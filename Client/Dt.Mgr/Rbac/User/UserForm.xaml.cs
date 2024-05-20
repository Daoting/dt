#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class UserForm : FvDlg
    {
        public UserForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
            Menu.Items.Remove(Menu["删除"]);
        }

        public Task<bool> Open(long p_id, bool p_enableAdd)
        {
            Menu["增加"].Visibility = p_enableAdd ? Visibility.Visible : Visibility.Collapsed;
            return Open(p_id);
        }

        public UserX Data
        {
            get { return _fv.Data.To<UserX>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            Data = await UserX.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await UserX.GetByID(p_id);
        }

        protected override async Task<bool> OnSave()
        {
            var d = Data;
            bool isNew = d.IsAdded;
            if (await d.Save())
            {
                if (isNew)
                    Kit.Msg("初始密码为4个1");
                return true;
            }
            return false;
        }
        
        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is UserWin win)
            {
                _ = win.MainList.Refresh(p_id);
            }
        }

        protected override void UpdateRelated(long p_id)
        {
            if (OwnWin is UserWin win)
            {
                win.GroupList.Update(p_id);
                win.RoleList.Update(p_id);
                win.MenuList.Update(p_id);
                win.PerList.Update(p_id);
            }
        }

        void OnPhotoChanged(FvCell arg1, object e)
        {
            Save();
        }
    }
}