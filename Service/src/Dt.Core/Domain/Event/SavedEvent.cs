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
    /// 保存后事件
    /// </summary>
    public class SavedEvent : IEvent
    {
        /// <summary>
        /// 实体主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// true 插入操作，false 更新操作
        /// </summary>
        public bool IsInsert { get; set; }
    }
}
