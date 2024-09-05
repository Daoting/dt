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
    public partial class RolePerList : List
    {
        public RolePerList()
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
                _lv.Data = await PermissionX.GetRolePermission(_parentID.Value);
            }
            else
            {
                _lv.Data = null;
            }
            Menu["添加"].IsEnabled = _parentID > 0;
        }
        
        void OnAddRelated(Mi e)
        {
            Per4RoleWin win;
            if (Kit.IsPhoneUI)
            {
                win = (Per4RoleWin)Kit.OpenWin(typeof(Per4RoleWin), null, Icons.Edge, _parentID.Value);
            }
            else
            {
                win = new Per4RoleWin(_parentID.Value);
                var dlg = new Dlg
                {
                    WinPlacement = DlgPlacement.TargetBottomLeft,
                    PlacementTarget = e,
                    ClipElement = e,
                    Height = Kit.ViewHeight / 2,
                    Width = 600,
                };

                dlg.LoadWin(win);
                dlg.Show();
            }

            win.Closed += async (s, e) =>
            {
                if (win.IsOK && await RbacDs.AddRolePers(_parentID.Value, win.SelectedIDs))
                {
                    await Refresh();
                }
            };
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

                if (await RbacDs.RemoveRolePers(_parentID.Value, ids))
                    await Refresh();
            }
        }
    }
}