﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class PerRoleList : List
    {
        public PerRoleList()
        {
            InitializeComponent();
            Menu = Menu.New(Mi.添加(OnAddRelated, enable: false), Mi.删除(OnDelRelated));
            _lv.AddMultiSelMenu(Menu);
            _lv.SetMenu(Menu.New(Mi.删除(OnDelRelated)));
        }

        protected override async Task OnQuery()
        {
            if (_parentID > 0)
            {
                _lv.Data = await RoleX.Query($"where exists (select role_id from cm_role_per b where a.id=b.role_id and per_id={_parentID})");
            }
            else
            {
                _lv.Data = null;
            }
            Menu["添加"].IsEnabled = _parentID > 0;
        }
        
        async void OnAddRelated(Mi e)
        {
            var dlg = new Role4PerDlg();
            if (await dlg.Show(_parentID.Value, e)
                && await RbacDs.AddPerRoles(_parentID.Value, dlg.SelectedIDs))
            {
                await Refresh();
            }
        }
        
        async void OnDelRelated(Mi e)
        {
            List<long> ids = null;
            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
            {
                ids = (from row in _lv.SelectedRows
                       select row.ID).ToList();
            }
            else
            {
                Row row = e.Row;
                if (row == null)
                    row = _lv.SelectedRow;

                if (row != null)
                    ids = new List<long> { row.ID };
            }

            if (ids != null && ids.Count > 0)
            {
                if (!await Kit.Confirm("确认要删除关联吗？"))
                {
                    Kit.Msg("已取消删除！");
                    return;
                }

                if (await RbacDs.RemovePerRoles(_parentID.Value, ids))
                    await Refresh();
            }
        }
    }
}