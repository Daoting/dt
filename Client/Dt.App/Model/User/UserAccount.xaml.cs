#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Model
{
    [View("用户账号")]
    public partial class UserAccount : Win
    {
        public UserAccount()
        {
            InitializeComponent();
            _lvUser.View = Resources[Kit.IsPhoneUI ? "TileView" : "TableView"];
            _lvUser.CellEx = typeof(UserViewEx);
            LoadAll();
        }

        async void LoadAll()
        {
            _lvUser.Data = await AtCm.Query<User>("用户-所有");
        }

        async void OnAddUser(object sender, Mi e)
        {
            if (await new EditUserDlg().Show(-1))
                _lvUser.Data = await AtCm.Query<User>("用户-最近修改");
        }

        async void OnEditUser(object sender, Mi e)
        {
            if (await new EditUserDlg().Show(e.Row.ID))
                _lvUser.Data = await AtCm.Query<User>("用户-最近修改");
        }

        void OnNaviToSearch(object sender, RoutedEventArgs e)
        {
            NaviTo("查找用户");
        }

        async void OnDelUser(object sender, Mi e)
        {
            if (!await Kit.Confirm($"确认要删除[{e.Row.Str("name")}]吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (await AtCm.DeleteBySvc(e.Data.To<User>()))
                LoadAll();
        }

        async void OnResetPwd(object sender, Mi e)
        {
            var usr = new User(ID: e.Row.ID);
            usr.IsAdded = false;
            string phone = e.Row.Str("phone");
            usr.Pwd = Kit.GetMD5(phone.Substring(phone.Length - 4));

            if (await AtCm.Save(usr, false))
                Kit.Msg("密码已重置为手机号后4位！");
            else
                Kit.Msg("重置密码失败！");
        }

        async void OnToggleExpired(object sender, Mi e)
        {
            bool expired = e.Row.Bool("expired");
            var usr = new User(ID: e.Row.ID, Expired: expired);
            usr.IsAdded = false;
            usr.Expired = !expired;

            string act = expired ? "启用" : "停用";
            if (await AtCm.Save(usr, false))
            {
                Kit.Msg($"账号[{e.Row.Str("name")}]已{act}！");
                LoadAll();
            }
            else
            {
                Kit.Msg(act + "失败！");
            }
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
            {
                long id = e.Row.ID;
                _lvRole.Data = await AtCm.Query("用户-关联角色", new { userid = id });
                _lvMenu.Data = await AtCm.Query("用户-可访问的菜单", new { userid = id });
                _lvPrv.Data = await AtCm.Query("用户-具有的权限", new { userid = id });
            }
            NaviTo("关联角色,拥有菜单,授予权限");
        }

        void OnUserDataChanged(object sender, object e)
        {
            _lvRole.Data = null;
            _lvMenu.Data = null;
            _lvPrv.Data = null;
        }

        async void OnSearch(object sender, string e)
        {
            if (e == "#全部")
            {
                LoadAll();
            }
            else if (e == "#最近修改")
            {
                _lvUser.Data = await AtCm.Query<User>("用户-最近修改");
            }
            else if (!string.IsNullOrEmpty(e))
            {
                _lvUser.Data = await AtCm.Query<User>("用户-模糊查询", new { input = $"%{e}%" });
            }
            NaviTo("用户列表");
        }

        class UserViewEx
        {
            public static void SetStyle(ViewItem p_item)
            {
                if (p_item.Row.Bool("expired"))
                {
                    p_item.Foreground = Res.GrayBrush;
                    p_item.FontStyle = Windows.UI.Text.FontStyle.Italic;
                }
            }
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lvRole.SelectionMode = SelectionMode.Multiple;
            _rMenu.Hide("添加", "选择");
            _rMenu.Show("移除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lvRole.SelectionMode = SelectionMode.Single;
            _rMenu.Show("添加", "选择");
            _rMenu.Hide("移除", "全选", "取消");
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lvRole.SelectAll();
        }

        async void OnAddRole(object sender, Mi e)
        {
            SelectRolesDlg dlg = new SelectRolesDlg();
            long userID = _lvUser.SelectedRow.ID;
            if (await dlg.Show(RoleRelations.User, userID.ToString(), e))
            {
                List<long> roles = new List<long>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    roles.Add(row.ID);
                }
                if (roles.Count > 0 && await AtCm.AddUserRole(userID, roles))
                    _lvRole.Data = await AtCm.Query("用户-关联角色", new { userid = userID });
            }
        }

        void OnRemoveRole(object sender, Mi e)
        {
            RemoveRole(_lvRole.SelectedRows);
        }

        void OnRemoveRole2(object sender, Mi e)
        {
            if (_lvRole.SelectionMode == SelectionMode.Multiple)
                RemoveRole(_lvRole.SelectedRows);
            else
                RemoveRole(new List<Row> { e.Row });
        }

        async void RemoveRole(IEnumerable<Row> p_rows)
        {
            var userID = _lvUser.SelectedRow.ID;
            List<long> roles = (from r in p_rows
                                select r.Long("roleid")).ToList();
            if (roles.Count > 0 && await AtCm.RemoveUserRoles(userID, roles))
                _lvRole.Data = await AtCm.Query("用户-关联角色", new { userid = userID });
        }
    }
}