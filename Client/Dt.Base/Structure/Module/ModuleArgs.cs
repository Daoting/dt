#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-27 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// List的Msg事件参数
    /// </summary>
    public class LvMsgArgs
    {
        /// <summary>
        /// 实体主键值
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 控制Form行为：打开、关闭、清空数据、无操作
        /// </summary>
        public FormAction Action { get; set; }
        
        /// <summary>
        /// 一对多的父ID，多对多时的关联ID
        /// </summary>
        public long? ParentID { get; set; }
        
        /// <summary>
        /// Lv中传递的实体数据，无用可不设置
        /// </summary>
        public Row Data { get; set; }
        
        /// <summary>
        /// 附加参数
        /// </summary>
        public object Tag { get; set; }
        
        /// <summary>
        /// 原始事件类别
        /// </summary>
        public LvEventType Event { get; set; }
    }

    /// <summary>
    /// UpdateList事件参数
    /// </summary>
    public class UpdateListArgs
    {
        /// <summary>
        /// 实体主键值
        /// </summary>
        public long? ID { get; set; }

        /// <summary>
        /// 实体数据
        /// </summary>
        public Row Data { get; set; }

        /// <summary>
        /// 原始事件类别
        /// </summary>
        public UpdateListEvent Event { get; set; }
    }

    /// <summary>
    /// UpdateRelated事件参数
    /// </summary>
    public class UpdateRelatedArgs
    {
        /// <summary>
        /// 实体主键值
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 实体数据，无用可不设置
        /// </summary>
        public Row Data { get; set; }

        /// <summary>
        /// 原始事件类别
        /// </summary>
        public UpdateRelatedEvent Event { get; set; }
    }

    /// <summary>
    /// Tree的Msg事件参数
    /// </summary>
    public class TvMsgArgs
    {
        /// <summary>
        /// 实体主键值
        /// </summary>
        public long? ID { get; set; }
        
        /// <summary>
        /// 一对多的父ID，多对多时的关联ID
        /// </summary>
        public long? ParentID { get; set; }

        /// <summary>
        /// Lv中传递的实体数据，无用可不设置
        /// </summary>
        public Row Data { get; set; }

        /// <summary>
        /// 附加参数
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 原始事件类别
        /// </summary>
        public TvEventType Event { get; set; }
    }
}
