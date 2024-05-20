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
    public partial class UserGroupList : LvTab
    {
        #region 变量
        long _releatedID;
        #endregion
        
        #region 构造
        public UserGroupList()
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
                _lv.Data = await GroupX.Query($"where exists ( select group_id from cm_user_group b where a.ID = b.group_id and user_id={_releatedID} )");
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
            var dlg = new Group4User();
            if (await dlg.Show(_releatedID, e)
                && await RbacDs.AddUserGroups(_releatedID, dlg.SelectedIDs))
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
            
            if (_lv.SelectionMode == SelectionMode.Multiple)
            {
                var ls = new List<UserGroupX>();
                foreach (var row in _lv.SelectedRows)
                {
                    var x = new UserGroupX(GroupID: row.ID, UserID: _releatedID);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Delete())
                    await Refresh();
            }
            else
            {
                var x = new UserGroupX(GroupID: e.Row.ID, UserID: _releatedID);
                if (await x.Delete())
                    await Refresh();
            }
        }
        #endregion
    }
}