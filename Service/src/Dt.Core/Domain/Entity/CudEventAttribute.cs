#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体对象增删改触发的事件类型标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CudEventAttribute : Attribute
    {
        public CudEventAttribute(CudEvent p_event)
        {
            Event = p_event;
        }

        /// <summary>
        /// 实体对象增删改触发的事件类型，可以触发多个事件
        /// </summary>
        public CudEvent Event { get; }
    }
}
