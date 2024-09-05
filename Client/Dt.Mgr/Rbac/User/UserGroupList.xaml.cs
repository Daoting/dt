#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class UserGroupList : List
    {
        public UserGroupList()
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
                _lv.Data = await GroupX.Query($"where exists ( select group_id from cm_user_group b where a.ID = b.group_id and user_id={_parentID.Value} )");
            }
            else
            {
                _lv.Data = null;
            }
            Menu["添加"].IsEnabled = _parentID > 0;
        }

        async void OnAddRelated(Mi e)
        {
            var dlg = new Group4User();
            if (await dlg.Show(_parentID.Value, e)
                && await RbacDs.AddUserGroups(_parentID.Value, dlg.SelectedIDs))
            {
                await Refresh();
            }
        }

        async void OnDelRelated(Mi e)
        {
            List<UserGroupX> ls = null;
            if (_lv.SelectionMode == SelectionMode.Multiple)
            {
                ls = new List<UserGroupX>();
                foreach (var row in _lv.SelectedRows)
                {
                    var x = new UserGroupX(GroupID: row.ID, UserID: _parentID.Value);
                    ls.Add(x);
                }
            }
            else
            {
                Row row = e.Row;
                if (row == null)
                    row = _lv.SelectedRow;

                if (row != null)
                    ls = new List<UserGroupX> { new UserGroupX(GroupID: row.ID, UserID: _parentID.Value) };
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