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

namespace Dt.MgrDemo
{
    public sealed partial class 角色4用户 : Dlg
    {
        public 角色4用户()
        {
            InitializeComponent();
        }
        
        public IEnumerable<Row> SelectedRows => _lv.SelectedRows;

        public async Task<bool> Show(long p_releatedID, FrameworkElement p_target)
        {
            _lv.Data = await 角色X.Query($"where not exists ( select role_id from demo_用户角色 b where a.ID = b.role_id and user_id={p_releatedID} )");
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
