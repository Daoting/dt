#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Crud
{
    public partial class 权限角色List : List
    {
        public 权限角色List()
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
                _lv.Data = await 角色X.Query($"where exists ( select role_id from crud_角色权限 b where a.ID = b.role_id and prv_id={_parentID} )");
            }
            else
            {
                _lv.Data = null;
            }
            Menu["添加"].IsEnabled = _parentID > 0;
        }
        
        async void OnAddRelated(Mi e)
        {
            var dlg = new 角色4权限();
            if (await dlg.Show(_parentID.Value, e))
            {
                var ls = new List<角色权限X>();
                foreach (var row in dlg.SelectedRows)
                {
                    var x = new 角色权限X(RoleID: row.ID, PrvID: _parentID.Value);
                    ls.Add(x);
                }
                if (ls.Count > 0 && await ls.Save())
                    await Refresh();
            }
        }
        
        async void OnDelRelated(Mi e)
        {
            List<角色权限X> ls = null;
            if (_lv.SelectionMode == SelectionMode.Multiple)
            {
                ls = new List<角色权限X>();
                foreach (var row in _lv.SelectedRows)
                {
                    var x = new 角色权限X(RoleID: row.ID, PrvID: _parentID.Value);
                    ls.Add(x);
                }
            }
            else
            {
                Row row = e.Row;
                if (row == null)
                    row = _lv.SelectedRow;

                if (row != null)
                    ls = new List<角色权限X> { new 角色权限X(RoleID: row.ID, PrvID: _parentID.Value) };
            }
            
            if (ls != null && ls.Count > 0)
            {
                if (!await Kit.Confirm("确认要删除关联吗？"))
                {
                    Kit.Msg("已取消删除！");
                    return;
                }

                if (await ls.Delete())
                    await Refresh();
            }
        }
    }
}