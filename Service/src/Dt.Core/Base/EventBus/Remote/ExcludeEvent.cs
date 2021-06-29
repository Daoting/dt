#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// 向所有服务副本进行组播时排除指定副本
    /// </summary>
    public class ExcludeEvent : IEvent
    {
        /// <summary>
        /// 排除的服务副本ID
        /// </summary>
        public virtual string ExcludeSvcID { get; } = Kit.SvcID;
    }
}