#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// WinItem的停靠状态
    /// </summary>
    public enum WinItemState
    {
        /// <summary>
        /// 停靠在左侧
        /// </summary>
        DockedLeft,

        /// <summary>
        /// 停靠在下侧
        /// </summary>
        DockedBottom,

        /// <summary>
        /// 停靠在右侧
        /// </summary>
        DockedRight,

        /// <summary>
        /// 停靠在上侧
        /// </summary>
        DockedTop,

        /// <summary>
        /// 浮动
        /// </summary>
        Floating
    }
}

