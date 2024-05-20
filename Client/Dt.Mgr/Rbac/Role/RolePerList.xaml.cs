﻿#region 文件描述
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
    public partial class RolePerList : LvTab
    {
        #region 变量
        long _releatedID;
        #endregion

        #region 构造
        public RolePerList()
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
            Menu["添加"].IsEnabled = _releatedID > 0;
            _ = Refresh();
        }
        #endregion

        #region 重写
        protected override Lv Lv => _lv;

        protected override async Task Query()
        {
            if (_releatedID > 0)
            {
                _lv.Data = await PermissionX.GetRolePermission(_releatedID);
            }
            else
            {
                _lv.Data = null;
            }
        }
        #endregion

        #region 交互
        void OnAdd(Mi e)
        {
            Per4RoleWin win;
            if (Kit.IsPhoneUI)
            {
                win = (Per4RoleWin)Kit.OpenWin(typeof(Per4RoleWin), null, Icons.Edge, _releatedID);
            }
            else
            {
                win = new Per4RoleWin(_releatedID);
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
                if (win.IsOK && await RbacDs.AddRolePers(_releatedID, win.SelectedIDs))
                {
                    await Refresh();
                }
            };
        }

        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除关联吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            List<long> ids;
            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
            {
                ids = (from row in _lv.SelectedRows
                       select row.ID).ToList();
            }
            else
            {
                ids = new List<long> { e.Row.ID };
            }

            if (await RbacDs.RemoveRolePers(_releatedID, ids))
                await Refresh();
        }
        #endregion
    }
}