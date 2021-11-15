#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
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
        }

        protected override void UpdateRoleState()
        {
            VisualStateManager.GoToState(this, ParentMi == null ? "TopSplit" : "SubSplit", true);
        }
    }
}
