#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 菜单分割行
    /// </summary>
    public partial class MiSplit : Mi
    {
        public MiSplit()
        {
            DefaultStyleKey = typeof(MiSplit);
            MinHeight = 0;
        }

        protected override void UpdateRoleState()
        {
            if (ParentMi != null || Owner.IsContextMenu)
                VisualStateManager.GoToState(this, "SubSplit", true);
            else
                VisualStateManager.GoToState(this, "TopSplit", true);
        }
    }
}
