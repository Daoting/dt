#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 更新后事件
    /// </summary>
    public class UpdateEvent<TEntity> : IEvent
        where TEntity : Entity
    {
        /// <summary>
        /// 实体
        /// </summary>
        [RpcJson]
        public TEntity Entity { get; set; }
    }
}
