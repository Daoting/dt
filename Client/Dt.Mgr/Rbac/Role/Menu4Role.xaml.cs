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
    public sealed partial class Menu4Role : Dlg
    {
        public Menu4Role()
        {
            InitializeComponent();
            Menu = Menu.New(Mi.确定(OnOK));
        }

        public List<long> SelectedIDs => (from row in _lv.SelectedRows
                                          select row.ID).ToList();

        public async Task<bool> Show(long p_releatedID, FrameworkElement p_target)
        {
            _lv.Data = await MenuX.Query($"where is_group='0' and not exists (select menu_id from cm_role_menu b where a.id=b.menu_id and role_id={p_releatedID}) order by dispidx");
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
