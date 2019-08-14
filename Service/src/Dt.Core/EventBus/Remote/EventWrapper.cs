#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// 远程事件传输的包装类
    /// </summary>
    internal class EventWrapper
    {
        /// <summary>
        /// 事件类型名称
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 事件内容
        /// </summary>
        public string Data { get; set; }
    }
}