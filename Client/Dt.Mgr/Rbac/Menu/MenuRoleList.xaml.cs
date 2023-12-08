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
    public sealed partial class MenuRoleList : Tab
    {
        #region 构造方法
        public MenuRoleList()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public void Update(long p_id)
        {
            _id = p_id;
            Menu["添加"].IsEnabled = true;
            Refresh();
        }

        public void Clear()
        {
            _id = -1;
            Menu["添加"].IsEnabled = false;
            _lv.Data = null;
        }

        public async void Refresh()
        {
            _lv.Data = await RoleX.Query($"where exists (select role_id from cm_role_menu b where a.id=b.role_id and menu_id={_id})");
        }
        #endregion

        #region 交互
        async void OnAdd(Mi e)
        {
            var dlg = new Role4MenuDlg();
            if (await dlg.Show(_id, e)
                && await RbacDs.AddMenuRoles(_id, dlg.SelectedIDs))
            {
                Refresh();
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

            if (await RbacDs.RemoveMenuRoles(_id, roleIDs))
                Refresh();
        }
        #endregion

        #region 选择
        void OnSelectAll(Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Multiple;
            Menu.HideExcept("删除", "全选", "取消");
        }

        void OnCancelMulti(Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Single;
            Menu.ShowExcept("删除", "全选", "取消");
        }
        #endregion

        #region 内部
        MenuWin _win => (MenuWin)OwnWin;
        long _id;
        #endregion
    }
}
