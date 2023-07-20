#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-06-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.多对多
{
    public sealed partial class 权限4角色Dlg : Dlg
    {
        public 权限4角色Dlg()
        {
            InitializeComponent();
        }
        
        public IEnumerable<Row> SelectedRows => _lv.SelectedRows;

        public async Task<bool> Show(long p_releatedID, FrameworkElement p_target)
        {
            _lv.Data = await 权限X.Query($"where not exists ( select prv_id from demo_角色权限 b where a.ID = b.prv_id and role_id={p_releatedID} )");
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
