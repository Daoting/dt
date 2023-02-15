﻿#region 文件描述
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

namespace Dt.Mgr.Model
{
    public sealed partial class RoleMenuList : Tab
    {
        long _roleID;

        public RoleMenuList()
        {
            InitializeComponent();
        }

        public void Update(long p_userID)
        {
            _roleID = p_userID;
            Menu["添加"].IsEnabled = true;
            Refresh();
        }

        public void Clear()
        {
            _roleID = -1;
            Menu["添加"].IsEnabled = false;
            _lv.Data = null;
        }

        async void Refresh()
        {
            _lv.Data = await AtCm.Query("角色-关联的菜单", new { roleid = _roleID });
        }

        async void OnAdd(object sender, Mi e)
        {
            var dlg = new SelectRoleMenuDlg();
            if (await dlg.Show(_roleID, e))
            {
                List<RoleMenuX> ls = new List<RoleMenuX>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    ls.Add(new RoleMenuX(_roleID, row.ID));
                }
                if (ls.Count > 0 && await ls.Save())
                {
                    Refresh();
                    await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "menu");
                }
            }
        }

        void OnRemove(object sender, Mi e)
        {
            DoRemove(_lv.SelectedRows);
        }

        void OnRemove2(object sender, Mi e)
        {
            if (_lv.SelectionMode == SelectionMode.Multiple)
                DoRemove(_lv.SelectedRows);
            else
                DoRemove(new List<Row> { e.Row });
        }

        async void DoRemove(IEnumerable<Row> p_rows)
        {
            List<RoleMenuX> ls = new List<RoleMenuX>();
            foreach (var row in p_rows)
            {
                ls.Add(new RoleMenuX(_roleID, row.Long("menuid")));
            }
            if (ls.Count > 0 && await ls.Delete())
            {
                Refresh();
                await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "menu");
            }
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

        RoleWin _win => (RoleWin)OwnWin;
    }
}
