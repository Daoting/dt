#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 领域事件的种类
    /// </summary>
    public enum DomainEvent
    {
        /// <summary>
        /// 不触发领域事件
        /// </summary>
        None,

        /// <summary>
        /// 触发本地领域事件
        /// </summary>
        Local,

        /// <summary>
        /// 触发远程领域事件
        /// </summary>
        Remote
    }
}
