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
        const string _tblName = "cm_user";

        public UserAccount()
        {
            InitializeComponent();
            _lvUser.ViewEx = typeof(UserViewEx);
            LoadAll();
        }

        async void LoadAll()
        {
            _lvUser.Data = await AtCm.Query("用户-所有");
        }

        void OnAddUser(object sender, Mi e)
        {
            new EditUserDlg().Show(-1);
        }

        void OnEditUser(object sender, Mi e)
        {
            new EditUserDlg().Show(e.Row.ID);
        }

        async void OnAddRole(object sender, Mi e)
        {
            SelectRolesDlg dlg = new SelectRolesDlg();
            long userID = _lvUser.SelectedRow.ID;
            if (await dlg.Show(RoleRelations.User, userID, e))
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

        async void OnRemoveRole(object sender, Mi e)
        {
            if (await AtCm.RemoveUserRole(_lvUser.SelectedRow.ID, _lvRole.SelectedRow.Long("roleid")))
                _lvRole.DeleteSelection();
        }

        void OnNaviToSearch(object sender, RoutedEventArgs e)
        {
            NaviTo("查找用户");
        }

        void OnSearch(object sender, RoutedEventArgs e)
        {
            NaviTo("用户列表");
        }

        async void OnDelUser(object sender, Mi e)
        {
            if (!await AtKit.Confirm($"确认要删除[{e.Row.Str("name")}]吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (await AtCm.DelRowByKey(e.Row.Str("id"), _tblName) == 1)
            {
                AtKit.Msg("删除成功！");
                LoadAll();
            }
            else
            {
                AtKit.Warn("删除失败！");
            }
        }

        async void OnResetPwd(object sender, Mi e)
        {
            Row row = Table.NewRow(_tblName, new { id = e.Row.ID });
            row.IsAdded = false;
            string phone = e.Row.Str("phone");
            row["pwd"] = AtKit.GetMD5(phone.Substring(phone.Length - 4));

            if (await AtCm.SaveRow(row, _tblName))
                AtKit.Msg("密码已重置为手机号后4位！");
            else
                AtKit.Msg("重置密码失败！");
        }

        async void OnToggleExpired(object sender, Mi e)
        {
            bool expired = e.Row.Bool("expired");
            Row row = Table.NewRow(_tblName, new { id = e.Row.ID, expired = expired });
            row.IsAdded = false;
            row["expired"] = !expired;

            string act = expired ? "启用" : "停用";
            if (await AtCm.SaveRow(row, _tblName))
            {
                AtKit.Msg($"账号[{e.Row.Str("name")}]已{act}！");
                LoadAll();
            }
            else
            {
                AtKit.Msg(act + "失败！");
            }
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            _lvRole.Data = await AtCm.Query("用户-关联角色", new { userid = e.Row.ID });
        }

        class UserViewEx
        {
            public static void SetStyle(ViewItem p_item)
            {
                if (p_item.Row.Bool("expired"))
                {
                    p_item.Foreground = AtRes.GrayBrush;
                    p_item.FontStyle = Windows.UI.Text.FontStyle.Italic;
                }
            }
        }
    }
}