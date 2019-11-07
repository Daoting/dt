#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 插入事件
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class InsertEvent<TEntity, TKey> : IEvent
        where TEntity : Root<TKey>
    {
        /// <summary>
        /// 实体主键
        /// </summary>
        public TKey ID { get; set; }
    }

    /// <summary>
    /// 插入事件，实体ID类型为long
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class InsertEvent<TEntity> : IEvent
        where TEntity : Root<long>
    {
        /// <summary>
        /// 实体主键
        /// </summary>
        public long ID { get; set; }
    }
}
