#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class MenuRoleList : LvTab
    {
        #region 变量
        long _releatedID;
        #endregion
        
        #region 构造
        public MenuRoleList()
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
            Menu["添加"].IsEnabled = true;
            _ = Refresh();
        }

        public void Clear()
        {
            _releatedID = -1;
            Menu["添加"].IsEnabled = false;
            _lv.Data = null;
        }
        #endregion

        #region 重写
        protected override Lv Lv => _lv;

        protected override async Task Query()
        {
            if (_releatedID > 0)
            {
                _lv.Data = await RoleX.Query($"where exists (select role_id from cm_role_menu b where a.id=b.role_id and menu_id={_releatedID})");
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
            var dlg = new Role4MenuDlg();
            if (await dlg.Show(_releatedID, e)
                && await RbacDs.AddMenuRoles(_releatedID, dlg.SelectedIDs))
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

            if (await RbacDs.RemoveMenuRoles(_releatedID, roleIDs))
                await Refresh();
        }
        #endregion
    }
}
