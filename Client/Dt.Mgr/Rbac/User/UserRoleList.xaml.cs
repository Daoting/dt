#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class UserRoleList : Tab
    {
        #region 构造方法
        public UserRoleList()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public void Update(long p_releatedID)
        {
            _releatedID = p_releatedID;
            Menu["添加"].IsEnabled = _releatedID > 0;
            Refresh();
        }

        public async void Refresh()
        {
            if (_releatedID > 0)
            {
                _lv.Data = await RoleX.Query("用户-关联角色", new { ReleatedID = _releatedID });
            }
            else
            {
                _lv.Data = null;
            }
        }
        #endregion

        #region 交互
        async void OnAdd(object sender, Mi e)
        {
            var dlg = new Role4UserDlg();
            if (await dlg.Show(_releatedID, e)
                && await RbacDs.AddUserRoles(_releatedID, dlg.SelectedIDs))
            {
                Refresh();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除关联吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            List<long> roleIDs;
            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
            {
                roleIDs = (from row in _lv.SelectedRows
                           select row.ID).ToList();
            }
            else
            {
                roleIDs = new List<long> { e.Row.ID };
            }

            if (await RbacDs.RemoveUserRoles(_releatedID, roleIDs))
                Refresh();
        }
        #endregion

        #region 选择
        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Multiple;
            Menu.HideExcept("删除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Single;
            Menu.ShowExcept("删除", "全选", "取消");
        }
        #endregion

        #region 内部
        UserWin _win => (UserWin)OwnWin;
        long _releatedID;
        #endregion
    }
}