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
    public sealed partial class Role4Group : Dlg
    {
        public Role4Group()
        {
            InitializeComponent();
            Menu = Menu.New(Mi.确定(OnOK));
        }

        public List<long> SelectedIDs => (from row in _lv.SelectedRows
                                          select row.ID).ToList();

        public async Task<bool> Show(long p_releatedID, FrameworkElement p_target)
        {
            _lv.Data = await RoleX.Query($"where not exists ( select role_id from cm_group_role b where a.ID = b.role_id and group_id={p_releatedID} ) and a.ID!=1");
            if (!Kit.IsPhoneUI)
            {
                WinPlacement = DlgPlacement.TargetBottomLeft;
                PlacementTarget = p_target;
                ClipElement = p_target;
                Height = Kit.ViewHeight / 2;
                Width = Kit.ViewWidth / 4;
            }
            return await ShowAsync();
        }
    }
}
