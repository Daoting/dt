#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-15 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 接口
    /// </summary>
    public partial class Lv : IViewItemHost, IMenuHost
    {
        #region IViewItemHost
        bool IViewItemHost.IsCustomItemStyle => ItemStyle != null;

        void IViewItemHost.SetItemStyle(ViewItem p_item)
        {
            ItemStyle?.Invoke(new ItemStyleArgs(p_item));
        }
        #endregion

        #region IMenuHost
        /// <summary>
        /// 切换上下文菜单或修改触发事件种类时通知宿主刷新
        /// </summary>
        void IMenuHost.UpdateContextMenu()
        {
            ReloadPanelContent();
        }
        #endregion

    }
}