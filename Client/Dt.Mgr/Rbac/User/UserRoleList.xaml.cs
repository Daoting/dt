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
    public partial class UserRoleList : LvTab
    {
        #region 变量
        long _releatedID;
        #endregion
        
        #region 构造
        public UserRoleList()
        {
            InitializeComponent();
            _lv.AddMultiSelMenu(Menu);
        }
        #endregion

        #region 公开
        public void Update(long p_releatedID)
        {
            if (_releatedID == p_releatedID)
                return;
            
            _releatedID = p_releatedID;
            Menu["添加"].IsEnabled = _releatedID > 0;
            _ = Refresh();
        }
        #endregion

        #region 重写
        protected override Lv Lv => _lv;

        protected override async Task Query()
        {
            if (_releatedID > 0)
            {
                _lv.Data = await RoleX.Query($"where exists ( select role_id from cm_user_role b where a.ID = b.role_id and user_id={_releatedID} )");
            }
            else
            {
                _lv.Data = null;
            }
        }
        #endregion

        #region 交互
        async void OnAdd(Mi e)
        {
            var dlg = new Role4User();
            if (await dlg.Show(_releatedID, e)
                && await RbacDs.AddUserRoles(_releatedID, dlg.SelectedIDs))
            {
                await Refresh();
            }
        }
        
        async void OnDel(Mi e)
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
                await Refresh();
        }
        #endregion
    }
}