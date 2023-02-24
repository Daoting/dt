﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.多对多
{
    public sealed partial class 角色4用户Dlg : Dlg
    {
        public 角色4用户Dlg()
        {
            InitializeComponent();
        }
        
        public IEnumerable<Row> SelectedRows => _lv.SelectedRows;

        public async Task<bool> Show(long p_releatedID, FrameworkElement p_target)
        {
            _lv.Data = await 角色X.Query("where not exists ( select RoleID from demo_用户角色 b where a.ID = b.RoleID and UserID=@ReleatedID )", new { ReleatedID = p_releatedID });
            if (!Kit.IsPhoneUI)
            {
                WinPlacement = DlgPlacement.TargetBottomLeft;
                PlacementTarget = p_target;
                ClipElement = p_target;
                Height = Kit.ViewHeight / 2;
                MaxWidth = 400;
            }
            return await ShowAsync();
        }
    }
}
