#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class Role4MenuDlg : Dlg
    {
        public Role4MenuDlg()
        {
            InitializeComponent();
        }

        public List<long> SelectedIDs => (from row in _lv.SelectedRows
                                          select row.ID).ToList();

        public async Task<bool> Show(long p_releatedID, FrameworkElement p_target)
        {
            _lv.Data = await RoleX.Query("菜单-未关联的角色", new { ReleatedID = p_releatedID });
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
