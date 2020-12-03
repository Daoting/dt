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
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Model
{
    [View("系统角色")]
    public partial class SysRole : Win
    {
        public SysRole()
        {
            InitializeComponent();
            LoadAll();
        }

        async void LoadAll()
        {
            _lvRole.Data = await AtCm.Query("角色-所有");
        }

        async void OnAddRole(object sender, Mi e)
        {
            if (await new EditRoleDlg().Show(-1))
                LoadAll();
        }

        async void OnEditRole(object sender, Mi e)
        {
            if (e.Row.ID > 1000)
            {
                if (await new EditRoleDlg().Show(e.Row.ID))
                    LoadAll();
            }
        }

        async void OnDelRole(object sender, Mi e)
        {
            if (e.Row.ID < 1000)
            {
                AtKit.Msg("系统角色无法删除！");
                return;
            }

            if (!await AtKit.Confirm($"确认要删除[{e.Row.Str("name")}]吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (await AtCm.DeleteRole(e.Row.ID))
            {
                AtKit.Msg("删除成功！");
                LoadAll();
            }
            else
            {
                AtKit.Warn("删除失败！");
            }
        }

        void OnNaviToSearch(object sender, RoutedEventArgs e)
        {
            NaviTo("查找角色");
        }

        async void OnSearch(object sender, string e)
        {
            if (e == "#全部")
            {
                LoadAll();
            }
            else if (e == "#系统角色")
            {
                _lvRole.Data = await AtCm.Query("角色-系统角色");
            }
            else if (!string.IsNullOrEmpty(e))
            {
                _lvRole.Data = await AtCm.Query("角色-模糊查询", new { name = $"%{e}%" });
            }
            NaviTo("角色列表");
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
            {
                long id = e.Row.ID;
                _lvUser.Data = await AtCm.Query("角色-关联用户", new { roleid = id });
                _lvMenu.Data = await AtCm.Query("角色-关联的菜单", new { roleid = id });
                _lvPrv.Data = await AtCm.Query("角色-关联的权限", new { roleid = id });
            }
            NaviTo("关联用户,拥有菜单,授予权限");
        }

        void OnDataChanged(object sender, object e)
        {
            _lvUser.Data = null;
            _lvMenu.Data = null;
            _lvPrv.Data = null;
        }

        #region User
        async void OnAddUser(object sender, Mi e)
        {
            var dlg = new SelectUserDlg();
            long roleID = _lvRole.SelectedRow.ID;
            if (await dlg.Show(roleID, e))
            {
                List<long> users = new List<long>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    users.Add(row.ID);
                }
                if (users.Count > 0 && await AtCm.AddRoleUser(roleID, users))
                    _lvUser.Data = await AtCm.Query("角色-关联用户", new { roleid = roleID });
            }
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lvUser.SelectionMode = SelectionMode.Multiple;
            _uMenu.Hide("添加", "选择");
            _uMenu.Show("移除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lvUser.SelectionMode = SelectionMode.Single;
            _uMenu.Show("添加", "选择");
            _uMenu.Hide("移除", "全选", "取消");
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lvUser.SelectAll();
        }

        void OnRemoveUser(object sender, Mi e)
        {
            RemoveUser(_lvUser.SelectedRows);
        }

        void OnRemoveUser2(object sender, Mi e)
        {
            if (_lvUser.SelectionMode == SelectionMode.Multiple)
                RemoveUser(_lvUser.SelectedRows);
            else
                RemoveUser(new List<Row> { e.Row });
        }

        async void RemoveUser(IEnumerable<Row> p_rows)
        {
            var roleID = _lvRole.SelectedRow.ID;
            List<long> users = (from r in p_rows
                                select r.Long("userid")).ToList();
            if (users.Count > 0 && await AtCm.RemoveRoleUsers(roleID, users))
                _lvUser.Data = await AtCm.Query("角色-关联用户", new { roleid = roleID });
        }
        #endregion

        #region Menu
        void OnMenuMultiMode(object sender, Mi e)
        {
            _lvMenu.SelectionMode = SelectionMode.Multiple;
            _mMenu.Hide("添加", "选择");
            _mMenu.Show("移除", "全选", "取消");
        }

        void OnMenuCancelMulti(object sender, Mi e)
        {
            _lvMenu.SelectionMode = SelectionMode.Single;
            _mMenu.Show("添加", "选择");
            _mMenu.Hide("移除", "全选", "取消");
        }

        void OnMenuSelectAll(object sender, Mi e)
        {
            _lvMenu.SelectAll();
        }

        async void OnAddMenu(object sender, Mi e)
        {
            long roleID = _lvRole.SelectedRow.ID;
            var dlg = new SelectRoleMenuDlg();
            if (await dlg.Show(roleID, e))
            {
                List<RoleMenu> ls = new List<RoleMenu>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    ls.Add(new RoleMenu(roleID, row.ID));
                }
                if (ls.Count > 0 && await AtCm.BatchSave(ls))
                {
                    _lvMenu.Data = await AtCm.Query("角色-关联的菜单", new { roleid = roleID });
                    await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "menu");
                }
            }
        }

        void OnRemoveMenu(object sender, Mi e)
        {
            RemoveMenu(_lvMenu.SelectedRows);
        }

        void OnRemoveMenu2(object sender, Mi e)
        {
            if (_lvMenu.SelectionMode == SelectionMode.Multiple)
                RemoveMenu(_lvMenu.SelectedRows);
            else
                RemoveMenu(new List<Row> { e.Row });
        }

        async void RemoveMenu(IEnumerable<Row> p_rows)
        {
            long roleID = _lvRole.SelectedRow.ID;
            List<RoleMenu> ls = new List<RoleMenu>();
            foreach (var row in p_rows)
            {
                ls.Add(new RoleMenu(roleID, row.Long("menuid")));
            }
            if (ls.Count > 0 && await AtCm.BatchDelete(ls))
            {
                _lvMenu.Data = await AtCm.Query("角色-关联的菜单", new { roleid = roleID });
                await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "menu");
            }
        }
        #endregion

        #region Prv
        void OnPrvMultiMode(object sender, Mi e)
        {
            _lvPrv.SelectionMode = SelectionMode.Multiple;
            _rMenu.Hide("添加", "选择");
            _rMenu.Show("移除", "全选", "取消");
        }

        void OnPrvCancelMulti(object sender, Mi e)
        {
            _lvPrv.SelectionMode = SelectionMode.Single;
            _rMenu.Show("添加", "选择");
            _rMenu.Hide("移除", "全选", "取消");
        }

        void OnPrvSelectAll(object sender, Mi e)
        {
            _lvPrv.SelectAll();
        }

        async void OnAddPrv(object sender, Mi e)
        {
            long roleID = _lvRole.SelectedRow.ID;
            var dlg = new SelectRolePrvDlg();
            if (await dlg.Show(roleID, e))
            {
                List<RolePrv> ls = new List<RolePrv>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    ls.Add(new RolePrv(roleID, row.Str("id")));
                }
                if (ls.Count > 0 && await AtCm.BatchSave(ls))
                {
                    _lvPrv.Data = await AtCm.Query("角色-关联的权限", new { roleid = roleID });
                    await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "privilege");
                }
            }
        }

        void OnRemovePrv(object sender, Mi e)
        {
            RemovePrv(_lvPrv.SelectedRows);
        }

        void OnRemovePrv2(object sender, Mi e)
        {
            if (_lvPrv.SelectionMode == SelectionMode.Multiple)
                RemovePrv(_lvPrv.SelectedRows);
            else
                RemovePrv(new List<Row> { e.Row });
        }

        async void RemovePrv(IEnumerable<Row> p_rows)
        {
            long roleID = _lvRole.SelectedRow.ID;
            List<RolePrv> ls = new List<RolePrv>();
            foreach (var row in p_rows)
            {
                ls.Add(new RolePrv(roleID, row.Str("prvid")));
            }
            if (ls.Count > 0 && await AtCm.BatchDelete(ls))
            {
                _lvPrv.Data = await AtCm.Query("角色-关联的权限", new { roleid = roleID });
                await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "privilege");
            }
        }
        #endregion
    }
}