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
    /// 删除事件
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class DeleteEvent<TEntity, TKey> : IEvent
        where TEntity : Root<TKey>
    {
        public DeleteEvent(TKey p_id)
        {
            ID = p_id;
        }

        /// <summary>
        /// 实体主键
        /// </summary>
        public TKey ID { get; }
    }

    /// <summary>
    /// 删除事件，实体ID类型为long
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DeleteEvent<TEntity> : IEvent
        where TEntity : Root<long>
    {
        public DeleteEvent(long p_id)
        {
            ID = p_id;
        }

        /// <summary>
        /// 实体主键
        /// </summary>
        public long ID { get; }
    }
}