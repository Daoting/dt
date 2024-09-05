#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class RoleMenuList : List
    {
        public RoleMenuList()
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
                _lv.Data = await MenuX.Query($"where exists (select menu_id from cm_role_menu b where a.id=b.menu_id and role_id={_parentID}) order by dispidx");
            }
            else
            {
                _lv.Data = null;
            }
            Menu["添加"].IsEnabled = _parentID > 0;
        }
        
        async void OnAddRelated(Mi e)
        {
            var dlg = new Menu4Role();
            if (await dlg.Show(_parentID.Value, e)
                && await RbacDs.AddRoleMenus(_parentID.Value, dlg.SelectedIDs))
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

                if (await RbacDs.RemoveRoleMenus(_parentID.Value, ids))
                    await Refresh();
            }
        }
    }
}