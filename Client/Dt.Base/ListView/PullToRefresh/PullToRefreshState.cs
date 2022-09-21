#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 下拉刷新过程的状态
    /// </summary>
    public enum PullToRefreshState
    {
        /// <summary>
        /// 闲置
        /// </summary>
        Idle,

        /// <summary>
        /// 交互中
        /// </summary>
        Interacting,

        /// <summary>
        /// 放弃刷新
        /// </summary>
        CancelRefresh,

        /// <summary>
        /// 刷新中
        /// </summary>
        Refreshing,
    }
}
