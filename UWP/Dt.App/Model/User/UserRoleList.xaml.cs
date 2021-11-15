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
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.App.Model
{
    public sealed partial class UserRoleList : Mv
    {
        long _userID;

        public UserRoleList()
        {
            InitializeComponent();
            Menu["移除"].Bind(IsEnabledProperty, _lv, "HasSelected");
        }

        public void Update(long p_userID)
        {
            _userID = p_userID;
            Menu["添加"].IsEnabled = true;
            Refresh();
        }

        public void Clear()
        {
            _userID = -1;
            Menu["添加"].IsEnabled = false;
            _lv.Data = null;
        }

        async void Refresh()
        {
            _lv.Data = await AtCm.Query("用户-关联角色", new { userid = _userID });
        }

        async void OnAdd(object sender, Mi e)
        {
            SelectRolesDlg dlg = new SelectRolesDlg();
            if (await dlg.Show(RoleRelations.User, _userID.ToString(), e))
            {
                List<long> roles = new List<long>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    roles.Add(row.ID);
                }
                if (roles.Count > 0 && await AtCm.AddUserRole(_userID, roles))
                    Refresh();
            }
        }

        void OnRemove(object sender, Mi e)
        {
            RemoveRole(_lv.SelectedRows);
        }

        void OnRemove2(object sender, Mi e)
        {
            if (_lv.SelectionMode == SelectionMode.Multiple)
                RemoveRole(_lv.SelectedRows);
            else
                RemoveRole(new List<Row> { e.Row });
        }

        async void RemoveRole(IEnumerable<Row> p_rows)
        {
            List<long> roles = (from r in p_rows
                                select r.Long("roleid")).ToList();
            if (roles.Count > 0 && await AtCm.RemoveUserRoles(_userID, roles))
                Refresh();
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lv.SelectionMode = SelectionMode.Multiple;
            Menu.Hide("添加", "选择");
            Menu.Show("移除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lv.SelectionMode = SelectionMode.Single;
            Menu.Show("添加", "选择");
            Menu.Hide("移除", "全选", "取消");
        }

        UserAccountWin _win => (UserAccountWin)_tab.OwnWin;
    }
}
